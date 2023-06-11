using Microsoft.Win32;
using ProjektInzynierskiWindowedApp.Logic.NoiseDetection;
using ProjektInzynierskiWindowedApp.Logic.NoiseRemoval;
using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using ProjektInzynierskiWindowedApp.Structures.Enums;
using System;
using System.Collections.Generic;
using System.IO;
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
using static System.Net.Mime.MediaTypeNames;
using Application = System.Windows.Application;

namespace ProjektInzynierskiWindowedApp
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

        private void btn_prezentacjaDzialania_Click(object sender, RoutedEventArgs e)
        {
            PresentationWindow window = new PresentationWindow();
            window.Show();
            this.Close();
        }

        private void btn_wykonajObliczenia_Click(object sender, RoutedEventArgs e)
        {
            Calculations calculationWindow = new Calculations();
            calculationWindow.Show();
            this.Close();
        }
    }
}
