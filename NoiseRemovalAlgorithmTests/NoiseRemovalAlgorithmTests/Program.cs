// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;

var bytes = File.ReadAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\test7.bmp");

var manager = new PixelArrayManager(bytes);
var width = manager.Width;
var height = manager.Height;

var arrayCopy = new Pixel[height, width];

for (int i = 0; i < height; i++)
{
    for (int j = 0; j < width; j++)
        arrayCopy[i, j] = manager.Pixels[i, j];
}

var fast = new FAST();
fast.WindowSize = 9;
fast.Width = width;
fast.Height = height;
fast.Threshold = 60;
fast.Pixels = arrayCopy;

var noiseArray = fast.DetectNoise();

for (int i = 0; i < height; i++)
{
    for (int j = 0; j < width; j++)
    {
        if (noiseArray[i, j])
            arrayCopy[i, j] = new Pixel(0, 0, 0);
        else
            arrayCopy[i, j] = new Pixel(255, 255, 255);
    }
}

var oneDimArray = manager.ConvertFrom2DArray(arrayCopy);
var amount = manager.Amount;

var bytesToSave = new byte[amount];
for (int i = 0; i < 55; i++)
    bytesToSave[i] = bytes[i];
bytesToSave = manager.PixelToByteArray(bytesToSave, oneDimArray, width, amount, manager.Step);

File.WriteAllBytes("noiseMap.bmp", bytesToSave);

var removal = new SumRemoval();
removal.Width = width;
removal.Height = height;
removal.Pixels = manager.Pixels;
removal.Threshold = 70;
removal.WindowSize = 9;

var result = removal.RemoveNoise();

var oneDimArrayResult = manager.ConvertFrom2DArray(result);

var bytesToSaveResult = new byte[amount];
for (int i = 0; i < 55; i++)
    bytesToSaveResult[i] = bytes[i];
bytesToSaveResult = manager.PixelToByteArray(bytesToSaveResult, oneDimArrayResult, width, amount, manager.Step);

File.WriteAllBytes("result.bmp", bytesToSaveResult);