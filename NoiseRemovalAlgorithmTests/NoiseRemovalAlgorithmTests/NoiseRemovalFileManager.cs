using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class NoiseRemovalFileManager
    {
        public string CorruptedImagesPath { get; set; }
        public string OutputMainFolderPath { get; set; }
        public int Iterations { get; set; } = 3;
        public int FASTThreshold { get; set; } = 40; //pobrane z publikacji
        public int FAPGThreshold { get; set; } = 44; // 1/10 zalecana z 440 gdzie to max odleglosc

        public string[] CorruptedImagesPaths { get; set; }

        public NoiseRemovalFileManager()
        {
        }

        public void ApplyFiltersOnAllImages()
        {
            LoadFileNames();

            FASTandAMF();
            FASTandWAF();
            FASTandVMF();

            FAPGandAMF();
            FAPGandWAF();
            FAPGandVMF();
        }

        private void LoadFileNames()
        {
            DirectoryInfo directory = new DirectoryInfo(CorruptedImagesPath);
            var files = directory.GetFiles("*.bmp");
            CorruptedImagesPaths = files.OrderBy(x => GetFileNumber(x.Name)).Select(x => x.FullName).ToArray();
        }

        private int GetFileNumber(string name)
        {
            name = name.Split('.')[0];
            name = name.Replace("noise", "");
            return Convert.ToInt32(name);
        }

        private void FAPGandAMF()
        {
            var i = 1;
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

                var detectedNoise = fapg.DetectNoise();

                var amf = new AMF();
                amf.Width = manager.ExtendedWidth;
                amf.Height = manager.ExtendedHeight;
                amf.Pixels = manager.ExtendedArray;
                amf.WindowSize = 9;
                amf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = amf.RemoveNoise();
                fapg.Pixels = result;
                detectedNoise = fapg.DetectNoise();

                amf.CorruptedPixels = detectedNoise;
                amf.Pixels = result;
                result = amf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fapg\\AMF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }
        private void FASTandAMF()
        {
            var i = 1;
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

                var detectedNoise = fast.DetectNoise();

                var amf = new AMF();
                amf.Width = manager.ExtendedWidth;
                amf.Height = manager.ExtendedHeight;
                amf.Pixels = manager.ExtendedArray;
                amf.WindowSize = 9;
                amf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = amf.RemoveNoise();
                fast.Pixels = result;
                detectedNoise = fast.DetectNoise();

                amf.CorruptedPixels = detectedNoise;
                amf.Pixels = result;
                result = amf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fast\\AMF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }

        private void FAPGandWAF()
        {
            var i = 1;
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

                var detectedNoise = fapg.DetectNoise();

                var waf = new WAF();
                waf.Width = manager.ExtendedWidth;
                waf.Height = manager.ExtendedHeight;
                waf.Pixels = manager.ExtendedArray;
                waf.WindowSize = 9;
                waf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = waf.RemoveNoise();
                fapg.Pixels = result;
                detectedNoise = fapg.DetectNoise();

                waf.CorruptedPixels = detectedNoise;
                waf.Pixels = result;
                result = waf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fapg\\WAF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }

        private void FASTandWAF()
        {
            var i = 1;
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

                var detectedNoise = fast.DetectNoise();

                var waf = new WAF();
                waf.Width = manager.ExtendedWidth;
                waf.Height = manager.ExtendedHeight;
                waf.Pixels = manager.ExtendedArray;
                waf.WindowSize = 9;
                waf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = waf.RemoveNoise();
                fast.Pixels = result;
                detectedNoise = fast.DetectNoise();

                waf.CorruptedPixels = detectedNoise;
                waf.Pixels = result;
                result = waf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fast\\WAF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }

        private void FASTandVMF()
        {
            var i = 1;
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

                var detectedNoise = fast.DetectNoise();

                var vmf = new VMF();
                vmf.Width = manager.ExtendedWidth;
                vmf.Height = manager.ExtendedHeight;
                vmf.Pixels = manager.ExtendedArray;
                vmf.WindowSize = 9;
                vmf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = vmf.RemoveNoise();
                fast.Pixels = result;
                detectedNoise = fast.DetectNoise();

                vmf.CorruptedPixels = detectedNoise;
                vmf.Pixels = result;
                result = vmf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fast\\VMF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }

        private void FAPGandVMF()
        {
            var i = 1;
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

                var detectedNoise = fapg.DetectNoise();

                var vmf = new VMF();
                vmf.Width = manager.ExtendedWidth;
                vmf.Height = manager.ExtendedHeight;
                vmf.Pixels = manager.ExtendedArray;
                vmf.WindowSize = 9;
                vmf.CorruptedPixels = detectedNoise;

                var result = new Pixel[1, 1];
                result = vmf.RemoveNoise();
                fapg.Pixels = result;
                detectedNoise = fapg.DetectNoise();

                vmf.CorruptedPixels = detectedNoise;
                vmf.Pixels = result;
                result = vmf.RemoveNoise();
                manager.ExtendedArray = result;

                File.WriteAllBytes(OutputMainFolderPath + "fapg\\VMF\\result" + i++.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
            }
        }

    }
}
