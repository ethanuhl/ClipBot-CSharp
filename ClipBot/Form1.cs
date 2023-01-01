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
        }

        public void button1_Click(object sender, EventArgs e)
        {
            DeleteVideoFiles();
            string startedAtDate = dateTimePicker1.Value.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo);
            string endedAtDate = dateTimePicker2.Value.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo);

            if (startedAtDate.Equals(DateTime.Now.ToString("yyyy-MM-dd'T'00:00:00.000000Z", DateTimeFormatInfo.InvariantInfo)) == false) 
            {
                string started_at = startedAtDate;
                string ended_at = endedAtDate;
                

                string gameName = textBox1.Text;
                string amount = numericUpDown1.Value.ToString();
                Form f2 = new Form2(gameName, amount, started_at, ended_at);
                f2.Show();
            }
            else
            {
                string started_at = null;
                string ended_at = null;
                string gameName = textBox1.Text;
                string amount = numericUpDown1.Value.ToString();
                Form f2 = new Form2(gameName, amount, started_at, ended_at);
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
        public void DeleteVideoFiles()
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
    }
}