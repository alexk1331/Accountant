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

namespace Accountant
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainWindowViewModel mv=new MainWindowViewModel();

        public MainWindow()
        {
            DataContext = mv;
            InitializeComponent();
            mv = new MainWindowViewModel(titlegrid, mainttabcc, DateTime.Now);
            DataContext = mv;

        }

        private void eventslist_Click(object sender, RoutedEventArgs e)
        {
            mv.eventlistdraw();

        }
        private void calendar_Click(object sender, RoutedEventArgs e)
        {
            
            mv.monthdaraw();
        }
        private void statlist_Click(object sender, RoutedEventArgs e)
        {
            mv.statisticdraw();
        }
    }
}
