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

namespace ProjektInzynierskiWindowedApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private byte[] Image { get; set; }
        public DetectionType DetectionType { get; set; } = DetectionType.FAST;
        public RemovalType RemovalType { get; set; } = RemovalType.Mean;
        public MainWindow()
        {
            InitializeComponent();
            InitializeDefaultValues();
        }

        private void InitializeDefaultValues()
        {
            cmb_DetectionType.Items.Add(DetectionType.FAST.ToString());
            cmb_DetectionType.Items.Add(DetectionType.FAPG.ToString());
            cmb_DetectionType.SelectedIndex = 0;

            cmb_RemovalType.Items.Add(RemovalType.Mean.ToString());
            cmb_RemovalType.Items.Add(RemovalType.Sum.ToString());
            cmb_RemovalType.SelectedIndex = 0;
        }

        private void btn_zaladujBitmape_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "bitmap (*bmp)|*.bmp|All files (*.*)|*.*";
            byte[] bytes = new byte[0];

            if (openFileDialog.ShowDialog() == true)
            {
                var filePath = openFileDialog.FileName;
                bytes = File.ReadAllBytes(filePath);
            }
            if (bytes.Count() > 0)
                Image = bytes;
        }

        private void btn_usunSzum_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Count() > 0)
            {
                var arrayManager = new PixelArrayManager(Image);
                var result = new Pixel[0, 0];

                var sum = new SumRemoval();
                sum.Pixels = arrayManager.ExtendedArray;
                sum.Width = arrayManager.ExtendedWidth;
                sum.Height = arrayManager.ExtendedHeight;
                sum.Threshold = 70;

                result = sum.RemoveNoise();
                arrayManager.ExtendedArray = result;

                var bytes = arrayManager.ReturnBytesFrom2DPixelArray();

                if (bytes.Count() > 0)
                    File.WriteAllBytes("result.bmp", bytes);
            }
        }

        private void cmb_DetectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DetectionType = (DetectionType)cmb_DetectionType.SelectedIndex;
        }

        private void cmb_RemovalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RemovalType = (RemovalType)cmb_DetectionType.SelectedIndex;
        }
    }
}
