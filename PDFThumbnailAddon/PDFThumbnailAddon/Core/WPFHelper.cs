using System.Windows;
using System.Windows.Media;

namespace PDFThumbnail
{
    public static class WPFHelper
    {
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            // confirm parent is valid.
            if (parent == null) return null;
            if (parent is T) return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement frameworkElement) frameworkElement.ApplyTemplate();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }
    }
}
