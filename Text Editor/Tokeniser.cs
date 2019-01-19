using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Editor
{
    class Tokeniser
    {
        public static int column(List<String> tokens, int index)
        {
            Debug.Assert(index >= 0 && index < tokens.Count);
            int length = 0;

            for (int i = 0; i < index; i++)
            {
                length += tokens.ElementAt(i).Length;
            }

            return length;
        }

        public static List<String> tokenise(String input, FileType fileType)
        {
            List<String> res = new List<string>();
            String token = "";

            int lastQuote = -1;
            char quoteType = '"';
            for (int i = 0; i < input.Length; i++)
            {
                char elem = input.ElementAt(i);

                if (lastQuote != -1)
                {
                    token += elem;
                    if (elem == quoteType)
                    {
                        lastQuote = -1;
                        res.Add(token);
                        token = "";
                    }
                } else
                {
                    if (fileType != FileType.TEXT && (elem == '"' || (fileType != FileType.VB && elem == '\'')))
                    {
                        lastQuote = i;
                        quoteType = elem;
                        token += elem;
                    } else if (!((elem >= 48 && elem <= 57) || elem == '_' ||
                        (elem >= 65 && elem <= 90) || (elem >= 97 && elem <= 122)))
                    {
                        if (elem == '.' && isNumber(token))
                        {
                            // is a number
                            token += elem;
                        }
                        else if (elem == '/' && input.Length > i + 1 && input.ElementAt(i + 1) == '/')
                        {
                            // is a comment
                            token += input.Substring(i);
                            res.Add(token);
                            token = "";
                            break;
                        }
                        else if (fileType == FileType.PROLOG && elem == '%')
                        {
                            // is a Prolog comment
                            token += input.Substring(i);
                            res.Add(token);
                            token = "";
                            break;
                        }
                        else if (fileType == FileType.WACC && elem == '#')
                        {
                            // is a WACC comment
                            token += input.Substring(i);
                            res.Add(token);
                            token = "";
                            break;
                        } else if (fileType == FileType.VB && elem == '\'')
                        {
                            // is a VB comment
                            token += input.Substring(i);
                            res.Add(token);
                            token = "";
                            break;
                        }
                        else
                        {
                            if (token != "")
                                res.Add(token);
                            token = "";
                            token += elem;
                            res.Add(token);
                            token = "";
                        }
                    } else
                    {
                        token += elem;
                    }
                }
            }

            if (token != "")
            {
                res.Add(token);
                token = "";
            }

            return res;
        }

        public static bool isNumber(String token)
        {
            if (token.Length > 0 && token.ElementAt(0) >= 48 && token.ElementAt(0) <= 57)
            {
                int periods = 0;
                bool possible = true;
                for (int n = 0; n < token.Length; n++)
                {
                    if (token.ElementAt(n) == '.')
                        periods++;

                    possible &= (token.ElementAt(n) >= 48 &&
                        token.ElementAt(n) <= 57) ||
                        token.ElementAt(n) == '.';
                }

                return possible && periods <= 1;
            }
            return false;
        }
    }
}
