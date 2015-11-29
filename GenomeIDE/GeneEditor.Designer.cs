namespace GenomeIDE
{
    partial class GeneEditor
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
            this.HexInput = new System.Windows.Forms.TextBox();
            this.HexLabel = new System.Windows.Forms.Label();
            this.GeneSelector = new System.Windows.Forms.ComboBox();
            this.OrLabel = new System.Windows.Forms.Label();
            this.GeneLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // HexInput
            // 
            this.HexInput.Location = new System.Drawing.Point(15, 29);
            this.HexInput.Name = "HexInput";
            this.HexInput.Size = new System.Drawing.Size(29, 22);
            this.HexInput.TabIndex = 0;
            this.HexInput.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.HexInput_KeyPress);
            // 
            // HexLabel
            // 
            this.HexLabel.AutoSize = true;
            this.HexLabel.Location = new System.Drawing.Point(12, 9);
            this.HexLabel.Name = "HexLabel";
            this.HexLabel.Size = new System.Drawing.Size(32, 17);
            this.HexLabel.TabIndex = 1;
            this.HexLabel.Text = "Hex";
            // 
            // GeneSelector
            // 
            this.GeneSelector.DropDownWidth = 350;
            this.GeneSelector.FormattingEnabled = true;
            this.GeneSelector.Location = new System.Drawing.Point(77, 27);
            this.GeneSelector.MaxDropDownItems = 16;
            this.GeneSelector.Name = "GeneSelector";
            this.GeneSelector.Size = new System.Drawing.Size(121, 24);
            this.GeneSelector.TabIndex = 2;
            this.GeneSelector.SelectedIndexChanged += new System.EventHandler(this.GeneSelector_SelectedIndexChanged);
            // 
            // OrLabel
            // 
            this.OrLabel.AutoSize = true;
            this.OrLabel.Location = new System.Drawing.Point(50, 32);
            this.OrLabel.Name = "OrLabel";
            this.OrLabel.Size = new System.Drawing.Size(21, 17);
            this.OrLabel.TabIndex = 3;
            this.OrLabel.Text = "or";
            // 
            // GeneLabel
            // 
            this.GeneLabel.AutoSize = true;
            this.GeneLabel.Location = new System.Drawing.Point(74, 9);
            this.GeneLabel.Name = "GeneLabel";
            this.GeneLabel.Size = new System.Drawing.Size(95, 17);
            this.GeneLabel.TabIndex = 4;
            this.GeneLabel.Text = "Choose Gene";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(15, 57);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 5;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            this.SaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(123, 57);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 23);
            this.CancelButton.TabIndex = 6;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // GeneEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(210, 89);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.GeneLabel);
            this.Controls.Add(this.OrLabel);
            this.Controls.Add(this.GeneSelector);
            this.Controls.Add(this.HexLabel);
            this.Controls.Add(this.HexInput);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "GeneEditor";
            this.Text = "GeneEditor";
            this.Load += new System.EventHandler(this.GeneEditor_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox HexInput;
        private System.Windows.Forms.Label HexLabel;
        private System.Windows.Forms.ComboBox GeneSelector;
        private System.Windows.Forms.Label OrLabel;
        private System.Windows.Forms.Label GeneLabel;
        private System.Windows.Forms.Button SaveButton;
        private new System.Windows.Forms.Button CancelButton;
    }
}