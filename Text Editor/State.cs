using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Editor
{
    class State
    {
        private List<String> text;
        private TextCursor cursor;

        public State(List<String> text, TextCursor cursor)
        {
            this.text = new List<string>();
            foreach (String line in text)
            {
                this.text.Add(line);
            }
            this.cursor = cursor;
        }

        public List<String> getText() { return text; }

        public TextCursor getCursor() { return cursor; }
    }
}
