using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AKKHelper
{
    public partial class DosyaKontrol : UserControl
    {
        public DosyaKontrol()
        {
            InitializeComponent();
        }

        private string dosyayolu;

        public string dosyaYolu
        {
            get { return linkLabel2.Text; }
            set { linkLabel2.Text = value; }
        }

    }
}
