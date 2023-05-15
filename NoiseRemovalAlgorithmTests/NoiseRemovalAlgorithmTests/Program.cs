// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;

var bytes = File.ReadAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\test6s.bmp");

var manager = new PixelArrayManager(bytes);
var width = manager.Width + 2;
var height = manager.Height + 2;

var arrayCopy = new Pixel[height, width];

arrayCopy[0, 0] = manager.Pixels[0, 0];
arrayCopy[height - 1, 0] = manager.Pixels[manager.Height - 1, 0];

arrayCopy[0, width - 1] = manager.Pixels[0, manager.Width - 1];
arrayCopy[height - 1, width - 1] = manager.Pixels[manager.Height - 1, manager.Width - 1];

var counter = 1;
for (int i = 0; i < manager.Width; i++)
{
    arrayCopy[0, counter] = manager.Pixels[0, i];
    arrayCopy[height - 1, counter] = manager.Pixels[manager.Height - 1, i];
    counter++;
}
counter = 1;
for (int i = 0; i < manager.Height; i++)
{
    arrayCopy[counter, 0] = manager.Pixels[i, 0];
    arrayCopy[counter, width - 1] = manager.Pixels[i, manager.Width - 1];
    counter++;
}

for (int i = 0; i < manager.Height; i++)
{
    for (int j = 0; j < manager.Width; j++)
    {
        arrayCopy[i + 1, j + 1] = manager.Pixels[i, j];
    }
}

var removal = new SumRemoval();
removal.Width = width;
removal.Height = height;
removal.Pixels = arrayCopy;
removal.Threshold = 70;
removal.WindowSize = 9;

var result = removal.RemoveNoise();

var shrinkResult = new Pixel[height - 1, width - 1];

for (int i = 1; i < height - 1; i++)
{
    for (int j = 1; j < width - 1; j++)
    {
        shrinkResult[i - 1, j - 1] = result[i, j];
    }
}

var amount = manager.Amount;

var oneDimArrayResult = manager.ConvertFrom2DArray(shrinkResult);

var bytesToSaveResult = new byte[amount];
for (int i = 0; i < 55; i++)
    bytesToSaveResult[i] = bytes[i];
bytesToSaveResult = manager.PixelToByteArray(bytesToSaveResult, oneDimArrayResult);

File.WriteAllBytes("result.bmp", bytesToSaveResult);