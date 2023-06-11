using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Logic.NoiseDetection
{
    public class NoiseDetectionResult
    {
        public double TruePositives { get; set; } = 0;
        public double FalsePositives { get; set; } = 0;
        public double FalseNegatives { get; set; } = 0;
        public double TrueNegatives { get; set; } = 0;
        public NoiseDetectionResult()
        {

        }

        public double CalculateMCC()
        {
            var res = ((TruePositives * TrueNegatives) - (FalsePositives * FalseNegatives))
                / Math.Sqrt((TruePositives + FalsePositives) * (TruePositives + FalseNegatives) * (TrueNegatives + FalsePositives) * (TrueNegatives + FalseNegatives));
            return res;
        }
    }
}
