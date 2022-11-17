using System;
using System.IO.Compression;
using System.IO;
using System.Text;

namespace DeepCore
{
    public class StringHelper
    {
        public static string GetGUID()
        {
            return Guid.NewGuid().ToString().Replace("-", "");
        }

        public static string GZip(string text)
        {
            using (var ms = new MemoryStream())
            {
                using (var gz = new GZipStream(ms, CompressionMode.Compress, true))
                {
                    using (var bs = new BufferedStream(gz, 8196))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(text);

                        bs.Write(buffer, 0, buffer.Length);
                        bs.Flush();
                    }

                    //gz.Write(buffer, 0, buffer.Length);
                    //gz.Flush();

                    //gz.Close(); 
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }

        public static string Unzip(string base64)
        {
            byte[] data = Convert.FromBase64String(base64);

            using (var ms = new MemoryStream(data))
            {
                using (var gz = new GZipStream(ms, CompressionMode.Decompress, true))
                {
                    using (var reader = new StreamReader(gz))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }
    }
}
