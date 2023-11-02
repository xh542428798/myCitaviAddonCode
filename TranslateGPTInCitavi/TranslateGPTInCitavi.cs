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
using OpenQA.Selenium.Chrome;
// 需要指定x64平台
namespace TranslateGPTInCitaviAddon
{
    public class TranslateGPTInCitavi
                        :
        CitaviAddOn<MainForm>
    {
        public override void OnHostingFormLoaded(MainForm mainForm)
        {

            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(7, "StartGPT", "Start GPT system", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.StartForm_Logo);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(8, "TranslateGPT", "Tranlate active Titles and Abstracts using GPT", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.OnlineSearch);
            mainForm.GetMainCommandbarManager().GetReferenceEditorCommandbar(MainFormReferenceEditorCommandbarId.Toolbar).InsertCommandbarButton(9, "EndGPT", "End GPT system", CommandbarItemStyle.ImageOnly, image: SwissAcademic.Citavi.Shell.Properties.Resources.SearchTask);
            base.OnHostingFormLoaded(mainForm);
        }

        // 在类的顶部声明变量
        private EdgeDriver driver;
        private WebDriverWait wait;
        private IWebElement textBox;

        public override void OnBeforePerformingCommand(MainForm mainForm, BeforePerformingCommandEventArgs e)
        {
            switch (e.Key)
            {
                case "StartGPT":
                    {
                        e.Handled = true;
                        EdgeOptions options = new EdgeOptions();
                        options.AddArgument("--disable-popup-blocking");
                        options.AddArgument("--ignore-certificate-errors");
                        options.AddArgument("--ignore-ssl-errors");
                        options.AddArgument("--allow-running-insecure-content");
                        options.AddArgument("--no-sandbox");
                        options.AddArgument("--disable-dev-shm-usage");
                        // options.AddArgument("--headless");
                        // 设置驱动程序路径
                        //var driverPath = "E:\\Downloads\\NPClymph\\CitaviAddonDebug\\TranslateAichatosCitavi\\packages\\Selenium.WebDriver.MSEdgeDriver.118.0.2088.41\\driver\\win64\\msedgedriver.exe";
                        // 设置 Selenium Manager 的路径
                        //string seleniumManagerPath = @"E:\Downloads\NPClymph\CitaviAddonDebug\TranslateAichatosCitavi\packages\Selenium.WebDriver.4.14.1\manager\windows\selenium-manager.exe";
                        //options.BinaryLocation = seleniumManagerPath;

                        this.driver = new EdgeDriver(options);
                        this.driver.Navigate().GoToUrl("https://chat.aichatos.top/#/chat/1698193693443");

                        this.wait = new WebDriverWait(this.driver, TimeSpan.FromSeconds(40));
                        this.textBox = this.wait.Until((d) =>
                        {
                            try
                            {
                                return this.driver.FindElement(By.CssSelector(".n-input__textarea-el"));
                            }
                            catch (Exception)
                            {
                                return null;
                            }
                        });

                        //Console.ReadKey();
                    }
                    break;
                case "TranslateGPT":
                    {
                        e.Handled = true;
                        List<Reference> references = Program.ActiveProjectShell.PrimaryMainForm.GetSelectedReferences();

                        foreach (Reference reference in references)
                        {
                            string questionHead = "请翻译下面文字成中文，请直接返回翻译结果：";
                            // 将翻译后的Title转成Subtitle和TranslateTitle
                            string q_title = reference.Title;
                            if (! string.IsNullOrEmpty(q_title))
                            {
                                string q_transTitle = GetMessages(this.wait, this.textBox, this.driver, questionHead + q_title);
                                reference.Subtitle = q_transTitle;
                                reference.TranslatedTitle = q_transTitle;
                            }

                            //MessageBox.Show(q_transTitle);
                           
                            string q_abstract = reference.Abstract.Text;
                            if (! string.IsNullOrEmpty(q_abstract))
                            {
                                string q_trans = GetMessages(this.wait, this.textBox, this.driver, questionHead + q_abstract);
                                //MessageBox.Show(q_trans);
                                // 将翻译后的Abstract转成TableOfContents
                                reference.TableOfContents.Text = q_trans;
                            }

                        }
                        //MessageBox.Show("SearchForm did not open");

                    }
                    break;
                case "EndGPT":
                    {
                        e.Handled = true;
                        // 结束程序
                        this.driver.Quit();
                        
                    }
                    break;
            }

            base.OnBeforePerformingCommand(mainForm, e);
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
                    //MessageBox.Show("GPT正在答复中...");
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
                    // MessageBox.Show(paragraph.Text);
                    paragraph_string = paragraph_string + "\n" + paragraph.Text;
                }

                return paragraph_string;
                // 处理最后一个元素,打印回答内容
                //MessageBox.Show(lastElement.Text);
            }
            else
            {
                return null;
            }
        }
    }
}
