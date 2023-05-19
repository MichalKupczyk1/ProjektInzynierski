// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;
using System;

for (int i = 1; i < 25; i++)
{
    var bytes = File.ReadAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\noisy_images\\noise" + i.ToString() + ".bmp");

    var manager = new PixelArrayManager(bytes);

    var fapg = new FAPG();
    fapg.Width = manager.ExtendedWidth;
    fapg.Height = manager.ExtendedHeight;
    fapg.Threshold = 40;
    fapg.Pixels = manager.ExtendedArray;
    fapg.WindowSize = 9;

    var detectedNoise = fapg.DetectNoise();

    var mean = new MeanRemoval();
    mean.Width = manager.ExtendedWidth;
    mean.Height = manager.ExtendedHeight;
    mean.Pixels = manager.ExtendedArray;
    mean.WindowSize = 9;
    mean.CorruptedPixels = detectedNoise;

    var result = mean.RemoveNoise();
    manager.ExtendedArray = result;
    File.WriteAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\fapg\\mean\\firstIter\\result" + i.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());

    fapg.Threshold = 50;
    fapg.Pixels = result;
    detectedNoise = fapg.DetectNoise();

    mean.CorruptedPixels = detectedNoise;
    mean.Pixels = result;
    result = mean.RemoveNoise();
    manager.ExtendedArray = result;

    File.WriteAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\fapg\\mean\\secondIter\\result" + i.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());

    fapg.Threshold = 60;
    fapg.Pixels = result;
    detectedNoise = fapg.DetectNoise();

    mean.CorruptedPixels = detectedNoise;
    mean.Pixels = result;
    result = mean.RemoveNoise();
    manager.ExtendedArray = result;

    File.WriteAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\fapg\\mean\\thirdIter\\result" + i.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());

    fapg.Threshold = 80;
    fapg.Pixels = result;
    detectedNoise = fapg.DetectNoise();

    mean.CorruptedPixels = detectedNoise;
    mean.Pixels = result;
    result = mean.RemoveNoise();
    manager.ExtendedArray = result;

    File.WriteAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\fapg\\mean\\fourthIter\\result" + i.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());

    fapg.Threshold = 90;
    fapg.Pixels = result;
    detectedNoise = fapg.DetectNoise();

    mean.CorruptedPixels = detectedNoise;
    mean.Pixels = result;
    result = mean.RemoveNoise();
    manager.ExtendedArray = result;

    File.WriteAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\fapg\\mean\\fifthIter\\result" + i.ToString() + ".bmp", manager.ReturnBytesFrom2DPixelArray());
}