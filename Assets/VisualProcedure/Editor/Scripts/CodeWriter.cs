using System.Text;

namespace VisualProcedure.Editor.Scripts
{
    public class CodeWriter
    {
        private int indent;

        private StringBuilder stringBuilder = new StringBuilder();

        public void Clear()
        {
            indent = 0;
            stringBuilder.Clear();
        }

        public void BeginBlock()
        {
            WriteLine("{");
            indent += 4;
        }

        public void EndBlock(string append = "")
        {
            indent -= 4;
            WriteLine(!string.IsNullOrEmpty(append) ? $"}}{append}" : "}");
        }

        public void WriteLine(string line)
        {
            for (var i = 0; i < indent; i++)
            {
                stringBuilder.Append(" ");
            }

            stringBuilder.AppendLine(line);
        }

        public void IncreaseIndent(int value)
        {
            indent += value;
            if (indent < 0)
            {
                indent = 0;
            }
        }

        public override string ToString()
        {
            return stringBuilder.ToString();
        }
    }
}