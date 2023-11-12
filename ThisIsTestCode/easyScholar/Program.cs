using System;
using System.Net;
using Newtonsoft.Json;


namespace easyScholar
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string secretKey = "bd6104d14938446ba7f7767bf719e162";
            string publicationName = "Journal of Cancer";

            // 构造请求 URL
            string apiUrl = "https://www.easyscholar.cc/open/getPublicationRank";
            string url = $"{apiUrl}?secretKey={secretKey}&publicationName={Uri.EscapeDataString(publicationName)}";

            try
            {
                
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.UTF8;
                    string response = client.DownloadString(url);

                    Console.WriteLine(response);

                    // 解析 JSON
                    dynamic result = JsonConvert.DeserializeObject(response);

                    int code = result.code;
                    string msg = result.msg;
                    dynamic data = result.data;

                    Console.WriteLine("code: " + code);
                    Console.WriteLine("msg: " + msg);

                    dynamic customRank = data.customRank;
                    dynamic officialRank = data.officialRank;

                    dynamic rankInfo = customRank.rankInfo;
                    foreach (var item in rankInfo)
                    {
                        string uuid = item.uuid;
                        string abbName = item.abbName;
                        string oneRankText = item.oneRankText;

                        Console.WriteLine("uuid: " + uuid);
                        Console.WriteLine("abbName: " + abbName);
                        Console.WriteLine("oneRankText: " + oneRankText);
                    }

                    dynamic allRank = officialRank.all;
                    string xju = allRank.xju;
                    string cug = allRank.cug;
                    string sciif = allRank.sciif;
                    string sci = allRank.sci;
                    string sciBase = allRank.sciBase;
                    string sciUp = allRank.sciUp;
                    string scu = allRank.scu;
                    string sciif5 = allRank.sciif5;
                    string jci = allRank.jci;

                    Console.WriteLine("xju: " + xju);
                    Console.WriteLine("cug: " + cug);
                    Console.WriteLine("sciif: " + sciif);
                    Console.WriteLine("sci: " + sci);
                    Console.WriteLine("sciBase: " + sciBase);
                    Console.WriteLine("sciUp: " + sciUp);
                    Console.WriteLine("scu: " + scu);
                    Console.WriteLine("sciif5: " + sciif5);
                    Console.WriteLine("jci: " + jci);

                    Console.ReadKey();
                }
            }
            catch (WebException ex)
            {
                Console.WriteLine($"请求错误：{ex.Message}");
                Console.ReadKey();
            }




        }

    }

}


