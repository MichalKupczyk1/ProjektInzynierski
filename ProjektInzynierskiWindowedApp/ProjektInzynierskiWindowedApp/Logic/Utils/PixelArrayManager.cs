using ProjektInzynierskiWindowedApp.Structures.BitmapClasses;
using System;
using System.Windows.Media.Media3D;

namespace ProjektInzynierskiWindowedApp.Logic.Utils
{
    public class PixelArrayManager
    {
        public long Width { get; set; }
        public long Height { get; set; }
        public long ExtendedHeight { get => Height + 2; }
        public long ExtendedWidth { get => Width + 2; }
        public int Step { get; set; }
        public int Amount { get; set; }
        public byte[] Bytes { get; set; }
        public Pixel[,] Pixels { get; set; }
        public Pixel[,] ExtendedArray { get; set; }

        public PixelArrayManager(byte[] bytes)
        {
            Bytes = bytes;
            var dimentions = CalculateWidthAndHeight(bytes);
            Width = dimentions.width;
            Height = dimentions.height;
            Step = CountStep(Width * 3);
            Amount = (int)(54 + (Width * Height * 3) + (Step * Height));
            var pixels = SaveToPixelArray(bytes);
            Pixels = SaveTo2DArray(pixels);
            Extend2DArray();
        }

        private void Extend2DArray()
        {
            long extendedWidth = Width + 2, extendedHeight = Height + 2;

            ExtendedArray = new Pixel[extendedHeight, extendedWidth];

            ExtendedArray[0, 0] = Pixels[0, 0];
            ExtendedArray[extendedHeight - 1, 0] = Pixels[Height - 1, 0];

            ExtendedArray[0, extendedWidth - 1] = Pixels[0, Width - 1];
            ExtendedArray[extendedHeight - 1, extendedWidth - 1] = Pixels[Height - 1, Width - 1];

            var counter = 1;
            for (int i = 0; i < Width; i++)
            {
                ExtendedArray[0, counter] = Pixels[0, i];
                ExtendedArray[extendedHeight - 1, counter] = Pixels[Height - 1, i];
                counter++;
            }
            counter = 1;
            for (int i = 0; i < Height; i++)
            {
                ExtendedArray[counter, 0] = Pixels[i, 0];
                ExtendedArray[counter, extendedWidth - 1] = Pixels[i, Width - 1];
                counter++;
            }

            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    ExtendedArray[i + 1, j + 1] = Pixels[i, j];
                }
            }
        }

        private void TransferDataFromExtendedArrayToPixels()
        {
            for (int i = 1; i < ExtendedHeight - 1; i++)
            {
                for (int j = 1; j < ExtendedWidth - 1; j++)
                {
                    Pixels[i - 1, j - 1] = ExtendedArray[i, j];
                }
            }
        }

        public byte[] ReturnBytesFrom2DPixelArray()
        {
            var oneDimArray = ConvertFrom2DArray();

            return PixelToByteArray(oneDimArray); ;
        }

        public int CountStep(long width)
        {
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

        private Pixel[] ConvertFrom2DArray()
        {
            TransferDataFromExtendedArrayToPixels();

            var tempPixels = new Pixel[Width * Height];
            var z = 0;

            for (int i = 0; i < Height; i++)
                for (int j = 0; j < Width; j++)
                    tempPixels[z++] = Pixels[i, j];

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

        public byte[] PixelToByteArray(Pixel[] pixels)
        {
            var result = new byte[Amount];

            for (int i = 0; i < 54; i++)
                result[i] = Bytes[i];
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
}