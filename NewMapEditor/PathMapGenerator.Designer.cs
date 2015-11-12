namespace NewMapEditor
{
    partial class PathMapGenerator
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
            this.FieldView = new Tao.Platform.Windows.SimpleOpenGlControl();
            this.Number_Label = new System.Windows.Forms.Label();
            this.Error_OutsideField = new System.Windows.Forms.Label();
            this.Error_InsideItem = new System.Windows.Forms.Label();
            this.OK = new System.Windows.Forms.Button();
            this.Ctrl_press = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // FieldView
            // 
            this.FieldView.AccumBits = ((byte)(0));
            this.FieldView.AutoCheckErrors = false;
            this.FieldView.AutoFinish = false;
            this.FieldView.AutoMakeCurrent = true;
            this.FieldView.AutoSwapBuffers = true;
            this.FieldView.BackColor = System.Drawing.Color.Black;
            this.FieldView.ColorBits = ((byte)(32));
            this.FieldView.DepthBits = ((byte)(16));
            this.FieldView.Location = new System.Drawing.Point(0, 41);
            this.FieldView.Name = "FieldView";
            this.FieldView.Size = new System.Drawing.Size(484, 421);
            this.FieldView.StencilBits = ((byte)(0));
            this.FieldView.TabIndex = 0;
            // 
            // Number_Label
            // 
            this.Number_Label.AutoSize = true;
            this.Number_Label.Location = new System.Drawing.Point(12, 9);
            this.Number_Label.Name = "Number_Label";
            this.Number_Label.Size = new System.Drawing.Size(179, 13);
            this.Number_Label.TabIndex = 1;
            this.Number_Label.Text = "Define central free point (Ctrl + Click)";
            // 
            // Error_OutsideField
            // 
            this.Error_OutsideField.AutoSize = true;
            this.Error_OutsideField.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_OutsideField.ForeColor = System.Drawing.Color.Red;
            this.Error_OutsideField.Location = new System.Drawing.Point(12, 22);
            this.Error_OutsideField.Name = "Error_OutsideField";
            this.Error_OutsideField.Size = new System.Drawing.Size(155, 16);
            this.Error_OutsideField.TabIndex = 2;
            this.Error_OutsideField.Text = "Point must be inside field";
            // 
            // Error_InsideItem
            // 
            this.Error_InsideItem.AutoSize = true;
            this.Error_InsideItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Error_InsideItem.ForeColor = System.Drawing.Color.Red;
            this.Error_InsideItem.Location = new System.Drawing.Point(12, 22);
            this.Error_InsideItem.Name = "Error_InsideItem";
            this.Error_InsideItem.Size = new System.Drawing.Size(216, 16);
            this.Error_InsideItem.TabIndex = 3;
            this.Error_InsideItem.Text = "Point must be outside physical item";
            // 
            // OK
            // 
            this.OK.Location = new System.Drawing.Point(397, 9);
            this.OK.Name = "OK";
            this.OK.Size = new System.Drawing.Size(75, 23);
            this.OK.TabIndex = 4;
            this.OK.Text = "OK";
            this.OK.UseVisualStyleBackColor = true;
            this.OK.Click += new System.EventHandler(this.OK_Click);
            // 
            // Ctrl_press
            // 
            this.Ctrl_press.AutoSize = true;
            this.Ctrl_press.Location = new System.Drawing.Point(361, 51);
            this.Ctrl_press.Name = "Ctrl_press";
            this.Ctrl_press.Size = new System.Drawing.Size(111, 13);
            this.Ctrl_press.TabIndex = 5;
            this.Ctrl_press.Text = "Hold Ctrl to move map";
            // 
            // PathMapGenerator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 462);
            this.Controls.Add(this.Ctrl_press);
            this.Controls.Add(this.OK);
            this.Controls.Add(this.Error_InsideItem);
            this.Controls.Add(this.Error_OutsideField);
            this.Controls.Add(this.Number_Label);
            this.Controls.Add(this.FieldView);
            this.KeyPreview = true;
            this.Name = "PathMapGenerator";
            this.Text = "Define Central Free Points";
            this.SizeChanged += new System.EventHandler(this.PathMapGenerator_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PathMapGenerator_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PathMapGenerator_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl FieldView;
        private System.Windows.Forms.Label Number_Label;
        private System.Windows.Forms.Label Error_OutsideField;
        private System.Windows.Forms.Label Error_InsideItem;
        private System.Windows.Forms.Button OK;
        private System.Windows.Forms.Label Ctrl_press;
    }
}