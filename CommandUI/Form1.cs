using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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

namespace CommandUI
{
    public partial class Form1 : Form
    {
        private List<CommandData> commands;

        public Form1()
        {
            InitializeComponent();
            LoadArgsFromJson();
            InitializeUIComponents();
        }

        private void LoadArgsFromJson()
        {
            try
            {
                string jsonFile = Path.Combine(Application.StartupPath, "Args.json");
                if (File.Exists(jsonFile))
                {
                    string jsonContent = File.ReadAllText(jsonFile);
                    commands = JsonConvert.DeserializeObject<List<CommandData>>(jsonContent);
                    int aaa = 1;
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
                Label labelCmd = new Label() { Text = commandData.Command };
                commandPanel.Controls.Add(labelCmd);
                flowLayoutPanel1.SetFlowBreak(labelCmd, true);
                if (commandData.Args != null)
                {
                    // Process each argument for this command
                    foreach (var arg in commandData.Args)
                    {
                        // Create label for each argument
                        Label labelArg = new Label
                        {
                            Text = arg.Name + ":",
                            AutoSize = true,
                            Margin = new Padding(3, 6, 3, 0)
                        };
                        commandPanel.Controls.Add(labelArg);

                        // Create appropriate control based on type
                        Control control = null;

                        switch (arg.Type?.ToLower())
                        {
                            case "dropbox":
                                ComboBox comboBox = new ComboBox
                                {
                                    Name = "cb_" + arg.Name,
                                    Width = 200,
                                    DropDownStyle = ComboBoxStyle.DropDownList
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
                                    Name = "txt_" + arg.Name,
                                    Width = 200,
                                    Text = arg.Value?.ToString()
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
                                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                                        {
                                            filePathTextBox.Text = openFileDialog.FileName;
                                            arg.Value = "\"" + openFileDialog.FileName + "\""; // Update the arg value

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


            }

            // Add a Submit button at the end
            Button submitButton = new Button
            {
                Text = "Submit",
                AutoSize = true,
                Margin = new Padding(3, 20, 3, 3)
            };
            submitButton.Click += SubmitButton_Click;
            flowLayoutPanel1.Controls.Add(submitButton);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {

            foreach (CommandData cd in commands)
            {
                string cmdStr = cd.ToString();
            }


        }

        //    private void SubmitButton_Click(object sender, EventArgs e)
        //    {
        //        // You can implement what happens when the submit button is clicked
        //        // For example, collecting all the values from the form controls
        //        StringBuilder result = new StringBuilder("Form values:\n");

        //        foreach (var arg in commands.Args)
        //        {
        //            string value = "not set";

        //            switch (arg.Type?.ToLower())
        //            {
        //                case "dropbox":
        //                    ComboBox comboBox = flowLayoutPanel1.Controls.Find("cb_" + arg.Name, true).FirstOrDefault() as ComboBox;
        //                    if (comboBox != null)
        //                    {
        //                        int selectedIndex = comboBox.SelectedIndex;
        //                        if (selectedIndex >= 0 && arg.Options != null && selectedIndex < arg.Options.Count)
        //                        {
        //                            value = arg.Options[selectedIndex].Value.ToString();
        //                        }
        //                    }
        //                    break;

        //                case "textbox":
        //                    TextBox textBox = flowLayoutPanel1.Controls.Find("txt_" + arg.Name, true).FirstOrDefault() as TextBox;
        //                    if (textBox != null)
        //                    {
        //                        value = textBox.Text;
        //                    }
        //                    break;

        //                case "checkbox":
        //                    CheckBox checkBox = flowLayoutPanel1.Controls.Find("chk_" + arg.Name, true).FirstOrDefault() as CheckBox;
        //                    if (checkBox != null)
        //                    {
        //                        value = checkBox.Checked.ToString();
        //                    }
        //                    break;

        //                case "radio":
        //                    FlowLayoutPanel radioPanel = flowLayoutPanel1.Controls.Find("pnl_" + arg.Name, true).FirstOrDefault() as FlowLayoutPanel;
        //                    if (radioPanel != null)
        //                    {
        //                        foreach (RadioButton rb in radioPanel.Controls.OfType<RadioButton>())
        //                        {
        //                            if (rb.Checked)
        //                            {
        //                                value = rb.Tag?.ToString() ?? rb.Text;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    break;
        //                case "openfiledialog":
        //                    TextBox fileDialogTextBox = flowLayoutPanel1.Controls.Find("txt_" + arg.Name, true).FirstOrDefault() as TextBox;
        //                    if (fileDialogTextBox != null)
        //                    {
        //                        value = fileDialogTextBox.Text;
        //                    }
        //                    break;
        //            }

        //            result.AppendLine($"{arg.Name}: {value}");
        //        }

        //        MessageBox.Show(result.ToString(), "Form Result");
        //    }
        //}

        public class CommandData
        {
            public string Command { get; set; }
            public List<ArgItem> Args { get; set; }
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();

                sb.Append(Command);

                foreach (ArgItem arg in Args)
                {
                    sb.Append(" ");
                    sb.Append(arg.ToString());
                }

                return sb.ToString();
            }
        }

        public class ArgItem
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Value { get; set; }
            public List<ArgOption> Options { get; set; }
            public override string ToString()
            {
                return $"--{Name} {Value} ";
            }
        }

        public class ArgOption
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }
    }
}
