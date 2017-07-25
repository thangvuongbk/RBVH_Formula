namespace RBVH_FORMULA.View
{
    partial class DisplayLog
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
            this.logView = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFormulaTD = new System.Windows.Forms.TextBox();
            this.txtCode = new System.Windows.Forms.TextBox();
            this.btnSourceFileBrowser = new System.Windows.Forms.Button();
            this.btnTDBrowser = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // logView
            // 
            this.logView.Location = new System.Drawing.Point(12, 122);
            this.logView.Name = "logView";
            this.logView.Size = new System.Drawing.Size(627, 284);
            this.logView.TabIndex = 0;
            this.logView.Text = "";
            this.logView.TextChanged += new System.EventHandler(this.myrichTextBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(365, 93);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.RBVHMain);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "TD Template";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Source File";
            // 
            // txtFormulaTD
            // 
            this.txtFormulaTD.Location = new System.Drawing.Point(89, 22);
            this.txtFormulaTD.Name = "txtFormulaTD";
            this.txtFormulaTD.Size = new System.Drawing.Size(446, 20);
            this.txtFormulaTD.TabIndex = 4;
            // 
            // txtCode
            // 
            this.txtCode.Location = new System.Drawing.Point(89, 62);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(446, 20);
            this.txtCode.TabIndex = 5;
            // 
            // btnSourceFileBrowser
            // 
            this.btnSourceFileBrowser.Location = new System.Drawing.Point(564, 57);
            this.btnSourceFileBrowser.Name = "btnSourceFileBrowser";
            this.btnSourceFileBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnSourceFileBrowser.TabIndex = 6;
            this.btnSourceFileBrowser.Text = "Browser";
            this.btnSourceFileBrowser.UseVisualStyleBackColor = true;
            this.btnSourceFileBrowser.Click += new System.EventHandler(this.eventCCodeBrowseClick);
            // 
            // btnTDBrowser
            // 
            this.btnTDBrowser.Location = new System.Drawing.Point(564, 17);
            this.btnTDBrowser.Name = "btnTDBrowser";
            this.btnTDBrowser.Size = new System.Drawing.Size(75, 23);
            this.btnTDBrowser.TabIndex = 7;
            this.btnTDBrowser.Text = "Browser";
            this.btnTDBrowser.UseVisualStyleBackColor = true;
            this.btnTDBrowser.Click += new System.EventHandler(this.btnTDBrowser_Click);
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(460, 93);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 8;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.eventCancelClick);
            // 
            // DisplayLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 421);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnTDBrowser);
            this.Controls.Add(this.btnSourceFileBrowser);
            this.Controls.Add(this.txtCode);
            this.Controls.Add(this.txtFormulaTD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.logView);
            this.Name = "DisplayLog";
            this.Text = "DisplayLog";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox logView;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFormulaTD;
        private System.Windows.Forms.TextBox txtCode;
        private System.Windows.Forms.Button btnSourceFileBrowser;
        private System.Windows.Forms.Button btnTDBrowser;
        private System.Windows.Forms.Button btnExit;
    }
}