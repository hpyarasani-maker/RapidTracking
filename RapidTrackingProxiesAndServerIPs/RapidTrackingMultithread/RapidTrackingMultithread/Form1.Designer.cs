namespace RapidTrackingMultithread
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
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtError = new System.Windows.Forms.TextBox();
            this.errorList = new System.Windows.Forms.ListBox();
            this.progress_seid12 = new System.Windows.Forms.Label();
            this.progress_seid6 = new System.Windows.Forms.Label();
            this.progress_seid2 = new System.Windows.Forms.Label();
            this.results3 = new System.Windows.Forms.ListBox();
            this.results2 = new System.Windows.Forms.ListBox();
            this.results1 = new System.Windows.Forms.ListBox();
            this.worklist3 = new System.Windows.Forms.ListBox();
            this.worklist2 = new System.Windows.Forms.ListBox();
            this.worklist1 = new System.Windows.Forms.ListBox();
            this.date_picker = new System.Windows.Forms.DateTimePicker();
            this.Process_btn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(663, 523);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(14, 13);
            this.label3.TabIndex = 31;
            this.label3.Text = "#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(663, 264);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 13);
            this.label2.TabIndex = 30;
            this.label2.Text = "#";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(663, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 29;
            this.label1.Text = "#";
            // 
            // txtError
            // 
            this.txtError.AcceptsReturn = true;
            this.txtError.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtError.Location = new System.Drawing.Point(767, 52);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtError.Size = new System.Drawing.Size(242, 686);
            this.txtError.TabIndex = 28;
            // 
            // errorList
            // 
            this.errorList.Location = new System.Drawing.Point(-2, 751);
            this.errorList.Name = "errorList";
            this.errorList.Size = new System.Drawing.Size(1016, 56);
            this.errorList.TabIndex = 27;
            // 
            // progress_seid12
            // 
            this.progress_seid12.Location = new System.Drawing.Point(407, 513);
            this.progress_seid12.Name = "progress_seid12";
            this.progress_seid12.Size = new System.Drawing.Size(320, 23);
            this.progress_seid12.TabIndex = 26;
            // 
            // progress_seid6
            // 
            this.progress_seid6.Location = new System.Drawing.Point(407, 254);
            this.progress_seid6.Name = "progress_seid6";
            this.progress_seid6.Size = new System.Drawing.Size(336, 23);
            this.progress_seid6.TabIndex = 25;
            // 
            // progress_seid2
            // 
            this.progress_seid2.Location = new System.Drawing.Point(437, 25);
            this.progress_seid2.Name = "progress_seid2";
            this.progress_seid2.Size = new System.Drawing.Size(296, 23);
            this.progress_seid2.TabIndex = 24;
            // 
            // results3
            // 
            this.results3.Location = new System.Drawing.Point(407, 539);
            this.results3.Name = "results3";
            this.results3.Size = new System.Drawing.Size(354, 199);
            this.results3.TabIndex = 23;
            // 
            // results2
            // 
            this.results2.Location = new System.Drawing.Point(407, 280);
            this.results2.Name = "results2";
            this.results2.Size = new System.Drawing.Size(354, 225);
            this.results2.TabIndex = 22;
            // 
            // results1
            // 
            this.results1.Location = new System.Drawing.Point(407, 52);
            this.results1.Name = "results1";
            this.results1.Size = new System.Drawing.Size(354, 199);
            this.results1.TabIndex = 21;
            // 
            // worklist3
            // 
            this.worklist3.Location = new System.Drawing.Point(281, 52);
            this.worklist3.Name = "worklist3";
            this.worklist3.Size = new System.Drawing.Size(120, 693);
            this.worklist3.TabIndex = 20;
            // 
            // worklist2
            // 
            this.worklist2.Location = new System.Drawing.Point(147, 52);
            this.worklist2.Name = "worklist2";
            this.worklist2.Size = new System.Drawing.Size(128, 693);
            this.worklist2.TabIndex = 19;
            // 
            // worklist1
            // 
            this.worklist1.Location = new System.Drawing.Point(13, 52);
            this.worklist1.Name = "worklist1";
            this.worklist1.Size = new System.Drawing.Size(128, 693);
            this.worklist1.TabIndex = 18;
            // 
            // date_picker
            // 
            this.date_picker.CustomFormat = "dd/mm/yyyy";
            this.date_picker.Location = new System.Drawing.Point(13, 17);
            this.date_picker.Name = "date_picker";
            this.date_picker.Size = new System.Drawing.Size(200, 20);
            this.date_picker.TabIndex = 16;
            // 
            // Process_btn
            // 
            this.Process_btn.Location = new System.Drawing.Point(253, 17);
            this.Process_btn.Name = "Process_btn";
            this.Process_btn.Size = new System.Drawing.Size(75, 23);
            this.Process_btn.TabIndex = 32;
            this.Process_btn.Text = "button1";
            this.Process_btn.UseVisualStyleBackColor = true;
            this.Process_btn.Click += new System.EventHandler(this.Process_btn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1026, 750);
            this.Controls.Add(this.Process_btn);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.errorList);
            this.Controls.Add(this.progress_seid12);
            this.Controls.Add(this.progress_seid6);
            this.Controls.Add(this.progress_seid2);
            this.Controls.Add(this.results3);
            this.Controls.Add(this.results2);
            this.Controls.Add(this.results1);
            this.Controls.Add(this.worklist3);
            this.Controls.Add(this.worklist2);
            this.Controls.Add(this.worklist1);
            this.Controls.Add(this.date_picker);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.ListBox errorList;
        private System.Windows.Forms.Label progress_seid12;
        private System.Windows.Forms.Label progress_seid6;
        private System.Windows.Forms.Label progress_seid2;
        private System.Windows.Forms.ListBox results3;
        private System.Windows.Forms.ListBox results2;
        private System.Windows.Forms.ListBox results1;
        private System.Windows.Forms.ListBox worklist3;
        private System.Windows.Forms.ListBox worklist2;
        private System.Windows.Forms.ListBox worklist1;
        private System.Windows.Forms.DateTimePicker date_picker;
        private System.Windows.Forms.Button Process_btn;
    }
}

