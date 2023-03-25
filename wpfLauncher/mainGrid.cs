using System;
using System.Collections.Generic;
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
//using static Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System;

namespace wpfLauncher
{
    class mainGrid : Grid
    {
        MediaElement mediaElement;

        public mainGrid()
        {
            mediaElement = new MediaElement();

            //グリッド分割数の設定
            int rowsCount = 10;
            int columnsCount = 10;

            //下から何行をメッセージ、広告エリアに設定するか
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

            //グリッドにimgを追加する
            extras ex = new extras();
            for (int r = 0; r < rowsCount - webRows; r++)
            {
                for (int l = 0; l < columnsCount; l++)
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    //img.Source = ex.GetImageFromAssets("partially-cloudy.png");
                    img.Source = ex.getImageSourceFromUri("https://illustimage.com/photo/dl/4976.png");

                    img.MouseDown += imageMouseDown;
                    img.Name = "img" + r.ToString() + l.ToString();
                    img.Margin = new Thickness(1);
                    Grid.SetRow(img, r);
                    Grid.SetColumn(img, l);
                    this.Children.Add(img);
                }
            }
            this.Background = Brushes.Navy;
            //下段左側
            Grid.SetRow(mediaElement, 8);
            Grid.SetColumn(mediaElement, 0);
            Grid.SetRowSpan(mediaElement, 2);
            Grid.SetColumnSpan(mediaElement, 5);
            this.Children.Add(mediaElement);
            mediaElement.LoadedBehavior = MediaState.Manual;
            mediaElement.Visibility = Visibility.Visible;
            mediaElement.Source = new Uri("Assets/nanohana909201.wmv", UriKind.Relative);
            mediaElement.ClipToBounds = true;
            mediaElement.StretchDirection = StretchDirection.Both;
            mediaElement.Stretch = Stretch.UniformToFill;
            mediaElement.MediaEnded += Media_Ended;
            mediaElement.Play();

            //下段右側
            
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

        private void imageMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.Controls.Image)
            {
                System.Windows.Controls.Image im = (System.Windows.Controls.Image)sender;
                MessageBox.Show(im.Name);
            }
        }
    }
}
