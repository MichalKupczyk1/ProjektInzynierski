using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public class CalculationResult
    {
        public int Id { get; set; }
        public double PSNR { get; set; }
        public double MAE { get; set; }
        public double NCD { get; set; }
        public double NCC { get; set; }

        public CalculationResult(int id, double PSNR, double MAE, double NCD, double NCC)
        {
            this.Id = id;
            this.PSNR = PSNR;
            this.MAE = MAE;
            this.NCD = NCD;
            this.NCC = NCC;
        }
    }
}
