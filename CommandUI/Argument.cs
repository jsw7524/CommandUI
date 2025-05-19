using System.Collections.Generic;
//using System.Text.Json;

namespace CommandUI
{



    public partial class Form1
    {
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
    }
}

