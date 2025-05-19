using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static CommandUI.Form1;

namespace CommandUI
{
    public class CommandProcessor
    {
        Executor _executor;
        SimpleLogger _simpleLogger;
        public CommandProcessor(Executor executor, SimpleLogger simpleLogger)
        {
            _executor = executor;
            _simpleLogger = simpleLogger;
        }
        public async Task Run(CommandData command)
        {
            if (command.Args.Any(a => a.Name == "--output_dir"))
            {
                command.Args.Remove(command.Args.Where(a => a.Name == "--output_dir").FirstOrDefault());
            }

            Argument outputDir = new Argument()
            {
                Name = "--output_dir",
                Label = "輸出路徑",
                Type = "textbox",
                Value = Path.Combine(Application.StartupPath,
                $"Outputs\\{DateTime.Now.ToString("yyyyMMdd")}\\{Path.GetFileName(command.Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value.Replace(" ", ""))}\\")
            };
            command.Args.Add(outputDir);
            //CommandData originalCmd = commands.First();
            List<string> audioFiles = command.Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value.Split(';').ToList();
            await Task.Factory.StartNew(() =>
            {
                foreach (string af in audioFiles)
                {
                    CommandData exeCMD = JsonConvert.DeserializeObject<CommandData>(JsonConvert.SerializeObject(command));
                    exeCMD.Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value = "\"" + af + "\"";
                    _executor.Execute(exeCMD);
                }
            });
            Process.Start("explorer.exe", outputDir.Value);
        }
    }
}

