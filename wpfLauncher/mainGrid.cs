using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Microsoft.WindowsAPICodePack.Shell;
using System.Windows.Shapes;
//using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace wpfLauncher
{
    class mainGrid : Grid
    {
        MediaElement mediaElement;
        ContextMenu contextMenu;
        int cellThickness = 2;

        public mainGrid()
        {
            mediaElement = new MediaElement();
            extras ex = new extras();
            Background = Brushes.Navy;

            contextMenu = new ContextMenu();
            MenuItem itemDelete = new MenuItem();
            itemDelete.Header = "Delete";
            itemDelete.Click += new RoutedEventHandler(cellItemDelete);
            contextMenu.Items.Add(itemDelete);

            //グリッド分割数の設定
            int rowsCount = 10;
            int columnsCount = 10;

            //メッセージ、広告エリアの設定　下からwebRows行確保
            int webRows = 2;
            int webColumns = columnsCount / 2;
            int messColumns = columnsCount - webRows;

            //グリッド作成
            for (int i = 0; i < columnsCount; i++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition());
            }

            for (int i = 0; i < rowsCount; i++)
            {
                this.RowDefinitions.Add(new RowDefinition());
            }

            //ランチャーエリアのプレイスイメージの配列を作成
            int rows = rowsCount - webRows;
            exImage[,] exImages = new exImage[rows, columnsCount];

            for (int rowIndex = 0; rowIndex < rows; rowIndex++)
            {
                for (int columnIndex = 0; columnIndex < columnsCount; columnIndex++)
                {
                    exImage img = new exImage();
                    img.Opacity = 0;
                    img.AllowDrop = true;
                    img.Drop += OnDrop;
                    img.Source = ex.GetImageFromAssets("dummy.png");
                    Border b = new Border();
                    b.BorderBrush = new SolidColorBrush(Colors.White);
                    b.BorderThickness = new Thickness(cellThickness);
                    Grid.SetRow(b, rowIndex);
                    Grid.SetColumn(b, columnIndex);
                    b.Child = img;
                    Children.Add(b);
                    exImages[rowIndex, columnIndex] = img;
                }
            }

            //csvファイルからアプリリストを取得
            List<string> appList = ex.lineLoad("Assets/pathList.csv");

            //アプリリストをグリッド内のimgに割り当てる
            foreach (string line in appList)
            {
                string[] fields = line.Split(',');
                int rowIndex = int.Parse(fields[0]);
                int columnIndex = int.Parse(fields[1]);
                string path = fields[2];

                //img.Source = ex.GetImageFromAssets("partially-cloudy.png");
                //img.Source = ex.getImageSourceFromUri(path);
                exImages[rowIndex, columnIndex].Source = ex.getImageSourceFromAPP(path);
                exImages[rowIndex, columnIndex].MouseLeftButtonDown += imageMouseDown;
                exImages[rowIndex, columnIndex].MouseLeftButtonUp += imageMouseUp;
                exImages[rowIndex, columnIndex].Name = "img" + rowIndex.ToString() + columnIndex.ToString();
                exImages[rowIndex, columnIndex].row = rowIndex;
                exImages[rowIndex, columnIndex].col = columnIndex;
                exImages[rowIndex, columnIndex].Margin = new Thickness(cellThickness);
                exImages[rowIndex, columnIndex].appPath = path;
                exImages[rowIndex, columnIndex].Opacity = 1;
                exImages[rowIndex, columnIndex].ContextMenu = contextMenu;
            }

            //メッセージ・広告エリア　下段左側
            Grid.SetRow(mediaElement, 8);
            Grid.SetColumn(mediaElement, 0);
            Grid.SetRowSpan(mediaElement, 2);
            Grid.SetColumnSpan(mediaElement, 5);
            this.Children.Add(mediaElement);
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Visibility = Visibility.Visible;
            mediaElement.Source = new Uri("Assets/preview.mp4", UriKind.Relative);
            mediaElement.ClipToBounds = true;
            mediaElement.StretchDirection = StretchDirection.Both;
            mediaElement.Stretch = Stretch.UniformToFill;
            mediaElement.MediaEnded += Media_Ended;
            mediaElement.Play();

            //メッセージ・広告エリア　下段右側
            WebBrowser web = new WebBrowser();
            web.ClipToBounds = true;
            var axIWebBrowser2 = typeof(WebBrowser).GetProperty("AxIWebBrowser2", BindingFlags.Instance | BindingFlags.NonPublic);
            var comObj = axIWebBrowser2.GetValue(web, null);
            comObj.GetType().InvokeMember("Silent", BindingFlags.SetProperty, null, comObj, new object[] { true });
            Grid.SetRow(web, 8);
            Grid.SetColumn(web, 5);
            Grid.SetRowSpan(web, 2);
            Grid.SetColumnSpan(web, 5);
            this.Children.Add(web);
            Uri uri = new Uri(@"http://www.microsoft.com");
            web.Navigate(uri);
        }
        private void Media_Ended(object sender, EventArgs e)
        {
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }
        private async void imageMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is exImage)
            {
                exImage im = (exImage)sender;
                im.Opacity = 1;

                //MessageBox.Show(im.appPath);
                if (File.Exists(im.appPath))
                {
                    ProcessStartInfo pi = new ProcessStartInfo();
                    pi.FileName = im.appPath;
                    pi.UseShellExecute = true;
                    // pi.Arguments = linkPropaty.Arguments;
                    
                    try
                    {
                        Process.Start(pi);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.ToString());
                    }
                }
            }
        }
        private void imageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is exImage)
            {
                exImage im = (exImage)sender;
                im.Opacity = 0.7;
            }
        }
        public void OnDrop(object sender, DragEventArgs e)
        {
            extras ex = new extras();

            e.Handled= true;
            string? filename = ex.IsSingleFile(e);

            if(filename == null)
            {
                //MessageBox.Show("ファイルは一つだけ選択してください。");
                return;
            }

            string filepath = ex.getOriginPath(filename);

            if (File.Exists(filepath))
            {
                using (var file = ShellFile.FromFilePath(filepath))
                {
                    file.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    exImage im = (exImage)sender;
                    im.Source = file.Thumbnail.BitmapSource;
                    im.appPath = filepath;
                    im.MouseLeftButtonUp += imageMouseUp;
                    im.MouseLeftButtonDown += imageMouseDown;
                    im.Margin = new Thickness(cellThickness);
                    im.Opacity = 1;
                    im.ContextMenu = contextMenu;
                }
            }
        }
        private void cellItemDelete(object sender, RoutedEventArgs e)
        {
            exImage im = (((e.Source as MenuItem).Parent as ContextMenu).PlacementTarget) as exImage;
            extras ex = new extras();
            im.Source = ex.GetImageFromAssets("dummy.png");
            im.appPath = "";
            im.MouseLeftButtonUp -= imageMouseUp;
            im.MouseLeftButtonDown -= imageMouseDown;
            im.Opacity = 0;
            im.ContextMenu = null;
        }
    }
}


