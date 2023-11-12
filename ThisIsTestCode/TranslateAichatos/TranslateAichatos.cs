using System;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace TranslateAichatos
{
    class Program
    {
        static void Main(string[] args)
        {
            // 启动浏览器
            EdgeOptions options = new EdgeOptions();
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--ignore-certificate-errors");
            options.AddArgument("--ignore-ssl-errors");
            options.AddArgument("--allow-running-insecure-content");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");
            // options.AddArgument("--headless");
            var driver = new EdgeDriver(options);
            driver.Navigate().GoToUrl("https://chat.aichatos.top/#/chat/1698193693443");
            
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


            // 创建SQLite连接
            string connectionString = "Data Source=E:\\Downloads\\NPClymph\\Radiology\\Radiology.ctv6;";
            var connection = new SQLiteConnection(connectionString);
            // 打开连接
            connection.Open();

            // 创建一个SQL查询
            string sql = "SELECT ID, Title FROM Reference";

            // 创建SQLiteCommand对象
            using (var command = new SQLiteCommand(sql, connection))
            {
                // 执行查询并获取结果集
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    int line = 0;
                    while (reader.Read())
                    {
                        // 读取Title字段的值
                        string q_title = reader.GetString(1);

                        // 进行翻译操作，这里只是一个示例，你需要替换为你的翻译程序
                        string questionHead = "请翻译下面文字成中文，请直接返回翻译结果：";
                        string translatedTitle = GetMessages(wait, textBox, driver, questionHead + q_title);
                        translatedTitle  = Regex.Replace(translatedTitle, @"^\s*$\n|\r\n?|\n", string.Empty, RegexOptions.Multiline);
                        // 更新TranslateTitle字段的值
                        byte[] idBytes = new byte[16]; // 假设 ID 的大小为 16 字节
                        long bytesRead = reader.GetBytes(0, 0, idBytes, 0, idBytes.Length);
                        // 转换为 Guid 类型
                        var id = new Guid(idBytes);


                        string updateSql = "UPDATE Reference SET TranslatedTitle=@translatedTitle WHERE ID=@id";
                        using (var updateCommand = new SQLiteCommand(updateSql, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@translatedTitle", translatedTitle);
                            updateCommand.Parameters.AddWithValue("@id", id);
                            updateCommand.ExecuteNonQuery();
                        }
                        line ++;
                        Console.WriteLine(line.ToString());

                    }
                }
            }
            connection.Close();
            // 退出
            Console.ReadKey();
            driver.Quit();
        }

        public static string GetMessages(WebDriverWait wait, IWebElement textBox, EdgeDriver driver, string massages)
        {
            // 在文本框中输入内容并按下回车键
            textBox.SendKeys(massages);
            textBox.SendKeys(OpenQA.Selenium.Keys.Enter);
            // 对 wait 再次等待40秒
            // wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            wait.Until((d) =>
            {
                try
                {
                    //MessageBox.Show("GPT正在答复中...");
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
