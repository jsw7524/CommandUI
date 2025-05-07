using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CommandUI
{
    public partial class Form1 : Form
    {
        private ArgsData argsData;

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
                    argsData = JsonSerializer.Deserialize<ArgsData>(jsonContent);
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
            if (argsData?.Args == null || flowLayoutPanel1 == null)
                return;

            foreach (var arg in argsData.Args)
            {
                // Create label for each argument
                Label labelArg = new Label
                {
                    Text = arg.Name + ":",
                    AutoSize = true,
                    Margin = new Padding(3, 6, 3, 0)
                };
                flowLayoutPanel1.Controls.Add(labelArg);

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
                                comboBox.SelectedIndex = 0;
                        }
                        control = comboBox;
                        break;

                    case "textbox":
                        TextBox textBox = new TextBox
                        {
                            Name = "txt_" + arg.Name,
                            Width = 200,
                            Text = arg.Value?.ToString()
                        };
                        control = textBox;
                        break;

                    case "checkbox":
                        CheckBox checkBox = new CheckBox
                        {
                            Name = "chk_" + arg.Name,
                            Text = "",
                            //Checked = arg.Value is JsonElement jsonElement && jsonElement.TryGetBoolean(out bool result) ? result : false;
                        };
                        control = checkBox;
                        break;

                    case "radio":
                        if (arg.Options != null)
                        {
                            // Replace the incorrect usage of FlowDirection.Vertical with FlowDirection.TopDown.  
                            // FlowDirection.TopDown is the correct property for vertical flow in FlowLayoutPanel.  

                            FlowLayoutPanel radioPanel = new FlowLayoutPanel
                            {
                                Name = "pnl_" + arg.Name,
                                FlowDirection = FlowDirection.TopDown, // Corrected from Vertical to TopDown  
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
                }

                if (control != null)
                {
                    control.Margin = new Padding(3, 3, 3, 10);
                    flowLayoutPanel1.Controls.Add(control);
                    flowLayoutPanel1.SetFlowBreak(control, true); // Add a break after each control
                }
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
            // You can implement what happens when the submit button is clicked
            // For example, collecting all the values from the form controls
            StringBuilder result = new StringBuilder("Form values:\n");

            foreach (var arg in argsData.Args)
            {
                string value = "not set";

                switch (arg.Type?.ToLower())
                {
                    case "dropbox":
                        ComboBox comboBox = flowLayoutPanel1.Controls.Find("cb_" + arg.Name, true).FirstOrDefault() as ComboBox;
                        if (comboBox != null)
                        {
                            int selectedIndex = comboBox.SelectedIndex;
                            if (selectedIndex >= 0 && arg.Options != null && selectedIndex < arg.Options.Count)
                            {
                                value = arg.Options[selectedIndex].Value.ToString();
                            }
                        }
                        break;

                    case "textbox":
                        TextBox textBox = flowLayoutPanel1.Controls.Find("txt_" + arg.Name, true).FirstOrDefault() as TextBox;
                        if (textBox != null)
                        {
                            value = textBox.Text;
                        }
                        break;

                    case "checkbox":
                        CheckBox checkBox = flowLayoutPanel1.Controls.Find("chk_" + arg.Name, true).FirstOrDefault() as CheckBox;
                        if (checkBox != null)
                        {
                            value = checkBox.Checked.ToString();
                        }
                        break;

                    case "radio":
                        FlowLayoutPanel radioPanel = flowLayoutPanel1.Controls.Find("pnl_" + arg.Name, true).FirstOrDefault() as FlowLayoutPanel;
                        if (radioPanel != null)
                        {
                            foreach (RadioButton rb in radioPanel.Controls.OfType<RadioButton>())
                            {
                                if (rb.Checked)
                                {
                                    value = rb.Tag?.ToString() ?? rb.Text;
                                    break;
                                }
                            }
                        }
                        break;
                }

                result.AppendLine($"{arg.Name}: {value}");
            }

            MessageBox.Show(result.ToString(), "Form Result");
        }
    }

    public class ArgsData
    {
        public List<ArgItem> Args { get; set; }
    }

    public class ArgItem
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public List<ArgOption> Options { get; set; }
    }

    public class ArgOption
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
