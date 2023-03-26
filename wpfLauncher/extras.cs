using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Shell;

namespace wpfLauncher
{
    class extras
    {
        public ImageSource getImageSourceFromAPP(string path)
        {
            ImageSource result = GetImageFromAssets("notFound.jpg");
            if (File.Exists(path))
            {
                using (var file = ShellFile.FromFilePath(path))
                {
                    file.Thumbnail.FormatOption = ShellThumbnailFormatOption.IconOnly;
                    result = file.Thumbnail.BitmapSource;
                }
            }

            return result;
        }

        public ImageSource getImageSourceFromUri(string uriString)
        {
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(uriString);
            bitmapImage.EndInit();

            return bitmapImage;
        }
        public ImageSource GetImageFromAssets(string filename)
        {
            string readPath = "Assets/" + filename;
            BitmapImage bpim = new BitmapImage();
            using (FileStream st = File.OpenRead(readPath))
            {
                bpim.BeginInit();
                bpim.StreamSource = st;
                bpim.DecodePixelWidth = 500;
                bpim.CacheOption = BitmapCacheOption.OnLoad;
                bpim.CreateOptions = BitmapCreateOptions.None;
                bpim.EndInit();
                bpim.Freeze();
            }
            return bpim;
        }

        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        public ImageSource getImageSourceFromResources(string filename)
        {

            IntPtr handle = Properties.Resources.partially_cloudy.GetHbitmap();
            ImageSource result = null;

            try
            {
                result = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());

            }
            finally
            {
                DeleteObject(handle);
            }

            return result;
        }

        private string getLinkTargetPath(string path)
        {
            string result = "";
            dynamic? shell = null;
            dynamic? lnk = null;

            try
            {
                var type = Type.GetTypeFromProgID("WScript.Shell");
                shell = Activator.CreateInstance(type);
                lnk = shell.CreateShortcut(path);


                if (string.IsNullOrEmpty(lnk.TargetPath))
                {
                    MessageBox.Show("リンク先が見つかりませんでした");
                }

                var linkPropaty = new
                {
                    lnk.Arguments,
                    lnk.Description,
                    lnk.FullName,
                    lnk.Hotkey,
                    lnk.IconLocation,
                    lnk.TargetPath,
                    lnk.WindowStyle,
                    lnk.WorkingDirectory
                };

                result = lnk.TargetPath;
            }
            finally
            {
                if (lnk != null) Marshal.ReleaseComObject(lnk);
                if (shell != null) Marshal.ReleaseComObject(shell);
            }
            return result;
        }

        public string getOriginPath(string path)
        {

            if (System.IO.Path.GetExtension(path) == ".lnk")
            {
                return getLinkTargetPath(path);
            }
            else
            {
                return path;
            }

        }

        public List<string> lineLoad(string path)
        {
            List<string> result = new List<string>();
            using(StreamReader sr = new StreamReader(path))
            {
                while(sr.Peek() != -1)
                {
                    string line = sr.ReadLine();
                    if (line == null)
                    {
                        continue;
                    }
                    else
                    {
                        result.Add(line);
                    }
                }
            }
            return result;
        }

        /*
                    var icon = System.Drawing.Icon.ExtractAssociatedIcon(filepath);
                    var bitmap = icon.ToBitmap();
                    using (var ms = new System.IO.MemoryStream())
                    {
                        // MemoryStreamに書き出す
                        bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                        // MemoryStreamをシーク
                        ms.Seek(0, System.IO.SeekOrigin.Begin);
                        // MemoryStreamからBitmapFrameを作成
                        // (BitmapFrameはBitmapSourceを継承しているのでそのまま渡せばOK)
                        System.Windows.Media.Imaging.BitmapSource bitmapSource =
                            System.Windows.Media.Imaging.BitmapFrame.Create(
                                ms,
                                System.Windows.Media.Imaging.BitmapCreateOptions.None,
                                System.Windows.Media.Imaging.BitmapCacheOption.OnLoad
                            );
                    }

        */
    }
}
