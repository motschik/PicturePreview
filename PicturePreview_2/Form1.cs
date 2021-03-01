using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace PicturePreview_2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Size = new Size(300, 300);
            pictureBox1.Location = new Point(0, 24);
            ClientSize = pictureBox1.Size + new Size(0, 24);
            pictureBox1.AllowDrop = true;
            setInitialText();

            //saveFileDialog1.FileName = "*.csv";
            saveFileDialog1.Filter = "CSV(*.csv)|*.csv|PNG(*.png)|*.png";
            saveFileDialog1.RestoreDirectory = true;
            openFileDialog1.Filter = "CSV(*.csv)|*.csv|PNG(*.png)|*.png|すべてのファイル(*.*)|*.*";
            openFileDialog1.RestoreDirectory = true;
        }

        Bitmap img;
        private bool IsImage = false;

        private void openFile(string fileName)
        {
            if (Path.GetExtension(fileName).Equals(".csv"))
            {
                try
                {
                    var csv = getCsv(fileName);
                    setPicture(csv);
                    IsImage = true;
                }
                catch (Exception ee)
                {
                    MessageBox.Show("ファイルが開けません。");
                }
            }
            else
            {
                //pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
                try
                {
                    pictureBox1.Image = Image.FromFile(fileName);
                    IsImage = true;
                    this.ClientSize = pictureBox1.Image.Size + new Size(0, 24);
                }
                catch (Exception ee)
                {
                    MessageBox.Show("ファイルが開けません。");
                }
            }
        }

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var str = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            openFile(str[0]);
        }

        private Mono getCsv(string fileName)
        {
            var ans = new Mono();
            string[] lines = File.ReadAllLines(fileName);

            try
            {
                foreach (var line in lines)
                {
                    
                    var tmp = new List<int>();
                    foreach (var col in line.Split(','))
                    {
                        int n;
                        double s;
                        if(Double.TryParse(col, out s))
                        {
                            if (s < 0)
                                tmp.Add(0);
                            else if (255 < s)
                                tmp.Add(255);
                            else
                                tmp.Add((int)s);
                        }
                        else
                            tmp.Add(0);
                    }
                    ans.Data.Add(tmp);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }

            return ans;
        }

        private void normalize(Mono data)
        {
            int max = 0;
            foreach(var row in data.Data)
            {
                foreach(var col in row)
                {
                    if (max < col) max = col;
                }
            }
            foreach (var row in data.Data)
            {
                for(int i = 0;i < row.Count; i++)
                {
                    row[i] = (int)((double)row[i] / max * 255);
                }
            }
        }

        private void unsigning(Mono data)
        {
            foreach (var row in data.Data)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    if (row[i] < 0)
                        row[i] = 0;
                }
            }
        }

        private void setPicture(Mono data)
        {
            img = new Bitmap(data.Width, data.Height);

            //normalize(data);
            //unsigning(data);

            int i=0, j=0;
            foreach(var row in data.Data)
            {
                i = 0;
                foreach(var col in row)
                {
                    img.SetPixel(i, j, Color.FromArgb(col, col, col));
                    i++;
                }
                j++;
            }
            //pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox1.Image = img;
            this.ClientSize = pictureBox1.Image.Size + new Size(0, 24);
        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                //ドラッグされたデータ形式を調べ、ファイルのときはコピーとする
                e.Effect = DragDropEffects.Copy;
            else
                //ファイル以外は受け付けない
                e.Effect = DragDropEffects.None;
        }

        private void setInitialText()
        {
            var canvas = new Bitmap(300, 300);
            var g = Graphics.FromImage(canvas);
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            var fnt = new Font("ＭＳ ゴシック", 10);
            g.DrawString("csvファイルをDrag & Dropしてください。", fnt, Brushes.Gray, 10, 120);
            pictureBox1.Image = canvas;
        }

        private void クリアToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap(300, 300);
            pictureBox1.Size = new Size(300, 300);
            setInitialText();
            ClientSize = pictureBox1.Size + new Size(0, 24);
            IsImage = false;
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Size = ClientSize - new Size(0, 24);
        }

        private void csvファイルで保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!IsImage)
            {
                MessageBox.Show("ファイルが開かれていません。");
                return;
            }
            
            var result = saveFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (saveFileDialog1.FilterIndex == 1)
                {
                    using (var sw = new StreamWriter(saveFileDialog1.FileName))
                    {
                        var img = new Bitmap(pictureBox1.Image);
                        for (int i = 0; i < img.Height; i++)
                        {
                            int j;
                            for (j = 0; j < img.Width - 1; j++)
                            {
                                sw.Write(((int)(img.GetPixel(j, i).GetBrightness() * 255)).ToString() + ",");
                            }
                            sw.Write(((int)(img.GetPixel(j, i).GetBrightness() * 255)).ToString());
                            if (i < img.Height - 1)
                                sw.Write("\n");
                        }
                    }
                }
                if (saveFileDialog1.FilterIndex == 2)
                {
                    pictureBox1.Image.Save(saveFileDialog1.FileName, ImageFormat.Png);
                }
            }
        }

        private void pngファイルで保存ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (!IsImage)
            {
                MessageBox.Show("ファイルが開かれていません。");
                return;
            }
            saveFileDialog2.AddExtension = true;
            saveFileDialog2.DefaultExt = ".png";
            var result = saveFileDialog2.ShowDialog();
            if (result == DialogResult.OK)
            {
                //取得したファイルパス
                pictureBox1.Image.Save(saveFileDialog2.FileName, ImageFormat.Png);
            }
        }

        private void 開くToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var result = openFileDialog1.ShowDialog();
            if(result == DialogResult.OK)
            {
                openFile(openFileDialog1.FileName);
            }
        }
    }
}
