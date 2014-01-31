namespace ArtSizeReader {
    partial class Mainform {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.InputTextBox = new System.Windows.Forms.TextBox();
            this.ResultDataGridView = new System.Windows.Forms.DataGridView();
            this.WithRatio = new System.Windows.Forms.CheckBox();
            this.ThresholdWidthTextBox = new System.Windows.Forms.TextBox();
            this.ThresholdHeightTextBox = new System.Windows.Forms.TextBox();
            this.MaxThresholdWidthTextBox = new System.Windows.Forms.TextBox();
            this.MaxFilesizeTextBox = new System.Windows.Forms.TextBox();
            this.MaxThresholdHeightTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.PlaylistPathTextbox = new System.Windows.Forms.TextBox();
            this.LogfilePathTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.BitrateTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.ResultDataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // InputTextBox
            // 
            this.InputTextBox.Location = new System.Drawing.Point(101, 5);
            this.InputTextBox.Name = "InputTextBox";
            this.InputTextBox.Size = new System.Drawing.Size(452, 20);
            this.InputTextBox.TabIndex = 0;
            // 
            // ResultDataGridView
            // 
            this.ResultDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultDataGridView.Location = new System.Drawing.Point(12, 189);
            this.ResultDataGridView.Name = "ResultDataGridView";
            this.ResultDataGridView.Size = new System.Drawing.Size(589, 259);
            this.ResultDataGridView.TabIndex = 1;
            // 
            // WithRatio
            // 
            this.WithRatio.AutoSize = true;
            this.WithRatio.Location = new System.Drawing.Point(334, 59);
            this.WithRatio.Name = "WithRatio";
            this.WithRatio.Size = new System.Drawing.Size(64, 17);
            this.WithRatio.TabIndex = 2;
            this.WithRatio.Text = "1:1 ratio";
            this.WithRatio.UseVisualStyleBackColor = true;
            // 
            // ThresholdWidthTextBox
            // 
            this.ThresholdWidthTextBox.Location = new System.Drawing.Point(101, 31);
            this.ThresholdWidthTextBox.Name = "ThresholdWidthTextBox";
            this.ThresholdWidthTextBox.Size = new System.Drawing.Size(100, 20);
            this.ThresholdWidthTextBox.TabIndex = 3;
            // 
            // ThresholdHeightTextBox
            // 
            this.ThresholdHeightTextBox.Location = new System.Drawing.Point(225, 31);
            this.ThresholdHeightTextBox.Name = "ThresholdHeightTextBox";
            this.ThresholdHeightTextBox.Size = new System.Drawing.Size(100, 20);
            this.ThresholdHeightTextBox.TabIndex = 4;
            // 
            // MaxThresholdWidthTextBox
            // 
            this.MaxThresholdWidthTextBox.Location = new System.Drawing.Point(101, 57);
            this.MaxThresholdWidthTextBox.Name = "MaxThresholdWidthTextBox";
            this.MaxThresholdWidthTextBox.Size = new System.Drawing.Size(100, 20);
            this.MaxThresholdWidthTextBox.TabIndex = 5;
            // 
            // MaxFilesizeTextBox
            // 
            this.MaxFilesizeTextBox.Location = new System.Drawing.Point(407, 31);
            this.MaxFilesizeTextBox.Name = "MaxFilesizeTextBox";
            this.MaxFilesizeTextBox.Size = new System.Drawing.Size(100, 20);
            this.MaxFilesizeTextBox.TabIndex = 6;
            // 
            // MaxThresholdHeightTextBox
            // 
            this.MaxThresholdHeightTextBox.Location = new System.Drawing.Point(225, 57);
            this.MaxThresholdHeightTextBox.Name = "MaxThresholdHeightTextBox";
            this.MaxThresholdHeightTextBox.Size = new System.Drawing.Size(100, 20);
            this.MaxThresholdHeightTextBox.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "File or Folder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Threshold:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(331, 34);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "Max. Filesize:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 60);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Max. Threshold:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 113);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "Playlist path:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 139);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(65, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "Logfile path:";
            // 
            // PlaylistPathTextbox
            // 
            this.PlaylistPathTextbox.Location = new System.Drawing.Point(88, 110);
            this.PlaylistPathTextbox.Name = "PlaylistPathTextbox";
            this.PlaylistPathTextbox.Size = new System.Drawing.Size(237, 20);
            this.PlaylistPathTextbox.TabIndex = 14;
            // 
            // LogfilePathTextbox
            // 
            this.LogfilePathTextbox.Location = new System.Drawing.Point(88, 136);
            this.LogfilePathTextbox.Name = "LogfilePathTextbox";
            this.LogfilePathTextbox.Size = new System.Drawing.Size(237, 20);
            this.LogfilePathTextbox.TabIndex = 15;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(207, 34);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(12, 13);
            this.label7.TabIndex = 16;
            this.label7.Text = "x";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(207, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "x";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(331, 113);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Min. Bitrate";
            // 
            // BitrateTextBox
            // 
            this.BitrateTextBox.Location = new System.Drawing.Point(397, 110);
            this.BitrateTextBox.Name = "BitrateTextBox";
            this.BitrateTextBox.Size = new System.Drawing.Size(100, 20);
            this.BitrateTextBox.TabIndex = 19;
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(613, 460);
            this.Controls.Add(this.BitrateTextBox);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.LogfilePathTextbox);
            this.Controls.Add(this.PlaylistPathTextbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MaxThresholdHeightTextBox);
            this.Controls.Add(this.MaxFilesizeTextBox);
            this.Controls.Add(this.MaxThresholdWidthTextBox);
            this.Controls.Add(this.ThresholdHeightTextBox);
            this.Controls.Add(this.ThresholdWidthTextBox);
            this.Controls.Add(this.WithRatio);
            this.Controls.Add(this.ResultDataGridView);
            this.Controls.Add(this.InputTextBox);
            this.Name = "Mainform";
            this.Text = "Mainform";
            ((System.ComponentModel.ISupportInitialize)(this.ResultDataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox InputTextBox;
        private System.Windows.Forms.DataGridView ResultDataGridView;
        private System.Windows.Forms.CheckBox WithRatio;
        private System.Windows.Forms.TextBox ThresholdWidthTextBox;
        private System.Windows.Forms.TextBox ThresholdHeightTextBox;
        private System.Windows.Forms.TextBox MaxThresholdWidthTextBox;
        private System.Windows.Forms.TextBox MaxFilesizeTextBox;
        private System.Windows.Forms.TextBox MaxThresholdHeightTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox PlaylistPathTextbox;
        private System.Windows.Forms.TextBox LogfilePathTextbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox BitrateTextBox;
    }
}