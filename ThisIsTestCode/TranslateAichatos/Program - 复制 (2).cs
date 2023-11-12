using System;
using System.Collections.Generic;
using System.Linq;
using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

using SwissAcademic.Controls;

using System.Reflection;

namespace TranslateAichatos
{
    public class TranslateAichatos
                        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {

            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(7, "TranslateAichatos", "TranslateAichatos", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.Filter);
            base.OnHostingFormLoaded(mainForm);
        }

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "ShowReferenceFromSearchForm":
                    {
                        e.Handled = true;
                        // 判断SearchForm是否已经打开
                        // 创建一个字符串列表，保存所有 Form 的名字
                        List<string> formNameList = new List<string>();
                        for (int i = 0; i < Application.OpenForms.Count; i++)
                        {
                            Form myform = Application.OpenForms[i];
                            if (!myform.Visible) continue;
                            formNameList.Add(myform.Name);
                            //form.Activate();
                            //MessageBox.Show(myform.Name);
                        }
                        // 判断一个名字是否在字符串列表中
                        string nameToCheck = "SearchForm";
                        bool isInList = formNameList.Contains(nameToCheck);
                        if (isInList)
                        {
                            // 名字在列表中
                            SearchForm searchForm = Program.ActiveProjectShell.SearchForm;
                            // 获取类型信息
                            Type type = typeof(SearchForm);
                            // 获取私有方法信息
                            MethodInfo methodInfo = type.GetMethod("GetSelectedReferences", BindingFlags.NonPublic | BindingFlags.Instance);
                            // 调用私有方法
                            List<Reference> references = (List<Reference>)methodInfo.Invoke(searchForm, null);
                            if (references.Count == 0)
                            {
                                MessageBox.Show("No reference selected in searchForm");
                            }
                            else
                            {
                                mainForm.ActiveReference = references[0]; //只会将第1个文献作为Active Reference显示 
                                mainForm.Activate();
                            }
                        }
                        else
                        {
                            // 名字不在列表中
                            MessageBox.Show("SearchForm did not open");
                        }
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
        }
    }
}

namespace TranslateAichatos
{
    class Program
    {
        static void Main(string[] args)
        {


            EdgeOptions options = new EdgeOptions();
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--headless");
            var driver = new EdgeDriver(options);
            driver.Navigate().GoToUrl("https://chat.aichatos.top/#/chat/1698193693443");
            

            // 等待页面加载完成并检查是否出现安全警告
            //WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            ////wait.Until(ExpectedConditions.ElementExists(By.Id("error-page-content")));
            //// 点击"高级"按钮
            //IWebElement advancedButton = driver.FindElement(By.XPath("//button[contains(text(), '高级')]"));
            //advancedButton.Click();
            //// 查找链接文本中包含“继续访问”的元素并进行点击操作
            //IList<IWebElement> elements = driver.FindElements(By.TagName("a"));
            //foreach (IWebElement element in elements)
            //{
            //    if (element.Text.Contains("继续访问"))
            //    {
            //        element.Click();
            //        break;
            //    }
            //}
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(40));
            IWebElement textBox = wait.Until((d) =>
            {
                try
                {
                    return driver.FindElement(By.CssSelector(".n-input__textarea-el"));
                }
                catch (Exception)
                {
                    return null;
                }
            });

            //wait2.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.Id("inputbox-id")));
            //// 找到文本框元素
            //IWebElement textBox = driver.FindElement(By.Id("inputbox-id"));

            // 在文本框中输入内容并按下回车键
            string myResponse = GetMessages(wait, textBox, driver, "请翻译成英语：请注意，上述示例仅作为参考，具体选择方法取决于对话框在HTML中的结构和属性。您可以根据对话框的HTML代码自行调整选择方法，并根据需要进一步处理对话框元素。");
            Console.WriteLine(myResponse);

            myResponse = GetMessages(wait, textBox, driver, "请翻译成中文，注意分成2段: Duplication check, Check for duplicate items when new items are added.\nItem type check\nWhen adding an item, if its item type is a web page and its URL contains the domain of the major scholarly publisher, prompt the user to ask if they have imported the wrong type of item.\nConvert title from \"heading case\" to \"sentence case\"\nThe Zotero's documentation recommends storing titles in \"sentence case\" format, which will allow CSL to perform a \"title case\" transformation on them 2. Zotero 7 has a built-in function to convert titles to \"sentence capitalization\", and some special case detection is preset, and this plugin expands on that by adding recognition of proper nouns, such as chemical formulas.\nDetailed test results are available at test/toSentenceCase.test.ts.\nLook up the journal abbreviation according to the journal full name\nThe plugin has a built-in dataset of about 96,000 journal abbreviations (from JabRef and Woodward Library), and the plugin will first look up the journal abbreviations in the local dataset;");
            Console.WriteLine(myResponse);

            // 结束程序
            Console.ReadKey();
            driver.Quit();
        }


        public static string GetMessages(WebDriverWait wait, IWebElement textBox, EdgeDriver driver, string massages)
        {
            // 在文本框中输入内容并按下回车键
            textBox.SendKeys(massages);
            textBox.SendKeys(OpenQA.Selenium.Keys.Enter);
            // 对 wait 再次等待40秒
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            wait.Until((d) =>
            {
                try
                {
                    Console.WriteLine("GPT正在答复中...");
                    //var button = ;
                    return !driver.FindElement(By.XPath("//button[contains(@class, 'n-button--warning-type') and contains(@class, 'n-button--medium-type')]")).Displayed;
                }
                catch (NoSuchElementException)
                {
                    // 如果找不到按钮，则表示它已经消失
                    return true;
                }
            });


            // 使用XPath选择器定位回答元素
            var answerElements = wait.Until((d) =>
            {
                try
                {
                    return driver.FindElements(By.XPath("//div[@class='markdown-body']"));
                }
                catch (Exception)
                {
                    return null;
                }
            });
            var lastElement = answerElements.LastOrDefault();
            if (lastElement != null)
            {
                var paragraphs = lastElement.FindElements(By.XPath("./p[not(contains(@style, 'display:none'))]"));

                string paragraph_string = null;

                foreach (var paragraph in paragraphs)
                {
                   // Console.WriteLine(paragraph.Text);
                    paragraph_string = paragraph_string + "\n" + paragraph.Text;
                }

                return paragraph_string;
                // 处理最后一个元素,打印回答内容
                //Console.WriteLine(lastElement.Text);
            }
            else
            {
                return null;
            }
        }

    }
}
