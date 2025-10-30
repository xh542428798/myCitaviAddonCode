using System.Windows;
using System.Windows.Media;

namespace PDFBookmarkExpandCollapse
{
    public static class WPFHelper
    {
        /// <summary>
        /// 查找父元素下的第一个指定类型的子元素（原版，无名称）
        /// </summary>
        public static T FindChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            if (parent is T) return parent as T;

            DependencyObject foundChild = null;

            if (parent is FrameworkElement frameworkElement) frameworkElement.ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                foundChild = FindChild<T>(child);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }

        /// <summary>
        /// 查找父元素下的第一个指定类型和名称的子元素（新增的重载版本）
        /// </summary>
        public static T FindChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;

            // 检查父元素本身是否符合条件
            if (parent is T && ((FrameworkElement)parent).Name == childName)
                return (T)parent;

            DependencyObject foundChild = null;

            // 如果父元素是FrameworkElement，应用其模板
            if (parent is FrameworkElement frameworkElement)
                frameworkElement.ApplyTemplate();

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 递归查找
                foundChild = FindChild<T>(child, childName);
                if (foundChild != null) break;
            }

            return foundChild as T;
        }
    }
}
