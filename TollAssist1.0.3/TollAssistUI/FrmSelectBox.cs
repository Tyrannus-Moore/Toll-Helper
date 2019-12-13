using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TollAssistUI
{
    public partial class FrmSelectBox : Form
    {
        public FrmSelectBox()
        {
            InitializeComponent();
        }

        
        private void FrmSelectBox_Load(object sender, EventArgs e)
        {
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.TransparencyKey = Color.White;
        }

        Pen p = new Pen(Brushes.Black, 4);
        private void FrmSelectBox_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Color.White);
            g.DrawRectangle(p, this.ClientRectangle);
        }
    }
}
