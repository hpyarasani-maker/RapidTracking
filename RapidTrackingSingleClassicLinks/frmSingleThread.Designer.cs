namespace RapidTrackingSingleClassicLinks
{
    partial class frmSingleThread
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
            this.lstKWs = new System.Windows.Forms.ListBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txtError = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblCount = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblDownloadedTime = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lstKWs
            // 
            this.lstKWs.FormattingEnabled = true;
            this.lstKWs.Location = new System.Drawing.Point(12, 12);
            this.lstKWs.Name = "lstKWs";
            this.lstKWs.Size = new System.Drawing.Size(310, 472);
            this.lstKWs.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(337, 48);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(510, 108);
            this.textBox1.TabIndex = 1;
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(337, 274);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(510, 210);
            this.txtError.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(334, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(14, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "#";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(756, 23);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(14, 13);
            this.lblCount.TabIndex = 10;
            this.lblCount.Text = "#";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(337, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 16);
            this.label2.TabIndex = 11;
            this.label2.Text = "Download Source Time:";
            // 
            // lblDownloadedTime
            // 
            this.lblDownloadedTime.AutoSize = true;
            this.lblDownloadedTime.Location = new System.Drawing.Point(509, 186);
            this.lblDownloadedTime.Name = "lblDownloadedTime";
            this.lblDownloadedTime.Size = new System.Drawing.Size(13, 13);
            this.lblDownloadedTime.TabIndex = 12;
            this.lblDownloadedTime.Text = "0";
            // 
            // frmSingleThread
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(859, 513);
            this.Controls.Add(this.lblDownloadedTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lstKWs);
            this.Name = "frmSingleThread";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmSingleThread_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstKWs;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblDownloadedTime;
    }
}

