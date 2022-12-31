using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Functions.Program;

namespace ClipBot
{
    public partial class Form2 : Form
    {
        public Form2(string gameName, string amount)
        {
            void Form2_Shown(Object sender, EventArgs e)
            {
                balls(gameName, amount);
                this.Close();
            }
            InitializeComponent();
            Shown += Form2_Shown;
        }

        

        private void balls(string gameName, string amount)
        {
            GetClipLinks(gameName, amount);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
