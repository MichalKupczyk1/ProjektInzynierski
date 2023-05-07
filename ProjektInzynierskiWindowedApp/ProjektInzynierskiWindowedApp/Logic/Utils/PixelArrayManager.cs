using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;

namespace ProjektInzynierskiWindowedApp.Logic.Utils
{
    public class PixelArrayManager
    {
        public PixelArrayManager()
        {
        }

        public int CountStep(int width)
        {
            return (width % 4 != 0) ? (short)(4 - (width % 4)) : 0;
        }

        public Pixel[] SaveToPixelArray(byte[] bytes, int step, int byteAmount, int width)
        {
            var pixelAmount = ((byteAmount - 57) / 3) + step;
            var pixels = new Pixel[pixelAmount];
            var z = 0;

            for (int i = 0; i < byteAmount - 57;)
            {
                if (step != 0 && (i + 3) % (width) == 0)
                {
                    i += step;
                    continue;
                }
                pixels[z] = new Pixel(bytes[i + 54], bytes[i + 55], bytes[i + 56]);
                i += 3;
                z++;
            }
            return pixels;
        }

        public Pixel[,] SaveTo2DArray(Pixel[] pixels, int width, int height)
        {
            var twoDimentionalArray = new Pixel[width, height];
            var count = 0;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    twoDimentionalArray[i, j] = pixels[count++];

            return twoDimentionalArray;
        }

        public Pixel[] ConvertFrom2DArray(Pixel[,] pixels, int width, int height)
        {
            var tempPixels = new Pixel[width * height];
            var z = 0;

            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    tempPixels[z++] = pixels[i, j];
            return tempPixels;
        }

        public (long width, long height) CalculateWidthAndHeight(byte[] data)
        {
            var width = (long)((int)data[0] + (256 * (int)data[1]) + ((Math.Pow(256, 2) * (int)data[2])) + (Math.Pow(256, 3) * (int)data[3])));
            var height = (long)((int)data[4] + (256 * (int)data[5]) + ((Math.Pow(256, 2) * (int)data[6])) + (Math.Pow(256, 3) * (int)data[7])));

            return (width, height);
        }
        public Coordinates[] ShuffleArray(Coordinates[] coordinates, int length)
        {
            var randomNumber = new Random();
            for (int i = 0; i < length; i++)
            {
                var index = randomNumber.Next(length);
                var temp = coordinates[index];
                coordinates[index] = coordinates[i];
                coordinates[i] = temp;
            }
            return coordinates;
        }

        public bool[,] AddNoise(Pixel[,] pixels, int width, int height, double noiseLevel)
        {
            bool[,] noiseArray = new bool[width, height];

            int length = width * height;
            Coordinates[] coordinates = new Coordinates[length];

            int z = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    coordinates[z].X = i;
                    coordinates[z].Y = j;
                    z++;
                }
            }

            coordinates = ShuffleArray(coordinates, length);
            var randomGenerator = new Random();

            for (int i = 0; i < length * noiseLevel; i++)
            {
                pixels[coordinates[i].X, coordinates[i].Y] = new Pixel((byte)randomGenerator.Next(255), (byte)randomGenerator.Next(255), (byte)randomGenerator.Next(255));
                noiseArray[coordinates[i].X, coordinates[i].Y] = false;
            }

            for (int i = (int)(length * noiseLevel); i < length; i++)
                noiseArray[coordinates[i].X, coordinates[i].Y] = true;

            return noiseArray;
        }

        public void PixelToByteArray(byte[] bytes, Pixel[] pixels, int width, int amount, int step)
        {
            int a = 0;
            //skipping header info which is always the same as in the original image
            for (int i = 54; i < amount - 3;)
            {
                bytes[i] = pixels[a].R;
                bytes[i + 1] = pixels[a].G;
                bytes[i + 2] = pixels[a].B;

                i += 3;
                a++;
                if ((i + 3) % width == 0)
                {
                    i += step;
                    continue;
                }
            }
        }
    }
}
