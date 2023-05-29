using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class NoiseDetectionResult
    {
        public int TruePositives { get; set; } = 0;
        public int FalsePositives { get; set; } = 0;
        public int FalseNegatives { get; set; } = 0;
        public int TrueNegatives { get; set; } = 0;
        public NoiseDetectionResult()
        {

        }
        public NoiseDetectionResult(int truePositives, int falsePositives, int falseNegatives, int trueNegatives)
        {
            TruePositives = truePositives;
            FalsePositives = falsePositives;
            FalseNegatives = falseNegatives;
            TrueNegatives = trueNegatives;
        }
    }
}
