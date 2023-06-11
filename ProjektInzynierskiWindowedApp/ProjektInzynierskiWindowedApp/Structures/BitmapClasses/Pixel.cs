using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Structures.BitmapClasses
{
    public class Pixel : ICloneable
    {
        public byte R { get; set; } = 0;
        public byte G { get; set; } = 0;
        public byte B { get; set; } = 0;
        public Pixel() { }
        public Pixel(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class Lab
    {
        public double L { get; set; } = 0;
        public double a { get; set; } = 0;
        public double b { get; set; } = 0;

        public Lab(double L, double a, double b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }
    }
}