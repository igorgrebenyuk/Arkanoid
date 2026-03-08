namespace ArkanoidGame
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
            components = new System.ComponentModel.Container();
            timer1 = new System.Windows.Forms.Timer(components);
            pbPaddle = new PictureBox();
            pbBall = new PictureBox();
            gameTimer = new System.Windows.Forms.Timer(components);
            ((System.ComponentModel.ISupportInitialize)pbPaddle).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pbBall).BeginInit();
            SuspendLayout();
            // 
            // pbPaddle
            // 
            pbPaddle.BackColor = Color.Blue;
            pbPaddle.Location = new Point(303, 529);
            pbPaddle.Name = "pbPaddle";
            pbPaddle.Size = new Size(150, 20);
            pbPaddle.TabIndex = 0;
            pbPaddle.TabStop = false;
            // 
            // pbBall
            // 
            pbBall.BackColor = Color.Red;
            pbBall.Location = new Point(368, 503);
            pbBall.Name = "pbBall";
            pbBall.Size = new Size(20, 20);
            pbBall.TabIndex = 1;
            pbBall.TabStop = false;
            // 
            // gameTimer
            // 
            gameTimer.Enabled = true;
            gameTimer.Interval = 20;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(784, 561);
            Controls.Add(pbBall);
            Controls.Add(pbPaddle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            KeyPreview = true;
            MaximizeBox = false;
            Name = "Form1";
            Text = "Арканоид";
            ((System.ComponentModel.ISupportInitialize)pbPaddle).EndInit();
            ((System.ComponentModel.ISupportInitialize)pbBall).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private PictureBox pbPaddle;
        private PictureBox pbBall;
    }
}
