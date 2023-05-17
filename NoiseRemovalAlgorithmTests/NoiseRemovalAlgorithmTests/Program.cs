// See https://aka.ms/new-console-template for more information

using NoiseRemovalAlgorithmTests;

for (int i = 1; i < 5; i++)
{
    var bytes = File.ReadAllBytes("C:\\Users\\Michal\\Desktop\\test_data\\test" + i.ToString() + ".bmp");

    var manager = new PixelArrayManager(bytes);
    var width = manager.ExtendedWidth;
    var height = manager.ExtendedHeight;
    var arrayCopy = manager.ExtendedArray;

    var removal = new SumRemoval();
    removal.Width = width;
    removal.Height = height;
    removal.Pixels = arrayCopy;
    removal.Threshold = 70;
    removal.WindowSize = 9;

    manager.ExtendedArray = removal.RemoveNoise();

    var res = manager.ReturnBytesFrom2DPixelArray();

    File.WriteAllBytes("result" + i.ToString() + ".bmp", res);
}