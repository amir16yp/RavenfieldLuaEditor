using System.Windows.Forms;
namespace RavenfieldLuaEditor
{
    partial class Form1
    {
        private System.Windows.Forms.Button btnImportFile;

        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnImportFile = new System.Windows.Forms.Button();
            // ... (add after other button declarations)

            // Setup btnImportFile properties
            this.btnImportFile.Location = new System.Drawing.Point(/* specify location - perhaps next to Save button */);
            this.btnImportFile.Name = "btnImportFile";
            this.btnImportFile.Size = new System.Drawing.Size(100, 23);
            this.btnImportFile.Text = "Import File";
            this.btnImportFile.UseVisualStyleBackColor = true;
            this.btnImportFile.Click += new System.EventHandler(this.btnImportFile_Click);
            this.btnImportFile.Enabled = false; // Initially disabled until a TextAsset is selected

            // Add the button to Controls
            this.Controls.Add(this.btnImportFile);

            btnOpen = new Button();
            btnSave = new Button();
            btnRefresh = new Button();
            lstTextAssets = new ListBox();
            txtContent = new TextBox();
            splitContainer1 = new SplitContainer();
            lblStatus = new Label();
            lblCurrentFile = new Label();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // btnOpen
            // 
            btnOpen.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnOpen.Location = new Point(12, 12);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(150, 29);
            btnOpen.TabIndex = 0;
            btnOpen.Text = "Open Bundle";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += btnOpen_Click;
            // 
            // btnSave
            // 
            btnSave.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSave.Enabled = false;
            btnSave.Location = new Point(12, 406);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(150, 29);
            btnSave.TabIndex = 1;
            btnSave.Text = "Save Bundle";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            btnRefresh.Enabled = false;
            btnRefresh.Location = new Point(12, 47);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(150, 29);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // lstTextAssets
            // 
            lstTextAssets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lstTextAssets.FormattingEnabled = true;
            lstTextAssets.ItemHeight = 20;
            lstTextAssets.Location = new Point(12, 82);
            lstTextAssets.Name = "lstTextAssets";
            lstTextAssets.Size = new Size(150, 304);
            lstTextAssets.TabIndex = 3;
            lstTextAssets.SelectedIndexChanged += lstTextAssets_SelectedIndexChanged;
            // 
            // txtContent
            // 
            txtContent.AcceptsReturn = true;
            txtContent.AcceptsTab = true;
            txtContent.Dock = DockStyle.Fill;
            txtContent.Font = new Font("Consolas", 10F, FontStyle.Regular, GraphicsUnit.Point);
            txtContent.Location = new Point(0, 0);
            txtContent.Multiline = true;
            txtContent.Name = "txtContent";
            txtContent.ScrollBars = ScrollBars.Both;
            txtContent.Size = new Size(622, 406);
            txtContent.TabIndex = 4;
            txtContent.WordWrap = false;
            txtContent.TextChanged += txtContent_TextChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(0, 12);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lblStatus);
            splitContainer1.Panel1.Controls.Add(btnOpen);
            splitContainer1.Panel1.Controls.Add(btnSave);
            splitContainer1.Panel1.Controls.Add(btnRefresh);
            splitContainer1.Panel1.Controls.Add(lstTextAssets);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(lblCurrentFile);
            splitContainer1.Panel2.Controls.Add(txtContent);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 174;
            splitContainer1.TabIndex = 5;
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lblStatus.AutoEllipsis = true;
            lblStatus.Location = new Point(12, 389);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(150, 20);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "Ready";
            // 
            // lblCurrentFile
            // 
            lblCurrentFile.Dock = DockStyle.Bottom;
            lblCurrentFile.Location = new Point(0, 406);
            lblCurrentFile.Name = "lblCurrentFile";
            lblCurrentFile.Padding = new Padding(5);
            lblCurrentFile.Size = new Size(622, 44);
            lblCurrentFile.TabIndex = 5;
            lblCurrentFile.Text = "No file loaded";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Name = "Form1";
            Text = "Ravenfield Lua Editor";
            Load += Form1_Load;
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Button btnOpen;
        private Button btnSave;
        private Button btnRefresh;
        private ListBox lstTextAssets;
        private TextBox txtContent;
        private SplitContainer splitContainer1;
        private Label lblStatus;
        private Label lblCurrentFile;
    }
}