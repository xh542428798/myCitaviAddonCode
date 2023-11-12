using SwissAcademic.Citavi;
using SwissAcademic.Citavi.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShortTitleFilterDemoAddon
{
    class Filter
        :
        IReferenceStringPropertyFilter
    {
        public string GetFilterResult(Reference reference, out bool handled)
        {
            handled = true;
            return "Hello World (" + reference.Title.Substring(0, Math.Min(15, reference.Title.Length)) + "...)";


        }
    }
}
