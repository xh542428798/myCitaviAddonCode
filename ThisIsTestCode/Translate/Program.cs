using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Google
{
    class Program
    {
        static void Main(string[] args)
        {
            Google();
            Console.ReadKey();
        }

        public static void Google()
        {
            string url = "https://translate.google.com/_/TranslateWebserverUi/data/batchexecute";
            string q = "The regional lymph node (RL) cells of patients with primary lung cancer exhibited no cytotoxicity to autologous tumor cells in a 4-hr 51Cr-release assay, but when cultured in the presence of Interleukin 2 (IL2), the RL cells did become cytotoxic to those target cells. When RL cells were included in a cytotoxic test of IL2-activated RL cells (autologous killer T cells; AK.T cells) and autologous target cells, the cytotoxicity of the AKT cells was significantly inhibited in 27 out of a total of 42 cases, but this suppression was observed against neither allogeneic effector cells (seven out of nine cases) nor natural killer cells (all seven cases tested). The cytotoxicity of AK.T cells to allogeneic target cells was inhibited by RL cells in three out of six cases. Nylon-wool column separation indicated that the cell population adhering to the nylon wool mediated the sup-pressive effect of the RL cells. These data suggested the presence of nylon-wool-adherent suppressor cells in the regional lymph nodes of patients with primary lung cancer which suppress the cytotoxicity of autologous killer lymphocytes to autologous tumor 《 cells = ，  》《. © < 1987 > Oxford University Press.";
            string from = "auto";
            string to = "zh";
            var from_data = "f.req=" + System.Web.HttpUtility.UrlEncode(
                string.Format("[[[\"MkEWBc\",\"[[\\\"{0}\\\",\\\"{1}\\\",\\\"{2}\\\",true],[null]]\", null, \"generic\"]]]",
                ReplaceString(q), from, to), Encoding.UTF8).Replace("+", "%20");
            byte[] postData = Encoding.UTF8.GetBytes(from_data);
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            client.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
            client.Headers.Add("ContentLength", postData.Length.ToString());
            client.Headers.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/93.0.4577.82 Safari/537.36");

            byte[] responseData = client.UploadData(url, "POST", postData);
            string content = Encoding.UTF8.GetString(responseData);
            Console.WriteLine(responseData);
            Console.WriteLine(content);


            Console.WriteLine(MatchResult(content));
        }

        /// <summary>
        /// 匹配翻译结果
        /// </summary>
        /// <returns></returns>
        public static string MatchResult(string content)
        {
            string result = "";
            string patttern = @",\[\[\\\""(.*?)\\\"",";
            Regex regex = new Regex(patttern);
            MatchCollection matchcollection = regex.Matches(content);
            if (matchcollection.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (Match match in matchcollection)
                {
                    list.Add(match.Groups[1].Value);
                }
                // 或者一行完成转换和声明
                List<string> newList = list.Distinct().ToList();

                result = string.Join(" ", newList.GetRange(1, newList.Count - 1));
                if (result.LastIndexOf(@"\""]]]],\""") > 0)
                {
                    result = result.Substring(0, result.LastIndexOf(@"\""]]]],"));
                }

                result = Regex.Replace(result, @"\\u([0-9A-Fa-f]{4})", match => {
                    string hex = match.Groups[1].Value;
                    int code = int.Parse(hex, NumberStyles.HexNumber);
                    return ((char)code).ToString();
                });

            }
            return result;
        }

        /// <summary>
        ///   替换部分字符串
        /// </summary>
        /// <param name="sPassed">需要替换的字符串</param>
        /// <returns></returns>
        public static string ReplaceString(string JsonString)
        {
            if (JsonString == null) { return JsonString; }
            if (JsonString.Contains("\\"))
            {
                JsonString = JsonString.Replace("\\", "\\\\");
            }
            if (JsonString.Contains("\'"))
            {
                JsonString = JsonString.Replace("\'", "\\\'");
            }
            if (JsonString.Contains("\""))
            {
                JsonString = JsonString.Replace("\"", "\\\\\\\"");
            }
            //去掉字符串的回车换行符
            JsonString = Regex.Replace(JsonString, @"[\n\r]", "");
            JsonString = JsonString.Trim();
            return JsonString;
        }


    }
}