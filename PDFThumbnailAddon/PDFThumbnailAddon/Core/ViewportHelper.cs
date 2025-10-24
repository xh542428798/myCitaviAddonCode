using System.Windows.Controls;
using System.Reflection;

namespace PDFThumbnail
{
    public static class ViewportHelper
    {
        public static bool IsInViewport(ScrollViewer sv, Control item)
        {
            if (item is ListBoxItem)
            {
                item = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;

                var scrollContentPresenter = (ScrollContentPresenter)sv.Template.FindName("PART_ScrollContentPresenter", sv);
                var isInViewportMethod = sv.GetType().GetMethod("IsInViewport", BindingFlags.NonPublic | BindingFlags.Instance);
                return (bool)isInViewportMethod.Invoke(sv, new object[] { scrollContentPresenter, item });
            }

            return false;
        }
    }
}
