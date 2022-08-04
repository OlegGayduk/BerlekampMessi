using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace CourseWork
{
    public partial class Brlkmp : Form
    {
        public string seq = "";

        // Функция для записи log файла
        public void logWrite(Exception e)
        {
            string path = @"G:\C#\WindowsFormsApp2\log.txt"; //путь для записи

            try
            {
                StreamWriter sw = new StreamWriter(path, true); // открываем поток для записи
                sw.WriteLine(Convert.ToString(e.Message) + " " + DateTime.UtcNow); // записываем новую строку

                sw.Close(); // закрываем поток
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } finally
            {
                GC.Collect();
            }
        }

        // Функция подсчета регистра сдвига
        private int lfsr(double k, byte length, int s, Form2 form)
        {
            try
            {
                form.Show();

                int x = 0;
                int y = 0;

                int d = 0;

                byte[] n = this.polynom();

                if (k >= 1167)
                {
                    form.pictureBox1.Height = Convert.ToInt32(k * n.Length / 1167 + 1);
                } else
                {
                    form.pictureBox1.Height = 1;
                }

                form.bmp = new Bitmap(1167, form.pictureBox1.Height);

                form.Update();

                using (var wr = new ImageWrapper(form.bmp))

                for (int i = 0; i < k; i++)
                {
                    d = s & 1;

                    for (byte j = 0; j < n.Length; j++)
                    {
                        if (d == 1) wr.SetPixel(new Point(x, y), 228, 61, 61);

                        x++;
                        if (x == 1168)
                        {
                            x = 0;
                            y++;
                        }

                        d ^= (s >> n[j]) & 1;
                    }

                    s = (d << length) | (s >> 1);
                }

                form.pictureBox1.Image = form.bmp;

                form.load = 1;
                form.k = k;

                return 1;
            }
            catch (Exception e)
            {
                form.Close();
                MessageBox.Show("" + e + "", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
                return 0;
            } finally
            {
                GC.Collect();
            }
        }

        // Получение коэффициентов полинома
        private byte[] polynom()
        {
            try
            {
                byte[] s = new byte[0];

                List<byte> itemsList = s.ToList<byte>();

                for (int i = (textBox5.Text.Length - 1); i >= 0; i--)
                {
                    if (textBox5.Text[i] == '1') if ((textBox5.Text.Length - i - 1) > 0 && i != 0) itemsList.Add(Convert.ToByte(textBox5.Text.Length - i - 1));
                }

                return itemsList.ToArray();
            } catch(Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
                return null;
            } finally
            {
                GC.Collect();
            }
        }

        // Проверка текстовых полей
        public byte check(TextBox box)
        {
            if (box.Text.Length == 0)
            {
                MessageBox.Show("Заполните все поля!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return 0;
            }

            if (this.seqCheck(box.Text) == 0)
            {
                MessageBox.Show("Неверный формат входных данных!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return 0;
            }

            return 1;
        }
    }
}

