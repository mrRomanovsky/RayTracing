namespace Lab6
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxViewVectorX = new System.Windows.Forms.TextBox();
            this.textBoxViewVectorZ = new System.Windows.Forms.TextBox();
            this.textBoxViewVectorY = new System.Windows.Forms.TextBox();
            this.textBoxCameraY = new System.Windows.Forms.TextBox();
            this.textBoxCameraZ = new System.Windows.Forms.TextBox();
            this.textBoxCameraX = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.textBoxVerticalAngle = new System.Windows.Forms.TextBox();
            this.BuildSceneButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(13, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(484, 340);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(702, 41);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(82, 13);
            this.label9.TabIndex = 41;
            this.label9.Text = "Вектор обзора";
            // 
            // textBoxViewVectorX
            // 
            this.textBoxViewVectorX.Location = new System.Drawing.Point(787, 38);
            this.textBoxViewVectorX.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxViewVectorX.Name = "textBoxViewVectorX";
            this.textBoxViewVectorX.Size = new System.Drawing.Size(24, 20);
            this.textBoxViewVectorX.TabIndex = 42;
            this.textBoxViewVectorX.Text = "-5";
            // 
            // textBoxViewVectorZ
            // 
            this.textBoxViewVectorZ.Location = new System.Drawing.Point(843, 38);
            this.textBoxViewVectorZ.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxViewVectorZ.Name = "textBoxViewVectorZ";
            this.textBoxViewVectorZ.Size = new System.Drawing.Size(24, 20);
            this.textBoxViewVectorZ.TabIndex = 43;
            this.textBoxViewVectorZ.Text = "-2";
            // 
            // textBoxViewVectorY
            // 
            this.textBoxViewVectorY.Location = new System.Drawing.Point(815, 38);
            this.textBoxViewVectorY.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxViewVectorY.Name = "textBoxViewVectorY";
            this.textBoxViewVectorY.Size = new System.Drawing.Size(24, 20);
            this.textBoxViewVectorY.TabIndex = 44;
            this.textBoxViewVectorY.Text = "-5";
            // 
            // textBoxCameraY
            // 
            this.textBoxCameraY.Location = new System.Drawing.Point(815, 12);
            this.textBoxCameraY.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCameraY.Name = "textBoxCameraY";
            this.textBoxCameraY.Size = new System.Drawing.Size(24, 20);
            this.textBoxCameraY.TabIndex = 70;
            this.textBoxCameraY.Text = "5";
            // 
            // textBoxCameraZ
            // 
            this.textBoxCameraZ.Location = new System.Drawing.Point(843, 12);
            this.textBoxCameraZ.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCameraZ.Name = "textBoxCameraZ";
            this.textBoxCameraZ.Size = new System.Drawing.Size(24, 20);
            this.textBoxCameraZ.TabIndex = 69;
            this.textBoxCameraZ.Text = "0";
            // 
            // textBoxCameraX
            // 
            this.textBoxCameraX.Location = new System.Drawing.Point(787, 12);
            this.textBoxCameraX.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxCameraX.Name = "textBoxCameraX";
            this.textBoxCameraX.Size = new System.Drawing.Size(24, 20);
            this.textBoxCameraX.TabIndex = 68;
            this.textBoxCameraX.Text = "5";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(702, 15);
            this.label18.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(46, 13);
            this.label18.TabIndex = 71;
            this.label18.Text = "Камера";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(702, 67);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(88, 13);
            this.label19.TabIndex = 72;
            this.label19.Text = "Угол вертикали";
            // 
            // textBoxVerticalAngle
            // 
            this.textBoxVerticalAngle.Location = new System.Drawing.Point(815, 64);
            this.textBoxVerticalAngle.Margin = new System.Windows.Forms.Padding(2);
            this.textBoxVerticalAngle.Name = "textBoxVerticalAngle";
            this.textBoxVerticalAngle.Size = new System.Drawing.Size(52, 20);
            this.textBoxVerticalAngle.TabIndex = 73;
            this.textBoxVerticalAngle.Text = "0";
            // 
            // BuildSceneButton
            // 
            this.BuildSceneButton.Location = new System.Drawing.Point(890, 244);
            this.BuildSceneButton.Margin = new System.Windows.Forms.Padding(2);
            this.BuildSceneButton.Name = "BuildSceneButton";
            this.BuildSceneButton.Size = new System.Drawing.Size(80, 39);
            this.BuildSceneButton.TabIndex = 79;
            this.BuildSceneButton.Text = "Построить сцену";
            this.BuildSceneButton.UseVisualStyleBackColor = true;
            this.BuildSceneButton.Click += new System.EventHandler(this.BuildSceneButtonClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(990, 557);
            this.Controls.Add(this.BuildSceneButton);
            this.Controls.Add(this.textBoxVerticalAngle);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.textBoxCameraY);
            this.Controls.Add(this.textBoxCameraZ);
            this.Controls.Add(this.textBoxCameraX);
            this.Controls.Add(this.textBoxViewVectorY);
            this.Controls.Add(this.textBoxViewVectorZ);
            this.Controls.Add(this.textBoxViewVectorX);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.pictureBox1);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxViewVectorX;
        private System.Windows.Forms.TextBox textBoxViewVectorZ;
        private System.Windows.Forms.TextBox textBoxViewVectorY;
        private System.Windows.Forms.TextBox textBoxCameraY;
        private System.Windows.Forms.TextBox textBoxCameraZ;
        private System.Windows.Forms.TextBox textBoxCameraX;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox textBoxVerticalAngle;
        private System.Windows.Forms.Button BuildSceneButton;
    }
}

