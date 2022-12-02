using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;


namespace QRCoderTests.Helpers
{
    public static class HelperFunctions
    {


        public static string GetAssemblyPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }


        public static string BitmapToHash(Image bmp)
        {
            byte[] imgBytes = null;
            using (var ms = new MemoryStream())
            {
                //bmp.Save(ms, new ImageFormat.Png);
                imgBytes = ms.ToArray();
                ms.Dispose();
            }
            return ByteArrayToHash(imgBytes);
        }

        public static string ByteArrayToHash(byte[] data)
        {
            var md5 = MD5.Create();
            var hash = md5.ComputeHash(data);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        public static string StringToHash(string data)
        {
            return ByteArrayToHash(Encoding.UTF8.GetBytes(data));
        }
    }
}
