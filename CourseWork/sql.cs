using System;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data.Common;

namespace CourseWork
{
    class sql
    {
        // Получение полинома из базы данных
        public string getPolynom(int rate)
        {
            SqlConnection conn = null; 

            try
            {
                conn = new SqlConnection("server=localhost;user=sa;database=polynoms;password=1;");

                conn.Open();

                string sql = "SELECT polynom FROM polynoms WHERE rate=" + rate;

                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = sql;

                Brlkmp form = new Brlkmp();

                using (DbDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            return reader.GetString(reader.GetOrdinal("polynom"));
                        }
                    }
                    else
                    {
                        throw new Exception("Результаты не найдены!");
                    }
                }
            } catch(Exception e) {
                MessageBox.Show(e.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                Brlkmp form = new Brlkmp();
                form.logWrite(e);
            } finally
            {
                conn.Close();
                conn.Dispose();
            }

            return "";
        }
    }
}
