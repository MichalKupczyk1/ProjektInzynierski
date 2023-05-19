using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class NoiseRemovalFileManager
    {
        public string NoisyImagesPath { get; set; }
        public string OutputMainFolderPath { get; set; }
        public string OriginalImagesPath { get; set; }
        public NoiseRemovalFileManager()
        {
        }

        public void ApplyFiltersOnAllImages()
        {
            FASTandAMF();
            /*
            FASTandWAF();
            FASTandVMF();

            FAPGandAMF();
            FAPGandWAF();
            FAPGandVMF();*/
        }

        private void FAPGandAMF()
        {
            for (int i = 1; i < 25; i++)
            {
                var bytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");

                var manager = new PixelArrayManager(bytes);

                var fapg = new FAPG();
                fapg.Width = manager.ExtendedWidth;
                fapg.Height = manager.ExtendedHeight;
                fapg.Threshold = 45;
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


                for (int z = 1; z < 6; z++)
                {
                    result = amf.RemoveNoise();
                    fapg.Pixels = result;
                    fapg.Threshold -= 5;
                    detectedNoise = fapg.DetectNoise();

                    amf.CorruptedPixels = detectedNoise;
                    amf.Pixels = result;
                    result = amf.RemoveNoise();
                    manager.ExtendedArray = result;

                    File.WriteAllBytes(OutputMainFolderPath + "fapg\\AMF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }
        }
        private void FASTandAMF()
        {
            var calculation = new CalculationManager();
            for (int i = 1; i < 2; i++)
            {
                var noisyImageBytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");
                var originalImageBytes = File.ReadAllBytes(OriginalImagesPath + "kodim" + i.ToString() + ".bmp");

                var originalManager = new PixelArrayManager(originalImageBytes);
                var originalNoisyImage = originalManager.ExtendedArray;

                var manager = new PixelArrayManager(noisyImageBytes);
                var fast = new FAST();
                fast.Width = manager.ExtendedWidth;
                fast.Height = manager.ExtendedHeight;
                fast.Threshold = 20;
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

                for (int z = 1; z < 6; z++)
                {
                    result = amf.RemoveNoise();
                    fast.Pixels = result;
                    fast.Threshold += 10;
                    detectedNoise = fast.DetectNoise();

                    amf.CorruptedPixels = detectedNoise;
                    amf.Pixels = result;
                    result = amf.RemoveNoise();
                    manager.ExtendedArray = result;

                    var psnr = calculation.CalculatePSNR(originalNoisyImage, result);
                    var mae = calculation.CalculateMAE(originalNoisyImage, result);
                    var ncd = calculation.CalculateNCD(originalNoisyImage, result);

                    File.WriteAllBytes(OutputMainFolderPath + "fast\\AMF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }
        }

        private void FAPGandWAF()
        {
            for (int i = 1; i < 25; i++)
            {
                var bytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");

                var manager = new PixelArrayManager(bytes);

                var fapg = new FAPG();
                fapg.Width = manager.ExtendedWidth;
                fapg.Height = manager.ExtendedHeight;
                fapg.Threshold = 45;
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


                for (int z = 1; z < 6; z++)
                {
                    result = waf.RemoveNoise();
                    fapg.Pixels = result;
                    fapg.Threshold -= 5;
                    detectedNoise = fapg.DetectNoise();

                    waf.CorruptedPixels = detectedNoise;
                    waf.Pixels = result;
                    result = waf.RemoveNoise();
                    manager.ExtendedArray = result;

                    File.WriteAllBytes(OutputMainFolderPath + "fapg\\WAF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }

        }

        private void FASTandWAF()
        {
            for (int i = 1; i < 25; i++)
            {
                var bytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");

                var manager = new PixelArrayManager(bytes);

                var fast = new FAST();
                fast.Width = manager.ExtendedWidth;
                fast.Height = manager.ExtendedHeight;
                fast.Threshold = 25;
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


                for (int z = 1; z < 6; z++)
                {
                    result = waf.RemoveNoise();
                    fast.Pixels = result;
                    fast.Threshold -= 2;
                    detectedNoise = fast.DetectNoise();

                    waf.CorruptedPixels = detectedNoise;
                    waf.Pixels = result;
                    result = waf.RemoveNoise();
                    manager.ExtendedArray = result;

                    File.WriteAllBytes(OutputMainFolderPath + "fast\\WAF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }
        }

        private void FASTandVMF()
        {
            for (int i = 1; i < 25; i++)
            {
                var bytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");

                var manager = new PixelArrayManager(bytes);

                var fast = new FAST();
                fast.Width = manager.ExtendedWidth;
                fast.Height = manager.ExtendedHeight;
                fast.Threshold = 20;
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

                for (int z = 1; z < 6; z++)
                {
                    result = vmf.RemoveNoise();
                    fast.Pixels = result;
                    fast.Threshold += 10;
                    detectedNoise = fast.DetectNoise();

                    vmf.CorruptedPixels = detectedNoise;
                    vmf.Pixels = result;
                    result = vmf.RemoveNoise();
                    manager.ExtendedArray = result;

                    File.WriteAllBytes(OutputMainFolderPath + "fast\\VMF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }
        }

        private void FAPGandVMF()
        {
            for (int i = 1; i < 25; i++)
            {
                var bytes = File.ReadAllBytes(NoisyImagesPath + "noise" + i.ToString() + ".bmp");

                var manager = new PixelArrayManager(bytes);

                var fapg = new FAPG();
                fapg.Width = manager.ExtendedWidth;
                fapg.Height = manager.ExtendedHeight;
                fapg.Threshold = 45;
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


                for (int z = 1; z < 6; z++)
                {
                    result = vmf.RemoveNoise();
                    fapg.Pixels = result;
                    fapg.Threshold -= 5;
                    detectedNoise = fapg.DetectNoise();

                    vmf.CorruptedPixels = detectedNoise;
                    vmf.Pixels = result;
                    result = vmf.RemoveNoise();
                    manager.ExtendedArray = result;

                    File.WriteAllBytes(OutputMainFolderPath + "fapg\\VMF\\" + i.ToString() + "iter" + z.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                }
            }
        }

    }
}
