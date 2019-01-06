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
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // variableTypesListBox
            // 
            this.variableTypesListBox.FormattingEnabled = true;
            this.variableTypesListBox.Location = new System.Drawing.Point(0, 0);
            this.variableTypesListBox.Name = "variableTypesListBox";
            this.variableTypesListBox.Size = new System.Drawing.Size(118, 277);
            this.variableTypesListBox.TabIndex = 0;
            this.variableTypesListBox.SelectedIndexChanged += new System.EventHandler(this.variableTypesListBox_SelectedIndexChanged);
            // 
            // variableNamesListBox
            // 
            this.variableNamesListBox.FormattingEnabled = true;
            this.variableNamesListBox.Location = new System.Drawing.Point(124, 0);
            this.variableNamesListBox.Name = "variableNamesListBox";
            this.variableNamesListBox.Size = new System.Drawing.Size(118, 277);
            this.variableNamesListBox.TabIndex = 1;
            this.variableNamesListBox.Click += new System.EventHandler(this.variablesListBox_Click);
            this.variableNamesListBox.SelectedIndexChanged += new System.EventHandler(this.variableListBox_SelectedIndexChanged);
            // 
            // chooseVariableButton
            // 
            this.chooseVariableButton.Location = new System.Drawing.Point(403, 10);
            this.chooseVariableButton.Name = "chooseVariableButton";
            this.chooseVariableButton.Size = new System.Drawing.Size(75, 39);
            this.chooseVariableButton.TabIndex = 2;
            this.chooseVariableButton.Text = "Choose Variable";
            this.chooseVariableButton.UseVisualStyleBackColor = true;
            this.chooseVariableButton.Click += new System.EventHandler(this.chooseVariableButton_Click);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.variableTypesListBox);
            this.panel1.Controls.Add(this.variableNamesListBox);
            this.panel1.Location = new System.Drawing.Point(7, 10);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(390, 280);
            this.panel1.TabIndex = 3;
            // 
            // ChooseVariableForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 288);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chooseVariableButton);
            this.Name = "ChooseVariableForm";
            this.Text = "ChooseVariableForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ChooseVariableForm_FormClosed);
            this.Load += new System.EventHandler(this.ChooseVariableForm_Load);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox variableTypesListBox;
        private System.Windows.Forms.ListBox variableNamesListBox;
        private System.Windows.Forms.Button chooseVariableButton;
        private System.Windows.Forms.Panel panel1;
    }
}