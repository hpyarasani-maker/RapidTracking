namespace RapidTrackingSingleThread
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
            this.results = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl1count = new System.Windows.Forms.Label();
            this.date_picker = new System.Windows.Forms.DateTimePicker();
            this.progress_lbl = new System.Windows.Forms.Label();
            this.worklist = new System.Windows.Forms.ListBox();
            this.errorList = new System.Windows.Forms.TextBox();
            this.rnd_lbl4 = new System.Windows.Forms.Label();
            this.iptxt_txt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // results
            // 
            this.results.FormattingEnabled = true;
            this.results.Location = new System.Drawing.Point(195, 94);
            this.results.Name = "results";
            this.results.Size = new System.Drawing.Size(412, 277);
            this.results.TabIndex = 31;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(587, 69);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 30;
            this.label1.Text = "#";
            // 
            // lbl1count
            // 
            this.lbl1count.AutoSize = true;
            this.lbl1count.Location = new System.Drawing.Point(581, 69);
            this.lbl1count.Name = "lbl1count";
            this.lbl1count.Size = new System.Drawing.Size(0, 13);
            this.lbl1count.TabIndex = 29;
            // 
            // date_picker
            // 
            this.date_picker.CustomFormat = "dd/mm/yyyy";
            this.date_picker.Location = new System.Drawing.Point(11, 14);
            this.date_picker.Name = "date_picker";
            this.date_picker.Size = new System.Drawing.Size(200, 20);
            this.date_picker.TabIndex = 26;
            // 
            // progress_lbl
            // 
            this.progress_lbl.Location = new System.Drawing.Point(233, 58);
            this.progress_lbl.Name = "progress_lbl";
            this.progress_lbl.Size = new System.Drawing.Size(258, 33);
            this.progress_lbl.TabIndex = 24;
            // 
            // worklist
            // 
            this.worklist.Location = new System.Drawing.Point(13, 94);
            this.worklist.Name = "worklist";
            this.worklist.Size = new System.Drawing.Size(176, 576);
            this.worklist.TabIndex = 23;
            // 
            // errorList
            // 
            this.errorList.Location = new System.Drawing.Point(195, 377);
            this.errorList.Multiline = true;
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(412, 293);
            this.errorList.TabIndex = 32;
            // 
            // rnd_lbl4
            // 
            this.rnd_lbl4.AutoSize = true;
            this.rnd_lbl4.Location = new System.Drawing.Point(217, 20);
            this.rnd_lbl4.Name = "rnd_lbl4";
            this.rnd_lbl4.Size = new System.Drawing.Size(14, 13);
            this.rnd_lbl4.TabIndex = 33;
            this.rnd_lbl4.Text = "#";
            // 
            // iptxt_txt
            // 
            this.iptxt_txt.Location = new System.Drawing.Point(614, 94);
            this.iptxt_txt.Multiline = true;
            this.iptxt_txt.Name = "iptxt_txt";
            this.iptxt_txt.Size = new System.Drawing.Size(307, 576);
            this.iptxt_txt.TabIndex = 34;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 732);
            this.Controls.Add(this.iptxt_txt);
            this.Controls.Add(this.rnd_lbl4);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.results);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl1count);
            this.Controls.Add(this.date_picker);
            this.Controls.Add(this.progress_lbl);
            this.Controls.Add(this.worklist);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox results;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl1count;
        private System.Windows.Forms.DateTimePicker date_picker;
        private System.Windows.Forms.Label progress_lbl;
        private System.Windows.Forms.ListBox worklist;
        private System.Windows.Forms.TextBox errorList;
        private System.Windows.Forms.Label rnd_lbl4;
        private System.Windows.Forms.TextBox iptxt_txt;
    }
}

