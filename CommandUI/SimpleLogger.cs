using System;
using System.IO;
//using System.Text.Json;

namespace CommandUI
{
    public class SimpleLogger : IDisposable
    {
        Stream stream { get; set; }
        StreamWriter writer { get; set; }
        public SimpleLogger(Stream st)
        {
            stream = st;
            writer = new StreamWriter(st);
        }

        public void WriteLog(string message)
        {
            writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] {message}");
            writer.Flush();
        }

        public void Dispose()
        {

            writer.Close();
            stream.Close();
        }
    }
}

