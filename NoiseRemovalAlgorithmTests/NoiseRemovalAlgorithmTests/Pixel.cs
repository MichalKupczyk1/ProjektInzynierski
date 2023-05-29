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
}
