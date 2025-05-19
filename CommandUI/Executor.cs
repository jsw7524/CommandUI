using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Text.Json;
using static CommandUI.Form1;

namespace CommandUI
{
    public class Executor
    {
        SimpleLogger _simpleLogger;
        public Executor(SimpleLogger simpleLogger)
        {
            _simpleLogger = simpleLogger;
        }
        public void Execute(CommandData command)
        {
            // Create the output directory if it doesn't exist
            Directory.CreateDirectory(command.Args.Where(a => a.Name == "--output_dir").FirstOrDefault().Value);

            _simpleLogger.WriteLog(command.ToString());

            Process cmd = new Process();
            cmd.StartInfo.FileName = command.ExePath;
            cmd.StartInfo.Arguments = command.GetArgs();
            //cmd.StartInfo.RedirectStandardInput = true;
            //cmd.StartInfo.RedirectStandardOutput = true;
            //cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.StartInfo.RedirectStandardError = true;
            cmd.Start();
            cmd.WaitForExit();
            string error = cmd.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
            {
                _simpleLogger.WriteLog(error);
            }
        }
    }
}

