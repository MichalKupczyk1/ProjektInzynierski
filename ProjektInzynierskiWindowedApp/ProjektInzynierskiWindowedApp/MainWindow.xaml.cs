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
                var pixels = arrayManager.Pixels;
                var detectedNoise = new bool[0, 0];
                var result = new Pixel[0, 0];

                if (DetectionType == DetectionType.FAST)
                {
                    var fast = new FAST();
                    fast.Pixels = arrayManager.Pixels;
                    fast.Width = arrayManager.Width;
                    fast.Height = arrayManager.Height;
                    //suggested value for FAST detection
                    fast.Threshold = 60;
                    detectedNoise = fast.DetectNoise();
                }
                else
                {
                    var fapg = new FAPG();
                    fapg.Pixels = arrayManager.Pixels;
                    fapg.Width = arrayManager.Width;
                    fapg.Height = arrayManager.Height;
                    //suggested value for FAST detection
                    fapg.Threshold = 60;
                    detectedNoise = fapg.DetectNoise();
                }
                if (RemovalType == RemovalType.Mean)
                {
                    var mean = new MeanRemoval();
                    mean.DetectedNoise = detectedNoise;
                    mean.Pixels = pixels;
                    mean.Width = arrayManager.Width;
                    mean.Height = arrayManager.Height;

                    result = mean.RemoveNoise();
                }
                else
                {
                    var sum = new SumRemoval();
                    sum.DetectedNoise = detectedNoise;
                    sum.Pixels = pixels;
                    sum.Width = arrayManager.Width;
                    sum.Height = arrayManager.Height;
                    sum.Threshold = 300;

                    result = sum.RemoveNoise();
                }
                var oneDimPixelArray = arrayManager.ConvertFrom2DArray(result);
                var imageCopy = Image;
                arrayManager.PixelToByteArray(ref imageCopy, oneDimPixelArray, arrayManager.Width, arrayManager.Amount, arrayManager.Step);
                if (imageCopy.Count() > 0)
                    Image = imageCopy;

                File.WriteAllBytes("result.bmp", Image);
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
