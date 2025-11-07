namespace TrendingMobileSingleThread
{
    partial class frmMultiThread
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
            this.SuspendLayout();
            // 
            // lstKWs
            // 
            this.lstKWs.FormattingEnabled = true;
            this.lstKWs.ItemHeight = 20;
            this.lstKWs.Location = new System.Drawing.Point(18, 18);
            this.lstKWs.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.lstKWs.Name = "lstKWs";
            this.lstKWs.Size = new System.Drawing.Size(463, 724);
            this.lstKWs.TabIndex = 0;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(506, 74);
            this.textBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(763, 164);
            this.textBox1.TabIndex = 1;
            // 
            // txtError
            // 
            this.txtError.Location = new System.Drawing.Point(506, 422);
            this.txtError.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtError.Multiline = true;
            this.txtError.Name = "txtError";
            this.txtError.Size = new System.Drawing.Size(763, 321);
            this.txtError.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(501, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "#";
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Location = new System.Drawing.Point(1100, 35);
            this.lblCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(18, 20);
            this.lblCount.TabIndex = 11;
            this.lblCount.Text = "#";
            // 
            // frmMultiThread
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1288, 789);
            this.Controls.Add(this.lblCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtError);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lstKWs);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "frmMultiThread";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.frmMultiThread_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lstKWs;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox txtError;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblCount;
    }
}

