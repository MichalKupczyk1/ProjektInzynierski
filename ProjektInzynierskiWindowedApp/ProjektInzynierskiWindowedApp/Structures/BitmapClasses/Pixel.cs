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
}
