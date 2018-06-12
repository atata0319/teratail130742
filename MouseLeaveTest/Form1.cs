using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MouseLeaveTest
{
    public partial class Form1 : Form
    {
        private bool flag = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panel1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel1.Visible = true;
            timer1.Start();
        }

        private void panel1_MouseEnter(object sender, EventArgs e)
        {
            Debug.Print("panel1_MouseEnter");
            flag = true;
        }

        private void panel1_MouseLeave(object sender, EventArgs e)
        {
            Debug.Print("panel1_MouseLeave");
            if (!panel1.ClientRectangle.Contains(panel1.PointToClient(Cursor.Position)))
            {
                flag = false;
                panel1.Visible = false;
                timer1.Stop();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (panel1.ClientRectangle.Contains(panel1.PointToClient(Cursor.Position)))
            {
                if (!flag)
                {
                    flag = true;
                }
            }
            else
            {
                if (flag)
                {
                    flag = false;
                    panel1.Visible = false;
                    timer1.Stop();
                }
            }
        }
    }
}
