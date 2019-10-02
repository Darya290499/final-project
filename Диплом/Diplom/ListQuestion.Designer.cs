namespace Diplom
{
    partial class ListQuestion
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
            this.personal = new System.Windows.Forms.Button();
            this.documents = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // personal
            // 
            this.personal.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.personal.Location = new System.Drawing.Point(12, 58);
            this.personal.Name = "personal";
            this.personal.Size = new System.Drawing.Size(173, 46);
            this.personal.TabIndex = 0;
            this.personal.UseVisualStyleBackColor = true;
            this.personal.Click += new System.EventHandler(this.personal_Click);
            // 
            // documents
            // 
            this.documents.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.documents.Location = new System.Drawing.Point(191, 58);
            this.documents.Name = "documents";
            this.documents.Size = new System.Drawing.Size(188, 46);
            this.documents.TabIndex = 1;
            this.documents.UseVisualStyleBackColor = true;
            this.documents.Click += new System.EventHandler(this.documents_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))));
            this.label1.Location = new System.Drawing.Point(35, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(313, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Какой список необходимо создать?";
            // 
            // ListQuestion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(389, 116);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.documents);
            this.Controls.Add(this.personal);
            this.MinimumSize = new System.Drawing.Size(268, 154);
            this.Name = "ListQuestion";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button personal;
        private System.Windows.Forms.Button documents;
        private System.Windows.Forms.Label label1;
    }
}