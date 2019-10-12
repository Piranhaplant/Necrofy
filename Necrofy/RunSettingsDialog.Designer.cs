namespace Necrofy
{
    partial class RunSettingsDialog
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
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.presetTree = new System.Windows.Forms.TreeView();
            this.presetsLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Left;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyGrid.Size = new System.Drawing.Size(223, 503);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.UseCompatibleTextRendering = true;
            // 
            // presetTree
            // 
            this.presetTree.HideSelection = false;
            this.presetTree.LabelEdit = true;
            this.presetTree.Location = new System.Drawing.Point(229, 25);
            this.presetTree.Name = "presetTree";
            this.presetTree.ShowLines = false;
            this.presetTree.ShowPlusMinus = false;
            this.presetTree.ShowRootLines = false;
            this.presetTree.Size = new System.Drawing.Size(128, 274);
            this.presetTree.TabIndex = 1;
            this.presetTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.presetTree_AfterLabelEdit);
            this.presetTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.presetTree_AfterSelect);
            // 
            // presetsLabel
            // 
            this.presetsLabel.AutoSize = true;
            this.presetsLabel.Location = new System.Drawing.Point(229, 9);
            this.presetsLabel.Name = "presetsLabel";
            this.presetsLabel.Size = new System.Drawing.Size(45, 13);
            this.presetsLabel.TabIndex = 2;
            this.presetsLabel.Text = "Presets:";
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(282, 468);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 3;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Image = global::Necrofy.Properties.Resources.minus;
            this.removeButton.Location = new System.Drawing.Point(276, 305);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(41, 23);
            this.removeButton.TabIndex = 5;
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Image = global::Necrofy.Properties.Resources.plus;
            this.addButton.Location = new System.Drawing.Point(229, 305);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(41, 23);
            this.addButton.TabIndex = 4;
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // RunSettingsDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(369, 503);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.presetsLabel);
            this.Controls.Add(this.propertyGrid);
            this.Controls.Add(this.presetTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "RunSettingsDialog";
            this.Text = "Run From Level Settings";
            this.Load += new System.EventHandler(this.RunSettingsDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.TreeView presetTree;
        private System.Windows.Forms.Label presetsLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button removeButton;
    }
}