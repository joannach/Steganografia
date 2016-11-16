using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using System.Threading;
using System.IO;
using System.Timers;
using System.Diagnostics;

namespace Steganografia
{
    public partial class Form1 : Form
    {
        int[,,][] t;
        char[] text;
        int[][] tab;
        Bitmap wczytana, bmp;
      //  int XY = 240;

        byte zmien(byte k, int b)
        {
            if (k % 2 == 1)
            {
                if (b == 0)
                    k--;
            }
            else
            {
                if (b == 1)
                    k++;
            }
            return k;
        }

        private void button_wczytaj1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string s = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Load(openFileDialog1.FileName);
                s = openFileDialog1.FileName;
            }

            wczytana = new Bitmap(s);

        }

        private void button_zapisz1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }

        private void button_wczytaj_tekst_Click(object sender, EventArgs e)
        {
            StreamReader sr;
            StringBuilder sb = new StringBuilder();
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            string s;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sr = new StreamReader(openFileDialog1.FileName);
                do
                {
                    s = sr.ReadLine();
                    sb.Append(s + Environment.NewLine);
                } while (s != null);

                sr.Close();
                textBox1.Text = sb.ToString();
            }
        }

        void szyfruj(Bitmap w)
        {
            bmp = new Bitmap(w);
            int ch = 0, bit = 0;
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    if (ch < tab.Length)
                    {
                        bmp.SetPixel(i, j, Color.FromArgb(
                            zmien(w.GetPixel(i, j).A, tab[ch][bit]),
                            zmien(w.GetPixel(i, j).R, tab[ch][bit + 1]),
                            zmien(w.GetPixel(i, j).G, tab[ch][bit + 2]),
                            zmien(w.GetPixel(i, j).B, tab[ch][bit + 3])));
                        bit += 4;

                        if (bit == 8)
                        {
                            bit = 0;
                            ch++;
                        }
                    }
                    else
                    {
                        i = bmp.Width;
                        j = bmp.Height;
                    }
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox2.Image = bmp;

        }

        private void button_ukryj_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length <= (wczytana.Height * wczytana.Width) / 2)
            {
                string s;
                int cc = 0;
                text = textBox1.Text.Select(c => char.Parse(c.ToString())).ToArray();
                tab = new int[text.Length][];

                for (int i = 0; i < text.Length; i++)
                {
                    s = Convert.ToString(Convert.ToInt16(text[i]), 2);
                    if (s.Length < 8)
                    {
                        cc = s.Length;
                        while (8 - cc > 0)
                        {
                            s = 0 + s;
                            cc++;
                        }
                    }
                    tab[i] = s.Select(c => int.Parse(c.ToString())).ToArray();
                }
                szyfruj(wczytana);               

            }
            else
            {
                MessageBox.Show("Obraz nie zmieści tylu danych." + Environment.NewLine +
                    "Maksymalna ilość znaków: " + (wczytana.Height* wczytana.Width) / 2);
            }
}

        private void button_odkryj_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(textBox3.Text) * 2 > bmp.Height * bmp.Width)
            {
                MessageBox.Show("Obraz nie mieści tylu danych." + Environment.NewLine +
                    "Maksymalna ilość znaków: " + (bmp.Height * bmp.Width) / 2);
            }
            else
            {
                int y = Convert.ToInt32(textBox3.Text) * 2;
                double z = (double)y / bmp.Height;
                int x = (int)Math.Ceiling(z);
                if (y > bmp.Height)
                    y = bmp.Height;
                if (x > bmp.Width)
                    x = bmp.Width;
                StringBuilder bulid = new StringBuilder();

                int a = 0, p = 0;

                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        if ((j + p) % 2 == 0)
                        {
                            a += 64 * (bmp.GetPixel(i, j).R % 2);
                            a += 32 * (bmp.GetPixel(i, j).G % 2);
                            a += 16 * (bmp.GetPixel(i, j).B % 2);
                        }
                        else
                        {
                            a += 8 * (bmp.GetPixel(i, j).A % 2);
                            a += 4 * (bmp.GetPixel(i, j).R % 2);
                            a += 2 * (bmp.GetPixel(i, j).G % 2);
                            a += 1 * (bmp.GetPixel(i, j).B % 2);

                            bulid.Append((char)a);
                            a = 0;
                        }
                    }
                    if (i % 2 == 0 && bmp.Height % 2 == 1)
                        p = 1;
                    else
                        p = 0;

                    if (i + 1 == x - 1)
                    {
                        if (((Convert.ToInt32(textBox3.Text) * 2) % y) == 0)
                        { }
                        else
                            y = (Convert.ToInt32(textBox3.Text) * 2) % y;
                    }
                }

                textBox2.Text = Convert.ToString(bulid);
            }
        }

        private void button_wczytaj2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            string s = "";
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox2.Load(openFileDialog1.FileName);
                s = openFileDialog1.FileName;
            }

            bmp = new Bitmap(s);
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button_zapisz2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image.Save(saveFileDialog1.FileName);
            }
        }
    }
}
