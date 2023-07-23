using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Diagnostics;
using System.Data.SqlClient;
namespace Process_Management
{
    public partial class Form1 : Form
    {
        int i = 0;
        public Form1()
        {
            InitializeComponent();
        }
        string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=D:\c#projects\Process_Management\result_db.mdf;Integrated Security=True;Connect Timeout=30";
        HashSet<string> uniqueItems = new HashSet<string>();
        private void Form1_Load(object sender, EventArgs e)
        {
            //label2.Text = Environment.UserName;
            using(SqlConnection con = new SqlConnection(conString))
            {
                con.Open();
                string query = "SELECT app_name,start_time,start_date,username FROM activity";
                SqlCommand command = new SqlCommand(query, con);
                SqlDataReader reder = command.ExecuteReader();
                while (reder.Read())
                {
                    dataGridView3.Rows.Add(reder["app_name"],reder["start_time"],reder["start_date"],reder["username"]);
                }
                reder.Close();
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            
            dataGridView1.Rows.Clear();
            try
            {
                Process[] processes = Process.GetProcesses();
                foreach (Process process in processes)
                {
                    if (process.MainWindowHandle != IntPtr.Zero)
                    {

                        //  comboBox1.Items.Add(process.ProcessName+process.StartTime );
                        dataGridView1.Rows.Add(process.ProcessName, process.StartTime.ToString("hh:mm:ss"), process.StartTime.ToString("yyyy-MM-dd"), Environment.UserName);
                    }

                }
            }
            catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
            
            i = 1;
            timer1.Interval = 5000;
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            if (i == 0)
            {
                timer1.Interval = 1000;
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            timer1.Stop();
            try
            {
                using (SqlConnection con = new SqlConnection(conString))
                {
                    con.Open();

                    foreach (DataGridViewRow row in dataGridView2.Rows)
                    {

                        if (!row.IsNewRow && row.Visible)
                        {
                            string data1 = row.Cells[0].Value.ToString();
                            string data2 = row.Cells[1].Value.ToString();
                            string data3 = row.Cells[2].Value.ToString();
                            string data4 = row.Cells[3].Value.ToString();
                            string query = $"INSERT INTO activity(app_name,start_time,start_date,username) VALUES('{data1}','{data2}','{data3}','{data4}')";
                            SqlCommand command = new SqlCommand(query, con);
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }catch(Exception x)
            {
                MessageBox.Show(x.Message);
            }
            Form1_Load(sender, e);
        }


        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
          
            foreach(DataGridViewRow row in dataGridView1.Rows)
            {
                if (!row.IsNewRow && row.Visible)
                {
                    string data1 = row.Cells[0].Value.ToString();
                    if (!uniqueItems.Contains(data1))
                    {
                        string data2 = row.Cells[1].Value.ToString();
                        string data3 = row.Cells[2].Value.ToString();
                        string data4 = row.Cells[3].Value.ToString();
                        dataGridView2.Rows.Add(data1, data2,data3,data4);
                        uniqueItems.Add(data1);
                    }
                }
            }
        }
    }
}
