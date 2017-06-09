using DPRec_Lib.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
using System.Windows.Threading;

namespace DPRec_Browser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<NamedTypeBase> alltypes = new List<NamedTypeBase>();
        private Dispatcher dp;
        
        public MainWindow()
        {
            InitializeComponent();
            dp = this.Dispatcher;
           

            var searchStream = Observable.FromEventPattern<TextChangedEventHandler, TextChangedEventArgs>(
               h => tbSearch.TextChanged += h,
               h => tbSearch.TextChanged -= h
               ).Select(x => ((TextBox)x.Sender).Text);

            searchStream                
                .Throttle(TimeSpan.FromMilliseconds(100)) 
                .Select(Search)
                .Switch()                                
                .Subscribe(OnSearchResult);

            Search("").Subscribe(OnSearchResult);

            

        }

        private void OnSearchResult(List<NamedTypeBase> list)
        {
            this.dp.Invoke(() => FillTreeView(list));
        }


        public IObservable<List<NamedTypeBase>> Search(string filter)
        {
            var filteredList = this.alltypes.Where(t => t.Name.ToLower().StartsWith(filter.ToLower()) || t.ToString().ToLower().StartsWith(filter.ToLower()));
            return Observable.Return(filteredList.ToList());
        }

        private void FillTreeView(IEnumerable<NamedTypeBase> elements)
        {
            tv1.Items.Clear();
            foreach (var item in elements)
            {
                TreeViewItem tvi = new TreeViewItem();
                tvi.DataContext = item;
                tvi.Header = item.ToString();

                TreeViewItem tvifield = new TreeViewItem();
                tvifield.Header = "Fields";
                tvifield.DataContext = null;

                TreeViewItem tvimethod = new TreeViewItem();
                tvimethod.Header = "Methods";
                tvimethod.DataContext = null;


                foreach (var field in item.Fields.OrderBy(x => x.Name))
                {
                    var tviCurrent = new TreeViewItem();
                    tviCurrent.Header = field.Name;
                    tviCurrent.DataContext = field;
                    tvifield.Items.Add(tviCurrent);
                }

                foreach (var method in item.Methods.OrderBy(x => x.Name))
                {
                    var tviCurrent = new TreeViewItem();
                    tviCurrent.Header = method.Name;
                    tviCurrent.DataContext = method;
                    tvimethod.Items.Add(tviCurrent);
                }

                tvi.Items.Add(tvimethod);
                tvi.Items.Add(tvifield);

                tv1.Items.Add(tvi);
            }
        }

        private void tv1_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            contentGrid.Children.Clear();
            if ((TreeViewItem)((TreeView)e.Source).SelectedItem==null)
            {
                contentGrid.Children.Clear();
                return;
            }
            var dc = ((TreeViewItem)((TreeView)e.Source).SelectedItem).DataContext;            
            if (dc is MethodBase)
            {                
                contentGrid.Children.Add(new MethodView() { DataContext = dc });
            }
            else if (dc is NamedTypeBase)
            {                
                contentGrid.Children.Add(new TypeView() { DataContext = dc });
            }
            else if (dc is FieldBase)
            {             
                contentGrid.Children.Add(new FieldView() { DataContext = dc });
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Data Sources |*.bin|All Files|*.*";
            var res = dialog.ShowDialog();
            if (res.HasValue && res.Value)
            {
                using (var f = File.OpenRead(dialog.FileName))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    alltypes = ((List<NamedTypeBase>)bf.Deserialize(f)).OrderBy(m => m.ToString()).ToList();
                    FillTreeView(alltypes);
                }
            }
        }
    }
}
