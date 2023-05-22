// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;
using System;


var manager = new NoiseRemovalFileManager();
manager.OutputMainFolderPath = "C:\\Users\\Michal\\Desktop\\test_data\\";
manager.CorruptedImagesPath = "C:\\Users\\Michal\\Desktop\\test_data\\noisy_images\\";
manager.ApplyFiltersOnAllImages();

var calculation = new CalculationManager();
calculation.ResultsPath = "C:\\Users\\Michal\\Desktop\\test_data\\calculation_results\\";
calculation.OriginalImagesPath = "C:\\Users\\Michal\\Desktop\\test_data\\original_images\\";
calculation.RestoredImagePath = "C:\\Users\\Michal\\Desktop\\test_data\\";
calculation.CalculateForAllCombinations();