using pdftron.PDF;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace PDFThumbnail
{

    public partial class ThumbnailsControl : UserControl
    {
        #region Constants

        private const int THUMBNAIL_WIDTH = 150;
        private const int THUMBNAIL_HEIGHT = 150;

        #endregion

        #region Fields

        private PDFViewWPF _viewer;
        private PDFDoc _doc;
        private bool _isSelectionChangeProgrammatic = false;
        private readonly List<int> _visiblePageIndexes = new List<int>();

        #endregion

        #region Constructors

        public ThumbnailsControl(PDFViewWPF viewer)
        {
            InitializeComponent();
            _viewer = viewer;
            InitializeViewer();
            Refresh();
        }

        #endregion

        #region Methods

        void InitializeViewer()
        {
            _viewer.CurrentPageNumberChanged += Viewer_CurrentPageNumberChanged;
            _viewer.OnThumbnailGenerated += Viewer_OnThumbnailGenerated;
        }

        public void Refresh()
        {
            Clear();

            if (_viewer == null) return;
            _doc = _viewer.GetDoc();
            if (_doc == null) return;

            PopulateThumbnailList();
            ItemsInViewHitTest();
        }

        public void Clear()
        {
            ThumbnailListBox.Items.Clear();
            _visiblePageIndexes.Clear();
            _viewer.CancelAllThumbRequests();
        }

        void PopulateThumbnailList()
        {
            for (int i = 0; i < _viewer.GetPageCount(); ++i)
            {
                var img = new Image()
                {
                    Margin = new Thickness(0, 0, 0, 0),
                    Stretch = Stretch.UniformToFill,
                };
                var border = new Border()
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Child = img,
                };
                var viewBox = new Viewbox()
                {
                    Margin = new Thickness(0),
                    Stretch = Stretch.Uniform,
                    Width = THUMBNAIL_WIDTH,
                    Height = THUMBNAIL_HEIGHT,
                    Child = border,
                };
                var text = new TextBlock()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Text = GetLabel(i),
                    Foreground = Brushes.Black,
                };
                var panel = new StackPanel();
                panel.Children.Add(viewBox);
                panel.Children.Add(text);

                var item = new ListBoxItem
                {
                    Content = panel,
                    Tag = i
                };

                ThumbnailListBox.Items.Add(item);
            }

            UpdateSelectedIndex();
        }

        string GetLabel(int pageNumber)
        {
            var pageLabel = _viewer.GetDoc().GetPageLabel(pageNumber+1);

            if (pageLabel == null || !pageLabel.IsValid()) return (pageNumber + 1).ToString();
            var labelTitle = pageLabel.GetLabelTitle(pageNumber+1);
            if(string.IsNullOrEmpty(labelTitle))return (pageNumber + 1).ToString();
            return $"{(pageNumber + 1).ToString()} ({labelTitle})";
        }

        void ItemsInViewHitTest()
        {
            try
            {
                var sv = GetScrollViewer();
                if (sv == null) return;
                if (VisualTreeHelper.GetParent(this) == null)
                    return;
                var pagesOnScreen = new List<int>();

                for (int i = 0; i < ThumbnailListBox.Items.Count; ++i)
                {
                    var item = ThumbnailListBox.Items[i] as ListBoxItem;
                    var listboxitem = ThumbnailListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                    var img = GetImageElement(item);
                    if (ViewportHelper.IsInViewport(sv, listboxitem))
                    {
                        if (img.Source == null && !_visiblePageIndexes.Contains(i))
                        {
                            _visiblePageIndexes.Add(i);
                            _viewer.GetThumbAsync(i + 1);
                        }
                    }
                    else
                    {
                        if (_visiblePageIndexes.Contains(i))
                        {
                            var image = GetImageElement(ThumbnailListBox.Items[i] as ListBoxItem);
                            if (image.Source != null)
                            {
                                image.Source = null;
                                (image.Parent as Border).BorderBrush = Brushes.Transparent;
                            }
                            _visiblePageIndexes.Remove(i);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }


        void UpdateSelectedIndex()
        {
            if (ThumbnailListBox.Items.Count == 0)
                ThumbnailListBox.SelectedIndex = -1;
            else if (ThumbnailListBox.Items.Count < _viewer.GetCurrentPage())
                ThumbnailListBox.SelectedIndex = 0;
            else if (ThumbnailListBox.SelectedIndex != _viewer.GetCurrentPage() - 1)
                ThumbnailListBox.SelectedIndex = _viewer.GetCurrentPage() - 1;

            ThumbnailListBox.ScrollIntoView(ThumbnailListBox.SelectedItem);
            ItemsInViewHitTest();
        }


        Image GetImageElement(ListBoxItem item) => (((item.Content as StackPanel).Children[0] as Viewbox)?.Child as Border)?.Child as Image;


        ScrollViewer GetScrollViewer() => ThumbnailListBox.Template.FindName("PART_scrollviewer", ThumbnailListBox) as ScrollViewer;

        #endregion

        #region EventHandlers

        void ThumbnailListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var item = e.AddedItems[0] as ListBoxItem;
                ThumbnailListBox.ScrollIntoView(ThumbnailListBox.SelectedItem);
                if (!_isSelectionChangeProgrammatic)
                {
                    _viewer.SetCurrentPage((int)item.Tag + 1);
                }
            }
        }

        void ThumbnailScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e) => ItemsInViewHitTest();

        void Viewer_CurrentPageNumberChanged(PDFViewWPF viewer, int currentPage, int totalPages)
        {
            _isSelectionChangeProgrammatic = true;
            UpdateSelectedIndex();
            _isSelectionChangeProgrammatic = false;
        }

        void Viewer_OnThumbnailGenerated(int pageNumber, byte[] thumb, int w, int h)
        {
            try
            {
                if (!ThumbnailListBox.Items.IsEmpty)
                {
                    var sv = GetScrollViewer();
                    var index = pageNumber - 1;
                    var item = ThumbnailListBox.Items[index] as ListBoxItem;
                    var listboxitem = ThumbnailListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    if (ViewportHelper.IsInViewport(sv, listboxitem))
                    {
                        var fmt = PixelFormats.Bgra32;
                        var bps = BitmapSource.Create(w, h, 96.0, 96.0, fmt, null, thumb, (w * fmt.BitsPerPixel + 7) / 8);

                        var image = GetImageElement(ThumbnailListBox.Items[index] as ListBoxItem);
                        image.Source = bps;

                        (image.Parent as Border).BorderThickness = new Thickness(1, 1, 4, 3);
                    }
                }
            }
            catch (Exception)
            {
            }

        }

        #endregion
    }
}
