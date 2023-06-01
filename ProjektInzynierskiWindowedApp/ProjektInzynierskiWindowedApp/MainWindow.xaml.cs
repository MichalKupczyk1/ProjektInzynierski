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
        public RemovalType RemovalType { get; set; } = RemovalType.AMF;
        public double Threshold { get; set; } = 40;

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

            cmb_RemovalType.Items.Add(RemovalType.AMF.ToString());
            cmb_RemovalType.Items.Add(RemovalType.VMF.ToString());
            cmb_RemovalType.Items.Add(RemovalType.WAF.ToString());
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

                detectedNoise = DetectNoise(arrayManager);
                result = RemoveNoise(arrayManager, detectedNoise);
                arrayManager.ExtendedArray = result;

                var bytes = arrayManager.ReturnBytesFrom2DPixelArray();

                if (bytes.Count() > 0)
                    File.WriteAllBytes("result.bmp", bytes);
                WyswietlBitmape("C:\\Users\\Michal\\source\\repos\\ProjektInzynierski\\ProjektInzynierskiWindowedApp\\ProjektInzynierskiWindowedApp\\bin\\Debug\\net6.0-windows\\result.bmp");
            }
        }

        private bool[,] DetectNoise(PixelArrayManager manager)
        {
            if (DetectionType == DetectionType.FAPG)
                return FAPGDetection(manager,Threshold);
            else
                return FASTDetection(manager, Threshold);
        }

        private Pixel[,] RemoveNoise(PixelArrayManager manager, bool[,] detectedNoise)
        {
            if (RemovalType == RemovalType.AMF)
                return AMFRemoval(manager, detectedNoise);
            else if (RemovalType == RemovalType.VMF)
                return VMFRemoval(manager, detectedNoise);
            else
                return WAFRemoval(manager, detectedNoise);
        }

        private Pixel[,] AMFRemoval(PixelArrayManager manager, bool[,] detectedNoise)
        {
            var amf = new AMF();
            amf.Pixels = manager.ExtendedArray;
            amf.Height = manager.ExtendedHeight;
            amf.Width = manager.ExtendedWidth;
            amf.CorruptedPixels = detectedNoise;
            return amf.RemoveNoise();
        }
        private Pixel[,] VMFRemoval(PixelArrayManager manager, bool[,] detectedNoise)
        {
            var vmf = new VMF();
            vmf.Pixels = manager.ExtendedArray;
            vmf.Height = manager.ExtendedHeight;
            vmf.Width = manager.ExtendedWidth;
            vmf.CorruptedPixels = detectedNoise;
            return vmf.RemoveNoise();
        }
        private Pixel[,] WAFRemoval(PixelArrayManager manager, bool[,] detectedNoise)
        {
            var waf = new WAF();
            waf.Pixels = manager.ExtendedArray;
            waf.Height = manager.ExtendedHeight;
            waf.Width = manager.ExtendedWidth;
            waf.CorruptedPixels = detectedNoise;
            return waf.RemoveNoise();
        }
        private bool[,] FASTDetection(PixelArrayManager manager, double threshold)
        {
            var fast = new FAST();
            fast.Width = manager.ExtendedWidth;
            fast.Height = manager.ExtendedHeight;
            fast.Threshold = threshold;
            fast.Pixels = manager.ExtendedArray;
            fast.WindowSize = 9;
            return fast.DetectNoise();
        }

        private bool[,] FAPGDetection(PixelArrayManager manager, double threshold)
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
