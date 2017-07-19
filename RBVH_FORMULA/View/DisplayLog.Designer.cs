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
            this.SuspendLayout();
            // 
            // logView
            // 
            this.logView.Location = new System.Drawing.Point(12, 25);
            this.logView.Name = "logView";
            this.logView.Size = new System.Drawing.Size(656, 381);
            this.logView.TabIndex = 0;
            this.logView.Text = "";
            this.logView.TextChanged += new System.EventHandler(this.myrichTextBox_TextChanged);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(505, 413);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Run";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.RBVHMain);
            // 
            // DisplayLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(706, 454);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.logView);
            this.Name = "DisplayLog";
            this.Text = "DisplayLog";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox logView;
        private System.Windows.Forms.Button button1;
    }
}