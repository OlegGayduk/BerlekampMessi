using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace CourseWork
{
    public partial class Form2 : Form
    {
        public int load = 0;
        public string seq = "";
        public double k = 0;
        public Bitmap bmp;

        public Form2()
        {
            InitializeComponent();

            openFileDialog1.Filter = "(*.emf)|*.emf";
        }

        // Запись последовательности в файл
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (load == 1)
                {
                    this.getSeq();
                    File.WriteAllText(textBox1.Text, seq);

                    MessageBox.Show("Последовательность записана!");
                } else
                {
                    throw new Exception("Не задана последовательность!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(ex);
            }
        }

        // Получение исходной последовательности посредством анализа графического изображения (сделано для ускорения работы программы)
        private void getSeq()
        {
            int tmp = 0;

            try
            {
                using (var wr = new ImageWrapper(bmp, true))
                    foreach (var p in wr)
                    {
                        if ((int)tmp == k) break;
                        if (wr[p].R == 228 && wr[p].G == 61 && wr[p].B == 61)
                        {
                            seq += "1";
                        }
                        else
                        {
                            seq += "0";
                        }
                        tmp++;
                        progressBar1.Value = ((tmp * 100) / (int)k);
                    }

                progressBar1.Value = 0;
            } catch(Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(e);
            } finally
            {
                GC.Collect();
            }
        }

        // Выбор папки для сохранения 
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                FolderBrowserDialog FBD = new FolderBrowserDialog();
                if (FBD.ShowDialog() == DialogResult.OK) textBox1.Text = FBD.SelectedPath + "seq.emf";

            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(ex);
            } finally
            {
                GC.Collect();
            }
        }

        // Открытие существующего файла с последовательностью
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;

                seq = File.ReadAllText(openFileDialog1.FileName);

                Brlkmp brlkmp = new Brlkmp();

                if(brlkmp.seqCheck(seq) != 0)
                {
                    this.draw(seq);
                } else
                {
                    throw new Exception("Неверный формат последовательности!");
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(ex);
            } finally
            {
                GC.Collect();
            }
        }

        // Отрисовка изображения
        private void draw(string seq)
        {
            try
            {
                int tmp = 0;

                pictureBox1.Height = Convert.ToInt32(seq.Length / 1167);
                bmp = new Bitmap(1167, pictureBox1.Height);

                this.Update();

                using (var wr = new ImageWrapper(bmp))
                    foreach (var p in wr)
                    {
                        if (seq[tmp] == '1') wr.SetPixel(p, 228, 61, 61);
                        tmp++;
                        progressBar1.Value = ((tmp * 100) / seq.Length);
                    }

                progressBar1.Value = 0;

                pictureBox1.Image = bmp;

                this.Update();

                load = 1;

                MessageBox.Show("Последовательность открыта!");
            } catch(Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(e);
            } finally
            {
                GC.Collect();
            }
        }
    }
}
