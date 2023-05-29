using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoiseRemovalAlgorithmTests
{
    public static class FileUtils
    {
        public static string CorruptedImagesPath { get => "C:\\Users\\Michal\\Desktop\\test_data\\noisy_images\\"; }
        public static string OriginalImagesPath { get => "C:\\Users\\Michal\\Desktop\\test_data\\original_images\\"; }
        public static string ResultsPath { get => "C:\\Users\\Michal\\Desktop\\test_data\\calculation_results\\"; }
        public static string OutputMainFolderPath { get => "C:\\Users\\Michal\\Desktop\\test_data\\"; }
        public static string FAST { get => "\\fast"; }
        public static string FAPG { get => "\\fapg"; }
        public static string WAF { get => "\\WAF\\"; }
        public static string VMF { get => "\\VMF\\"; }
        public static string AMF { get => "\\AMF\\"; }

        public static int GetFileNumber(string name, string fileName)
        {
            name = name.Split('.')[0];
            name = name.Replace(fileName, "");
            return Convert.ToInt32(name);
        }
    }
}
