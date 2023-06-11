using ProjektInzynierskiWindowedApp.Logic.NoiseDetection;
using ProjektInzynierskiWindowedApp.Logic.NoiseRemoval;
using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Managers
{
    public class NoiseRemovalFileManager
    {
        public double FASTThreshold { get; set; } = 32;
        public double FAPGThreshold { get; set; } = 35;
        public string[] CorruptedImagesPaths { get; set; }

        public NoiseRemovalFileManager(double fast, double fapg)
        {
            this.FASTThreshold = fast;
            this.FAPGThreshold = fapg;
        }

        public void ApplyFiltersOnAllImages()
        {
            LoadFileNames();
            RemoveNoiseWithFAST();
            RemoveNoiseWithFAPG();
        }

        private void LoadFileNames()
        {
            DirectoryInfo directory = new DirectoryInfo(FileUtils.CorruptedImagesPath);
            var files = directory.GetFiles("*.bmp");
            CorruptedImagesPaths = files.OrderBy(x => FileUtils.GetFileNumber(x.Name, "noise")).Select(x => x.FullName).ToArray();
        }

        private void RemoveNoiseWithFAST()
        {
            var noiseMaps = ReturnFASTNoiseMaps();
            var fileNumber = 0;

            var amfSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAST + FileUtils.AMF;
            var wafSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAST + FileUtils.WAF;
            var vmfSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAST + FileUtils.VMF;

            FileUtils.CheckDirectoryAndCreateIfNotExists(amfSaveDir);
            FileUtils.CheckDirectoryAndCreateIfNotExists(wafSaveDir);
            FileUtils.CheckDirectoryAndCreateIfNotExists(vmfSaveDir);
            foreach (var path in CorruptedImagesPaths)
            {
                var bytes = File.ReadAllBytes(path);
                AMF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, amfSaveDir);
                WAF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, wafSaveDir);
                VMF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, vmfSaveDir);
                fileNumber++;
            }
        }

        private void RemoveNoiseWithFAPG()
        {
            var noiseMaps = ReturnFAPGNoiseMaps();
            var fileNumber = 0;

            var amfSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAPG + FileUtils.AMF;
            var wafSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAPG + FileUtils.WAF;
            var vmfSaveDir = FileUtils.OutputMainFolderPath + FileUtils.FAPG + FileUtils.VMF;
            FileUtils.CheckDirectoryAndCreateIfNotExists(amfSaveDir);
            FileUtils.CheckDirectoryAndCreateIfNotExists(wafSaveDir);
            FileUtils.CheckDirectoryAndCreateIfNotExists(vmfSaveDir);

            foreach (var path in CorruptedImagesPaths)
            {
                var bytes = File.ReadAllBytes(path);
                AMF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, amfSaveDir);
                WAF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, wafSaveDir);
                VMF(bytes, noiseMaps.ElementAt(fileNumber), fileNumber + 1, vmfSaveDir);
                fileNumber++;
            }
        }

        private List<bool[,]> ReturnFASTNoiseMaps()
        {
            var result = new List<bool[,]>();
            var fileNumber = 1;
            var fastNoiseMapsDir = FileUtils.OutputMainFolderPath + "fast\\noise_maps\\";
            FileUtils.CheckDirectoryAndCreateIfNotExists(fastNoiseMapsDir);
            foreach (var path in CorruptedImagesPaths)
            {
                var bytes = File.ReadAllBytes(path);
                var manager = new PixelArrayManager(bytes);

                var fast = new FAST();
                fast.Width = manager.ExtendedWidth;
                fast.Height = manager.ExtendedHeight;
                fast.Threshold = FASTThreshold;
                fast.Pixels = manager.ExtendedArray;
                fast.WindowSize = 9;

                var noiseMap = fast.DetectNoise();
                manager.ExtendedArray = manager.ConvertNoiseMapToPixelArray(noiseMap);

                File.WriteAllBytes(fastNoiseMapsDir + "noise_map" + fileNumber++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                result.Add(noiseMap);
            }
            return result;
        }

        private List<bool[,]> ReturnFAPGNoiseMaps()
        {
            var result = new List<bool[,]>();
            var fileNumber = 1;
            var fapgNoiseMapsDir = FileUtils.OutputMainFolderPath + "fapg\\noise_maps\\";
            FileUtils.CheckDirectoryAndCreateIfNotExists(fapgNoiseMapsDir);

            foreach (var path in CorruptedImagesPaths)
            {
                var bytes = File.ReadAllBytes(path);
                var manager = new PixelArrayManager(bytes);

                var fapg = new FAPG();
                fapg.Width = manager.ExtendedWidth;
                fapg.Height = manager.ExtendedHeight;
                fapg.Threshold = FAPGThreshold;
                fapg.Pixels = manager.ExtendedArray;
                fapg.WindowSize = 9;

                var noiseMap = fapg.DetectNoise();
                manager.ExtendedArray = manager.ConvertNoiseMapToPixelArray(noiseMap);

                File.WriteAllBytes(fapgNoiseMapsDir + "noise_map" + fileNumber++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                result.Add(fapg.DetectNoise());
            }
            return result;
        }

        private void AMF(byte[] bytes, bool[,] detectedNoise, int fileNumber, string saveDir)
        {
            var manager = new PixelArrayManager(bytes);

            var amf = new AMF();
            amf.Width = manager.ExtendedWidth;
            amf.Height = manager.ExtendedHeight;
            amf.Pixels = manager.ExtendedArray;
            amf.WindowSize = 9;
            amf.CorruptedPixels = detectedNoise;
            manager.ExtendedArray = amf.RemoveNoise();

            File.WriteAllBytes(saveDir + "result" + fileNumber.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
        }

        private void VMF(byte[] bytes, bool[,] detectedNoise, int fileNumber, string saveDir)
        {
            var manager = new PixelArrayManager(bytes);

            var vmf = new VMF();
            vmf.Width = manager.ExtendedWidth;
            vmf.Height = manager.ExtendedHeight;
            vmf.Pixels = manager.ExtendedArray;
            vmf.WindowSize = 9;
            vmf.CorruptedPixels = detectedNoise;
            manager.ExtendedArray = vmf.RemoveNoise();

            File.WriteAllBytes(saveDir + "result" + fileNumber.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
        }

        private void WAF(byte[] bytes, bool[,] detectedNoise, int fileNumber, string saveDir)
        {
            var manager = new PixelArrayManager(bytes);

            var waf = new WAF();
            waf.Width = manager.ExtendedWidth;
            waf.Height = manager.ExtendedHeight;
            waf.Pixels = manager.ExtendedArray;
            waf.WindowSize = 9;
            waf.CorruptedPixels = detectedNoise;
            manager.ExtendedArray = waf.RemoveNoise();

            File.WriteAllBytes(saveDir + "result" + fileNumber.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
        }
    }
}
