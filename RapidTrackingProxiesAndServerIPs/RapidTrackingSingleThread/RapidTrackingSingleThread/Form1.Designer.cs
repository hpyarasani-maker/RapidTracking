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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.date_picker = new System.Windows.Forms.DateTimePicker();
            this.errorList = new System.Windows.Forms.ListBox();
            this.progress_lbl = new System.Windows.Forms.Label();
            this.worklist = new System.Windows.Forms.ListBox();
            this.process_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // results
            // 
            this.results.FormattingEnabled = true;
            this.results.Location = new System.Drawing.Point(195, 94);
            this.results.Name = "results";
            this.results.Size = new System.Drawing.Size(571, 355);
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
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(385, 35);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(253, 20);
            this.textBox1.TabIndex = 28;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(493, 6);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 27;
            this.button1.Text = "Send Data";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // date_picker
            // 
            this.date_picker.CustomFormat = "dd/mm/yyyy";
            this.date_picker.Location = new System.Drawing.Point(11, 14);
            this.date_picker.Name = "date_picker";
            this.date_picker.Size = new System.Drawing.Size(200, 20);
            this.date_picker.TabIndex = 26;
            // 
            // errorList
            // 
            this.errorList.Location = new System.Drawing.Point(195, 455);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(571, 212);
            this.errorList.TabIndex = 25;
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
            // process_btn
            // 
            this.process_btn.Location = new System.Drawing.Point(251, 8);
            this.process_btn.Name = "process_btn";
            this.process_btn.Size = new System.Drawing.Size(128, 47);
            this.process_btn.TabIndex = 22;
            this.process_btn.Text = "Update";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 677);
            this.Controls.Add(this.results);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl1count);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.date_picker);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.progress_lbl);
            this.Controls.Add(this.worklist);
            this.Controls.Add(this.process_btn);
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
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DateTimePicker date_picker;
        private System.Windows.Forms.ListBox errorList;
        private System.Windows.Forms.Label progress_lbl;
        private System.Windows.Forms.ListBox worklist;
        private System.Windows.Forms.Button process_btn;
    }
}

