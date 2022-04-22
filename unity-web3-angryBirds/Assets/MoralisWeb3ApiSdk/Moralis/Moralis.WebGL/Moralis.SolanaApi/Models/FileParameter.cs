using System;
using System.IO;

namespace Moralis.WebGL.SolanaApi.Core.Models
{
    public class FileParameter
    {
        public FileParameter() { }

        public long ContentLength { get; set; }
        public Action<Stream> Writer { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
        public string Name { get; set; }

        public static FileParameter Create(string name, byte[] data, string filename, string? contentType)
        {
            FileParameter fileParameter = new FileParameter()
            {
                ContentLength = data.Length,
                ContentType = contentType != null ? contentType : "application/json",
                FileName = filename,
                Name = name,
                Writer = s =>
                {
                    using (var file = new StreamReader(new MemoryStream(data)))
                    {
                        file.BaseStream.CopyTo(s);
                    }
                }
            };

            return fileParameter;
        }

        public static FileParameter Create(string name, byte[] data, string filename)
        {
            FileParameter fileParameter = new FileParameter()
            {
                ContentLength = data.Length,
                ContentType = "application/octet-stream",
                FileName = filename,
                Name = name,
                Writer = s =>
                {
                    using (var file = new StreamReader(new MemoryStream(data)))
                    {
                        file.BaseStream.CopyTo(s);
                    }
                }
            };

            return fileParameter;
        }

        public static FileParameter Create(string name, Action<Stream> writer, long contentLength, string fileName, string? contentType = null)
        {
            FileParameter fileParameter = new FileParameter()
            {
                ContentType = contentType != null ? contentType : "application/octet-stream",
                Name = name,
                Writer = writer
            };

            return fileParameter;
        }
    }
}
