using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ClipBot.Program;
using ClipBot;
using static ClipBot.Form2;
using System.Globalization;

namespace ClipBot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            DeleteVideoFiles();
            dateTimePicker1.Value = DateTime.Now;
            dateTimePicker2.Value = DateTime.Now;
            
        }

        public void button1_Click(object sender, EventArgs e)
        {
            DeleteVideoFiles();
            string broadcaster_id = textBox3.Text;




            string startedAtDate = dateTimePicker1.Value.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo);
            string endedAtDate = dateTimePicker2.Value.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo);

            if (startedAtDate.Equals(DateTime.Now.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo)) == false && checkBox1.Checked == true) 
            {
                string started_at = startedAtDate;
                string ended_at = endedAtDate;


                string gameName = textBox1.Text;
                string amount = numericUpDown1.Value.ToString();
                Form f2 = new Form2(gameName, amount, started_at, ended_at, broadcaster_id);
                f2.Show();
            }
            else
            {
                string started_at = null;
                string ended_at = null;
                string gameName = textBox1.Text;
                string amount = numericUpDown1.Value.ToString();
                Form f2 = new Form2(gameName, amount, started_at, ended_at, broadcaster_id);
                f2.Show();
            }
            
        }
        
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void DeleteVideoFiles()
        {
            string sourceDir = Directory.GetCurrentDirectory();
            string[] mp4List = Directory.GetFiles(sourceDir, "*.mp4");

            foreach (string f in mp4List)
            {
                if (!f.Equals("edited.mp4"))
                {
                    File.Delete(f);
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox1.Checked)
            {
                dateTimePicker1.Visible = true;
                dateTimePicker2.Visible = true;
                button1.Location = new System.Drawing.Point(39, 129);
                this.Size = new Size(342, 216);
                label1.Visible = true;
            }
            else
            {
                dateTimePicker1.Visible = false;
                dateTimePicker2.Visible = false;
                button1.Location = new System.Drawing.Point(39, 91);
                this.Size = new Size(342, 191);
                label1.Visible = false;
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}