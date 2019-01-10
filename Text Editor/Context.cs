using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Text_Editor
{
    class Context
    {
        private List<String> text;
        private TextCursor cursor;
        private String filepath;
        private Window window;
        private FileType fileType;

        // visual
        private bool uFlag = false;
        private String tab;
        private int textSize;
        private int topLine; // highest visible line
        private int firstColumn;
        private int printableL;
        private int printableC;
        private Bitmap bitmap;
        private Bitmap highlight;

        private Context(Window window)
        {
            text = new List<string>() { "" };
            cursor = new TextCursor();
            filepath = "!!!"; // !!! is default; no set filepath // if context is opened from file, use different constructor
            this.window = window;
            fileType = FileType.TEXT;

            uFlag = true;
            tab = Settings.TAB;
            textSize = Settings.TEXT_SIZE;
            topLine = 0;
            firstColumn = 0;

            update();
        }

        private Context(Window window, List<String> text, String filepath)
        {
            this.text = text;
            cursor = new TextCursor();
            this.filepath = filepath;
            this.window = window;
            fileType = FileType.TEXT;

            if (filepath.Contains(".bb") &&
                filepath.LastIndexOf(".bb") == filepath.Length - ".bb".Length)
            {
                fileType = FileType.BAREBONES;
            } else if (filepath.Contains(".java") &&
                filepath.LastIndexOf(".java") == filepath.Length - ".java".Length)
            {
                fileType = FileType.JAVA;
            } else if (filepath.Contains(".cs") &&
              filepath.LastIndexOf(".cs") == filepath.Length - ".cs".Length)
            {
                fileType = FileType.CSHARP;
            } else if (filepath.Contains(".pl") &&
            filepath.LastIndexOf(".pl") == filepath.Length - ".pl".Length)
            {
                fileType = FileType.PROLOG;
            }

            uFlag = true;
            tab = Settings.TAB;
            textSize = Settings.TEXT_SIZE;
            topLine = 0;
            firstColumn = 0;

            update();
        }

        public static Context open(Window window, List<String> text, String filepath)
        {
            return new Context(window, text, filepath);
        }

        public static Context getNew(Window window)
        {
            return new Context(window);
        }

        public TextCursor getCursor() { return cursor; }

        public FileType getFileType() { return fileType; }

        public void setFileType(FileType fileType) { this.fileType = fileType; }

        public String getFilepath() { return filepath; }

        public void setFilepath(String filepath) { this.filepath = filepath; }

        public String getTabName()
        {
            if (filepath == "!!!")
                return "<untitled>";

            if (filepath.Contains('\\'))
                return filepath.Substring(filepath.LastIndexOf('\\') + 1);

            return filepath;
        }

        public List<String> getText() { return text; }

        public Bitmap render() {
            Bitmap toRender = new Bitmap(bitmap.Width, bitmap.Height);

            using (Graphics g = Graphics.FromImage(toRender))
            {
                // Background
                g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.BACKGROUND)),
                    0, 0, bitmap.Width, bitmap.Height);

                // Line highlight
                g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.LINE_HIGHLIGHT)),
                        0, (int)((cursor.getLine() - topLine) * (76 * (textSize / 40f))),
                        2000, (int)(76 * (textSize / 40f)));

                int linesPrinted = text.Count - topLine;

                // Line number separator
                g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.CURSOR)),
                    7 + (5 * (48 * (textSize / 40f))), 0, 3,
                    Math.Min(printableL, linesPrinted) * (76 * (textSize / 40f)));

                // Selection
                if (cursor.isSelecting())
                {
                    int l1;
                    int c1;
                    int l2;
                    int c2;

                    if (cursor.getLine() < cursor.getFromL() ||
                        (cursor.getLine() == cursor.getFromL() &&
                        cursor.getColumn() < cursor.getFromC()))
                    {
                        l1 = cursor.getLine();
                        c1 = cursor.getColumn();
                        l2 = cursor.getFromL();
                        c2 = cursor.getFromC();
                    } else
                    {
                        l2 = cursor.getLine();
                        c2 = cursor.getColumn();
                        l1 = cursor.getFromL();
                        c1 = cursor.getFromC();
                    }

                    // first line
                    if (l1 == l2)
                    {
                        g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.TEXT_SELECTION)),
                        3 + (int)((c1 - firstColumn + 6) * (48 * (textSize / 40f))),
                        (int)((l1 - topLine) * (76 * (textSize / 40f))),
                        (int)((c2 - c1) * (48 * (textSize / 40f))),
                        (int)(76 * (textSize / 40f)));
                    } else
                    {
                        g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.TEXT_SELECTION)),
                        3 + (int)((c1 - firstColumn + 6) * (48 * (textSize / 40f))),
                        (int)((l1 - topLine) * (76 * (textSize / 40f))),
                        (int)((text.ElementAt(l1).Length - c1) * (48 * (textSize / 40f))),
                        (int)(76 * (textSize / 40f)));
                    }

                    // intermediate lines
                    for (int i = l1 + 1 - topLine; i < l2 - topLine && i < printableL; i++)
                    {
                        g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.TEXT_SELECTION)),
                        3 + (int)((6 - firstColumn) * (48 * (textSize / 40f))),
                        (int)(i * (76 * (textSize / 40f))),
                        (int)(text.ElementAt(i + topLine).Length * (48 * (textSize / 40f))),
                        (int)(76 * (textSize / 40f)));
                    }

                    // last line
                    if (l1 != l2)
                    {
                        g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.TEXT_SELECTION)),
                        3 + (int)((6 - firstColumn) * (48 * (textSize / 40f))),
                        (int)((l2 - topLine) * (76 * (textSize / 40f))),
                        (int)(c2 * (48 * (textSize / 40f))),
                        (int)(76 * (textSize / 40f)));
                    }
                }

                // Cursor
                if (cursor.isVisible())
                {
                    g.FillRectangle(new SolidBrush(Settings.getColor(Settings.Purpose.CURSOR)),
                        3 + (int)((cursor.getColumn() - firstColumn + 6) * (48 * (textSize / 40f))),
                        (int)((cursor.getLine() - topLine) * (76 * (textSize / 40f))),
                        2, (int)(76 * (textSize / 40f)));
                }

                // Text
                if (fileType != FileType.TEXT)
                {
                    g.DrawImage(highlight, 0, 0);
                }
                else
                {
                    g.DrawImage(bitmap, 0, 0);
                }
            }

            return toRender;
        }

        public void update()
        {
            cursor.update();

            int height = window.getSize().Height - Window.TAB_HEIGHT;
            int lastPrintableL = printableL;
            printableL = (int)Math.Floor(height / (76 * (textSize / 40f)));
            int lastPrintableC = printableC;
            printableC = (int)Math.Floor(window.getSize().Width / (48 * (textSize / 40f))) - 6;

            if (printableL != lastPrintableL || printableC != lastPrintableC)
                uFlag = true;

            if (uFlag)
            {
                redraw();
                uFlag = false;
            }
        }

        public void redraw()
        {
            int maxWidth = 0;
            foreach (String line in text)
            {
                maxWidth = Math.Max(maxWidth, line.Length + 8);
            }

            maxWidth *= (int)(48 * (textSize / 40f));
            int height = (int)(printableL * (76 * (textSize / 40f)));

            bitmap = new Bitmap(maxWidth, height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                for (int i = topLine;
                    i <= topLine + printableL && i < text.Count; i++)
                {
                    String lineNum = (i + 1).ToString();
                    while (lineNum.Length < 5)
                    {
                        if (lineNum.Length % 2 == 0)
                        {
                            lineNum = " " + lineNum;
                        } else
                        {
                            lineNum += " ";
                        }
                    }
                    String line = "";
                    if (text.ElementAt(i).Length > firstColumn)
                        line = text.ElementAt(i).Substring(firstColumn);
                    g.DrawImage(TextFont.def.print(lineNum + " " + line,
                        textSize, Settings.getColor(Settings.Purpose.TEXT)),
                        3, (int)((i - topLine) * (76 * (textSize / 40f))));
                }
            }

            if (fileType != FileType.TEXT)
            {
                // Syntax highlighting
                highlight = new Bitmap(bitmap.Width, bitmap.Height);

                using (Graphics g = Graphics.FromImage(highlight))
                {
                    for (int i = topLine;
                        i <= topLine + printableL && i < text.Count; i++)
                    {
                        String line = text.ElementAt(i);
                        List<String> tokens = Tokeniser.tokenise(line, fileType);
                        List<String>[] keywords = Settings.keywords(fileType);

                        String lineNum = (i + 1).ToString();
                        while (lineNum.Length < 5)
                        {
                            if (lineNum.Length % 2 == 0)
                            {
                                lineNum = " " + lineNum;
                            }
                            else
                            {
                                lineNum += " ";
                            }
                        }

                        g.DrawImage(TextFont.def.print(lineNum,
                            textSize, Settings.getColor(Settings.Purpose.TEXT)),
                            3, (int)((i - topLine) * (76 * (textSize / 40f))));

                        for (int j = 0; j < tokens.Count; j++)
                        {
                            String token = tokens.ElementAt(j);
                            Color c = Color.FromArgb(255, 255, 255);

                            for (int k = 0; k < keywords.Length; k++)
                            {
                                if (keywords[k].Contains(token))
                                {
                                    switch (k)
                                    {
                                        case 0:
                                            c = Settings.getColor(Settings.Purpose.SYNTAX1);
                                            break;
                                        case 1:
                                            c = Settings.getColor(Settings.Purpose.SYNTAX2);
                                            break;
                                    }
                                }
                            }

                            // if no keyword found, could still be string or number
                            if (c == Color.FromArgb(255, 255, 255))
                            {
                                if ((token.Length > 1 && token.ElementAt(0) == '"' &&
                                    token.ElementAt(token.Length - 1) == '"') ||
                                    (token.Length > 1 && token.ElementAt(0) == '\'' &&
                                    token.ElementAt(token.Length - 1) == '\''))
                                {
                                    c = Settings.getColor(Settings.Purpose.STRING);
                                }
                                else if (Tokeniser.isNumber(token))
                                {
                                    c = Settings.getColor(Settings.Purpose.NUM);
                                }
                                else {
                                    // Comment possibility
                                    switch (fileType)
                                    {
                                        case FileType.PROLOG:
                                            if (token.Length >= 1 && token.IndexOf("%") == 0)
                                            {
                                                c = Settings.getColor(Settings.Purpose.COMMENT);
                                            }
                                            break;
                                        default:
                                            if (token.Length >= 2 && token.IndexOf("//") == 0)
                                            {
                                                c = Settings.getColor(Settings.Purpose.COMMENT);
                                            }
                                            break;
                                    }
                                }
                            }

                            int x = Tokeniser.column(tokens, j);
                            while (x < firstColumn)
                            {
                                if (token.Length > 0)
                                    token = token.Substring(1);
                                x++;
                            }
                            g.DrawImage(TextFont.def.print(token, textSize, c),
                                3 + (int)((x + 6 - firstColumn) * (48 * (textSize / 40f))),
                                (int)((i - topLine) * (76 * (textSize / 40f))));
                        }
                    }
                }
            }
        }

        public void selectAll()
        {
            cursor.setFrom(0, 0);
            cursor.set(text.Count - 1, text.ElementAt(text.Count - 1).Length);

            boundsUpdate();
        }

        private void collapse()
        {
            // Collapses selection
            if (cursor.isSelecting())
            {
                int l1;
                int c1;
                int l2;
                int c2;

                if (cursor.getLine() < cursor.getFromL() ||
                    (cursor.getLine() == cursor.getFromL() &&
                    cursor.getColumn() < cursor.getFromC()))
                {
                    l1 = cursor.getLine();
                    c1 = cursor.getColumn();
                    l2 = cursor.getFromL();
                    c2 = cursor.getFromC();
                }
                else
                {
                    l2 = cursor.getLine();
                    c2 = cursor.getColumn();
                    l1 = cursor.getFromL();
                    c1 = cursor.getFromC();
                }

                String leftTop = text.ElementAt(l1).Substring(0, c1);
                String rightBottom = text.ElementAt(l2).Substring(c2);

                text.RemoveAt(l1);
                text.Insert(l1, leftTop + rightBottom);

                for (int i = l1 + 1; i <= l2; i++)
                {
                    text.RemoveAt(l1 + 1);
                }

                cursor.set(l1, c1);
                cursor.flush();
            }
        }

        public void type(String toType)
        {
            collapse();

            int l = cursor.getLine();
            int c = cursor.getColumn();
            String copy = text.ElementAt(l);
            text.RemoveAt(l);
            text.Insert(l, copy.Substring(0, c) + toType + copy.Substring(c));

            cursor.increment(toType.Length);
            uFlag = true;

            cursor.flush();

            if (Settings.SMART_TYPING && fileType != FileType.TEXT)
                smartTyping(toType);

            boundsUpdate();
        }

        public void tabStroke()
        {
            collapse();

            int l = cursor.getLine();
            int c = cursor.getColumn();
            String copy = text.ElementAt(l);
            text.RemoveAt(l);
            text.Insert(l, copy.Substring(0, c) + tab + copy.Substring(c));

            cursor.increment(tab.Length);
            uFlag = true;

            cursor.flush();

            if (Settings.SMART_TYPING && fileType != FileType.TEXT)
                smartTyping("tab");

            boundsUpdate();
        }

        private void smartTyping(String prompt)
        {
            int l = cursor.getLine();
            int c = cursor.getColumn();
            String copy = text.ElementAt(l);

            switch (prompt)
            {
                case "{":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + "}" + copy.Substring(c));
                    break;
                case "(":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + ")" + copy.Substring(c));
                    break;
                case "[":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + "]" + copy.Substring(c));
                    break;
                case "<":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + ">" + copy.Substring(c));
                    break;
                case "\"":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + "\"" + copy.Substring(c));
                    break;
                case "'":
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + "'" + copy.Substring(c));
                    break;
                case "}":
                    if (copy.Length > c && copy.ElementAt(c) == '}')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "]":
                    if (copy.Length > c && copy.ElementAt(c) == ']')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case ")":
                    if (copy.Length > c && copy.ElementAt(c) == ')')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case ">":
                    if (copy.Length > c && copy.ElementAt(c) == '>')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del{":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == '}')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del(":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == ')')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del[":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == ']')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del<":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == '>')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del\"":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == '"')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "del'":
                    if (text.ElementAt(l).Length >= c + 1 && text.ElementAt(l).ElementAt(c) == '\'')
                    {
                        text.RemoveAt(l);
                        text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                    }
                    break;
                case "enter":
                    String leadingSpaces = "";
                    String nextSpaces = "";

                    for (int i = 0; i < text.ElementAt(l - 1).Length; i++)
                    {
                        if (text.ElementAt(l - 1).ElementAt(i) != ' ')
                            break;
                        leadingSpaces += " ";
                    }

                    if (text.Count > l + 1)
                    {
                        for (int i = 0; i < text.ElementAt(l + 1).Length; i++)
                        {
                            if (text.ElementAt(l + 1).ElementAt(i) != ' ')
                                break;
                            nextSpaces += " ";
                        }
                    }

                    if (nextSpaces.Length > leadingSpaces.Length)
                        leadingSpaces = nextSpaces;

                    text.RemoveAt(l);
                    text.Insert(l, leadingSpaces + copy);
                    cursor.set(l, leadingSpaces.Length);
                    break;
            }
        }

        public void paste(String toPaste)
        {
            collapse();

            List<String> pasting = new List<string>();

            if (toPaste.Contains("\r\n"))
            {
                while (toPaste.Contains("\r\n"))
                {
                    pasting.Add(toPaste.Substring(0, toPaste.IndexOf("\r\n")));
                    toPaste = toPaste.Substring(toPaste.IndexOf("\r\n") + 2);
                }
            } else if (toPaste.Contains("\n"))
            {
                while (toPaste.Contains("\n"))
                {
                    pasting.Add(toPaste.Substring(0, toPaste.IndexOf("\n")));
                    toPaste = toPaste.Substring(toPaste.IndexOf("\n") + 1);
                }
            }

            // works both as else case and as last line once no more \n new line exists
            pasting.Add(toPaste);

            for (int i = 0; i < pasting.Count; i++)
            {
                if (i > 0)
                    enter();

                type(pasting.ElementAt(i));
            }

            cursor.flush();
            boundsUpdate();
        }

        public void setClipboard(bool collapseText)
        {
            int l1;
            int c1;
            int l2;
            int c2;

            if (cursor.getLine() < cursor.getFromL() ||
                (cursor.getLine() == cursor.getFromL() &&
                cursor.getColumn() < cursor.getFromC()))
            {
                l1 = cursor.getLine();
                c1 = cursor.getColumn();
                l2 = cursor.getFromL();
                c2 = cursor.getFromC();
            }
            else
            {
                l2 = cursor.getLine();
                c2 = cursor.getColumn();
                l1 = cursor.getFromL();
                c1 = cursor.getFromC();
            }

            List<String> selected = new List<string>();

            for (int i = l1; i <= l2; i++)
            {
                if (i != l1 && i != l2)
                {
                    selected.Add(text.ElementAt(i));
                }
                else if (i == l1)
                {
                    selected.Add(text.ElementAt(i).Substring(c1));
                }
                else
                {
                    selected.Add(text.ElementAt(i).Substring(0, c2));
                }
            }

            String toClipboard = "";

            for (int i = 0; i < selected.Count; i++)
            {
                if (i > 0)
                    toClipboard += "\r\n";
                toClipboard += selected.ElementAt(i);
            }

            Clipboard.SetText(toClipboard);

            if (collapseText)
            {
                collapse();
                uFlag = true;
            }
        }

        public void enter()
        {
            collapse();

            int l = cursor.getLine();
            int c = cursor.getColumn();
            String leftOf = text.ElementAt(l).Substring(0, c);
            String rightOf = text.ElementAt(l).Substring(c);
            text.RemoveAt(l);
            text.Insert(l, leftOf);
            text.Insert(l + 1, rightOf);
            cursor.set(l + 1, 0);

            if (Settings.SMART_TYPING && fileType != FileType.TEXT)
                smartTyping("enter");

            uFlag = true;

            cursor.flush();
            boundsUpdate();
        }

        public void down(bool keep)
        {
            int l = cursor.getLine();
            int c = cursor.getColumn();

            if (text.Count > l + 1)
            {
                if (text.ElementAt(l + 1).Length >= c)
                {
                    cursor.set(l + 1, c);
                } else
                {
                    cursor.set(l + 1, text.ElementAt(l + 1).Length);
                }
            }

            if (!keep)
                cursor.flush();
            boundsUpdate();
        }

        public void up(bool keep)
        {
            int l = cursor.getLine();
            int c = cursor.getColumn();

            if (l > 0)
            {
                if (text.ElementAt(l - 1).Length >= c)
                {
                    cursor.set(l - 1, c);
                }
                else
                {
                    cursor.set(l - 1, text.ElementAt(l - 1).Length);
                }
            }

            if (!keep)
                cursor.flush();
            boundsUpdate();
        }

        public void left(bool keep)
        {
            int l = cursor.getLine();
            int c = cursor.getColumn();

            if (c > 0)
                cursor.increment(-1);
            else if (l > 0)
            {
                cursor.set(l - 1, text.ElementAt(l - 1).Length);
            }

            if (!keep)
                cursor.flush();
            boundsUpdate();
        }

        public void right(bool keep)
        {
            int l = cursor.getLine();
            int c = cursor.getColumn();

            if (c < text.ElementAt(l).Length)
            {
                cursor.increment(1);
            } else if (l < text.Count - 1)
            {
                cursor.set(l + 1, 0);
            }

            if (!keep)
                cursor.flush();
            boundsUpdate();
        }

        public void backspace()
        {
            if (cursor.isSelecting())
            {
                collapse();
            } else
            {
                int l = cursor.getLine();
                int c = cursor.getColumn();

                if (c == 0)
                {
                    if (l > 0)
                    {
                        String lastLine = text.ElementAt(l - 1);
                        String thisLine = text.ElementAt(l);
                        text.RemoveAt(l);
                        text.RemoveAt(l - 1);
                        text.Insert(l - 1, lastLine + thisLine);

                        cursor.set(l - 1, lastLine.Length);
                    }
                }
                else
                {
                    String copy = text.ElementAt(l);
                    char deleted = copy.ElementAt(c - 1);
                    String replace = copy.Substring(0, c - 1) + copy.Substring(c);
                    text.RemoveAt(l);
                    text.Insert(l, replace);

                    cursor.increment(-1);

                    if (Settings.SMART_TYPING && fileType != FileType.TEXT)
                    {
                        switch (deleted)
                        {
                            case '{':
                            case '(':
                            case '[':
                            case '"':
                            case '\'':
                            case '<':
                                smartTyping("del" + deleted);
                                break;
                        }
                    }
                }

                cursor.flush();
                boundsUpdate();
            }
            uFlag = true;
        }

        public void del()
        {
            if (cursor.isSelecting())
            {
                collapse();
            } else
            {
                int l = cursor.getLine();
                int c = cursor.getColumn();

                if (c < text.ElementAt(l).Length)
                {
                    String copy = text.ElementAt(l);
                    text.RemoveAt(l);
                    text.Insert(l, copy.Substring(0, c) + copy.Substring(c + 1));
                }
                else
                {
                    if (text.Count > 0 && l + 1 < text.Count)
                    {
                        String nextLine = text.ElementAt(l + 1);
                        String thisLine = text.ElementAt(l);
                        text.RemoveAt(l + 1);
                        text.RemoveAt(l);
                        text.Insert(l, thisLine + nextLine);
                    }
                }

                cursor.flush();
                boundsUpdate();
            }
            uFlag = true;
        }

        public void scroll(int amount)
        {
            if (amount > 0)
            {
                topLine = Math.Max(0, topLine - amount);
            } else
            {
                topLine = Math.Min(text.Count - 1, topLine - amount);
            }

            uFlag = true;
            // update();
        }

        private void boundsUpdate()
        {
            while (cursor.getLine() - topLine >= printableL)
            {
                topLine++;
                uFlag = true;
            }

            while (cursor.getLine() < topLine)
            {
                topLine--;
                uFlag = true;
            }

            while (firstColumn > 0 && cursor.getColumn() - 3 < firstColumn)
            {
                firstColumn--;
                uFlag = true;
            }

            while ((cursor.getColumn() - firstColumn) + 3 >= printableC)
            {
                firstColumn++;
                uFlag = true;
            }
        }

        public void changeTextSize(int change)
        {
            if (textSize + change >= 5 && textSize + change <= 40 && Math.Abs(change) == 1)
            {
                textSize += change;
                uFlag = true;
            }
        }

        public void mouseHandler(bool down, int x, int y)
        {
            int l = (int)Math.Floor(y / (76 * (textSize / 40f))) + topLine;
            int c = (int)Math.Round((x - 3) / (48 * (textSize / 40f))) - (6 - firstColumn);

            l = Math.Max(0, Math.Min(l, text.Count - 1));
            c = Math.Min(c, text.ElementAt(l).Length);

            if (c < 0 && text.Count > l + 1)
            {
                cursor.setFrom(l, 0);
                cursor.set(l + 1, 0);
            } else
            {
                cursor.set(l, c);

                if (down)
                    cursor.setFrom(l, c);
            }

            boundsUpdate();
        }
    }
}
