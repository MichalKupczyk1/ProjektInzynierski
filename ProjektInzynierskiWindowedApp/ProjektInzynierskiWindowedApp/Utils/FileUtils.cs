using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektInzynierskiWindowedApp.Utils
{
    public static class FileUtils
    {
        public static string OriginalImagesPath { get; set; }
        public static string CorruptedImagesPath { get; set; }
        public static string ResultsPath { get; set; }
        public static string OutputMainFolderPath { get; set; }
        public static string FAST { get => "fast\\"; }
        public static string FAPG { get => "fapg\\"; }
        public static string WAF { get => "WAF\\"; }
        public static string VMF { get => "VMF\\"; }
        public static string AMF { get => "AMF\\"; }

        public static int GetFileNumber(string name, string fileName)
        {
            name = name.Split('.')[0];
            name = name.Replace(fileName, "");
            return Convert.ToInt32(name);
        }

        public static void CheckDirectoryAndCreateIfNotExists(string dir)
        {
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
        }
    }
}
