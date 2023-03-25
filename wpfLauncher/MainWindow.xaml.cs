using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Interop;
using Microsoft.WindowsAPICodePack.Shell;
using System.Security;

namespace wpfLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        string filepath = "";

        public MainWindow()
        {
            InitializeComponent();
  




            extras ex = new extras();
            ImageSource iSource = ex.GetImageFromAssets("notFound.jpg");

            //iSource = ex.getImageSourceFromResources("partially-cloudy.png");
            // iSource = ex.GetImageFromAssets("partially-cloudy.png");

           // Im1.Source = iSource;

        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (filenames.Count() > 1)
            {
                MessageBox.Show("ファイルは一つだけ選択してください。");
            }
            extras ex = new extras();
            filepath = ex.getOriginPath(filenames[0]);

            if (File.Exists(filepath))
            {
                using (var file = ShellFile.FromFilePath(filepath))
                {
                    file.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    Im1.Source = file.Thumbnail.BitmapSource;
                }
            }
        }

        private void Im1_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (File.Exists(filepath))
            {
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = filepath;
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
}
