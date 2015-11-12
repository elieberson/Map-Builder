namespace NewMapEditor
{
    partial class AddPoly
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
            this.Name_Label = new System.Windows.Forms.Label();
            this.Name_Box = new System.Windows.Forms.TextBox();
            this.Wall_But = new System.Windows.Forms.RadioButton();
            this.Object_But = new System.Windows.Forms.RadioButton();
            this.Region_But = new System.Windows.Forms.RadioButton();
            this.Field_But = new System.Windows.Forms.RadioButton();
            this.ErrorName = new System.Windows.Forms.Label();
            this.ErrorType = new System.Windows.Forms.Label();
            this.AccRobot = new System.Windows.Forms.CheckBox();
            this.OK = new System.Windows.Forms.Button();
            this.ErrorName2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Name_Label
            // 
            this.Name_Label.AutoSize = true;
            this.Name_Label.Location = new System.Drawing.Point(12, 15);
            this.Name_Label.Name = "Name_Label";
            this.Name_Label.Size = new System.Drawing.Size(35, 13);
            this.Name_Label.TabIndex = 0;
            this.Name_Label.Text = "Name";
            // 
            // Name_Box
            // 
            this.Name_Box.Location = new System.Drawing.Point(53, 12);
            this.Name_Box.Name = "Name_Box";
            this.Name_Box.Size = new System.Drawing.Size(150, 20);
            this.Name_Box.TabIndex = 1;
            // 
            // Wall
            // 
            this.Wall_But.AutoSize = true;
            this.Wall_But.Location = new System.Drawing.Point(12, 42);
            this.Wall_But.Name = "Wall";
            this.Wall_But.Size = new System.Drawing.Size(46, 17);
            this.Wall_But.TabIndex = 2;
            this.Wall_But.TabStop = true;
            this.Wall_But.Text = "Wall";
            this.Wall_But.UseVisualStyleBackColor = true;
            this.Wall_But.Click += new System.EventHandler(this.Wall_Click);
            // 
            // Object
            // 
            this.Object_But.AutoSize = true;
            this.Object_But.Location = new System.Drawing.Point(12, 65);
            this.Object_But.Name = "Object";
            this.Object_But.Size = new System.Drawing.Size(56, 17);
            this.Object_But.TabIndex = 3;
            this.Object_But.TabStop = true;
            this.Object_But.Text = "Object";
            this.Object_But.UseVisualStyleBackColor = true;
            this.Object_But.Click += new System.EventHandler(this.Object_Click);
            // 
            // Region
            // 
            this.Region_But.AutoSize = true;
            this.Region_But.Location = new System.Drawing.Point(12, 88);
            this.Region_But.Name = "Region";
            this.Region_But.Size = new System.Drawing.Size(59, 17);
            this.Region_But.TabIndex = 4;
            this.Region_But.TabStop = true;
            this.Region_But.Text = "Region";
            this.Region_But.UseVisualStyleBackColor = true;
            this.Region_But.Click += new System.EventHandler(this.Region_Click);
            // 
            // Field
            // 
            this.Field_But.AutoSize = true;
            this.Field_But.Location = new System.Drawing.Point(12, 111);
            this.Field_But.Name = "Field";
            this.Field_But.Size = new System.Drawing.Size(47, 17);
            this.Field_But.TabIndex = 5;
            this.Field_But.TabStop = true;
            this.Field_But.Text = "Field";
            this.Field_But.UseVisualStyleBackColor = true;
            this.Field_But.Click += new System.EventHandler(this.Field_Click);
            // 
            // ErrorName
            // 
            this.ErrorName.AutoSize = true;
            this.ErrorName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorName.ForeColor = System.Drawing.Color.Red;
            this.ErrorName.Location = new System.Drawing.Point(37, 131);
            this.ErrorName.Name = "ErrorName";
            this.ErrorName.Size = new System.Drawing.Size(168, 16);
            this.ErrorName.TabIndex = 6;
            this.ErrorName.Text = "Error: Must enter new name";
            // 
            // ErrorType
            // 
            this.ErrorType.AutoSize = true;
            this.ErrorType.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorType.ForeColor = System.Drawing.Color.Red;
            this.ErrorType.Location = new System.Drawing.Point(64, 131);
            this.ErrorType.Name = "ErrorType";
            this.ErrorType.Size = new System.Drawing.Size(139, 16);
            this.ErrorType.TabIndex = 7;
            this.ErrorType.Text = "Error: Must select type";
            // 
            // AccRobot
            // 
            this.AccRobot.AutoSize = true;
            this.AccRobot.Location = new System.Drawing.Point(69, 43);
            this.AccRobot.Name = "AccRobot";
            this.AccRobot.Size = new System.Drawing.Size(126, 17);
            this.AccRobot.TabIndex = 8;
            this.AccRobot.Text = "Accommodate Robot";
            this.AccRobot.UseVisualStyleBackColor = true;
            this.AccRobot.Click += new System.EventHandler(this.AccRobot_Click);
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(128, 105);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 9;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // ErrorName2
            // 
            this.ErrorName2.AutoSize = true;
            this.ErrorName2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ErrorName2.ForeColor = System.Drawing.Color.Red;
            this.ErrorName2.Location = new System.Drawing.Point(21, 131);
            this.ErrorName2.Name = "ErrorName2";
            this.ErrorName2.Size = new System.Drawing.Size(182, 16);
            this.ErrorName2.TabIndex = 10;
            this.ErrorName2.Text = "Error: Cannot use \\ / : * ? \" < > |";
            // 
            // AddPoly
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 156);
            this.Controls.Add(this.ErrorName2);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.AccRobot);
            this.Controls.Add(this.ErrorType);
            this.Controls.Add(this.ErrorName);
            this.Controls.Add(this.Field_But);
            this.Controls.Add(this.Region_But);
            this.Controls.Add(this.Object_But);
            this.Controls.Add(this.Wall_But);
            this.Controls.Add(this.Name_Box);
            this.Controls.Add(this.Name_Label);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "AddPoly";
            this.Text = "Add new item";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label Name_Label;
        private System.Windows.Forms.TextBox Name_Box;
        private System.Windows.Forms.RadioButton Wall_But;
        private System.Windows.Forms.RadioButton Object_But;
        private System.Windows.Forms.RadioButton Region_But;
        private System.Windows.Forms.RadioButton Field_But;
        private System.Windows.Forms.Label ErrorName;
        private System.Windows.Forms.Label ErrorType;
        private System.Windows.Forms.CheckBox AccRobot;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Label ErrorName2;
    }
}