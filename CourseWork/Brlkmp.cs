using System;
using System.Windows.Forms;

namespace CourseWork
{
    public partial class Brlkmp : Form
    {
        private byte[] s;

        public Brlkmp()
        {
            InitializeComponent();

            openFileDialog1.Filter = "(*.emf)|*.emf";
        }

        // Проверка символов последовательности на соответствие только 0 и 1
        public int seqCheck(string text)
        {
            try
            {
                for (int i = 0; i < text.Length; i++) if ((text[i] != '0') && (text[i] != '1')) return 0;

                return 1;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
                return 0;
            }
        }

        // Запись текстовой последовательности в байтовый массив для последующей работы
        private void seqDef(string text)
        {
            try
            {
                s = new byte[text.Length];

                for (int i = 0; i < text.Length; i++) s[i] = byte.Parse(Convert.ToString(text[i]));

            } catch(Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
            }
        }

        // Функция перевода в 10-ую систему счисления 
        private int dec(string text)
        {
            try
            {
                int s = 0;

                foreach (var c in text)
                {
                    s <<= 1;
                    s += c - '0';
                }

                return s;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
                return 0;
            }
        }

        // Функция подсчета алгоритма Берлекэмпа-Месси
        private void brlkmp(byte[] s)
        {
            try
            {
                int L, N, m, d;
                int n = s.Length;
                byte[] c = new byte[n];
                byte[] b = new byte[n];
                byte[] t = new byte[n];

                // Инициализация
                b[0] = c[0] = 1;
                N = L = 0;
                m = -1;

                //Ядро алгоритма
                while (N < n)
                {
                    d = (int)s[N];
                    for (int i = 1; i <= L; i++) d ^= c[i] & s[N - i];            //(d+=c[i]*s[N-i] mod 2)
                    if (d == 1)
                    {
                        Array.Copy(c, t, n);    //T(D)<-C(D)
                        for (int i = 0; (i + N - m) < n; i++) c[i + N - m] ^= b[i];
                        if (L <= (N >> 1))
                        {
                            L = N + 1 - L;
                            m = N;
                            Array.Copy(t, b, n);    //B(D)<-T(D)
                        }
                    }
                    N++;
                }

                this.print(L, c);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
            }
        }

        // Распечатка результатов алгоритма Берклекэмпа-Месси
        private void print(int L, byte[] c)
        {
            try
            {
                textBox2.Text = Convert.ToString(L);

                int a = 0;
                int l = 0;
                int y = 0;
                int[] S = new int[c.Length];

                for (int i = (c.Length - 1); i >= 0; i--)
                {
                    if (i > y)
                    {
                        if (c[i] == 1)
                        {
                            y = i;
                            S[a++] = l++;
                        }
                    }
                    else
                    {
                        if (c[i] == 1) S[a++] = l;
                        l++;
                    }
                }

                for (int i = l - 1; i >= 0; i--)
                {
                    if (S[i] > 0 && S[i] != 1)
                    {
                        textBox3.Text += "x^" + S[i] + " + ";
                    }
                    else if (S[i] > 0 && S[i] == 1)
                    {
                        textBox3.Text += "x + ";
                    }
                }

                textBox3.Text += "1";
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(e);
            }
        }


        // Обработчки кликов на кнопки
        // Кнопка подсчета Берклекэмпа-Месси
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                textBox2.Text = "";
                textBox3.Text = "";

                if (s.Length != 0) this.brlkmp(s);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(ex);
            }
        }

        // Кнопка подсчета регистра сдвига
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check(textBox4) == 0) return;
                if (this.check(textBox5) == 0) return;

                int res = this.dec(textBox4.Text);

                if (res != 0)
                {
                    Form2 form = new Form2();
                    if (this.lfsr(Math.Pow(2, textBox4.Text.Length) - 1, Convert.ToByte(textBox4.Text.Length - 1), res, form) == 0) throw new Exception("Ошибка при подсчете последовательности! Попробуйте снова!");
                }
                else
                {
                    throw new Exception("Ошибка при преобразовании строки! Попробуйте снова!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(ex);
            } finally
            {
                GC.Collect();
            }
        }

        // Кнопка для открытия текстового редактора
        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            form.Show();
        }

        // Кнопка для получения полинома из базы
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.check(textBox4) == 0)
                {
                    MessageBox.Show("Не задано начальное состояние!", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    return;
                }

                sql sql = new sql();

                string res = sql.getPolynom(textBox4.Text.Length);

                if (res != "")
                {
                    textBox5.Text = res;
                } else
                {
                    throw new Exception("Искомый полином не найден! Попробуйте еще раз!");
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(ex);
            } finally
            {
                GC.Collect();
            }
        }

        
        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (openFileDialog1.ShowDialog() == DialogResult.Cancel) return;

                string fileText = System.IO.File.ReadAllText(openFileDialog1.FileName);

                if(this.seqCheck(fileText) != 0) { 
                    this.seqDef(fileText);

                    MessageBox.Show("Последовательность открыта!", "Успех!", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    textBox1.Text = openFileDialog1.FileName;
                } else
                {
                    throw new Exception("Неверный формат последовательности!");
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                this.logWrite(ex);
            }
        }
    }
}
