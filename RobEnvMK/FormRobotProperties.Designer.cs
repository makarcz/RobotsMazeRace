namespace RobEnvMK
{
    partial class FormRobotProperties
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
            this.labelScript = new System.Windows.Forms.Label();
            this.labelPriority = new System.Windows.Forms.Label();
            this.labelTexture = new System.Windows.Forms.Label();
            this.comboBoxScript = new System.Windows.Forms.ComboBox();
            this.comboBoxPriority = new System.Windows.Forms.ComboBox();
            this.comboBoxTexture = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // labelScript
            // 
            this.labelScript.AutoSize = true;
            this.labelScript.Location = new System.Drawing.Point(12, 20);
            this.labelScript.Name = "labelScript";
            this.labelScript.Size = new System.Drawing.Size(72, 13);
            this.labelScript.TabIndex = 0;
            this.labelScript.Text = "Path to script:";
            // 
            // labelPriority
            // 
            this.labelPriority.AutoSize = true;
            this.labelPriority.Location = new System.Drawing.Point(12, 47);
            this.labelPriority.Name = "labelPriority";
            this.labelPriority.Size = new System.Drawing.Size(41, 13);
            this.labelPriority.TabIndex = 1;
            this.labelPriority.Text = "Priority:";
            // 
            // labelTexture
            // 
            this.labelTexture.AutoSize = true;
            this.labelTexture.Location = new System.Drawing.Point(12, 75);
            this.labelTexture.Name = "labelTexture";
            this.labelTexture.Size = new System.Drawing.Size(79, 13);
            this.labelTexture.TabIndex = 2;
            this.labelTexture.Text = "Path to texture:";
            // 
            // comboBoxScript
            // 
            this.comboBoxScript.FormattingEnabled = true;
            this.comboBoxScript.Location = new System.Drawing.Point(97, 17);
            this.comboBoxScript.Name = "comboBoxScript";
            this.comboBoxScript.Size = new System.Drawing.Size(267, 21);
            this.comboBoxScript.TabIndex = 3;
            this.comboBoxScript.SelectionChangeCommitted += new System.EventHandler(this.comboBoxScript_SelectionChangeCommitted);
            // 
            // comboBoxPriority
            // 
            this.comboBoxPriority.FormattingEnabled = true;
            this.comboBoxPriority.Location = new System.Drawing.Point(97, 44);
            this.comboBoxPriority.Name = "comboBoxPriority";
            this.comboBoxPriority.Size = new System.Drawing.Size(114, 21);
            this.comboBoxPriority.TabIndex = 4;
            this.comboBoxPriority.SelectionChangeCommitted += new System.EventHandler(this.comboBoxPriority_SelectionChangeCommitted);
            // 
            // comboBoxTexture
            // 
            this.comboBoxTexture.FormattingEnabled = true;
            this.comboBoxTexture.Location = new System.Drawing.Point(97, 72);
            this.comboBoxTexture.Name = "comboBoxTexture";
            this.comboBoxTexture.Size = new System.Drawing.Size(267, 21);
            this.comboBoxTexture.TabIndex = 5;
            this.comboBoxTexture.SelectionChangeCommitted += new System.EventHandler(this.comboBoxTexture_SelectionChangeCommitted);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(208, 117);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 6;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Location = new System.Drawing.Point(289, 117);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 7;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FormRobotProperties
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(387, 156);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.comboBoxTexture);
            this.Controls.Add(this.comboBoxPriority);
            this.Controls.Add(this.comboBoxScript);
            this.Controls.Add(this.labelTexture);
            this.Controls.Add(this.labelPriority);
            this.Controls.Add(this.labelScript);
            this.Name = "FormRobotProperties";
            this.Text = "Robot Properties Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelScript;
        private System.Windows.Forms.Label labelPriority;
        private System.Windows.Forms.Label labelTexture;
        private System.Windows.Forms.ComboBox comboBoxScript;
        private System.Windows.Forms.ComboBox comboBoxPriority;
        private System.Windows.Forms.ComboBox comboBoxTexture;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}