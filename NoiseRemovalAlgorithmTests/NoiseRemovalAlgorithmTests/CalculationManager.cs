namespace NoiseRemovalAlgorithmTests
{
    public class CalculationManager
    {
        public string NoisyImagePath { get; set; }
        public string RestoredImagePath { get; set; }
        public CalculationManager()
        {
        }

        public double CalculateNCD(Pixel[,] noisyImage, Pixel[,] restoredImage)
        {
            var sum = 0.0;
            var height = noisyImage.GetLength(0);
            var width = noisyImage.GetLength(1);

            var noisyImageLab = new Lab[height, width];
            var restoredImageLab = new Lab[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    noisyImageLab[i, j] = CalculateLab(noisyImage[i, j]);
                    restoredImageLab[i, j] = CalculateLab(restoredImage[i, j]);
                }
            }

            return sum;
        }

        private Lab CalculateLab(Pixel pixel)
        {
            var xyz = ConvertPixelToXYZ(pixel);
            return ConvertXYZToLab(xyz);
        }

        private Lab ConvertXYZToLab(XYZ xyz)
        {
            var result = new Lab(0, 0, 0);
            xyz = DivideByRefs(xyz);

            var fx = CalculateFunction(xyz.X);
            var fy = CalculateFunction(xyz.Y);
            var fz = CalculateFunction(xyz.Z);

            result.L = CalculateL(xyz.Y);
            result.a = 500 * (fx - fy);
            result.b = 200 * (fy - fz);

            return result;
        }

        private float CalculateFunction(float val)
        {
            if (val > 0.008856f)
                return (float)(Math.Pow(val, 1.0 / 3));
            return val * 7.787f + 16 / 116f;
        }

        private XYZ DivideByRefs(XYZ xyz)
        {
            var xref = 0.95047f;
            var yref = 1f;
            var zref = 1.08883f;

            xyz.X /= xref;
            xyz.Y /= yref;
            xyz.Z /= zref;

            return xyz;
        }

        private float CalculateL(float y)
        {
            if (y > 0.008856f)
                return (float)(116 * Math.Pow(y, 1.0 / 3) - 16);
            return 903.3f * y;
        }

        private XYZ ConvertPixelToXYZ(Pixel pixel)
        {
            var result = new XYZ(0, 0, 0);

            var r = (float)pixel.R / 255;
            var g = (float)pixel.G / 255;
            var b = (float)pixel.B / 255;

            r = CheckValue(r);
            g = CheckValue(g);
            b = CheckValue(b);

            r *= 100;
            g *= 100;
            b *= 100;

            result.X = r * 0.4124f + g * 0.3576f + b * 0.1805f;
            result.Y = r * 0.2126f + g * 0.7152f + b * 0.0722f;
            result.Z = r * 0.0193f + g * 0.01192f + b * 0.9505f;

            return result;
        }

        private float CheckValue(float val)
        {
            if (val > 0.04045)
                return (float)Math.Pow((val + 0.055) / 1.055, 2.4);
            return val / 12.92f;
        }


        public double CalculatePSNR(Pixel[,] noisyImage, Pixel[,] restoredImage)
        {
            var sum = 0.0;
            var height = noisyImage.GetLength(0);
            var width = noisyImage.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += MSE(noisyImage[i, j], restoredImage[i, j]);
                }
            }
            var mse = sum * (1.0 / (3 * width * height));

            var rmse = Math.Sqrt(mse);

            return 20.0 * Math.Log10(255 / rmse);
        }

        public double CalculateMAE(Pixel[,] noisyImage, Pixel[,] restoredImage)
        {
            var sum = 0;
            var height = noisyImage.GetLength(0);
            var width = noisyImage.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    sum += MAE(noisyImage[i, j], restoredImage[i, j]);
                }
            }
            return sum * (1.0 / (3 * width * height));
        }

        private int MAE(Pixel left, Pixel right)
        {
            var R = Math.Abs(left.R - right.R);
            var G = Math.Abs(left.G - right.G);
            var B = Math.Abs(left.B - right.B);

            return R + G + B;
        }

        private double MSE(Pixel left, Pixel right)
        {
            var R = Math.Pow(left.R - right.R, 2);
            var G = Math.Pow(left.G - right.G, 2);
            var B = Math.Pow(left.B - right.B, 2);

            return R + G + B;
        }
    }
}
