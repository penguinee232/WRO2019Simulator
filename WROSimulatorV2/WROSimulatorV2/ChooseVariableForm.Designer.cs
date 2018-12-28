namespace WROSimulatorV2
{
    partial class ChooseVariableForm
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
            this.variableTypesListBox = new System.Windows.Forms.ListBox();
            this.variableNamesListBox = new System.Windows.Forms.ListBox();
            this.chooseVariableButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // variableTypesListBox
            // 
            this.variableTypesListBox.FormattingEnabled = true;
            this.variableTypesListBox.Location = new System.Drawing.Point(13, 13);
            this.variableTypesListBox.Name = "variableTypesListBox";
            this.variableTypesListBox.Size = new System.Drawing.Size(120, 264);
            this.variableTypesListBox.TabIndex = 0;
            this.variableTypesListBox.SelectedIndexChanged += new System.EventHandler(this.variableTypesListBox_SelectedIndexChanged);
            // 
            // variableNamesListBox
            // 
            this.variableNamesListBox.FormattingEnabled = true;
            this.variableNamesListBox.Location = new System.Drawing.Point(139, 13);
            this.variableNamesListBox.Name = "variableNamesListBox";
            this.variableNamesListBox.Size = new System.Drawing.Size(120, 264);
            this.variableNamesListBox.TabIndex = 1;
            this.variableNamesListBox.SelectedIndexChanged += new System.EventHandler(this.variableNamesListBox_SelectedIndexChanged);
            // 
            // chooseVariableButton
            // 
            this.chooseVariableButton.Location = new System.Drawing.Point(265, 13);
            this.chooseVariableButton.Name = "chooseVariableButton";
            this.chooseVariableButton.Size = new System.Drawing.Size(75, 39);
            this.chooseVariableButton.TabIndex = 2;
            this.chooseVariableButton.Text = "Choose Variable";
            this.chooseVariableButton.UseVisualStyleBackColor = true;
            this.chooseVariableButton.Click += new System.EventHandler(this.chooseVariableButton_Click);
            // 
            // ChooseVariableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 286);
            this.Controls.Add(this.chooseVariableButton);
            this.Controls.Add(this.variableNamesListBox);
            this.Controls.Add(this.variableTypesListBox);
            this.Name = "ChooseVariableForm";
            this.Text = "ChooseVariableForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChooseVariableForm_FormClosed);
            this.Load += new System.EventHandler(this.ChooseVariableForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox variableTypesListBox;
        private System.Windows.Forms.ListBox variableNamesListBox;
        private System.Windows.Forms.Button chooseVariableButton;
    }
}