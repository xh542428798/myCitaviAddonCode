// Decompiled with JetBrains decompiler
// Type: DoiPdfAddon.Scihub
// Assembly: DoiPdfAddon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0C5CE0D2-E537-4326-8556-278B9840211C
// Assembly location: E:\OneDrive - 中山大学\Appdata_my\各种软件Setting样式\Citavi重复\Addons\DoiPdfAddon.dll

using System.Collections.Generic;
using System.Net;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Filters;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;

namespace DoiPdfAddon
{
  internal class Scihub
  {
    private List<string> scihubUrls;

    public Scihub() => this.scihubUrls = Scihub.GetSciHubUrls();

    private static List<string> GetSciHubUrls()
    {
      List<string> sciHubUrls = new List<string>();
      sciHubUrls.Add("https://www.sci-hub.shop");
      string html = Scihub.GetHtml("http://tool.yovisun.com/scihub/");
      if (!string.IsNullOrEmpty(html))
      {
        NodeList nodeList = new Parser(new Lexer(html)).Parse((NodeFilter) new TagNameFilter("a"));
        for (int idx = 0; idx < nodeList.Count; ++idx)
        {
          string attribute = (nodeList[idx] as ITag).GetAttribute("href");
          if (attribute.Contains("https://sci-hub"))
            sciHubUrls.Add(attribute);
        }
      }
      return sciHubUrls;
    }

    private static string GetHtml(string url)
    {
      try
      {
        WebClient webClient = new WebClient();
        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        return webClient.DownloadString(url);
      }
      catch
      {
        return "";
      }
    }

    public string GetPdfUrl(string doi)
    {
      foreach (string scihubUrl in this.scihubUrls)
      {
        string html = Scihub.GetHtml(scihubUrl + "/" + doi);
        if (!string.IsNullOrEmpty(html))
        {
          NodeList nodeList = new Parser(new Lexer(html)).Parse((NodeFilter) new TagNameFilter("iframe"));
          for (int idx = 0; idx < nodeList.Count; ++idx)
          {
            try
            {
              ITag tag = nodeList[idx] as ITag;
              if (!(tag.GetAttribute("id") != "pdf"))
              {
                string pdfUrl = tag.GetAttribute("src");
                if (!string.IsNullOrEmpty(html))
                {
                  if (pdfUrl.StartsWith("//"))
                    pdfUrl = "https:" + pdfUrl;
                  return pdfUrl;
                }
              }
            }
            catch
            {
            }
          }
        }
      }
      return "";
    }
  }
}
