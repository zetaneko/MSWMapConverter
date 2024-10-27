namespace MSWMapConverterGUI
{
    partial class Form1
    {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            groupBox1 = new GroupBox();
            MapLB = new ListBox();
            label2 = new Label();
            regionCB = new ComboBox();
            label1 = new Label();
            mswPathBtn = new Button();
            groupBox2 = new GroupBox();
            label4 = new Label();
            WZFolderPathTB = new TextBox();
            WZFolderPathBtn = new Button();
            label3 = new Label();
            mswPathTB = new TextBox();
            convertBtn = new Button();
            progressBar1 = new ProgressBar();
            conversionStatusLabel = new Label();
            logDetails = new RichTextBox();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(MapLB);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(regionCB);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(193, 379);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Map Selector";
            // 
            // MapLB
            // 
            MapLB.FormattingEnabled = true;
            MapLB.ItemHeight = 15;
            MapLB.Location = new Point(6, 112);
            MapLB.Name = "MapLB";
            MapLB.SelectionMode = SelectionMode.MultiExtended;
            MapLB.Size = new Size(176, 259);
            MapLB.TabIndex = 4;
            MapLB.SelectedValueChanged += MapSelected;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 45);
            label2.Name = "label2";
            label2.Size = new Size(44, 15);
            label2.TabIndex = 3;
            label2.Text = "Region";
            // 
            // regionCB
            // 
            regionCB.FormattingEnabled = true;
            regionCB.Location = new Point(6, 63);
            regionCB.Name = "regionCB";
            regionCB.Size = new Size(176, 23);
            regionCB.TabIndex = 2;
            regionCB.SelectedIndexChanged += RegionSelected;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 94);
            label1.Name = "label1";
            label1.Size = new Size(31, 15);
            label1.TabIndex = 1;
            label1.Text = "Map";
            // 
            // mswPathBtn
            // 
            mswPathBtn.Location = new Point(248, 37);
            mswPathBtn.Name = "mswPathBtn";
            mswPathBtn.Size = new Size(100, 23);
            mswPathBtn.TabIndex = 2;
            mswPathBtn.Text = "Select folder";
            mswPathBtn.UseVisualStyleBackColor = true;
            mswPathBtn.Click += mswPathBtn_Click;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(WZFolderPathTB);
            groupBox2.Controls.Add(WZFolderPathBtn);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(mswPathTB);
            groupBox2.Controls.Add(mswPathBtn);
            groupBox2.Location = new Point(211, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(354, 121);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Directories";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 72);
            label4.Name = "label4";
            label4.Size = new Size(140, 15);
            label4.TabIndex = 8;
            label4.Text = "WZ Extracted Folder Path";
            // 
            // WZFolderPathTB
            // 
            WZFolderPathTB.Enabled = false;
            WZFolderPathTB.Location = new Point(6, 90);
            WZFolderPathTB.Name = "WZFolderPathTB";
            WZFolderPathTB.Size = new Size(236, 23);
            WZFolderPathTB.TabIndex = 7;
            WZFolderPathTB.TextAlign = HorizontalAlignment.Right;
            WZFolderPathTB.TextChanged += WZFolderPathSet;
            // 
            // WZFolderPathBtn
            // 
            WZFolderPathBtn.Location = new Point(248, 90);
            WZFolderPathBtn.Name = "WZFolderPathBtn";
            WZFolderPathBtn.Size = new Size(100, 23);
            WZFolderPathBtn.TabIndex = 6;
            WZFolderPathBtn.Text = "Select folder";
            WZFolderPathBtn.UseVisualStyleBackColor = true;
            WZFolderPathBtn.Click += WZFolderPathBtn_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 19);
            label3.Name = "label3";
            label3.Size = new Size(185, 15);
            label3.TabIndex = 5;
            label3.Text = "MSW Extracted World Folder Path";
            // 
            // mswPathTB
            // 
            mswPathTB.Enabled = false;
            mswPathTB.Location = new Point(6, 37);
            mswPathTB.Name = "mswPathTB";
            mswPathTB.Size = new Size(236, 23);
            mswPathTB.TabIndex = 3;
            mswPathTB.TextAlign = HorizontalAlignment.Right;
            // 
            // convertBtn
            // 
            convertBtn.Enabled = false;
            convertBtn.Location = new Point(211, 322);
            convertBtn.Name = "convertBtn";
            convertBtn.Size = new Size(354, 69);
            convertBtn.TabIndex = 4;
            convertBtn.Text = "Convert";
            convertBtn.UseVisualStyleBackColor = true;
            convertBtn.Click += convertBtn_Click;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(211, 293);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(354, 23);
            progressBar1.TabIndex = 5;
            // 
            // conversionStatusLabel
            // 
            conversionStatusLabel.AutoSize = true;
            conversionStatusLabel.Location = new Point(211, 275);
            conversionStatusLabel.Name = "conversionStatusLabel";
            conversionStatusLabel.Size = new Size(0, 15);
            conversionStatusLabel.TabIndex = 6;
            // 
            // logDetails
            // 
            logDetails.BackColor = Color.White;
            logDetails.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            logDetails.Location = new Point(211, 139);
            logDetails.Name = "logDetails";
            logDetails.ReadOnly = true;
            logDetails.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            logDetails.Size = new Size(351, 133);
            logDetails.TabIndex = 7;
            logDetails.Text = "";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(574, 394);
            Controls.Add(logDetails);
            Controls.Add(conversionStatusLabel);
            Controls.Add(progressBar1);
            Controls.Add(convertBtn);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "MSW Map Converter";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private GroupBox groupBox1;
        private Label label1;
        private ComboBox regionCB;
        private Label label2;
        private ListBox MapLB;
        private Button mswPathBtn;
        private GroupBox groupBox2;
        private TextBox mswPathTB;
        private Label label3;
        private Label label4;
        private TextBox WZFolderPathTB;
        private Button WZFolderPathBtn;
        private Button convertBtn;
        private ProgressBar progressBar1;
        private Label conversionStatusLabel;
        private RichTextBox logDetails;
    }
}
