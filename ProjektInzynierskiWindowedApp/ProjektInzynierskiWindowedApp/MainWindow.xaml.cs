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
        private byte[] Image { get; set; }
        public DetectionType DetectionType { get; set; } = DetectionType.FAST;
        public RemovalType RemovalType { get; set; } = RemovalType.Mean;
        public int Iters { get; set; } = 5;
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
            cmb_RemovalType.SelectedIndex = 0;
            Iters = 5;
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
                WyswietlBitmape(filePath);
            }
            if (bytes.Count() > 0)
                Image = bytes;
        }

        private void WyswietlBitmape(string path)
        {
            ImageSource imageSource = new BitmapImage(new Uri(path));
            this.img_Image.Source = imageSource;
            this.img_Image.Stretch = Stretch.None;
        }

        private void btn_usunSzum_Click(object sender, RoutedEventArgs e)
        {
            if (Image.Count() > 0)
            {
                var arrayManager = new PixelArrayManager(Image);
                var result = new Pixel[0, 0];
                var detectedNoise = new bool[0, 0];

                for (int i = 0; i < Iters; i++)
                {
                    detectedNoise = DetectNoise(arrayManager, i);
                    result = RemoveNoise(arrayManager, detectedNoise);

                    arrayManager.ExtendedArray = result;
                }
                var bytes = arrayManager.ReturnBytesFrom2DPixelArray();

                if (bytes.Count() > 0)
                    File.WriteAllBytes("result.bmp", bytes);
                WyswietlBitmape("C:\\Users\\Michal\\source\\repos\\ProjektInzynierski\\ProjektInzynierskiWindowedApp\\ProjektInzynierskiWindowedApp\\bin\\Debug\\net6.0-windows\\result.bmp");
            }
        }

        private bool[,] DetectNoise(PixelArrayManager manager, int iteration)
        {
            if (DetectionType == DetectionType.FAPG)
                return FAPGDetection(manager, 40 + (10 * (iteration - 1)));
            else
                return FASTDetection(manager, 40);
        }

        private Pixel[,] RemoveNoise(PixelArrayManager manager, bool[,] detectedNoise)
        {
            return MeanRemoval(manager, detectedNoise);
        }

        private Pixel[,] MeanRemoval(PixelArrayManager manager, bool[,] detectedNoise)
        {
            var mean = new MeanRemoval();
            mean.Pixels = manager.ExtendedArray;
            mean.Height = manager.ExtendedHeight;
            mean.Width = manager.ExtendedWidth;
            mean.DetectedNoise = detectedNoise;
            return mean.RemoveNoise();
        }
        private bool[,] FASTDetection(PixelArrayManager manager, int threshold)
        {
            var fast = new FAST();
            fast.Width = manager.ExtendedWidth;
            fast.Height = manager.ExtendedHeight;
            fast.Threshold = threshold;
            fast.Pixels = manager.ExtendedArray;
            fast.WindowSize = 9;
            return fast.DetectNoise();
        }

        private bool[,] FAPGDetection(PixelArrayManager manager, int threshold)
        {
            var fapg = new FAPG();
            fapg.Width = manager.ExtendedWidth;
            fapg.Height = manager.ExtendedHeight;
            fapg.Threshold = threshold;
            fapg.Pixels = manager.ExtendedArray;
            fapg.WindowSize = 9;
            return fapg.DetectNoise();
        }

        private void cmb_DetectionType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.DetectionType = (DetectionType)cmb_DetectionType.SelectedIndex;
        }

        private void cmb_RemovalType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.RemovalType = (RemovalType)cmb_RemovalType.SelectedIndex;
        }
    }
}
