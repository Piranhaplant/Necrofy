namespace Necrofy
{
    partial class BuildResultsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.Level = new System.Windows.Forms.DataGridViewImageColumn();
            this.File = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Description = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.entryBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToResizeRows = false;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Level,
            this.File,
            this.Description});
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.ReadOnly = true;
            this.dataGrid.RowHeadersVisible = false;
            this.dataGrid.Size = new System.Drawing.Size(856, 526);
            this.dataGrid.TabIndex = 0;
            // 
            // Level
            // 
            this.Level.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Level.FillWeight = 3F;
            this.Level.HeaderText = "";
            this.Level.Name = "Level";
            this.Level.ReadOnly = true;
            this.Level.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Level.Width = 25;
            // 
            // File
            // 
            this.File.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.File.FillWeight = 10F;
            this.File.HeaderText = "File";
            this.File.Name = "File";
            this.File.ReadOnly = true;
            // 
            // Description
            // 
            this.Description.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Description.FillWeight = 90F;
            this.Description.HeaderText = "Description";
            this.Description.Name = "Description";
            this.Description.ReadOnly = true;
            // 
            // entryBindingSource
            // 
            this.entryBindingSource.DataSource = typeof(Necrofy.BuildResults.Entry);
            // 
            // BuildResultsWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 526);
            this.Controls.Add(this.dataGrid);
            this.DockAreas = ((WeifenLuo.WinFormsUI.Docking.DockAreas)(((((WeifenLuo.WinFormsUI.Docking.DockAreas.Float | WeifenLuo.WinFormsUI.Docking.DockAreas.DockLeft) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockRight) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockTop) 
            | WeifenLuo.WinFormsUI.Docking.DockAreas.DockBottom)));
            this.HideOnClose = true;
            this.Name = "BuildResultsWindow";
            this.Text = "Build Results";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.entryBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGrid;
        private System.Windows.Forms.BindingSource entryBindingSource;
        private System.Windows.Forms.DataGridViewImageColumn Level;
        private System.Windows.Forms.DataGridViewTextBoxColumn File;
        private System.Windows.Forms.DataGridViewTextBoxColumn Description;
    }
}