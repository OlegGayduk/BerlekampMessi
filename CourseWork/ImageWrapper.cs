using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace CourseWork
{
    public class ImageWrapper : IDisposable, IEnumerable<Point>
    {
        // Ширина изображения
        public int Width { get; private set; }

        // Высота изображения
        public int Height { get; private set; }

        // Цвет по-умолачнию (используется при выходе координат за пределы изображения)
        public Color DefaultColor { get; set; }

        private byte[] data; // буфер исходного изображения
        private byte[] outData;// выходной буфер
        private int stride;
        private BitmapData bmpData;
        private Bitmap bmp;

        // Создание обертки поверх bitmap
        public ImageWrapper(Bitmap bmp, bool copySourceToOutput = false)
        {
            try
            {
                Width = bmp.Width;
                Height = bmp.Height;
                this.bmp = bmp;

                bmpData = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
                stride = bmpData.Stride;

                data = new byte[stride * Height];
                System.Runtime.InteropServices.Marshal.Copy(bmpData.Scan0, data, 0, data.Length);

                outData = copySourceToOutput ? (byte[])data.Clone() : new byte[stride * Height];
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

        // Возвращает пиксел из исходнго изображения, либо заносит пиксел в выходной буфер
        public Color this[int x, int y]
        {
            get
            {
                try
                {
                    var i = GetIndex(x, y);
                    return i < 0 ? DefaultColor : Color.FromArgb(data[i + 3], data[i + 2], data[i + 1], data[i]);
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Brlkmp form = new Brlkmp();
                    form.logWrite(e);
                    return DefaultColor;
                }
            }

            set
            {
                try
                {
                    var i = GetIndex(x, y);
                    if (i >= 0)
                    {
                        outData[i] = value.B;
                        outData[i + 1] = value.G;
                        outData[i + 2] = value.R;
                        outData[i + 3] = value.A;
                    };
                } catch(Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                    Brlkmp form = new Brlkmp();
                    form.logWrite(e);
                }
            }
        }

        // Возвращает пиксел из исходнго изображения, либо заносит пиксел в выходной буфер
        public Color this[Point p]
        {
            get { return this[p.X, p.Y]; }
            set { this[p.X, p.Y] = value; }
        }

        // Заносит в выходной буфер значение цвета, заданные в double.
        // Допускает выход double за пределы 0-255.
        public void SetPixel(Point p, double r, double g, double b)
        {
            try
            {
                if (r < 0) r = 0;
                if (r >= 256) r = 255;
                if (g < 0) g = 0;
                if (g >= 256) g = 255;
                if (b < 0) b = 0;
                if (b >= 256) b = 255;

                this[p.X, p.Y] = Color.FromArgb((int)r, (int)g, (int)b);
            } catch(Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(e);
            }
        }

        int GetIndex(int x, int y)
        {
            return (x < 0 || x >= Width || y < 0 || y >= Height) ? -1 : x * 4 + y * stride;
        }

        // Заносит в bitmap выходной буфер и снимает лок.
        // Этот метод обязателен к исполнению (либо явно, лмбо через using)
        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.Copy(outData, 0, bmpData.Scan0, outData.Length);
            bmp.UnlockBits(bmpData);
        }

        // Перечисление всех точек изображения
        public IEnumerator<Point> GetEnumerator()
        {
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    yield return new Point(x, y);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        // Меняет местами входной и выходной буферы
        public void SwapBuffers()
        {
            var temp = data;
            data = outData;
            outData = temp;
        }
    }
}
