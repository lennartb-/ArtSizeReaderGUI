using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArtSizeReader {
    public partial class Mainform : Form {
        public Mainform() {
            InitializeComponent();

            foreach (Control c in Controls) {
                c.Font = SystemFonts.DefaultFont;
            }
        }
    }
}
