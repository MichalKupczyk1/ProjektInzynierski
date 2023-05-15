using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class PixelArrayManager
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public int Step { get; set; }
        public int Amount { get; set; }
        public Pixel[,] Pixels { get; set; }

        public PixelArrayManager(byte[] bytes)
        {
            var dimentions = CalculateWidthAndHeight(bytes);
            Width = dimentions.width;
            Height = dimentions.height;
            Step = CountStep(Width);
            Amount = bytes.Length;
            var pixels = SaveToPixelArray(bytes);
            Pixels = SaveTo2DArray(pixels);
        }

        public byte[] ReturnBytesFrom2DPixelArray(Pixel[,] pixels, byte[] bytes, int amount)
        {
            var oneDimArray = ConvertFrom2DArray(pixels);

            return PixelToByteArray(bytes, oneDimArray);
        }

        public int CountStep(long width)
        {
            width *= 3;
            return (width % 4 != 0) ? (short)(4 - (width % 4)) : 0;
        }

        public Pixel[] SaveToPixelArray(byte[] bytes)
        {
            var pixels = new Pixel[Width * Height];
            var z = 0;
            var i = 0;
            var counter = 0;

            for (i = 0; i < bytes.Length - 54;)
            {
                if (Step != 0 && counter != 0 && (counter / 3) % Width == 0)
                {
                    i += Step;
                    counter = 0;
                    continue;
                }
                pixels[z++] = new Pixel(bytes[i + 54], bytes[i + 55], bytes[i + 56]);
                i += 3;

                if (Step != 0)
                    counter += 3;
            }
            return pixels;
        }

        public Pixel[,] SaveTo2DArray(Pixel[] pixels)
        {
            var twoDimentionalArray = new Pixel[Height, Width];
            var count = 0;

            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    twoDimentionalArray[i, j] = pixels[count++];

            return twoDimentionalArray;
        }

        public Pixel[] ConvertFrom2DArray(Pixel[,] pixels)
        {
            var tempPixels = new Pixel[Width * Height];
            var z = 0;

            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    tempPixels[z++] = pixels[i, j];
            return tempPixels;
        }

        public (long width, long height) CalculateWidthAndHeight(byte[] data)
        {
            var width = (long)((int)data[18] + (256 * (int)data[19]) + ((Math.Pow(256, 2) * (int)data[20])) + (Math.Pow(256, 3) * (int)data[21]));
            var height = (long)((int)data[22] + (256 * (int)data[23]) + ((Math.Pow(256, 2) * (int)data[24])) + (Math.Pow(256, 3) * (int)data[25]));

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

        public byte[] PixelToByteArray(byte[] bytes, Pixel[] pixels)
        {
            var result = new byte[Amount];

            for (int i = 0; i < 54; i++)
                result[i] = bytes[i];
            int a = 0;
            var counter = 0;
            //skipping header info which is always the same as in the original image
            for (int i = 54; i < Amount - 3;)
            {
                if (Step != 0 && counter != 0 && counter / 3 % Width == 0)
                {
                    i += Step;
                    counter = 0;
                }
                result[i] = pixels[a].R;
                result[i + 1] = pixels[a].G;
                result[i + 2] = pixels[a].B;
                i += 3;
                counter += 3;
                a++;
            }
            return result;
        }
    }

    public class Coordinates
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
