using System.IO;
using System.Text.Json;

namespace NoiseRemovalAlgorithmTests
{
    public class CalculationManager
    {
        public CalculationManager()
        {
        }

        public void CalculateForAllCombinations()
        {
            CalculatePixelDetectionRatio(FileUtils.FAST, "fastdetection.json");
            CalculatePixelDetectionRatio(FileUtils.FAPG, "fapgdetection.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.AMF, "fastamf.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.VMF, "fastvmf.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAST + FileUtils.WAF, "fastwaf.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.AMF, "fapgamf.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.VMF, "fapgvmf.json");
            CalculateAndSaveResultsForCombination(FileUtils.FAPG + FileUtils.WAF, "fapgwaf.json");
        }

        public void CalculatePixelDetectionRatio(string subFolderPath, string outputFile)
        {
            var detectionNoiseMaps = GetAllFileNamesStartingFromMainFolder(subFolderPath + "\\noise_maps");
            var originalNoiseMaps = GetAllFileNamesStartingFromMainFolder("\\noisy_images\\noise_maps");
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
            File.WriteAllText(FileUtils.ResultsPath + outputFile, JsonSerializer.Serialize(result));
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

                var psnr = CalculatePSNR(originalImageManager.ExtendedArray, restoredImageManager.ExtendedArray);
                var mae = CalculateMAE(originalImageManager.ExtendedArray, restoredImageManager.ExtendedArray);
                var ncd = CalculateNCD(originalImageManager.ExtendedArray, restoredImageManager.ExtendedArray);

                result.Add(new CalculationResult(iterator++, psnr, mae, ncd));
            }
            File.WriteAllText(FileUtils.ResultsPath + outputFile, JsonSerializer.Serialize(result));
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
                    distanceSum += Math.Sqrt(Math.Pow(originalImage[i, j].R - restoredImage[i, j].R, 2)
                        + Math.Pow(originalImage[i, j].G - restoredImage[i, j].G, 2)
                        + Math.Pow(originalImage[i, j].B - restoredImage[i, j].B, 2));

                    sum += Math.Sqrt(Math.Pow(originalImage[i, j].R, 2) + Math.Pow(originalImage[i, j].G, 2) + Math.Pow(originalImage[i, j].B, 2));
                }
            }
            return distanceSum / sum;
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
