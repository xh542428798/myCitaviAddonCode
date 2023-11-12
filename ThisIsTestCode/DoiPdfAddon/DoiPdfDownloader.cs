// Decompiled with JetBrains decompiler
// Type: DoiPdfAddon.DoiPdfDownloader
// Assembly: DoiPdfAddon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0C5CE0D2-E537-4326-8556-278B9840211C
// Assembly location: E:\OneDrive - 中山大学\Appdata_my\各种软件Setting样式\Citavi重复\Addons\DoiPdfAddon.dll

using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace DoiPdfAddon
{
  internal class DoiPdfDownloader
  {
    public static string CommandKey = "DoiPdfDownloader.Command";
    public static string CommandName = "With doi";

    public static void run(MainForm mainForm, Reference reference, bool silent = false)
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      int flag = 0;
      string[] errorMsg = new string[4]
      {
        "",
        "The reference has no DOI.",
        "Bad pdf url.",
        "Error happened."
      };
      GenericProgressDialog.RunAction((Form) null, (Action<CancellationToken>) (c =>
      {
        Thread thread = new Thread((ThreadStart) (() => flag = DoiPdfDownloader.run_stage2(reference)));
        thread.Start();
        do
        {
          if (flag != 0)
            goto label_1;
        }
        while (!c.IsCancellationRequested);
        goto label_4;
label_1:
        if (flag <= 0 || silent)
          return;
        int num;
        ((Control) mainForm).Invoke((Delegate) (() => num = (int) CitaviMessageBox.Show((Form) mainForm, errorMsg[flag], "Error", MessageBoxIcon.Hand, true)));
        return;
label_4:
        thread.Abort();
      }), "Download in progress", new int?(1), cancellationTokenSource);
    }

    private static int run_stage2(Reference reference)
    {
      try
      {
        string doi = reference.Doi;
        if (string.IsNullOrEmpty(doi))
          return 1;
        string pdfUrl = new Scihub().GetPdfUrl(doi);
        if (string.IsNullOrEmpty(pdfUrl))
          return 2;
        WebClient webClient = new WebClient();
        webClient.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
        string str = Path.GetTempPath() + "tmp.pdf";
        webClient.DownloadFile(new Uri(pdfUrl), str);
        if (System.IO.File.Exists(str) && new FileInfo(str).Length > 100L)
          ((CitaviEntityCollection<Location>) reference.Locations).Add(new Location(reference, (LocationType) 0, (string) null, "")
          {
            Address = new LinkedResource(str, (AttachmentAction) 2, (AttachmentNaming) 2)
          });
      }
      catch
      {
        return 3;
      }
      return -1;
    }
  }
}
