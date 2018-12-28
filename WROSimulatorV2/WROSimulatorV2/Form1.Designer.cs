namespace WROSimulatorV2
{
    partial class Form1
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.setVariableButton = new System.Windows.Forms.Button();
            this.unSetVariableButton = new System.Windows.Forms.Button();
            this.commandListBox = new System.Windows.Forms.ListBox();
            this.addCommandButton = new System.Windows.Forms.Button();
            this.removeCommandButton = new System.Windows.Forms.Button();
            this.runCommandsTreeView = new System.Windows.Forms.TreeView();
            this.fieldPictureBox = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.runCommandsButton = new System.Windows.Forms.Button();
            this.autoResetRobotCheckBox = new System.Windows.Forms.CheckBox();
            this.resetRobotButton = new System.Windows.Forms.Button();
            this.setAsStartButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveAsButton = new System.Windows.Forms.Button();
            this.openFileButton = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.fieldPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(407, 765);
            this.panel1.TabIndex = 0;
            // 
            // setVariableButton
            // 
            this.setVariableButton.Location = new System.Drawing.Point(794, 7);
            this.setVariableButton.Name = "setVariableButton";
            this.setVariableButton.Size = new System.Drawing.Size(120, 28);
            this.setVariableButton.TabIndex = 1;
            this.setVariableButton.Text = "Set Variable";
            this.setVariableButton.UseVisualStyleBackColor = true;
            this.setVariableButton.Click += new System.EventHandler(this.setVariableButton_Click);
            // 
            // unSetVariableButton
            // 
            this.unSetVariableButton.Location = new System.Drawing.Point(794, 41);
            this.unSetVariableButton.Name = "unSetVariableButton";
            this.unSetVariableButton.Size = new System.Drawing.Size(120, 28);
            this.unSetVariableButton.TabIndex = 2;
            this.unSetVariableButton.Text = "Un Set Variable";
            this.unSetVariableButton.UseVisualStyleBackColor = true;
            this.unSetVariableButton.Click += new System.EventHandler(this.unSetVariableButton_Click);
            // 
            // commandListBox
            // 
            this.commandListBox.FormattingEnabled = true;
            this.commandListBox.Location = new System.Drawing.Point(658, 7);
            this.commandListBox.Name = "commandListBox";
            this.commandListBox.Size = new System.Drawing.Size(120, 160);
            this.commandListBox.TabIndex = 3;
            // 
            // addCommandButton
            // 
            this.addCommandButton.Location = new System.Drawing.Point(658, 173);
            this.addCommandButton.Name = "addCommandButton";
            this.addCommandButton.Size = new System.Drawing.Size(120, 40);
            this.addCommandButton.TabIndex = 4;
            this.addCommandButton.Text = "Add Command";
            this.addCommandButton.UseVisualStyleBackColor = true;
            this.addCommandButton.Click += new System.EventHandler(this.addCommandButton_Click);
            // 
            // removeCommandButton
            // 
            this.removeCommandButton.Location = new System.Drawing.Point(658, 219);
            this.removeCommandButton.Name = "removeCommandButton";
            this.removeCommandButton.Size = new System.Drawing.Size(120, 40);
            this.removeCommandButton.TabIndex = 5;
            this.removeCommandButton.Text = "Remove Command";
            this.removeCommandButton.UseVisualStyleBackColor = true;
            this.removeCommandButton.Click += new System.EventHandler(this.removeCommandButton_Click);
            // 
            // runCommandsTreeView
            // 
            this.runCommandsTreeView.Location = new System.Drawing.Point(413, 0);
            this.runCommandsTreeView.Name = "runCommandsTreeView";
            this.runCommandsTreeView.Size = new System.Drawing.Size(239, 259);
            this.runCommandsTreeView.TabIndex = 7;
            this.runCommandsTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.runCommandsTreeView_AfterSelect);
            this.runCommandsTreeView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.runCommandsTreeView_KeyDown);
            // 
            // fieldPictureBox
            // 
            this.fieldPictureBox.Image = global::WROSimulatorV2.Properties.Resources.WRO_2018_1;
            this.fieldPictureBox.Location = new System.Drawing.Point(413, 265);
            this.fieldPictureBox.Name = "fieldPictureBox";
            this.fieldPictureBox.Size = new System.Drawing.Size(1034, 500);
            this.fieldPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.fieldPictureBox.TabIndex = 8;
            this.fieldPictureBox.TabStop = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 16;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // runCommandsButton
            // 
            this.runCommandsButton.BackColor = System.Drawing.Color.Green;
            this.runCommandsButton.ForeColor = System.Drawing.Color.White;
            this.runCommandsButton.Location = new System.Drawing.Point(794, 75);
            this.runCommandsButton.Name = "runCommandsButton";
            this.runCommandsButton.Size = new System.Drawing.Size(120, 184);
            this.runCommandsButton.TabIndex = 9;
            this.runCommandsButton.Text = "Run Commands";
            this.runCommandsButton.UseVisualStyleBackColor = false;
            this.runCommandsButton.Click += new System.EventHandler(this.runCommandsButton_Click);
            // 
            // autoResetRobotCheckBox
            // 
            this.autoResetRobotCheckBox.AutoSize = true;
            this.autoResetRobotCheckBox.Checked = true;
            this.autoResetRobotCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoResetRobotCheckBox.Location = new System.Drawing.Point(920, 41);
            this.autoResetRobotCheckBox.Name = "autoResetRobotCheckBox";
            this.autoResetRobotCheckBox.Size = new System.Drawing.Size(111, 17);
            this.autoResetRobotCheckBox.TabIndex = 10;
            this.autoResetRobotCheckBox.Text = "Auto Reset Robot";
            this.autoResetRobotCheckBox.UseVisualStyleBackColor = true;
            // 
            // resetRobotButton
            // 
            this.resetRobotButton.Location = new System.Drawing.Point(920, 7);
            this.resetRobotButton.Name = "resetRobotButton";
            this.resetRobotButton.Size = new System.Drawing.Size(120, 28);
            this.resetRobotButton.TabIndex = 11;
            this.resetRobotButton.Text = "Reset Robot";
            this.resetRobotButton.UseVisualStyleBackColor = true;
            this.resetRobotButton.Click += new System.EventHandler(this.resetRobotButton_Click);
            // 
            // setAsStartButton
            // 
            this.setAsStartButton.Location = new System.Drawing.Point(1046, 7);
            this.setAsStartButton.Name = "setAsStartButton";
            this.setAsStartButton.Size = new System.Drawing.Size(120, 28);
            this.setAsStartButton.TabIndex = 12;
            this.setAsStartButton.Text = "Set as Start";
            this.setAsStartButton.UseVisualStyleBackColor = true;
            this.setAsStartButton.Click += new System.EventHandler(this.setAsStartButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Enabled = false;
            this.saveButton.Location = new System.Drawing.Point(921, 185);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(119, 28);
            this.saveButton.TabIndex = 13;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveAsButton
            // 
            this.saveAsButton.Location = new System.Drawing.Point(1045, 185);
            this.saveAsButton.Name = "saveAsButton";
            this.saveAsButton.Size = new System.Drawing.Size(119, 28);
            this.saveAsButton.TabIndex = 14;
            this.saveAsButton.Text = "Save As";
            this.saveAsButton.UseVisualStyleBackColor = true;
            this.saveAsButton.Click += new System.EventHandler(this.saveAsButton_Click);
            // 
            // openFileButton
            // 
            this.openFileButton.Location = new System.Drawing.Point(920, 219);
            this.openFileButton.Name = "openFileButton";
            this.openFileButton.Size = new System.Drawing.Size(119, 28);
            this.openFileButton.TabIndex = 15;
            this.openFileButton.Text = "Open";
            this.openFileButton.UseVisualStyleBackColor = true;
            this.openFileButton.Click += new System.EventHandler(this.openFileButton_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.Filter = "WRO Program files (*.wro)|*.wro";
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.Filter = "WRO Program files (*.wro)|*.wro";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1167, 767);
            this.Controls.Add(this.openFileButton);
            this.Controls.Add(this.saveAsButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.setAsStartButton);
            this.Controls.Add(this.resetRobotButton);
            this.Controls.Add(this.autoResetRobotCheckBox);
            this.Controls.Add(this.runCommandsButton);
            this.Controls.Add(this.fieldPictureBox);
            this.Controls.Add(this.runCommandsTreeView);
            this.Controls.Add(this.removeCommandButton);
            this.Controls.Add(this.addCommandButton);
            this.Controls.Add(this.commandListBox);
            this.Controls.Add(this.unSetVariableButton);
            this.Controls.Add(this.setVariableButton);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fieldPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button setVariableButton;
        private System.Windows.Forms.Button unSetVariableButton;
        private System.Windows.Forms.ListBox commandListBox;
        private System.Windows.Forms.Button addCommandButton;
        private System.Windows.Forms.Button removeCommandButton;
        private System.Windows.Forms.TreeView runCommandsTreeView;
        private System.Windows.Forms.PictureBox fieldPictureBox;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button runCommandsButton;
        private System.Windows.Forms.CheckBox autoResetRobotCheckBox;
        private System.Windows.Forms.Button resetRobotButton;
        private System.Windows.Forms.Button setAsStartButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button saveAsButton;
        private System.Windows.Forms.Button openFileButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}

