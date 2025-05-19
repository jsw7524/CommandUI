using System.Collections.Generic;
using System.Text;
//using System.Text.Json;

namespace CommandUI
{



    public partial class Form1
    {
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
    }
}

