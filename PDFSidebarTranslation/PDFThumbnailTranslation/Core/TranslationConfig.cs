using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;

namespace PDFThumbnailTranslation
{
    // 用于序列化到JSON的简单数据类
    public class TranslationSettings
    {
        public string ApiKey { get; set; }
        public string Prompt { get; set; }
        public bool AutoTranslate { get; set; } // <--- 新增这一行
    }

    public static class TranslationConfig
    {
        private static readonly string ConfigFilePath = Path.Combine(Application.StartupPath, "TranslationConfig.json");

        // 默认配置
        private static readonly TranslationSettings DefaultSettings = new TranslationSettings
        {
            ApiKey = "", // 你的默认API Key
            Prompt = "你是一个医学英语专家，正在进行医学文献翻译。无需深入思考或长篇解释，请将以下内容翻译成中文，只返回翻译后的内容：",
            AutoTranslate = false // <--- 新增这一行，默认为 false
        };

        // 读取配置
        public static TranslationSettings LoadSettings()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    // 注意：旧版项目可能需要手动添加 Newtonsoft.Json.dll 的引用
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<TranslationSettings>(json);
                }
            }
            catch (Exception ex)
            {
                // 如果读取失败，可以记录日志或直接返回默认配置
                MessageBox.Show("读取翻译配置失败，将使用默认配置。\n错误信息: " + ex.Message, "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return DefaultSettings;
        }

        // 保存配置
        public static void SaveSettings(TranslationSettings settings)
        {
            try
            {
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(settings, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(ConfigFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show("保存翻译配置失败。\n错误信息: " + ex.Message, "配置错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
