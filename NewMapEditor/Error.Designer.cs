namespace NewMapEditor
{
    partial class Error
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
            this.Closer = new System.Windows.Forms.Button();
            this.Error_PointInside = new System.Windows.Forms.Label();
            this.Error_Label = new System.Windows.Forms.Label();
            this.Error_SaveInside = new System.Windows.Forms.Label();
            this.Error_Multi = new System.Windows.Forms.Label();
            this.Error_BadPoly = new System.Windows.Forms.Label();
            this.Error_NoFields = new System.Windows.Forms.Label();
            this.Error_Open = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Close
            // 
            this.Closer.Location = new System.Drawing.Point(146, 45);
            this.Closer.Name = "Close";
            this.Closer.Size = new System.Drawing.Size(75, 23);
            this.Closer.TabIndex = 0;
            this.Closer.Text = "OK";
            this.Closer.UseVisualStyleBackColor = true;
            this.Closer.Click += new System.EventHandler(this.Close_Click);
            // 
            // Error_PointInside
            // 
            this.Error_PointInside.AutoSize = true;
            this.Error_PointInside.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_PointInside.ForeColor = System.Drawing.Color.Red;
            this.Error_PointInside.Location = new System.Drawing.Point(12, 26);
            this.Error_PointInside.Name = "Error_PointInside";
            this.Error_PointInside.Size = new System.Drawing.Size(328, 16);
            this.Error_PointInside.TabIndex = 1;
            this.Error_PointInside.Text = "A point on a field cannot be placed inside another item";
            // 
            // Error_Label
            // 
            this.Error_Label.AutoSize = true;
            this.Error_Label.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_Label.ForeColor = System.Drawing.Color.Red;
            this.Error_Label.Location = new System.Drawing.Point(166, 9);
            this.Error_Label.Name = "Error_Label";
            this.Error_Label.Size = new System.Drawing.Size(40, 16);
            this.Error_Label.TabIndex = 2;
            this.Error_Label.Text = "Error:";
            // 
            // Error_SaveInside
            // 
            this.Error_SaveInside.AutoSize = true;
            this.Error_SaveInside.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_SaveInside.ForeColor = System.Drawing.Color.Red;
            this.Error_SaveInside.Location = new System.Drawing.Point(28, 26);
            this.Error_SaveInside.Name = "Error_SaveInside";
            this.Error_SaveInside.Size = new System.Drawing.Size(291, 16);
            this.Error_SaveInside.TabIndex = 3;
            this.Error_SaveInside.Text = "At least one point of a field is inside another item";
            // 
            // Error_Multi
            // 
            this.Error_Multi.AutoSize = true;
            this.Error_Multi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_Multi.ForeColor = System.Drawing.Color.Red;
            this.Error_Multi.Location = new System.Drawing.Point(46, 26);
            this.Error_Multi.Name = "Error_Multi";
            this.Error_Multi.Size = new System.Drawing.Size(255, 16);
            this.Error_Multi.TabIndex = 4;
            this.Error_Multi.Text = "Cannot add two fields with the same name";
            // 
            // Error_BadPoly
            // 
            this.Error_BadPoly.AutoSize = true;
            this.Error_BadPoly.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_BadPoly.ForeColor = System.Drawing.Color.Red;
            this.Error_BadPoly.Location = new System.Drawing.Point(46, 26);
            this.Error_BadPoly.Name = "Error_BadPoly";
            this.Error_BadPoly.Size = new System.Drawing.Size(259, 16);
            this.Error_BadPoly.TabIndex = 5;
            this.Error_BadPoly.Text = "New point makes self-intersecting polygon";
            // 
            // Error_NoFields
            // 
            this.Error_NoFields.AutoSize = true;
            this.Error_NoFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_NoFields.ForeColor = System.Drawing.Color.Red;
            this.Error_NoFields.Location = new System.Drawing.Point(73, 26);
            this.Error_NoFields.Name = "Error_NoFields";
            this.Error_NoFields.Size = new System.Drawing.Size(193, 16);
            this.Error_NoFields.TabIndex = 6;
            this.Error_NoFields.Text = "Fields are required for a saving";
            // 
            // Error_Open
            // 
            this.Error_Open.AutoSize = true;
            this.Error_Open.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_Open.ForeColor = System.Drawing.Color.Red;
            this.Error_Open.Location = new System.Drawing.Point(134, 26);
            this.Error_Open.Name = "Error_Open";
            this.Error_Open.Size = new System.Drawing.Size(98, 16);
            this.Error_Open.TabIndex = 7;
            this.Error_Open.Text = "File Open Error";
            // 
            // Error
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 80);
            this.Controls.Add(this.Error_Open);
            this.Controls.Add(this.Error_NoFields);
            this.Controls.Add(this.Error_BadPoly);
            this.Controls.Add(this.Error_Multi);
            this.Controls.Add(this.Error_SaveInside);
            this.Controls.Add(this.Error_Label);
            this.Controls.Add(this.Error_PointInside);
            this.Controls.Add(this.Closer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Error";
            this.Text = "Error";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Closer;
        private System.Windows.Forms.Label Error_PointInside;
        private System.Windows.Forms.Label Error_Label;
        private System.Windows.Forms.Label Error_SaveInside;
        private System.Windows.Forms.Label Error_Multi;
        private System.Windows.Forms.Label Error_BadPoly;
        private System.Windows.Forms.Label Error_NoFields;
        private System.Windows.Forms.Label Error_Open;
    }
}