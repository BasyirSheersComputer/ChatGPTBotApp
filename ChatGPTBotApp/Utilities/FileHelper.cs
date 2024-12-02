using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatGPTBotApp.Utilities
{
    public static class FileHelper
    {
        public static bool IsLargeFile(string filePath, int sizeInMB)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Length > sizeInMB * 1024 * 1024;
        }

        public static string GetFileExtension(string filePath) => Path.GetExtension(filePath).ToLower();
    }
}
