using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Text_Editor.Properties;

namespace Text_Editor
{
    class TextFont
    {
        public static TextFont def = from(Resources.asciibar_texteditor);

        private Bitmap[] glyphs = new Bitmap[96];
        private Bitmap[] white = new Bitmap[96];
        private Bitmap[] red255 = new Bitmap[96];
        private Bitmap[] green200 = new Bitmap[96];
        private Bitmap[] red200blue200 = new Bitmap[96];
        private Bitmap[] red200green200 = new Bitmap[96];

        private TextFont(Bitmap source)
        {
            generate(source);
            
            for (int i = 0; i < 96; i++)
            {
                white[i] = recolor(glyphs[i], Color.FromArgb(255, 255, 255));
                red255[i] = recolor(glyphs[i], Color.FromArgb(255, 0, 0));
                green200[i] = recolor(glyphs[i], Color.FromArgb(0, 200, 0));
                red200blue200[i] = recolor(glyphs[i], Color.FromArgb(200, 0, 200));
                red200green200[i] = recolor(glyphs[i], Color.FromArgb(200, 200, 0));
            }
        }

        public static TextFont from(Bitmap source)
        {
            return new TextFont(source);
        }

        private void generate(Bitmap source)
        {
            for (int i = 0; i < 96; i++)
            {
                glyphs[i] = new Bitmap(48, 76);

                using (Graphics g = Graphics.FromImage(glyphs[i]))
                {
                    g.DrawImage(source, -4 - (52 * i), 0);
                }
            }
        }

        public Bitmap print(String text, int size, Color color)
        {
            Debug.Assert(size >= 1 && size <= 40);
            float resize = size / 40f;
            Size xy = new Size((int)(48 * resize), (int)(76 * resize));
            Bitmap res = new Bitmap((int)(48 * resize * text.Length) + 1, xy.Height);

            for (int i = 0; i < text.Length; i++)
            {
                int address = text.ElementAt(i) - 32;
                address = Math.Min(Math.Max(0, address), 95);

                using (Graphics g = Graphics.FromImage(res))
                {
                    if (color == Color.FromArgb(0, 0, 0))
                    {
                        g.DrawImage(glyphs[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    } else if (color == Color.FromArgb(255, 255, 255)) {
                        g.DrawImage(white[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    }
                    else if (color == Color.FromArgb(255, 0, 0))
                    {
                        g.DrawImage(red255[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    }
                    else if (color == Color.FromArgb(0, 200, 0))
                    {
                        g.DrawImage(green200[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    }
                    else if (color == Color.FromArgb(200, 0, 200))
                    {
                        g.DrawImage(red200blue200[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    }
                    else if (color == Color.FromArgb(200, 200, 0))
                    {
                        g.DrawImage(red200green200[address], (int)(48 * resize * i), 0,
                        xy.Width, xy.Height);
                    }
                    else
                    {
                        g.DrawImage(recolor(glyphs[address], color), xy.Width * i, 0,
                        xy.Width, xy.Height);
                    }
                }
            }

            return res;
        }

        private Bitmap recolor(Bitmap input, Color color)
        {
            Bitmap output = new Bitmap(input);

            using (Graphics g = Graphics.FromImage(output))
            {
                for (int x = 0; x < output.Width; x += 4)
                {
                    for (int y = 0; y < output.Height; y += 4)
                    {
                        if (output.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                        {
                            g.FillRectangle(new SolidBrush(color), x, y, 4, 4);
                        }
                    }
                }
            }

            return output;
        }
    }
}
