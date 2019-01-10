using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Text_Editor
{
    class TypingHandler
    {
        public static String typed(KeyEventArgs e)
        {
            String typed = "";

            switch (e.KeyCode)
            {
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    if (!e.Control)
                    {
                        typed = e.KeyCode.ToString();
                        if (!e.Shift)
                            typed = typed.ToLower();
                    }
                    break;
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                    typed = e.KeyCode.ToString().Substring(1);
                    if (e.Shift)
                    {
                        switch (e.KeyCode)
                        {
                            case Keys.D0:
                                typed = ")";
                                break;
                            case Keys.D1:
                                typed = "!";
                                break;
                            case Keys.D2:
                                typed = "@";
                                break;
                            case Keys.D3:
                                typed = "#";
                                break;
                            case Keys.D4:
                                typed = "$";
                                break;
                            case Keys.D5:
                                typed = "%";
                                break;
                            case Keys.D6:
                                typed = "^";
                                break;
                            case Keys.D7:
                                typed = "&";
                                break;
                            case Keys.D8:
                                typed = "*";
                                break;
                            case Keys.D9:
                                typed = "(";
                                break;
                        }
                    }
                    break;
                case Keys.Space:
                    typed = " ";
                    break;
                case Keys.OemPeriod:
                    typed = ".";
                    if (e.Shift)
                        typed = ">";
                    break;
                case Keys.Oemcomma:
                    typed = ",";
                    if (e.Shift)
                        typed = "<";
                    break;
                case Keys.OemSemicolon:
                    typed = ";";
                    if (e.Shift)
                        typed = ":";
                    break;
                case Keys.OemQuotes:
                    typed = "'";
                    if (e.Shift)
                        typed = "\"";
                    break;
                case Keys.OemOpenBrackets:
                    typed = "[";
                    if (e.Shift)
                        typed = "{";
                    break;
                case Keys.OemCloseBrackets:
                    typed = "]";
                    if (e.Shift)
                        typed = "}";
                    break;
                case Keys.OemBackslash:
                    typed = "\\";
                    if (e.Shift)
                        typed = "|";
                    break;
                case Keys.OemQuestion:
                    typed = "/";
                    if (e.Shift)
                        typed = "?";
                    break;
                case Keys.Oemplus:
                    typed = "=";
                    if (e.Shift)
                        typed = "+";
                    break;
                case Keys.OemMinus:
                    typed = "-";
                    if (e.Shift)
                        typed = "_";
                    break;
                case Keys.Oemtilde:
                    typed = "`";
                    if (e.Shift)
                        typed = "~";
                    break;
            }
            return typed;
        }
    }
}
