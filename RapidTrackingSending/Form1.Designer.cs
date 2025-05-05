using System.Threading.Tasks;

namespace Oxylabs_BulkKeywords
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblIP = new System.Windows.Forms.Label();
            this.results = new System.Windows.Forms.TextBox();
            this.date_picker = new System.Windows.Forms.DateTimePicker();
            this.errorList = new System.Windows.Forms.ListBox();
            this.progress_lbl = new System.Windows.Forms.Label();
            this.worklist = new System.Windows.Forms.ListBox();
            this.lblCount = new System.Windows.Forms.Label();
            this.rd_lbl = new System.Windows.Forms.Label();
            this.rnd_lbl = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblIP
            // 
            this.lblIP.AutoSize = true;
            this.lblIP.Location = new System.Drawing.Point(701, 17);
            this.lblIP.Name = "lblIP";
            this.lblIP.Size = new System.Drawing.Size(31, 13);
            this.lblIP.TabIndex = 15;
            this.lblIP.Text = "ip_lbl";
            // 
            // results
            // 
            this.results.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.results.Location = new System.Drawing.Point(257, 74);
            this.results.Multiline = true;
            this.results.Name = "results";
            this.results.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.results.Size = new System.Drawing.Size(660, 63);
            this.results.TabIndex = 14;
            // 
            // date_picker
            // 
            this.date_picker.CustomFormat = "dd/mm/yyyy";
            this.date_picker.Location = new System.Drawing.Point(12, 10);
            this.date_picker.Name = "date_picker";
            this.date_picker.Size = new System.Drawing.Size(228, 20);
            this.date_picker.TabIndex = 13;
            // 
            // errorList
            // 
            this.errorList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.errorList.Location = new System.Drawing.Point(257, 210);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(660, 368);
            this.errorList.TabIndex = 12;
            // 
            // progress_lbl
            // 
            this.progress_lbl.Location = new System.Drawing.Point(336, 10);
            this.progress_lbl.Name = "progress_lbl";
            this.progress_lbl.Size = new System.Drawing.Size(272, 23);
            this.progress_lbl.TabIndex = 11;
            // 
            // worklist
            // 
            this.worklist.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.worklist.HorizontalScrollbar = true;
            this.worklist.Location = new System.Drawing.Point(12, 36);
            this.worklist.Name = "worklist";
            this.worklist.Size = new System.Drawing.Size(228, 550);
            this.worklist.TabIndex = 10;
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(254, 164);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(14, 13);
            this.lblCount.TabIndex = 16;
            this.lblCount.Text = "#";
            // 
            // rd_lbl
            // 
            this.rd_lbl.AutoSize = true;
            this.rd_lbl.Location = new System.Drawing.Point(336, 58);
            this.rd_lbl.Name = "rd_lbl";
            this.rd_lbl.Size = new System.Drawing.Size(70, 13);
            this.rd_lbl.TabIndex = 17;
            this.rd_lbl.Text = "RandomTime";
            // 
            // rnd_lbl
            // 
            this.rnd_lbl.AutoSize = true;
            this.rnd_lbl.Location = new System.Drawing.Point(423, 58);
            this.rnd_lbl.Name = "rnd_lbl";
            this.rnd_lbl.Size = new System.Drawing.Size(0, 13);
            this.rnd_lbl.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(929, 599);
            this.Controls.Add(this.rnd_lbl);
            this.Controls.Add(this.rd_lbl);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.lblIP);
            this.Controls.Add(this.results);
            this.Controls.Add(this.date_picker);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.progress_lbl);
            this.Controls.Add(this.worklist);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblIP;
        private System.Windows.Forms.TextBox results;
        private System.Windows.Forms.DateTimePicker date_picker;
        private System.Windows.Forms.ListBox errorList;
        private System.Windows.Forms.Label progress_lbl;
        private System.Windows.Forms.ListBox worklist;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label rd_lbl;
        private System.Windows.Forms.Label rnd_lbl;
    }
}

