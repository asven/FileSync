namespace FileSync
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
            this.SourceTextBox = new System.Windows.Forms.TextBox();
            this.SourceBrowseButton = new System.Windows.Forms.Button();
            this.DestinationBrowseButton = new System.Windows.Forms.Button();
            this.DestinationTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SaveSyncButton = new System.Windows.Forms.Button();
            this.CurrentSyncsGridView = new System.Windows.Forms.DataGridView();
            this.StartSyncButton = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.CurrentSyncsGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // SourceTextBox
            // 
            this.SourceTextBox.Location = new System.Drawing.Point(13, 145);
            this.SourceTextBox.Name = "SourceTextBox";
            this.SourceTextBox.Size = new System.Drawing.Size(202, 20);
            this.SourceTextBox.TabIndex = 1;
            // 
            // SourceBrowseButton
            // 
            this.SourceBrowseButton.Location = new System.Drawing.Point(221, 142);
            this.SourceBrowseButton.Name = "SourceBrowseButton";
            this.SourceBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.SourceBrowseButton.TabIndex = 2;
            this.SourceBrowseButton.Text = "Browse...";
            this.SourceBrowseButton.UseVisualStyleBackColor = true;
            this.SourceBrowseButton.Click += new System.EventHandler(this.SourceBrowseButton_Click);
            // 
            // DestinationBrowseButton
            // 
            this.DestinationBrowseButton.Location = new System.Drawing.Point(538, 145);
            this.DestinationBrowseButton.Name = "DestinationBrowseButton";
            this.DestinationBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.DestinationBrowseButton.TabIndex = 4;
            this.DestinationBrowseButton.Text = "Browse...";
            this.DestinationBrowseButton.UseVisualStyleBackColor = true;
            this.DestinationBrowseButton.Click += new System.EventHandler(this.DestinationBrowseButton_Click);
            // 
            // DestinationTextBox
            // 
            this.DestinationTextBox.BackColor = System.Drawing.Color.White;
            this.DestinationTextBox.Location = new System.Drawing.Point(316, 145);
            this.DestinationTextBox.Name = "DestinationTextBox";
            this.DestinationTextBox.Size = new System.Drawing.Size(216, 20);
            this.DestinationTextBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 129);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Source";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(313, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Destination";
            // 
            // SaveSyncButton
            // 
            this.SaveSyncButton.Location = new System.Drawing.Point(538, 184);
            this.SaveSyncButton.Name = "SaveSyncButton";
            this.SaveSyncButton.Size = new System.Drawing.Size(75, 23);
            this.SaveSyncButton.TabIndex = 7;
            this.SaveSyncButton.Text = "Save Sync";
            this.SaveSyncButton.UseVisualStyleBackColor = true;
            this.SaveSyncButton.Click += new System.EventHandler(this.SaveSyncButton_Click);
            // 
            // CurrentSyncsGridView
            // 
            this.CurrentSyncsGridView.AllowUserToAddRows = false;
            this.CurrentSyncsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CurrentSyncsGridView.Location = new System.Drawing.Point(12, 4);
            this.CurrentSyncsGridView.Name = "CurrentSyncsGridView";
            this.CurrentSyncsGridView.ReadOnly = true;
            this.CurrentSyncsGridView.Size = new System.Drawing.Size(601, 122);
            this.CurrentSyncsGridView.TabIndex = 8;
            // 
            // StartSyncButton
            // 
            this.StartSyncButton.Location = new System.Drawing.Point(15, 225);
            this.StartSyncButton.Name = "StartSyncButton";
            this.StartSyncButton.Size = new System.Drawing.Size(598, 63);
            this.StartSyncButton.TabIndex = 9;
            this.StartSyncButton.Text = "Start All Syncs Now";
            this.StartSyncButton.UseVisualStyleBackColor = true;
            this.StartSyncButton.Click += new System.EventHandler(this.StartSyncButton_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 332);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(601, 23);
            this.progressBar.TabIndex = 10;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 316);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(24, 13);
            this.statusLabel.TabIndex = 11;
            this.statusLabel.Text = "Idle";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 371);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.StartSyncButton);
            this.Controls.Add(this.CurrentSyncsGridView);
            this.Controls.Add(this.SaveSyncButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DestinationBrowseButton);
            this.Controls.Add(this.DestinationTextBox);
            this.Controls.Add(this.SourceBrowseButton);
            this.Controls.Add(this.SourceTextBox);
            this.Name = "Form1";
            this.Text = "Sync";
            ((System.ComponentModel.ISupportInitialize)(this.CurrentSyncsGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox SourceTextBox;
        private System.Windows.Forms.Button SourceBrowseButton;
        private System.Windows.Forms.Button DestinationBrowseButton;
        private System.Windows.Forms.TextBox DestinationTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button SaveSyncButton;
        private System.Windows.Forms.DataGridView CurrentSyncsGridView;
        private System.Windows.Forms.Button StartSyncButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label statusLabel;
    }
}

