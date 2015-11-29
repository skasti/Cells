namespace GenomeIDE
{
    partial class MainForm
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
            this.components = new System.ComponentModel.Container();
            this.DnaView = new System.Windows.Forms.TreeView();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadGenomeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveGenomeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveAsButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitButton = new System.Windows.Forms.ToolStripMenuItem();
            this.geneLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveGeneLibraryButton = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.GenomeEditor = new System.Windows.Forms.ListBox();
            this.EditorContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.EditGeneButton = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoveGeneButton = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertSubMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertBeforeButton = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertAfterButton = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertButton = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuStrip.SuspendLayout();
            this.EditorContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // DnaView
            // 
            this.DnaView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DnaView.Location = new System.Drawing.Point(146, 31);
            this.DnaView.Name = "DnaView";
            this.DnaView.ShowNodeToolTips = true;
            this.DnaView.Size = new System.Drawing.Size(486, 502);
            this.DnaView.TabIndex = 0;
            // 
            // MenuStrip
            // 
            this.MenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.geneLibraryToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(644, 28);
            this.MenuStrip.TabIndex = 1;
            this.MenuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.LoadGenomeButton,
            this.SaveGenomeButton,
            this.SaveAsButton,
            this.ExitButton});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // LoadGenomeButton
            // 
            this.LoadGenomeButton.Name = "LoadGenomeButton";
            this.LoadGenomeButton.Size = new System.Drawing.Size(135, 26);
            this.LoadGenomeButton.Text = "Open";
            this.LoadGenomeButton.Click += new System.EventHandler(this.LoadGenomeButton_Click);
            // 
            // SaveGenomeButton
            // 
            this.SaveGenomeButton.Name = "SaveGenomeButton";
            this.SaveGenomeButton.Size = new System.Drawing.Size(135, 26);
            this.SaveGenomeButton.Text = "Save";
            this.SaveGenomeButton.Click += new System.EventHandler(this.SaveGenomeButton_Click);
            // 
            // SaveAsButton
            // 
            this.SaveAsButton.Name = "SaveAsButton";
            this.SaveAsButton.Size = new System.Drawing.Size(135, 26);
            this.SaveAsButton.Text = "Save As";
            // 
            // ExitButton
            // 
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(135, 26);
            this.ExitButton.Text = "Exit";
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // geneLibraryToolStripMenuItem
            // 
            this.geneLibraryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.reloadToolStripMenuItem,
            this.SaveGeneLibraryButton});
            this.geneLibraryToolStripMenuItem.Name = "geneLibraryToolStripMenuItem";
            this.geneLibraryToolStripMenuItem.Size = new System.Drawing.Size(100, 24);
            this.geneLibraryToolStripMenuItem.Text = "GeneLibrary";
            // 
            // reloadToolStripMenuItem
            // 
            this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
            this.reloadToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.reloadToolStripMenuItem.Text = "Reload";
            // 
            // SaveGeneLibraryButton
            // 
            this.SaveGeneLibraryButton.Name = "SaveGeneLibraryButton";
            this.SaveGeneLibraryButton.Size = new System.Drawing.Size(181, 26);
            this.SaveGeneLibraryButton.Text = "Save";
            this.SaveGeneLibraryButton.Click += new System.EventHandler(this.SaveGeneLibraryButton_Click);
            // 
            // OpenDialog
            // 
            this.OpenDialog.FileName = "openFileDialog1";
            // 
            // GenomeEditor
            // 
            this.GenomeEditor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.GenomeEditor.ContextMenuStrip = this.EditorContextMenu;
            this.GenomeEditor.FormattingEnabled = true;
            this.GenomeEditor.IntegralHeight = false;
            this.GenomeEditor.ItemHeight = 16;
            this.GenomeEditor.Location = new System.Drawing.Point(12, 31);
            this.GenomeEditor.Name = "GenomeEditor";
            this.GenomeEditor.Size = new System.Drawing.Size(128, 500);
            this.GenomeEditor.TabIndex = 2;
            // 
            // EditorContextMenu
            // 
            this.EditorContextMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.EditorContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.EditGeneButton,
            this.RemoveGeneButton,
            this.InsertSubMenu,
            this.InsertButton});
            this.EditorContextMenu.Name = "EditorContextMenu";
            this.EditorContextMenu.Size = new System.Drawing.Size(139, 108);
            this.EditorContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.EditorContextMenu_Opening);
            // 
            // EditGeneButton
            // 
            this.EditGeneButton.Name = "EditGeneButton";
            this.EditGeneButton.Size = new System.Drawing.Size(138, 26);
            this.EditGeneButton.Text = "Edit";
            this.EditGeneButton.Click += new System.EventHandler(this.EditGeneButton_Click);
            // 
            // RemoveGeneButton
            // 
            this.RemoveGeneButton.Name = "RemoveGeneButton";
            this.RemoveGeneButton.Size = new System.Drawing.Size(138, 26);
            this.RemoveGeneButton.Text = "Remove";
            this.RemoveGeneButton.Click += new System.EventHandler(this.RemoveGeneButton_Click);
            // 
            // InsertSubMenu
            // 
            this.InsertSubMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.InsertBeforeButton,
            this.InsertAfterButton});
            this.InsertSubMenu.Name = "InsertSubMenu";
            this.InsertSubMenu.Size = new System.Drawing.Size(138, 26);
            this.InsertSubMenu.Text = "Insert";
            // 
            // InsertBeforeButton
            // 
            this.InsertBeforeButton.Name = "InsertBeforeButton";
            this.InsertBeforeButton.Size = new System.Drawing.Size(128, 26);
            this.InsertBeforeButton.Text = "Before";
            this.InsertBeforeButton.Click += new System.EventHandler(this.InsertBeforeButton_Click);
            // 
            // InsertAfterButton
            // 
            this.InsertAfterButton.Name = "InsertAfterButton";
            this.InsertAfterButton.Size = new System.Drawing.Size(128, 26);
            this.InsertAfterButton.Text = "After";
            this.InsertAfterButton.Click += new System.EventHandler(this.InsertAfterButton_Click);
            // 
            // InsertButton
            // 
            this.InsertButton.Name = "InsertButton";
            this.InsertButton.Size = new System.Drawing.Size(138, 26);
            this.InsertButton.Text = "Insert";
            this.InsertButton.Click += new System.EventHandler(this.InsertButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 545);
            this.Controls.Add(this.GenomeEditor);
            this.Controls.Add(this.DnaView);
            this.Controls.Add(this.MenuStrip);
            this.MainMenuStrip = this.MenuStrip;
            this.Name = "MainForm";
            this.Text = "Genome IDE";
            this.Load += new System.EventHandler(this.Form_Load);
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.EditorContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView DnaView;
        private System.Windows.Forms.MenuStrip MenuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadGenomeButton;
        private System.Windows.Forms.ToolStripMenuItem SaveGenomeButton;
        private System.Windows.Forms.ToolStripMenuItem ExitButton;
        private System.Windows.Forms.OpenFileDialog OpenDialog;
        private System.Windows.Forms.SaveFileDialog SaveDialog;
        private System.Windows.Forms.ToolStripMenuItem SaveAsButton;
        private System.Windows.Forms.ListBox GenomeEditor;
        private System.Windows.Forms.ContextMenuStrip EditorContextMenu;
        private System.Windows.Forms.ToolStripMenuItem EditGeneButton;
        private System.Windows.Forms.ToolStripMenuItem RemoveGeneButton;
        private System.Windows.Forms.ToolStripMenuItem InsertSubMenu;
        private System.Windows.Forms.ToolStripMenuItem InsertBeforeButton;
        private System.Windows.Forms.ToolStripMenuItem InsertAfterButton;
        private System.Windows.Forms.ToolStripMenuItem InsertButton;
        private System.Windows.Forms.ToolStripMenuItem geneLibraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem reloadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SaveGeneLibraryButton;
    }
}

