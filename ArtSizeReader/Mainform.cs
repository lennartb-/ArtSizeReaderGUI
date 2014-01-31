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

        private string[] ParseOptionsIntoArgs() {
            List<String> options = new List<String>();
            options.AddMany("--input", InputTextBox.Text);
            options.AddMany("--threshold", ThresholdWidthTextBox.Text + "x" + ThresholdHeightTextBox.Text);
            options.AddMany("--max-threshold", MaxThresholdWidthTextBox.Text + "x" + MaxThresholdHeightTextBox.Text);
            options.AddMany("--size", MaxFilesizeTextBox.Text);            
            options.AddMany("--logfile", LogfilePathTextbox.Text);
            options.AddMany("--playlist", PlaylistPathTextbox.Text);
            
            if(WithRatio.Checked){
                options.Add("--ratio");
            }

            return options.ToArray();
        }

        private void AnalyzeButton_Click(object sender, EventArgs e) {
            Program.ParseOptions(ParseOptionsIntoArgs());
        }
    }
    /// <summary>
    /// From: http://answers.unity3d.com/questions/524128/c-adding-multiple-elements-to-a-list-on-one-line.html
    /// </summary>
    public static class ListExtenstions {
        public static void AddMany<T>(this List<T> list, params T[] elements) {
            for (int i = 0; i < elements.Length; i++) {
                list.Add(elements[i]);
            }
        }
    }
}
