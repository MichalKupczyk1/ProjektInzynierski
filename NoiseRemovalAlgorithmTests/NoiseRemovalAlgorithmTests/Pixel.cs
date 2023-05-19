using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class Pixel
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }
        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
    }

    public class Lab
    {
        public float L { get; set; }
        public float a { get; set; }
        public float b { get; set; }
        public Lab(float L, float a, float b)
        {
            this.L = L;
            this.a = a;
            this.b = b;
        }
    }
    public class XYZ
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        public XYZ(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }

}
