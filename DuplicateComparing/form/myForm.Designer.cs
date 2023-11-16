using System.Windows.Forms;
using DiffPlex.WindowsForms.Controls;

namespace DuplicateComparing
{
    partial class myForm
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Cancel = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panelLeft = new System.Windows.Forms.RichTextBox();
            this.panelRight = new System.Windows.Forms.RichTextBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // UsingleftButton
            // 
            this.UsingleftButton.AutoSize = true;
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
            this.UsingrightButton.AutoSize = true;
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
            this.CombineButton.AutoSize = true;
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
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.UsingleftButton);
            this.flowLayoutPanel1.Controls.Add(this.CombineButton);
            this.flowLayoutPanel1.Controls.Add(this.UsingrightButton);
            this.flowLayoutPanel1.Controls.Add(this.Cancel);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 322);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(572, 58);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // Cancel
            // 
            this.Cancel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.Cancel.AutoSize = true;
            this.Cancel.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Cancel.Location = new System.Drawing.Point(429, 3);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(134, 52);
            this.Cancel.TabIndex = 10;
            this.Cancel.Text = "Cancel merge";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.panelLeft, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panelRight, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(572, 322);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // panelLeft
            // 
            this.panelLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelLeft.Location = new System.Drawing.Point(3, 3);
            this.panelLeft.Name = "panelLeft";
            this.panelLeft.Size = new System.Drawing.Size(280, 316);
            this.panelLeft.TabIndex = 0;
            this.panelLeft.Text = "";
            // 
            // panelRight
            // 
            this.panelRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelRight.Location = new System.Drawing.Point(289, 3);
            this.panelRight.Name = "panelRight";
            this.panelRight.Size = new System.Drawing.Size(280, 316);
            this.panelRight.TabIndex = 1;
            this.panelRight.Text = "";
            // 
            // myForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 380);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Font = new System.Drawing.Font("Microsoft YaHei UI", 12F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "myForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Duplicates Comparing";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button UsingleftButton;
        private System.Windows.Forms.Button UsingrightButton;
        private System.Windows.Forms.Button CombineButton;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button Cancel;
        private TableLayoutPanel tableLayoutPanel1;
        private RichTextBox panelLeft;
        private RichTextBox panelRight;
    }
}