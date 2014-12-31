namespace MultRangerWIconExpress
{
    partial class MainForm
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
            this.hWindowControl1 = new HalconDotNet.HWindowControl();
            this.initialize_button = new System.Windows.Forms.Button();
            this.capture_button = new System.Windows.Forms.Button();
            this.disconnect_button = new System.Windows.Forms.Button();
            this.MessegeTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // hWindowControl1
            // 
            this.hWindowControl1.BackColor = System.Drawing.Color.Black;
            this.hWindowControl1.BorderColor = System.Drawing.Color.Black;
            this.hWindowControl1.ImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl1.Location = new System.Drawing.Point(12, 12);
            this.hWindowControl1.Name = "hWindowControl1";
            this.hWindowControl1.Size = new System.Drawing.Size(237, 340);
            this.hWindowControl1.TabIndex = 0;
            this.hWindowControl1.WindowSize = new System.Drawing.Size(237, 340);
            // 
            // initialize_button
            // 
            this.initialize_button.Location = new System.Drawing.Point(12, 358);
            this.initialize_button.Name = "initialize_button";
            this.initialize_button.Size = new System.Drawing.Size(75, 23);
            this.initialize_button.TabIndex = 1;
            this.initialize_button.Text = "Initialize";
            this.initialize_button.UseVisualStyleBackColor = true;
            this.initialize_button.Click += new System.EventHandler(this.initialize_button_Click);
            // 
            // capture_button
            // 
            this.capture_button.Location = new System.Drawing.Point(93, 358);
            this.capture_button.Name = "capture_button";
            this.capture_button.Size = new System.Drawing.Size(75, 23);
            this.capture_button.TabIndex = 2;
            this.capture_button.Text = "Capture";
            this.capture_button.UseVisualStyleBackColor = true;
            this.capture_button.Click += new System.EventHandler(this.capture_button_Click);
            // 
            // disconnect_button
            // 
            this.disconnect_button.Location = new System.Drawing.Point(174, 358);
            this.disconnect_button.Name = "disconnect_button";
            this.disconnect_button.Size = new System.Drawing.Size(75, 23);
            this.disconnect_button.TabIndex = 3;
            this.disconnect_button.Text = "Disconnect";
            this.disconnect_button.UseVisualStyleBackColor = true;
            this.disconnect_button.Click += new System.EventHandler(this.disconnect_button_Click);
            // 
            // MessegeTextBox
            // 
            this.MessegeTextBox.Location = new System.Drawing.Point(271, 12);
            this.MessegeTextBox.Name = "MessegeTextBox";
            this.MessegeTextBox.Size = new System.Drawing.Size(206, 327);
            this.MessegeTextBox.TabIndex = 4;
            this.MessegeTextBox.Text = "";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(502, 393);
            this.Controls.Add(this.MessegeTextBox);
            this.Controls.Add(this.disconnect_button);
            this.Controls.Add(this.capture_button);
            this.Controls.Add(this.initialize_button);
            this.Controls.Add(this.hWindowControl1);
            this.Name = "MainForm";
            this.Text = "Main Form";
            this.ResumeLayout(false);

        }

        #endregion

        private HalconDotNet.HWindowControl hWindowControl1;
        private System.Windows.Forms.Button initialize_button;
        private System.Windows.Forms.Button capture_button;
        private System.Windows.Forms.Button disconnect_button;
        private System.Windows.Forms.RichTextBox MessegeTextBox;
    }
}

