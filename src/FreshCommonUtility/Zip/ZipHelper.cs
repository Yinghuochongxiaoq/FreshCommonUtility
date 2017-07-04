using System;
using System.IO;
using System.IO.Compression;

namespace FreshCommonUtility.Zip
{
    /// <summary>
    /// Compress，Decompress helper class
    /// </summary>
    public class ZipHelper
    {
        /// <summary>
        /// Compress send in string use GZip algorithms，return Base64 char
        /// </summary>
        /// <param name="rawString">Need compress string</param>
        /// <returns>Zip Base64 char</returns>
        public static string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString);
            byte[] zippedData = Compress(rawData);
            return Convert.ToBase64String(zippedData);
        }

        /// <summary>  
        ///  Decompress send in string use GZip algorithms
        /// </summary>  
        /// <param name="zippedString">GZip string</param>  
        /// <returns>Old string</returns>  
        public static string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            byte[] zippedData = Convert.FromBase64String(zippedString);
            return System.Text.Encoding.UTF8.GetString(Decompress(zippedData));
        }

        /// <summary>
        /// GZip compress
        /// </summary>
        /// <param name="rawData"></param>  
        /// <returns></returns>  
        public static byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Dispose();
            return ms.ToArray();
        }

        /// <summary>
        /// ZIP decompress
        /// </summary>
        /// <param name="zippedData"></param>  
        /// <returns></returns>
        public static byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Dispose();
            return outBuffer.ToArray();
        }
    }
}
