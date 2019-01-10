using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Text_Editor.Properties;

namespace Text_Editor
{
    public partial class Form1 : Form
    {
        List<Window> windows = new List<Window>() { Window.single() };
        int activeIndex = 0;
        bool down = false;
        bool selectScrolling = false;
        Point mousePoint = new Point(0, 0);

        Bitmap canvas;

        public Form1()
        {
            InitializeComponent();

            // Added
            this.MouseWheel += new MouseEventHandler(Form1_MouseWheel);
        }

        private void pictureBox1_MouseLeave(object sender, EventArgs e)
        {
            if (down)
                selectScrolling = true;
        }

        private void pictureBox1_MouseEnter(object sender, EventArgs e)
        {
            selectScrolling = false;
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            Control control = (Control)sender;
            control.Capture = false;

            down = true;

            foreach (Window window in windows)
            {
                if (e.X >= window.getPoint().X && e.X < window.getPoint().X + window.getSize().Width &&
                    e.Y >= window.getPoint().Y && e.Y < window.getPoint().Y + window.getSize().Height)
                {
                    window.mouseHandler(true, e.X - window.getPoint().X, e.Y - window.getPoint().Y);
                    break;
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            down = false;

            foreach (Window window in windows)
            {
                if (e.X >= window.getPoint().X && e.X < window.getPoint().X + window.getSize().Width &&
                    e.Y >= window.getPoint().Y && e.Y < window.getPoint().Y + window.getSize().Height)
                {
                    window.mouseHandler(false, e.X - window.getPoint().X, e.Y - window.getPoint().Y);
                    break;
                }
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            mousePoint = new Point(e.X, e.Y);

            if (e.Y > Window.TAB_HEIGHT)
            {
                Cursor = Cursors.IBeam;
            } else
            {
                Cursor = Cursors.Arrow;
            }

            if (down)
            {
                foreach (Window window in windows)
                {
                    if (e.X >= window.getPoint().X && e.X < window.getPoint().X + window.getSize().Width &&
                        e.Y >= window.getPoint().Y && e.Y < window.getPoint().Y + window.getSize().Height)
                    {
                        window.mouseHandler(false, e.X - window.getPoint().X, e.Y - window.getPoint().Y);
                        break;
                    }
                }
            }
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta != 0)
            {
                int amount = Math.Sign(e.Delta) * 4;
                windows.ElementAt(activeIndex).getActive().scroll(amount);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String s = "";

            if (Environment.GetCommandLineArgs() != null && Environment.GetCommandLineArgs().Length >= 2)
                s = Environment.GetCommandLineArgs().ElementAt(1);

            if (s != "")
            {
                List<String> text = File.ReadAllLines(s).ToList();

                windows.ElementAt(0).deleteContext();
                windows.ElementAt(0).addContext(Context.open(windows.ElementAt(0), text, s));
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // Shortcuts
            if (e.Control)
            {
                String filepath = "";
                List<String> text;

                switch (e.KeyCode)
                {
                    case Keys.N:
                        // New file
                        windows.ElementAt(activeIndex).addContext(
                            Context.getNew(windows.ElementAt(activeIndex)));
                        break;
                    case Keys.Delete:
                    case Keys.Back:
                        if (windows.ElementAt(activeIndex).numContexts() == 1)
                        {
                            // TODO - temp fix
                            Close();
                        } else
                        {
                            windows.ElementAt(activeIndex).deleteContext();
                        }
                        break;
                    case Keys.Left:
                        windows.ElementAt(activeIndex).changeContext(-1);
                        break;
                    case Keys.Right:
                        windows.ElementAt(activeIndex).changeContext(1);
                        break;
                    case Keys.Oemplus:
                        // Increase font size
                        windows.ElementAt(activeIndex).getActive().changeTextSize(1);
                        break;
                    case Keys.OemMinus:
                        // Decrease font size
                        windows.ElementAt(activeIndex).getActive().changeTextSize(-1);
                        break;
                    case Keys.O:
                        // Open
                        openFileDialog1.ShowDialog();
                        filepath = openFileDialog1.FileName;

                        text = File.ReadAllLines(filepath).ToList();
                        windows.ElementAt(activeIndex).addContext(
                            Context.open(windows.ElementAt(activeIndex),
                            text, filepath));
                        break;
                    case Keys.V:
                        // PASTE
                        if (Clipboard.ContainsText())
                        {
                            String toPaste = Clipboard.GetText();
                            windows.ElementAt(activeIndex).getActive().paste(toPaste);
                        }
                        break;
                    case Keys.X:
                        // CUT
                        windows.ElementAt(activeIndex).getActive().setClipboard(true);
                        break;
                    case Keys.C:
                        // CUT
                        windows.ElementAt(activeIndex).getActive().setClipboard(false);
                        break;
                    case Keys.A:
                        // SELECT ALL
                        windows.ElementAt(activeIndex).getActive().selectAll();
                        break;
                    case Keys.S:
                        // SAVE + SAVE AS
                        filepath = windows.ElementAt(activeIndex).getActive().getFilepath();
                        FileType fileType = windows.ElementAt(activeIndex).getActive().getFileType();

                        if (filepath == "!!!" || e.Shift)
                        {
                            saveFileDialog1.ShowDialog();
                            filepath = saveFileDialog1.FileName;
                            
                            switch (fileType)
                            {
                                case FileType.TEXT:
                                    if (!filepath.Contains("."))
                                        filepath += ".txt";
                                    break;
                                case FileType.BAREBONES:
                                    if (!filepath.Contains("."))
                                        filepath += ".bb";
                                    break;
                            }

                            windows.ElementAt(activeIndex).getActive().setFilepath(filepath);

                            if (windows.ElementAt(activeIndex).getActive().getFilepath().Contains(".bb") &&
                                windows.ElementAt(activeIndex).getActive().getFilepath().LastIndexOf(".bb") ==
                                windows.ElementAt(activeIndex).getActive().getFilepath().Length - ".bb".Length)
                            {
                                windows.ElementAt(activeIndex).getActive().setFileType(FileType.BAREBONES);
                            } else if (windows.ElementAt(activeIndex).getActive().getFilepath().Contains(".java") &&
                                windows.ElementAt(activeIndex).getActive().getFilepath().LastIndexOf(".java") ==
                                windows.ElementAt(activeIndex).getActive().getFilepath().Length - ".java".Length)
                            {
                                windows.ElementAt(activeIndex).getActive().setFileType(FileType.JAVA);
                            } else if (windows.ElementAt(activeIndex).getActive().getFilepath().Contains(".cs") &&
                              windows.ElementAt(activeIndex).getActive().getFilepath().LastIndexOf(".cs") ==
                              windows.ElementAt(activeIndex).getActive().getFilepath().Length - ".cs".Length)
                            {
                                windows.ElementAt(activeIndex).getActive().setFileType(FileType.CSHARP);
                            }
                        }

                        windows.ElementAt(activeIndex).getActive().redraw();

                        using (StreamWriter sw = new StreamWriter(filepath))
                        {
                            text = windows.ElementAt(activeIndex).getActive().getText();

                            foreach (String line in text)
                            {
                                sw.WriteLine(line);
                            }
                        }
                        break;
                }
                return;
            }

            if (e.KeyCode == Keys.Back)
            {
                windows.ElementAt(activeIndex).getActive().backspace();
            } else if (e.KeyCode == Keys.Delete) {
                windows.ElementAt(activeIndex).getActive().del();
            } else if (e.KeyCode == Keys.Enter)
            {
                windows.ElementAt(activeIndex).getActive().enter();
            }
            else if (e.KeyCode == Keys.Tab)
            {
                windows.ElementAt(activeIndex).getActive().tabStroke();
            } else if (e.KeyCode == Keys.Up)
            {
                windows.ElementAt(activeIndex).getActive().up(e.Shift);
            } else if (e.KeyCode == Keys.Down)
            {
                windows.ElementAt(activeIndex).getActive().down(e.Shift);
            } else if (e.KeyCode == Keys.Left)
            {
                windows.ElementAt(activeIndex).getActive().left(e.Shift);
            } else if (e.KeyCode == Keys.Right)
            {
                windows.ElementAt(activeIndex).getActive().right(e.Shift);
            } else
            {
                String toType = TypingHandler.typed(e);
                windows.ElementAt(activeIndex).getActive().type(toType);

                update();
                render();
            }
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (pictureBox1.Size.Width > 0 && pictureBox1.Size.Height > 0)
            {
                update();
                render();
            }
            GC.Collect();
        }

        private void update()
        {
            if (selectScrolling) {
                windows.ElementAt(activeIndex).getActive().scroll(
                    Math.Sign((windows.ElementAt(activeIndex).getSize().Width / 2) - mousePoint.Y) * 4);
                windows.ElementAt(activeIndex).getActive().mouseHandler(false, mousePoint.X, mousePoint.Y);
            }

            foreach (Window window in windows)
            {
                if (pictureBox1.Size.Width > 0 && pictureBox1.Size.Height > 0)
                    window.setSize(pictureBox1.Size);
            }

            windows.ElementAt(activeIndex).getActive().update();
        }

        private void render()
        {
            canvas = new Bitmap(pictureBox1.Size.Width, pictureBox1.Size.Height);

            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.DrawImage(windows.ElementAt(activeIndex).render(),
                    windows.ElementAt(activeIndex).getPoint());
            }

            pictureBox1.Image = canvas;
        }
    }
}
