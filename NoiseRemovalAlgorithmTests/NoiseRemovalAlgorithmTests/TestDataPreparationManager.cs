﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class TestDataPreparationManager
    {
        public string[] FileNames { get; set; }
        public double NoiseLevel { get; set; }

        public TestDataPreparationManager(double noiseLevel)
        {
            NoiseLevel = noiseLevel;
        }

        public void ApplyNoiseToAllImages()
        {
            DirectoryInfo directory = new DirectoryInfo(FileUtils.OriginalImagesPath);
            var files = directory.GetFiles("*.bmp");
            FileNames = files.OrderBy(x => FileUtils.GetFileNumber(x.Name, "kodim")).Select(x => x.FullName).ToArray();

            var fileNumber = 1;
            foreach (var file in FileNames)
            {
                var bytes = File.ReadAllBytes(file);

                var manager = new PixelArrayManager(bytes);
                var noiseMapManager = new PixelArrayManager(bytes);

                var noiseMap = manager.AddNoise(NoiseLevel);
                noiseMapManager.ExtendedArray = noiseMapManager.ConvertNoiseMapToPixelArray(noiseMap);

                File.WriteAllBytes(FileUtils.CorruptedImagesPath + "noise" + fileNumber.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
                File.WriteAllBytes(FileUtils.CorruptedImagesPath + "noise_maps//noiseMap" + fileNumber.ToString() + ".bmp", noiseMapManager.ReturnBytesFrom2DPixelArray());
                fileNumber++;
            }
        }
    }
}
