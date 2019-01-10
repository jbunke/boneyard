using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Text_Editor
{
    class Window
    {
        private static Size MAX = new Size(800, 428); // resize of pictureBox1 changes this
        public static readonly int TAB_HEIGHT = 30;

        private List<Context> contexts;
        private int activeIndex = -1;
        private Point point;
        private Size size;

        private Window()
        {
            activeIndex = 0;
            point = new Point(0, 0);
            size = MAX;
            contexts = new List<Context>() { Context.getNew(this) };
        }

        public static Window single()
        {
            return new Window();
        }

        public Point getPoint() { return point; }

        public Size getSize() { return size; }

        public void setSize(Size size) { this.size = size; }

        public int numContexts() { return contexts.Count; }

        public void addContext(Context toAdd)
        {
            activeIndex = contexts.Count;
            contexts.Add(toAdd);
        }

        public void deleteContext()
        {
            contexts.RemoveAt(activeIndex);

            if (contexts.Count == activeIndex)
                activeIndex--;
        }

        public void changeContext(int move)
        {
            activeIndex += move;

            if (activeIndex < 0)
                activeIndex += contexts.Count;

            if (activeIndex >= contexts.Count)
                activeIndex %= contexts.Count;
        }

        public void mouseHandler(bool down, int x, int y)
        {
            if (y > TAB_HEIGHT)
            {
                contexts.ElementAt(activeIndex).mouseHandler(down, x, y - TAB_HEIGHT);
            } else
            {
                int tabWidth = Math.Min(Math.Max(80, size.Width / 8), 200);
                activeIndex = (int)Math.Floor((x - 3) / (float)(tabWidth + 4));
            }
        }

        public Bitmap render()
        {
            Bitmap bitmap = new Bitmap(size.Width, size.Height);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                // Background
                g.FillRectangle(new SolidBrush(
                        Settings.getColor(Settings.Purpose.BACKGROUND)),
                    0, 0, size.Width, size.Height);

                // Tabs
                int tabWidth = Math.Min(Math.Max(80, size.Width / 8), 200);
                for (int i = 0; i < contexts.Count; i++)
                {
                    Context c = contexts.ElementAt(i);

                    Bitmap tab = new Bitmap(tabWidth, TAB_HEIGHT);
                    using (Graphics t = Graphics.FromImage(tab))
                    {
                        if (i == activeIndex)
                        {
                            t.FillRectangle(new SolidBrush(
                            Settings.getColor(Settings.Purpose.SELECTED_TAB)),
                            0, 0, tabWidth, TAB_HEIGHT - 10);
                        } else
                        {
                            t.FillRectangle(new SolidBrush(
                            Settings.getColor(Settings.Purpose.TAB)),
                            0, 0, tabWidth, TAB_HEIGHT - 10);
                        }

                        t.DrawImage(TextFont.def.print(c.getTabName(),
                            8, Settings.getColor(Settings.Purpose.TEXT)), 5, 3);
                    }

                    g.DrawImage(tab, 3 + ((tabWidth + 4) * i), 5);
                }

                // Active tab
                g.DrawImage(contexts.ElementAt(activeIndex).render(), 0, TAB_HEIGHT);
            }

            return bitmap;
        }

        public Context getActive()
        {
            Debug.Assert(contexts != null && contexts.Count > 0);
            return contexts.ElementAt(activeIndex);
        }
    }
}
