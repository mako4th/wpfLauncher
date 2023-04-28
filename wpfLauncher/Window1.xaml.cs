using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace wpfLauncher
{
    /// <summary>
    /// Window1.xaml の相互作用ロジック
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();

            this.MouseLeftButtonDown += (sender, e) => { this.DragMove(); };


            /* TabControl tabs = new TabControl();//タブコントロール全体
             TabItem item = new TabItem();//タブのページ自体
             item.Name = "label1";
             item.Header = "labaaaaaaaaaael1";//タブのタイトル
             item.Content = new mainGrid();//タブページのコンテンツ
            tabs.Items.Add(item);//ページ追加
             */

           // AddChild(tabs);
        }
    }
}
