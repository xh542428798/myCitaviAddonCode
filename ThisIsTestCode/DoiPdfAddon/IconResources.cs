// Decompiled with JetBrains decompiler
// Type: DoiPdfAddon.IconResources
// Assembly: DoiPdfAddon, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 0C5CE0D2-E537-4326-8556-278B9840211C
// Assembly location: E:\OneDrive - 中山大学\Appdata_my\各种软件Setting样式\Citavi重复\Addons\DoiPdfAddon.dll

using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace DoiPdfAddon
{
  [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
  [DebuggerNonUserCode]
  [CompilerGenerated]
  internal class IconResources
  {
    private static ResourceManager resourceMan;
    private static CultureInfo resourceCulture;

    internal IconResources()
    {
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static ResourceManager ResourceManager
    {
      get
      {
        if (IconResources.resourceMan == null)
          IconResources.resourceMan = new ResourceManager("DoiPdfAddon.IconResources", typeof (IconResources).Assembly);
        return IconResources.resourceMan;
      }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    internal static CultureInfo Culture
    {
      get => IconResources.resourceCulture;
      set => IconResources.resourceCulture = value;
    }

    internal static Bitmap arxiv => (Bitmap) IconResources.ResourceManager.GetObject(nameof (arxiv), IconResources.resourceCulture);

    internal static Bitmap doi => (Bitmap) IconResources.ResourceManager.GetObject(nameof (doi), IconResources.resourceCulture);

    internal static Bitmap download => (Bitmap) IconResources.ResourceManager.GetObject(nameof (download), IconResources.resourceCulture);
  }
}
