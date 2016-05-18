namespace photoel2
{
    partial class PhotoElForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PhotoElForm));
            this.pbControls = new System.Windows.Forms.PictureBox();
            this.lvLog = new photoel2.NotebookTable();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvMonitor = new photoel2.NotebookTable();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.workspace = new photoel2.PhotoElWorkspace();
            this.lblDirections = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbControls)).BeginInit();
            this.SuspendLayout();
            // 
            // pbControls
            // 
            this.pbControls.BackColor = System.Drawing.Color.White;
            this.pbControls.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pbControls.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbControls.Image = global::photoel2.Properties.Resources.controls_large;
            this.pbControls.Location = new System.Drawing.Point(807, 897);
            this.pbControls.Name = "pbControls";
            this.pbControls.Size = new System.Drawing.Size(56, 264);
            this.pbControls.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbControls.TabIndex = 2;
            this.pbControls.TabStop = false;
            this.pbControls.Visible = false;
            this.pbControls.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbControls_MouseClick);
            // 
            // lvLog
            // 
            this.lvLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvLog.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lvLog.BackgroundImage")));
            this.lvLog.BackgroundImageTiled = true;
            this.lvLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader7,
            this.columnHeader3,
            this.columnHeader4});
            this.lvLog.Font = new System.Drawing.Font("Grundschrift", 16F);
            this.lvLog.GridSize = 0;
            this.lvLog.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvLog.Location = new System.Drawing.Point(1085, 811);
            this.lvLog.Margin = new System.Windows.Forms.Padding(4);
            this.lvLog.MultiSelect = false;
            this.lvLog.Name = "lvLog";
            this.lvLog.Size = new System.Drawing.Size(1038, 463);
            this.lvLog.TabIndex = 1;
            this.lvLog.UseCompatibleStateImageBehavior = false;
            this.lvLog.View = System.Windows.Forms.View.Details;
            this.lvLog.Visible = false;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Metal";
            this.columnHeader1.Width = 279;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "λ (nm)";
            this.columnHeader2.Width = 120;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "D (cm)";
            this.columnHeader7.Width = 120;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "V (V)";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "I (nA)";
            this.columnHeader4.Width = 120;
            // 
            // lvMonitor
            // 
            this.lvMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lvMonitor.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lvMonitor.BackgroundImage")));
            this.lvMonitor.BackgroundImageTiled = true;
            this.lvMonitor.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvMonitor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader6});
            this.lvMonitor.Font = new System.Drawing.Font("Grundschrift", 16F);
            this.lvMonitor.GridSize = 0;
            this.lvMonitor.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvMonitor.Location = new System.Drawing.Point(11, 811);
            this.lvMonitor.Margin = new System.Windows.Forms.Padding(4);
            this.lvMonitor.MultiSelect = false;
            this.lvMonitor.Name = "lvMonitor";
            this.lvMonitor.Size = new System.Drawing.Size(999, 519);
            this.lvMonitor.TabIndex = 0;
            this.lvMonitor.UseCompatibleStateImageBehavior = false;
            this.lvMonitor.View = System.Windows.Forms.View.Details;
            this.lvMonitor.Visible = false;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Property";
            this.columnHeader5.Width = 409;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Value";
            this.columnHeader6.Width = 180;
            // 
            // workspace
            // 
            this.workspace.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.workspace.BackColor = System.Drawing.Color.White;
            this.workspace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.workspace.Location = new System.Drawing.Point(11, 15);
            this.workspace.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.workspace.Name = "workspace";
            this.workspace.Size = new System.Drawing.Size(2112, 766);
            this.workspace.TabIndex = 0;
            // 
            // lblDirections
            // 
            this.lblDirections.Font = new System.Drawing.Font("Arial", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDirections.Location = new System.Drawing.Point(968, 1032);
            this.lblDirections.Name = "lblDirections";
            this.lblDirections.Size = new System.Drawing.Size(210, 106);
            this.lblDirections.TabIndex = 3;
            this.lblDirections.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PhotoElForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.AntiqueWhite;
            this.ClientSize = new System.Drawing.Size(2133, 1396);
            this.Controls.Add(this.lblDirections);
            this.Controls.Add(this.pbControls);
            this.Controls.Add(this.lvLog);
            this.Controls.Add(this.lvMonitor);
            this.Controls.Add(this.workspace);
            this.HelpButton = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(900, 700);
            this.Name = "PhotoElForm";
            this.Text = "PhotoElectric Effect";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pbControls)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private PhotoElWorkspace workspace;
        private NotebookTable lvMonitor;
        private NotebookTable lvLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.PictureBox pbControls;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.Label lblDirections;
    }
}

