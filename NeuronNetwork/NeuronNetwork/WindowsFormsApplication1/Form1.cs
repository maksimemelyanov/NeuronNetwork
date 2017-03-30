using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using NeuronNetwork;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Input = 0;
            Output = 0;
            Layers = new List<int>();
            categories = new List<string>();
            UpdateInfo();
        }

        public NeuronNetwork.NeuronNetwork n;
        public int Input;
        public int Output;
        public List<int> Layers;
        public List<string> categories;

        public List<double> NormalizeImage(Bitmap bmp)
        {
            List<double> result = new List<double>();
            int x = bmp.Width;
            int y = bmp.Height;
            long totalR = 0;
            long totalG = 0;
            long totalB = 0;
            List<List<double>> gray = new List<List<double>>();
            double totalT = 0;
            for (int j = 0; j < y; j++)
            {
                List<double> row = new List<double>();
                for (int i = 0; i < x; i++)
                {
                    byte R = bmp.GetPixel(i, j).R;
                    byte G = bmp.GetPixel(i, j).G;
                    byte B = bmp.GetPixel(i, j).B;

                    totalR += R;
                    totalG += G;
                    totalB += B;
                    row.Add((0.229 * R + 0.587 * G + 0.114 * B) / 255);
                    totalT += ((0.229 * R + 0.587 * G + 0.114 * B) / 255);
                }
                gray.Add(row);
            }
            totalT = totalT / (x * y);
            List<double> vh = new List<double>();
            List<double> hh = new List<double>();
            for (int j = 0; j < y; j++)
            {
                double h = 0;
                for (int i = 0; i < x; i++)
                {
                    if (gray[j][i] > totalT)
                        h += gray[j][i];
                }
                hh.Add(h / x);
            }


            for (int i = 0; i < x; i++)
            {
                double v = 0;
                for (int j = 0; j < y; j++)
                {
                    if (gray[i][j] > totalT)
                        v += gray[i][j];
                }
                vh.Add(v / y);
            }
            result.Add(totalR / (x * y * 255));
            result.Add(totalG / (x * y * 255));
            result.Add(totalB / (x * y * 255));
            result.AddRange(vh);
            result.AddRange(hh);
            return result;
        }






        private void button1_Click(object sender, EventArgs e)
        {

            List<Tuple<List<double>, List<double>>> inputs = new List<Tuple<List<double>, List<double>>>();

            for (int k = 0; k < 50; k++)
            {
                Bitmap b = new Bitmap("1/" + k + ".png");
                List<double> im = NormalizeImage(b);
                List<double> res = new List<double>();
                for (int r = 0; r < 5; r++)
                {
                    if (k % 5 == r)
                        res.Add(1);
                    else
                    {
                        res.Add(0);
                    }
                }
                inputs.Add(new Tuple<List<double>, List<double>>(im, res));
            }


            n = new NeuronNetwork.NeuronNetwork(new List<int>() { 83, 120, 30, 5 }, new List<string>());

            n.Studying(inputs, (int)numericUpDown2.Value);


            MessageBox.Show("Я обучилась!");
        }

      

   

        private void button5_Click(object sender, EventArgs e)
        {
            listBox2.Items.Remove(listBox2.SelectedItem);
            UpdateInfo();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            listBox2.Items.Add(textBox3.Text);
            UpdateInfo();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            switch (comboBox1.Text)
            {
                case "40*40":
                    Input = 83;
                    break;
                default:
                    Input = 0;
                    break;
            }
            label6.Text = Input.ToString();
            Output = listBox2.Items.Count;
            label9.Text = Output.ToString();
            int layers_count = (int)numericUpDown1.Value;
            Layers = new List<int>();
            for (int i = 0; i < layers_count; i++)
            {
                Layers.Add((int)(Input * 1.5));
            }
            label7.Text = Layers.Count.ToString();
            if (n == null)
            {
                button7.Enabled = false;
                button9.Enabled = false;
                button10.Enabled = false;
            }
            else
            {
                button7.Enabled = true;
                button9.Enabled = true;
                button10.Enabled = true;
            }

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            UpdateInfo();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<int> all_layers = new List<int>();
            all_layers.Add(Input);
            all_layers.AddRange(Layers);
            all_layers.Add(Output);
            List<string> cat = new List<string>();
            foreach (var c in listBox2.Items)
            {
                cat.Add(c.ToString());
            }
            n = new NeuronNetwork.NeuronNetwork(all_layers, cat);
            categories = cat;
            (dataGridView1.Columns[1] as DataGridViewComboBoxColumn).Items.Clear();
            foreach (var c in categories)
                (dataGridView1.Columns[1] as DataGridViewComboBoxColumn).Items.Add(c);
            UpdateInfo();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (n != null)
            {
                SaveFileDialog sf = new SaveFileDialog();
                sf.Filter = "Двоичные файлы | *.bin"; ;
                sf.ShowDialog();
                if (sf.FileName != "")
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    using (FileStream fs = new FileStream(sf.FileName, FileMode.OpenOrCreate))
                    {
                        formatter.Serialize(fs, n);
                    }
                }
            }
            else
            {
                MessageBox.Show("Нет сети для сохранения!", "Ошибка!");
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {

            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = true;
            op.ShowDialog();
            List<string> files = op.FileNames.ToList();
            int c = 0;
            foreach (var f in files)
            {
                try
                {
                    List<string> p = f.Split('_').ToList();
                    string[] sep = new string[1];
                    sep[0] = "\\";
                    p = (p[0].Split(sep, StringSplitOptions.RemoveEmptyEntries)).ToList();
                    c = int.Parse(p.Last());

                }
                catch (Exception)
                {

                }
                finally
                {
                    DataGridViewRow dgvr = new DataGridViewRow();
                    DataGridViewTextBoxCell d1 = new DataGridViewTextBoxCell();
                    d1.Value = f;
                    DataGridViewComboBoxCell d2 = new DataGridViewComboBoxCell();
                    foreach (var k in categories)
                        d2.Items.Add(k);
                    d2.Value = categories[c];
                    dgvr.Cells.Add(d1);
                    dgvr.Cells.Add(d2);
                    dataGridView1.Rows.Add(dgvr);

                }


            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string f = dataGridView1.SelectedCells[0].Value.ToString();
                Bitmap b = new Bitmap(f);
                pictureBox1.Image = b;
            }
            catch (Exception)
            {

            }


        }

        private void button10_Click(object sender, EventArgs e)
        {
            List<Tuple<List<double>, List<double>>> inputs = new List<Tuple<List<double>, List<double>>>();
            foreach (DataGridViewRow d in dataGridView1.Rows)
            {
                if (d.Cells[0].Value != null)
                {
                    Bitmap b = new Bitmap(d.Cells[0].Value.ToString());
                    List<double> im = NormalizeImage(b);
                    List<double> res = new List<double>();
                    int q = categories.IndexOf(d.Cells[1].Value.ToString());
                    for (int r = 0; r < categories.Count; r++)
                    {
                        if (q == r)
                            res.Add(1);
                        else
                        {
                            res.Add(0);
                        }
                    }
                    inputs.Add(new Tuple<List<double>, List<double>>(im, res));
                }
            }
            Stopwatch st = new Stopwatch();
            st.Start();
            n.Studying(inputs, (int)numericUpDown2.Value);
            st.Stop();
            long time = st.ElapsedMilliseconds;
            time = time / 1000;
            MessageBox.Show("Обучение выполнено за " + time + " сек.", "Обучение окончено");

        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Multiselect = true;
            op.ShowDialog();
            List<string> files = op.FileNames.ToList();
            foreach (var f in files)
            {
                DataGridViewRow dgvr = new DataGridViewRow();
                DataGridViewTextBoxCell d1 = new DataGridViewTextBoxCell();
                d1.Value = f;
                DataGridViewComboBoxCell d2 = new DataGridViewComboBoxCell();
                DataGridViewComboBoxCell d3 = new DataGridViewComboBoxCell();
                foreach (var k in categories)
                {
                    d2.Items.Add(k);
                    d3.Items.Add(k);
                }
                    
                d2.Value = categories[0];
                d3.Value = categories[0];
                dgvr.Cells.Add(d1);
                dgvr.Cells.Add(d2);
                dgvr.Cells.Add(d3);
                dataGridView2.Rows.Add(dgvr);
            }


            foreach (DataGridViewRow d in dataGridView2.Rows)
            {
                if (d.Cells[0].Value != null)
                {
                    Bitmap b = new Bitmap(d.Cells[0].Value.ToString());
                    List<double> im = NormalizeImage(b);
                    n.SetStart(im);
                    n.Calculate();
                    List<double> res = n.GetResult();
                    int c = 0;
                    for (int r = 0; r < res.Count; r++)
                    {
                        if (res[r] == 1)
                            c = r;
                    }
                    d.Cells[1].Value = categories[c];
                }
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            List<Tuple<List<double>, List<double>>> inputs = new List<Tuple<List<double>, List<double>>>();
            foreach (DataGridViewRow d in dataGridView2.Rows)
            {
                if (d.Cells[0].Value != null)
                {
                    Bitmap b = new Bitmap(d.Cells[0].Value.ToString());
                    List<double> im = NormalizeImage(b);
                    List<double> res = new List<double>();
                    int q = categories.IndexOf(d.Cells[2].Value.ToString());
                    for (int r = 0; r < categories.Count; r++)
                    {
                        if (q == r)
                            res.Add(1);
                        else
                        {
                            res.Add(0);
                        }
                    }
                    inputs.Add(new Tuple<List<double>, List<double>>(im, res));
                }
            }
            Stopwatch st = new Stopwatch();
            st.Start();
            n.Studying(inputs, (int)numericUpDown2.Value);
            st.Stop();
            long time = st.ElapsedMilliseconds;
            time = time / 1000;
            MessageBox.Show("Обучение выполнено за " + time + " сек.", "Обучение окончено");
        }

        private void tabPage3_Click(object sender, EventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                string f = dataGridView2.SelectedCells[0].Value.ToString();
                Bitmap b = new Bitmap(f);
                pictureBox2.Image = b;
            }
            catch (Exception)
            {

            }

        }

        private void button8_Click(object sender, EventArgs e)
        {
            OpenFileDialog sf = new OpenFileDialog();
            sf.Filter = "Двоичные файлы | *.bin"; ;
            sf.ShowDialog();
            if (sf.FileName != "")
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (var stream = new FileStream(sf.FileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    n = (NeuronNetwork.NeuronNetwork)formatter.Deserialize(stream);
                }
                Input = n.Input.Count;
                categories = n.Categories;
                listBox2.Items.Clear();
                listBox2.Items.AddRange(categories.ToArray());
                UpdateInfo();
            }
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            dataGridView2.Rows.Clear();
        }



    }
}
