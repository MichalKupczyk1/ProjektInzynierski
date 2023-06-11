using ProjektInzynierskiWindowedApp.Managers;
using ProjektInzynierskiWindowedApp.Utils;
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
using System.Windows.Shapes;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows.Controls.Primitives;

namespace ProjektInzynierskiWindowedApp
{
    /// <summary>
    /// Interaction logic for Calculations.xaml
    /// </summary>
    public partial class Calculations : Window
    {
        public double NoiseLevel { get; set; } = 0.2;
        public double FASTThreshold { get; set; } = 40;
        public double FAPGThreshold { get; set; } = 40;
        public Calculations()
        {
            InitializeComponent();
        }

        private void btn_originalPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName + "\\";
                this.txt_originalPath.Text = path;
                FileUtils.OriginalImagesPath = path;
            }
        }

        private void btn_noisyPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName + "\\";
                this.txt_noisyPath.Text = path;
                FileUtils.CorruptedImagesPath = path;
            }
        }

        private void btn_resultPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName + "\\";
                this.txt_resultsPath.Text = path;
                FileUtils.ResultsPath = path;
            }

        }

        private void btn_outputPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var path = dialog.FileName + "\\";
                this.txt_outputPath.Text = path;
                FileUtils.OutputMainFolderPath = path;
            }
        }

        private void txt_fapg_TextChanged(object sender, TextChangedEventArgs e)
        {
            var val = 0.0;
            if (this.txt_fapg != null)
            {
                this.txt_fapg.Text.Replace('.', ',');
                if (Double.TryParse(this.txt_fapg.Text, out val))
                {
                    val = Convert.ToDouble(this.txt_fapg.Text);
                    if (val <= 0)
                        val = 0.1;
                    this.FAPGThreshold = val;
                }
            }
        }

        private void txt_fast_TextChanged(object sender, TextChangedEventArgs e)
        {
            var val = 0.0;
            if (this.txt_fast != null)
            {
                this.txt_fast.Text.Replace('.', ',');

                if (Double.TryParse(this.txt_fast.Text, out val))
                {
                    val = Convert.ToDouble(this.txt_fast.Text);
                    if (val <= 0)
                        val = 0.1;
                    this.FASTThreshold = val;
                }
            }
        }

        private void txt_noiseLevel_TextChanged(object sender, TextChangedEventArgs e)
        {
            var val = 0.0;
            if (this.txt_noise_level != null)
            {
                if (Double.TryParse(this.txt_noise_level.Text, out val))
                {
                    val = Convert.ToDouble(this.txt_noise_level.Text);
                    if (val >= 0 && val <= 100)
                        val /= 100.0;
                    else
                        val = 20.0;
                    this.NoiseLevel = val;
                }
            }
        }

        private void btn_wykonaj_Click(object sender, RoutedEventArgs e)
        {
            var testDataManager = new TestDataPreparationManager(NoiseLevel);
            testDataManager.ApplyNoiseToAllImages();

            var manager = new NoiseRemovalFileManager(FASTThreshold, FAPGThreshold);
            manager.ApplyFiltersOnAllImages();

            var calculation = new CalculationManager();
            calculation.CalculateForAllCombinations();

            MessageBox.Show("Zakończono wykonywanie wszystkich operacji", "Zakończono");
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            MainWindow window = new MainWindow();
            window.Show();
            base.OnClosing(e);
        }
    }
}
