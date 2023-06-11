using ProjektInzynierskiWindowedApp.Logic.NoiseDetection;
using ProjektInzynierskiWindowedApp.Logic.Utils;
using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using ProjektInzynierskiWindowedApp.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Managers
{
    public class CalculationManager
    {
        public CalculationManager()
        {
        }

        public void CalculateForAllCombinations()
        {
            CalculatePixelDetectionRatio(FileUtils.FAST, "fastdetection.txt");
            CalculatePixelDetectionRatio(FileUtils.FAPG, "fapgdetection.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.AMF, "fastamf.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.VMF, "fastvmf.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.WAF, "fastwaf.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.AMF, "fapgamf.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.VMF, "fapgvmf.txt");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.WAF, "fapgwaf.txt");
        }

        private void SaveDetectionRatioToTextFiles(List<NoiseDetectionResult> results, string path)
        {

            var fileContent = "MCC\n";

            fileContent += ReturnListOfDataAsString(results.Select(x => x.CalculateMCC().ToString()).ToList());

            File.WriteAllText(path, fileContent);
        }

        private void SaveCalculationResultsToTextFiles(List<CalculationResult> result, string path)
        {
            var PSNR = "PSNR\n";
            PSNR += ReturnListOfDataAsString(result.Select(x => x.PSNR.ToString()).ToList());

            var MAE = "MAE\n";
            MAE += ReturnListOfDataAsString(result.Select(x => x.MAE.ToString()).ToList());

            var NCD = "NCD\n";
            NCD += ReturnListOfDataAsString(result.Select(x => x.NCD.ToString()).ToList());

            var fileContent = PSNR + MAE + NCD;
            File.WriteAllText(path, fileContent);
        }

        private string ReturnListOfDataAsString(List<string> strings)
        {
            var result = "";
            foreach (var str in strings)
            {
                result += str + "\n";
            }
            return result;
        }

        public void CalculatePixelDetectionRatio(string subFolderPath, string outputFile)
        {
            var detectionNoiseMaps = GetAllFileNamesStartingFromMainFolder(subFolderPath + "\\noise_maps");
            var originalNoiseMaps = GetAllFileNamesStartingFromMainFolder("\\noise_maps");
            int iterator = 0;
            var result = new List<NoiseDetectionResult>();

            foreach (var file in originalNoiseMaps)
            {
                var originalNoiseMapBytes = File.ReadAllBytes(file);
                var detectedNoiseMapBytes = File.ReadAllBytes(detectionNoiseMaps[iterator++]);

                var originalManager = new PixelArrayManager(originalNoiseMapBytes);
                var detectedManager = new PixelArrayManager(detectedNoiseMapBytes);

                var noiseDetectionResult = CalculateNoiseDetection(originalManager.Pixels, detectedManager.Pixels);
                result.Add(noiseDetectionResult);
            }
            SaveDetectionRatioToTextFiles(result, FileUtils.ResultsPath + outputFile);
        }

        private NoiseDetectionResult CalculateNoiseDetection(Pixel[,] originalNoise, Pixel[,] detectedNoise)
        {
            var height = originalNoise.GetLength(0);
            var width = originalNoise.GetLength(1);
            var result = new NoiseDetectionResult();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (originalNoise[i, j].R == 0) //zaszumiony piksel
                    {
                        if (originalNoise[i, j].R == detectedNoise[i, j].R)
                        {
                            //poprawnie wykryty szum
                            result.TruePositives++;
                        }
                        else
                        {
                            //pominięty szum
                            result.FalseNegatives++;
                        }
                    }
                    else //nienaruszony piksel
                    {
                        if (originalNoise[i, j].R == detectedNoise[i, j].R)
                        {
                            //poprawnie zaznaczony nienaruszony piksel
                            result.TrueNegatives++;
                        }
                        else
                        {
                            //szum oznaczony jako nienaruszony piksel
                            result.FalsePositives++;
                        }
                    }
                }
            }
            return result;
        }

        public void CalculateAndSaveResultsForCombination(string subFolderPath, string outputFile)
        {
            var fileNames = GetAllFileNamesStartingFromMainFolder(subFolderPath);
            var originalFiles = GetAllOriginalFileNames();
            var result = new List<CalculationResult>();
            var iterator = 0;

            foreach (var file in fileNames)
            {
                var restoredImageBytes = File.ReadAllBytes(file);
                var restoredImageManager = new PixelArrayManager(restoredImageBytes);

                var originalImageBytes = File.ReadAllBytes(originalFiles[iterator]);
                var originalImageManager = new PixelArrayManager(originalImageBytes);

                var psnr = CalculatePSNR(originalImageManager.Pixels, restoredImageManager.Pixels);
                var mae = CalculateMAE(originalImageManager.Pixels, restoredImageManager.Pixels);
                var ncd = CalculateNCD(originalImageManager.Pixels, restoredImageManager.Pixels);

                result.Add(new CalculationResult(iterator++, psnr, mae, ncd));
            }
            SaveCalculationResultsToTextFiles(result, FileUtils.ResultsPath + outputFile);
        }

        private string[] GetAllFileNamesStartingFromMainFolder(string subFolderPath)
        {
            return GetFileNamesFromDirectory(FileUtils.OutputMainFolderPath + subFolderPath);
        }

        private string[] GetFileNamesFromDirectory(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);
            var files = directory.GetFiles("*.bmp");
            return files.Select(x => x.FullName).ToArray();
        }

        private string[] GetAllOriginalFileNames()
        {
            DirectoryInfo directory = new DirectoryInfo(FileUtils.OriginalImagesPath);
            var files = directory.GetFiles("*.bmp");
            return files.Select(x => x.FullName).ToArray();
        }

        public double CalculateNCD(Pixel[,] originalImage, Pixel[,] restoredImage)
        {
            var sum = 0.0;
            var distanceSum = 0.0;
            var height = originalImage.GetLength(0);
            var width = originalImage.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var originalLab = ConvertRGBToLab(originalImage[i, j]);
                    var restoredLab = ConvertRGBToLab(restoredImage[i, j]);

                    distanceSum += Math.Sqrt(Math.Pow(originalLab.L - restoredLab.L, 2)
                        + Math.Pow(originalLab.a - restoredLab.a, 2)
                        + Math.Pow(originalLab.b - restoredLab.b, 2));

                    sum += Math.Sqrt(Math.Pow(originalLab.L, 2) + Math.Pow(originalLab.a, 2) + Math.Pow(originalLab.b, 2));
                }
            }
            return distanceSum / sum;
        }

        public Lab ConvertRGBToLab(Pixel pixel)
        {
            var XYZ = ConvertRGBToXYZ(pixel.R, pixel.G, pixel.B);
            return ConvertXYZToLAB(XYZ.X, XYZ.Y, XYZ.Z);
        }

        public (double X, double Y, double Z) ConvertRGBToXYZ(double r, double b, double g)
        {
            r /= 255.0; g /= 255.0; b /= 255.0;

            r = CheckRGBValue(r);
            g = CheckRGBValue(g);
            b = CheckRGBValue(b);

            var matrix = CreateAndReturnXYZMatrix();

            double X = matrix[0, 0] * r + matrix[0, 1] * g + matrix[0, 2] * b;
            double Y = matrix[1, 0] * r + matrix[1, 1] * g + matrix[1, 2] * b;
            double Z = matrix[2, 0] * r + matrix[2, 1] * g + matrix[2, 2] * b;

            return (X, Y, Z);
        }

        public Lab ConvertXYZToLAB(double x, double y, double z)
        {
            //dzielenie przez wartości referencyjne
            x /= 95.0489;
            y /= 100;
            z /= 108.884;

            x = CheckXYZValue(x);
            y = CheckXYZValue(y);
            z = CheckXYZValue(z);

            double L = 116 * y - 16;
            double a = 500 * (x - y);
            double b = 200 * (y - z);

            return new Lab(L, a, b);
        }

        private double[,] CreateAndReturnXYZMatrix()
        {
            double[,] matrix = new double[3, 3];

            //macierz referencyjna d65
            matrix[0, 0] = 0.412453;
            matrix[0, 1] = 0.357580;
            matrix[0, 2] = 0.180423;
            matrix[1, 0] = 0.212671;
            matrix[1, 1] = 0.715160;
            matrix[1, 2] = 0.072169;
            matrix[2, 0] = 0.019334;
            matrix[2, 1] = 0.119193;
            matrix[2, 2] = 0.950227;

            return matrix;
        }

        public double CheckRGBValue(double val)
        {
            if (val > 0.04045)
                return Math.Pow((val + 0.055) / 1.055, 2.4) * 100;
            return val / 12.92 * 100;
        }

        private double CheckXYZValue(double val)
        {
            if (val > 0.008856)
                return Math.Cbrt(val);
            return (7.787 * val + 16) / 116;
        }

        public double CalculatePSNR(Pixel[,] originalImage, Pixel[,] restoredImage)
        {
            var sum = 0.0;
            var height = originalImage.GetLength(0);
            var width = originalImage.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += MSE(originalImage[i, j], restoredImage[i, j]);
                }
            }
            var mse = sum * (1.0 / (3 * width * height));

            var rmse = Math.Sqrt(mse);

            return 20.0 * Math.Log10(255 / rmse);
        }

        public double CalculateMAE(Pixel[,] originalImage, Pixel[,] restoredImage)
        {
            var sum = 0;
            var height = originalImage.GetLength(0);
            var width = originalImage.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += MAE(originalImage[i, j], restoredImage[i, j]);
                }
            }
            return sum * (1.0 / (3 * width * height));
        }

        private int MAE(Pixel left, Pixel right)
        {
            var R = Math.Abs(left.R - right.R);
            var G = Math.Abs(left.G - right.G);
            var B = Math.Abs(left.B - right.B);

            return R + G + B;
        }

        private double MSE(Pixel left, Pixel right)
        {
            var R = Math.Pow(left.R - right.R, 2);
            var G = Math.Pow(left.G - right.G, 2);
            var B = Math.Pow(left.B - right.B, 2);

            return R + G + B;
        }
    }
}
