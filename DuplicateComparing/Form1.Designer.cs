namespace DuplicateComparing
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.Ref1 = new System.Windows.Forms.Label();
            this.Ref2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Location = new System.Drawing.Point(153, 91);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(157, 264);
            this.panel1.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Location = new System.Drawing.Point(498, 91);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(157, 264);
            this.panel2.TabIndex = 4;
            // 
            // Ref1
            // 
            this.Ref1.AutoSize = true;
            this.Ref1.Location = new System.Drawing.Point(153, 56);
            this.Ref1.Name = "Ref1";
            this.Ref1.Size = new System.Drawing.Size(41, 12);
            this.Ref1.TabIndex = 5;
            this.Ref1.Text = "label1";
            this.Ref1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Ref2
            // 
            this.Ref2.AutoSize = true;
            this.Ref2.Location = new System.Drawing.Point(496, 56);
            this.Ref2.Name = "Ref2";
            this.Ref2.Size = new System.Drawing.Size(41, 12);
            this.Ref2.TabIndex = 5;
            this.Ref2.Text = "label1";
            this.Ref2.Click += new System.EventHandler(this.label1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.Ref2);
            this.Controls.Add(this.Ref1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label Ref1;
        private System.Windows.Forms.Label Ref2;
    }
}