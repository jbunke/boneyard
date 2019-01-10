using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Editor
{
    class TextCursor
    {
        private int line; // 0-based
        private int column; // 0-based
        private int fromL; // fromL and fromC are the beginning of the selection
        private int fromC;
        private int count; // for flashing
        private bool visible;

        public TextCursor()
        {
            line = 0;
            column = 0;
            fromL = 0;
            fromC = 0;
            count = 0;
            visible = false;
        }

        public TextCursor(int line, int column)
        {
            this.line = line;
            this.column = column;
            count = 0;
            visible = false;
        }

        public void set(int line, int column)
        {
            this.line = line;
            this.column = column;
            visible = true;
        }

        public void setFrom(int fromL, int fromC)
        {
            this.fromL = fromL;
            this.fromC = fromC;
        }

        public void flush()
        {
            fromL = line;
            fromC = column;
        }

        public void increment(int typeLength)
        {
            column += typeLength;
            visible = true;
        }

        public bool isSelecting() { return line != fromL || column != fromC; }

        public bool isVisible() { return visible; }

        public int getLine() { return line; }

        public int getColumn() { return column; }

        public int getFromL() { return fromL; }

        public int getFromC() { return fromC; }

        public void update()
        {
            count++;
            count %= 4;

            if (count == 0)
                visible = !visible;
        }
    }
}
