namespace Paladin_Retribution.Interface.GUI
{
    partial class ConfigForm
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.b_Save = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_hotkeys = new System.Windows.Forms.TabPage();
            this.pg_Hotkeys = new System.Windows.Forms.PropertyGrid();
            this.l_version = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tab_hotkeys.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 81F));
            this.tableLayoutPanel1.Controls.Add(this.b_Save, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.l_version, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(334, 312);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // b_Save
            // 
            this.b_Save.Location = new System.Drawing.Point(256, 290);
            this.b_Save.Name = "b_Save";
            this.b_Save.Size = new System.Drawing.Size(75, 19);
            this.b_Save.TabIndex = 12;
            this.b_Save.Text = "Save";
            this.b_Save.UseVisualStyleBackColor = true;
            this.b_Save.Click += new System.EventHandler(this.b_Save_Click);
            // 
            // tabControl1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.tabControl1, 2);
            this.tabControl1.Controls.Add(this.tab_hotkeys);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(328, 281);
            this.tabControl1.TabIndex = 13;
            // 
            // tab_hotkeys
            // 
            this.tab_hotkeys.Controls.Add(this.pg_Hotkeys);
            this.tab_hotkeys.Location = new System.Drawing.Point(4, 22);
            this.tab_hotkeys.Name = "tab_hotkeys";
            this.tab_hotkeys.Padding = new System.Windows.Forms.Padding(3);
            this.tab_hotkeys.Size = new System.Drawing.Size(320, 255);
            this.tab_hotkeys.TabIndex = 0;
            this.tab_hotkeys.Text = "Hotkeys";
            this.tab_hotkeys.UseVisualStyleBackColor = true;
            // 
            // pg_Hotkeys
            // 
            this.pg_Hotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pg_Hotkeys.Location = new System.Drawing.Point(3, 3);
            this.pg_Hotkeys.Name = "pg_Hotkeys";
            this.pg_Hotkeys.Size = new System.Drawing.Size(314, 249);
            this.pg_Hotkeys.TabIndex = 0;
            // 
            // l_version
            // 
            this.l_version.AutoSize = true;
            this.l_version.Dock = System.Windows.Forms.DockStyle.Fill;
            this.l_version.Location = new System.Drawing.Point(3, 287);
            this.l_version.Name = "l_version";
            this.l_version.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.l_version.Size = new System.Drawing.Size(247, 25);
            this.l_version.TabIndex = 14;
            this.l_version.Text = "version";
            this.l_version.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 312);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigForm";
            this.Text = "[Shineey] Configuration";
            this.Load += new System.EventHandler(this.ConfigForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tab_hotkeys.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button b_Save;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_hotkeys;
        private System.Windows.Forms.Label l_version;
        private System.Windows.Forms.PropertyGrid pg_Hotkeys;

    }
}