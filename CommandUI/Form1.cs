using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ComboBox = System.Windows.Forms.ComboBox;
using TextBox = System.Windows.Forms.TextBox;
using System.Diagnostics;
using static CommandUI.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace CommandUI
{


    public partial class Form1 : Form
    {
        private List<CommandData> commands;

        public Form1()
        {
            InitializeComponent();
            快速模式ToolStripMenuItem_Click(null, null);
        }



        private void LoadArgsFromJson(string settingFile)
        {
            try
            {
                string jsonFile = Path.Combine(Application.StartupPath, settingFile);
                if (File.Exists(jsonFile))
                {
                    string jsonContent = File.ReadAllText(jsonFile);
                    commands = JsonConvert.DeserializeObject<List<CommandData>>(jsonContent);
                }
                else
                {
                    MessageBox.Show("Args.json file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Args.json: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void InitializeUIComponents()
        {
            if (commands == null || flowLayoutPanel1 == null)
                return;

            // Clear existing controls
            flowLayoutPanel1.Controls.Clear();

            // Process each command data
            foreach (var commandData in commands)
            {
                if (commandData.Visible == false)
                {
                    continue;
                }
                // Create a group box for each command
                GroupBox commandGroup = new GroupBox
                {
                    //Text = commandData.Command,
                    AutoSize = true,
                    Dock = DockStyle.Top,
                    //Padding = new Padding(5),
                    //Margin = new Padding(1)
                };

                FlowLayoutPanel commandPanel = new FlowLayoutPanel
                {
                    FlowDirection = FlowDirection.TopDown,
                    AutoSize = true,
                    WrapContents = false,
                    Dock = DockStyle.Fill,
                    //Padding = new Padding(5)
                };
                //Label labelCmd = new Label() { Text = commandData.ExePath };
                //commandPanel.Controls.Add(labelCmd);
                //flowLayoutPanel1.SetFlowBreak(labelCmd, true);
                if (commandData.Args != null)
                {
                    // Process each argument for this command
                    foreach (var arg in commandData.Args)
                    {

                        // Create label for each argument
                        Label labelArg = new Label
                        {
                            Text = arg.Label + ":",
                            AutoSize = true,
                            Margin = new Padding(3, 6, 3, 0),
                            Visible = arg.Visible
                        };
                        commandPanel.Controls.Add(labelArg);

                        // Create appropriate control based on type
                        Control control = null;

                        switch (arg.Type?.ToLower())
                        {
                            case "dropbox":
                                ComboBox comboBox = new ComboBox
                                {
                                    AutoSize = true,
                                    Name = "cb_" + arg.Name,
                                    Width = 200,
                                    DropDownStyle = ComboBoxStyle.DropDownList,
                                    Visible = arg.Visible

                                };

                                if (arg.Options != null)
                                {
                                    foreach (var option in arg.Options)
                                    {
                                        comboBox.Items.Add(option.Name);
                                    }
                                    if (comboBox.Items.Count > 0)
                                    {
                                        comboBox.SelectedIndex = 0;
                                        arg.Value = arg.Options[comboBox.SelectedIndex].Value.ToString();
                                    }
                                }
                                comboBox.SelectedIndexChanged += (sender, e) =>
                                {
                                    // Handle selection change if needed
                                    int selectedIndex = comboBox.SelectedIndex;
                                    if (selectedIndex >= 0 && arg.Options != null && selectedIndex < arg.Options.Count)
                                    {
                                        arg.Value = arg.Options[selectedIndex].Value.ToString();
                                    }
                                };
                                control = comboBox;
                                break;

                            case "textbox":
                                TextBox textBox = new TextBox
                                {
                                    AutoSize = true,
                                    Name = "txt_" + arg.Name,
                                    Width = 200,
                                    Text = arg.Value?.ToString(),
                                    Visible = arg.Visible

                                };
                                textBox.TextChanged += (sender, e) =>
                                {
                                    // Handle text changed event if needed
                                    arg.Value = textBox.Text;
                                };
                                control = textBox;
                                break;

                            case "checkbox":
                                CheckBox checkBox = new CheckBox
                                {
                                    Name = "chk_" + arg.Name,
                                    Text = "",
                                    Checked = !string.IsNullOrEmpty(arg.Value) && arg.Value.ToLower() == "true"
                                };
                                control = checkBox;
                                break;

                            case "radio":
                                if (arg.Options != null)
                                {
                                    FlowLayoutPanel radioPanel = new FlowLayoutPanel
                                    {
                                        Name = "pnl_" + arg.Name,
                                        FlowDirection = FlowDirection.TopDown,
                                        AutoSize = true,
                                        Width = 200
                                    };

                                    foreach (var option in arg.Options)
                                    {
                                        RadioButton radioButton = new RadioButton
                                        {
                                            Name = "rb_" + arg.Name + "_" + option.Name,
                                            Text = option.Name,
                                            Tag = option.Value
                                        };
                                        radioPanel.Controls.Add(radioButton);
                                    }

                                    // Select first radio button by default
                                    if (radioPanel.Controls.Count > 0 && radioPanel.Controls[0] is RadioButton firstRadio)
                                    {
                                        firstRadio.Checked = true;
                                    }

                                    control = radioPanel;
                                }
                                break;
                            case "openfiledialog":
                                Button openFileButton = new Button
                                {
                                    Name = "btn_" + arg.Name,
                                    Text = "Browse...",
                                    AutoSize = true
                                };

                                TextBox filePathTextBox = new TextBox
                                {
                                    Name = "txt_" + arg.Name,
                                    Width = 200,
                                    ReadOnly = true,
                                    Text = arg.Value?.ToString(),
                                };

                                openFileButton.Click += (sender, e) =>
                                {
                                    using (OpenFileDialog openFileDialog = new OpenFileDialog())
                                    {
                                        openFileDialog.Multiselect = true;
                                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            filePathTextBox.Text = openFileDialog.FileNames.Aggregate((a, b) => a + ";" + b);
                                            arg.Value = filePathTextBox.Text; // Update the arg value

                                            Size size = TextRenderer.MeasureText(filePathTextBox.Text, filePathTextBox.Font);
                                            filePathTextBox.Width = size.Width;
                                            filePathTextBox.Height = size.Height;

                                        }
                                    }
                                };

                                FlowLayoutPanel fileDialogPanel = new FlowLayoutPanel
                                {
                                    FlowDirection = FlowDirection.LeftToRight,
                                    AutoSize = true
                                };
                                fileDialogPanel.Controls.Add(filePathTextBox);
                                fileDialogPanel.Controls.Add(openFileButton);

                                control = fileDialogPanel;
                                break;
                        }

                        if (control != null)
                        {
                            //control.Margin = new Padding(3, 3, 3, 10);
                            commandPanel.Controls.Add(control);
                            commandPanel.SetFlowBreak(control, true); // Add a break after each control
                        }
                    }
                }
                // Add the command panel to the group box
                commandGroup.Controls.Add(commandPanel);

                // Add the group box to the main flow layout panel
                flowLayoutPanel1.Controls.Add(commandGroup);
                flowLayoutPanel1.SetFlowBreak(commandGroup, true);


                Button exeButton = new Button()
                {
                    Text = "轉錄文字",
                    AutoSize = true,
                    Dock = DockStyle.Bottom,
                    Name = "btnExe",
                    Tag = commandData,
                };
                exeButton.Click += button1_Click;
                commandGroup.Controls.Add(exeButton);
            }
        }



        public class CommandData
        {
            public string ExePath { get; set; }
            public bool Visible { set; get; }
            public List<Argument> Args { get; set; }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(ExePath);
                if (Args != null)
                {
                    sb.Append(GetArgs());
                }
                return sb.ToString();
            }

            public string GetArgs()
            {
                StringBuilder sb = new StringBuilder();
                foreach (Argument arg in Args)
                {
                    sb.Append(" ");
                    sb.Append(arg.ToString());
                }
                return sb.ToString();
            }

        }

        public class Argument
        {
            public string Name { get; set; }
            public string Label { get; set; }
            public bool Visible { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public List<ArgumentOption> Options { get; set; }
            public override string ToString()
            {
                return $"{Name} {Value} ";
            }
        }

        public class ArgumentOption
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
        public Control FindControlRecursive(Control parent, string name)
        {
            foreach (Control child in parent.Controls)
            {
                if (child.Name == name)
                    return child;
                Control found = FindControlRecursive(child, name);
                if (found != null)
                    return found;
            }
            return null;
        }
        private async void button1_Click(object sender, EventArgs e)
        {
            if ("" == commands.First().Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value)
            {
                MessageBox.Show("請選擇錄音檔路徑", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            Button exeBtn = (Button)sender;
            exeBtn.Text = "轉錄中...";
            Executor executor = new Executor();
            try
            {
                if (commands.First().Args.Any(a => a.Name == "--output_dir"))
                {
                    commands.First().Args.Remove(commands.First().Args.Where(a => a.Name == "--output_dir").FirstOrDefault());
                }
                Argument outputDir = new Argument()
                {
                    Name = "--output_dir",
                    Label = "輸出路徑",
                    Type = "textbox",
                    Value = Path.Combine(Application.StartupPath,
                    $"Outputs\\{DateTime.Now.ToString("yyyyMMdd")}\\{Path.GetFileName(commands.First().Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value)}\\")
                };
                commands.First().Args.Add(outputDir);
                foreach (Control item in this.Controls)
                {
                    item.Enabled = false;
                }


                CommandData originalCmd = commands.First();

                List<string> audioFiles = commands.First().Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value.Split(';').ToList();
                await Task.Factory.StartNew(() =>
                {
                    foreach (string af in audioFiles)
                    {
                        CommandData exeCMD = JsonConvert.DeserializeObject<CommandData>(JsonConvert.SerializeObject(originalCmd));
                        exeCMD.Args.Where(a => a.Label == "錄音檔路徑").FirstOrDefault().Value = af;
                        executor.Run(exeCMD);
                    }
                });
                //open directory
                exeBtn.Text =  "轉錄文字";

                Process.Start("explorer.exe", outputDir.Value);

            }
            catch
            {
                throw;
            }
            finally
            {
                foreach (Control item in this.Controls)
                {
                    item.Enabled = true;
                }
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {


        }

        private void 快速模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            快速模式ToolStripMenuItem.CheckOnClick = true;
            自定義模式ToolStripMenuItem.CheckOnClick = false;
            快速模式ToolStripMenuItem.BackColor = Color.LightBlue;
            自定義模式ToolStripMenuItem.BackColor = Form1.DefaultBackColor;
            LoadArgsFromJson("QuickMode.json");
            InitializeUIComponents();
            this.Height = flowLayoutPanel1.Height + 10;
            this.Width = flowLayoutPanel1.Width + 10;
        }

        private void 自定義模式ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            快速模式ToolStripMenuItem.CheckOnClick = false;
            自定義模式ToolStripMenuItem.CheckOnClick = true;
            快速模式ToolStripMenuItem.BackColor = Form1.DefaultBackColor;
            自定義模式ToolStripMenuItem.BackColor = Color.LightBlue;
            LoadArgsFromJson("CustomMode.json");
            InitializeUIComponents();
            this.Height = flowLayoutPanel1.Height + 10;
            this.Width = flowLayoutPanel1.Width + 10;
        }
    }

    public class Executor
    {
        public void RunMultiline(IEnumerable<CommandData> orders)
        {
            Process cmd = new Process();
            try
            {
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                foreach (CommandData order in orders)
                {
                    string cd = order.ToString();
                    cmd.StandardInput.WriteLine(cd);
                    cmd.StandardInput.Flush();
                }
                cmd.StandardInput.Close();
                cmd.WaitForExit();
            }
            catch
            {
                Console.WriteLine(cmd.StandardOutput.ReadToEnd());
                int i = 1;
            }
        }

        public void Run(CommandData command)
        {
            // Create the output directory if it doesn't exist
            Directory.CreateDirectory(command.Args.Where(a => a.Name == "--output_dir").FirstOrDefault().Value);

            string logFilePath = Path.Combine(command.Args.Where(a => a.Name == "--output_dir").FirstOrDefault().Value, "Log.txt");
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")} INSTRUC] {command.ToString()}");
                writer.Flush();
            }
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
            // Save output to file
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                if (!string.IsNullOrEmpty(error))
                    writer.WriteLine($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}] ERROR {error}");
                writer.Flush();
            }
        }
    }
}

