using System.Windows.Forms;

namespace DuplicateComparing
{
    partial class ComparingForm
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
        
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.UsingleftButton = new System.Windows.Forms.Button();
            this.UsingrightButton = new System.Windows.Forms.Button();
            this.CombineButton = new System.Windows.Forms.Button();
            this.panelLeft = new System.Windows.Forms.TextBox();
            this.panelRight = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsingleftButton
            // 
            this.UsingleftButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UsingleftButton.Location = new System.Drawing.Point(4, 3);
            this.UsingleftButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.UsingleftButton.Name = "UsingleftButton";
            this.UsingleftButton.Size = new System.Drawing.Size(134, 52);
            this.UsingleftButton.TabIndex = 5;
            this.UsingleftButton.Text = "Using left";
            this.UsingleftButton.UseVisualStyleBackColor = true;
            this.UsingleftButton.Click += new System.EventHandler(this.UsingleftButton_Click);
            // 
            // UsingrightButton
            // 
            this.UsingrightButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.UsingrightButton.Location = new System.Drawing.Point(288, 3);
            this.UsingrightButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.UsingrightButton.Name = "UsingrightButton";
            this.UsingrightButton.Size = new System.Drawing.Size(134, 52);
            this.UsingrightButton.TabIndex = 5;
            this.UsingrightButton.Text = "Using right";
            this.UsingrightButton.UseVisualStyleBackColor = true;
            this.UsingrightButton.Click += new System.EventHandler(this.UsingrightButton_Click);
            // 
            // CombineButton
            // 
            this.CombineButton.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CombineButton.Location = new System.Drawing.Point(146, 3);
            this.CombineButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CombineButton.Name = "CombineButton";
            this.CombineButton.Size = new System.Drawing.Size(134, 52);
            this.CombineButton.TabIndex = 5;
            this.CombineButton.Text = "Combine two";
            this.CombineButton.UseVisualStyleBackColor = true;
            this.CombineButton.Click += new System.EventHandler(this.CombineButton_Click);
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(4, 3);
            this.panelLeft.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelLeft.Multiline = true;
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.ReadOnly = true;
            this.panelLeft.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.panelLeft.Size = new System.Drawing.Size(429, 218);
            this.panelLeft.TabIndex = 6;
            // 
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(441, 3);
            this.panelRight.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panelRight.Multiline = true;
            this.panelRight.Name = "panelRight";
            this.panelRight.ReadOnly = true;
            this.panelRight.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.panelRight.Size = new System.Drawing.Size(429, 218);
            this.panelRight.TabIndex = 7;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panelLeft, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelRight, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(874, 224);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.UsingleftButton);
            this.flowLayoutPanel1.Controls.Add(this.CombineButton);
            this.flowLayoutPanel1.Controls.Add(this.UsingrightButton);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(241, 281);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(429, 64);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // ComparingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 361);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ComparingForm";
            this.Text = "Duplicates Comparing";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button UsingleftButton;
        private System.Windows.Forms.Button UsingrightButton;
        private System.Windows.Forms.Button CombineButton;
        private System.Windows.Forms.TextBox panelLeft;
        private System.Windows.Forms.TextBox panelRight;
        private TableLayoutPanel tableLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel1;
    }
}