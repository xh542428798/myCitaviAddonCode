using System;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using SwissAcademic.Citavi.Shell;
using Newtonsoft.Json; // 确保引用了 Newtonsoft.Json
using System.Collections.Generic;
using System.Windows.Input;

namespace PDFThumbnailTranslation
{
    public partial class TranslationControl : UserControl
    {
        private readonly MainForm _mainForm;
        private readonly HttpClient _httpClient;

        public TranslationControl(MainForm mainForm)
        {
            InitializeComponent();
            _mainForm = mainForm;
            _httpClient = new HttpClient();

            // 加载配置到界面
            LoadConfigToUI();
        }

        private void LoadConfigToUI()
        {
            var settings = TranslationConfig.LoadSettings();
            ApiKeyTextBox.Text = settings.ApiKey;
            PromptTextBox.Text = settings.Prompt;
            AutoTranslateCheckBox.IsChecked = settings.AutoTranslate; // <--- 新增这一行
        }

        private void SaveConfigButton_Click(object sender, RoutedEventArgs e)
        {
            var settings = new TranslationSettings
            {
                ApiKey = ApiKeyTextBox.Text,
                Prompt = PromptTextBox.Text,
                AutoTranslate = AutoTranslateCheckBox.IsChecked == true // <--- 新增这一行
            };
            TranslationConfig.SaveSettings(settings);
            MessageBox.Show("配置已保存！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void TranslateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(SourceTextBox.Text))
            {
                MessageBox.Show("请先选择要翻译的文本。", "提示", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (string.IsNullOrEmpty(ApiKeyTextBox.Text))
            {
                MessageBox.Show("API Key 不能为空。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            TranslateButton.IsEnabled = false;
            TranslateButton.Content = "翻译中...";
            TargetTextBox.Text = "正在请求翻译，请稍候...";

            try
            {
                string translatedText = await TranslateTextAsync(SourceTextBox.Text, ApiKeyTextBox.Text, PromptTextBox.Text);
                TargetTextBox.Text = translatedText;
            }
            catch (Exception ex)
            {
                TargetTextBox.Text = "翻译失败！\n错误信息: " + ex.Message;
            }
            finally
            {
                TranslateButton.IsEnabled = true;
                TranslateButton.Content = "翻译";
            }
        }

        // 异步翻译方法，内部逻辑与你的宏类似
        private async System.Threading.Tasks.Task<string> TranslateTextAsync(string textToTranslate, string apiKey, string prompt)
        {
            string apiUrl = "https://open.bigmodel.cn/api/paas/v4/chat/completions";
            string fullPrompt = prompt + "\n" + textToTranslate;

            var requestBody = new
            {
                model = "glm-4-flash-250414",
                messages = new[]
                {
                    new { role = "system", content = prompt }, // 将prompt作为系统消息
                    new { role = "user", content = textToTranslate } // 将待翻译文本作为用户消息
                }
            };

            string jsonContent = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            ZhipuApiResponse responseObj = JsonConvert.DeserializeObject<ZhipuApiResponse>(responseBody);
            return responseObj.Choices[0].Message.Content;
        }
        // 当用户在PDF预览界面选中文本时，更新到原文框
        public void OnSelectionChanged()
        {
            // 直接使用我们新的、可靠的辅助方法获取选中文本
            string selectedText = _mainForm.PreviewControl.GetSelectedTextFromPdf();

            // --- 核心新逻辑 ---
            // 只有在勾选了“自动翻译”时，才自动填充并翻译
            if (AutoTranslateCheckBox.IsChecked == true)
            {
                if (!string.IsNullOrEmpty(selectedText))
                {
                    // 1. 填充文本
                    SourceTextBox.Text = selectedText;

                    // 2. 开始自动翻译
                    _ = TranslateTextAsync(SourceTextBox.Text, ApiKeyTextBox.Text, PromptTextBox.Text)
                        .ContinueWith(task =>
                        {
                            // 使用 Dispatcher 确保在UI线程更新结果
                            Dispatcher.Invoke(() =>
                            {
                                if (task.IsFaulted)
                                {
                                    TargetTextBox.Text = "翻译失败！\n错误信息: " + task.Exception?.InnerException?.Message;
                                }
                                else if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
                                {
                                    TargetTextBox.Text = task.Result;
                                }
                            });
                        });
                }
                else
                {
                    // 如果没有选中文本，但自动翻译是开启的，就清空界面
                    SourceTextBox.Text = "";
                    TargetTextBox.Text = "";
                }
            }
            // --- 如果没有勾选“自动翻译”，则什么都不做 ---
            // 这样，SourceTextBox 的内容就不会被PDF选中的文本覆盖，用户可以自由编辑
        }


        // 在 TranslationControl 类中添加这个新方法
        private void ClearSourceButton_Click(object sender, RoutedEventArgs e)
        {
            // 直接清空原文文本框的内容
            SourceTextBox.Text = "";
        }
        // 清空翻译界面的内容
        public void ClearContent()
        {
            SourceTextBox.Text = "";
            TargetTextBox.Text = "";
        }

    }

    // 与你的宏中完全相同的API响应类
    public class ZhipuApiResponse
    {
        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; }
    }
    public class Choice
    {
        [JsonProperty("message")]
        public Message Message { get; set; }
    }
    public class Message
    {
        [JsonProperty("content")]
        public string Content { get; set; }
    }
}
