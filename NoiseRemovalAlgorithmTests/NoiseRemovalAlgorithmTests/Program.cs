// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;
/*
var testDataManager = new TestDataPreparationManager(0.1);
testDataManager.ApplyNoiseToAllImages();

var manager = new NoiseRemovalFileManager();
manager.ApplyFiltersOnAllImages();
*/
var calculation = new CalculationManager();
calculation.CalculateForAllCombinations();