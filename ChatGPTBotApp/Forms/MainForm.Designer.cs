namespace ChatGPTBotApp
{
    partial class MainForm
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
            chatBox = new RichTextBox();
            inputBox = new TextBox();
            sendButton = new Button();
            uploadButton = new Button();
            SuspendLayout();
            // 
            // chatBox
            // 
            chatBox.Location = new Point(34, 35);
            chatBox.Name = "chatBox";
            chatBox.ReadOnly = true;
            chatBox.Size = new Size(550, 529);
            chatBox.TabIndex = 0;
            chatBox.Text = "";
            // 
            // inputBox
            // 
            inputBox.Dock = DockStyle.Bottom;
            inputBox.Font = new Font("Segoe UI", 15.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            inputBox.Location = new Point(0, 625);
            inputBox.Multiline = true;
            inputBox.Name = "inputBox";
            inputBox.Size = new Size(621, 52);
            inputBox.TabIndex = 1;
            // 
            // sendButton
            // 
            sendButton.Location = new Point(475, 584);
            sendButton.Name = "sendButton";
            sendButton.RightToLeft = RightToLeft.Yes;
            sendButton.Size = new Size(109, 35);
            sendButton.TabIndex = 2;
            sendButton.Text = "Send";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // uploadButton
            // 
            uploadButton.Location = new Point(34, 584);
            uploadButton.Name = "uploadButton";
            uploadButton.Size = new Size(181, 35);
            uploadButton.TabIndex = 3;
            uploadButton.Text = "Upload File";
            uploadButton.UseVisualStyleBackColor = true;
            uploadButton.Click += uploadButton_Click;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(621, 677);
            Controls.Add(uploadButton);
            Controls.Add(sendButton);
            Controls.Add(inputBox);
            Controls.Add(chatBox);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ForeColor = Color.Black;
            Margin = new Padding(4);
            Name = "MainForm";
            Text = "Chat with GPT-kun";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;
        private Button uploadButton;
    }
}
