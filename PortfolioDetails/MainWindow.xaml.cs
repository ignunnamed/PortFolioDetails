using System;
using System.Collections.Generic;
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
using CsvReader;
using System.Threading;
using System.IO;
using StockPortFolio;
using Microsoft.Win32;

namespace PortfolioDetails
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }


        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Reader rd = new Reader(selectedFileNameTxtBox.Text);
            var ss = rd.GetData();
            var portFolio = new stockHoldingDetails(userNameTextBox.Text, ss);
            portFolio.BuildPortFolio();
            dataGrid.ItemsSource = portFolio.StockProtfolio;
            MessageBox.Show(portFolio.UserName +" Portfolio XIRR is " + portFolio.Xirr);
        }


        private void selectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialogBox = new OpenFileDialog();
            if (dialogBox.ShowDialog() == true)
            {
                FileInfo fi = new FileInfo(dialogBox.FileName);
                selectedFileNameTxtBox.Text = fi.FullName;
            }
        }
    }


}
