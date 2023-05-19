// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;
using System;

var manager = new NoiseRemovalFileManager();
manager.OutputMainFolderPath = "C:\\Users\\Michal\\Desktop\\test_data\\";
manager.NoisyImagesPath = "C:\\Users\\Michal\\Desktop\\test_data\\noisy_images\\";
manager.OriginalImagesPath = "C:\\Users\\Michal\\Desktop\\test_data\\original_images\\";
manager.ApplyFiltersOnAllImages();