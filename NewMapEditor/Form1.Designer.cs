namespace NewMapEditor
{
    partial class MainGUI
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.occupancyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.metricMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMetricMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generatePathMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hideOccupancyGridToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearMapToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.Delete = new System.Windows.Forms.Button();
            this.Add = new System.Windows.Forms.Button();
            this.Walls_Label = new System.Windows.Forms.Label();
            this.Objects_Label = new System.Windows.Forms.Label();
            this.Regions_Label = new System.Windows.Forms.Label();
            this.Fields_Label = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.Walls_Box = new System.Windows.Forms.ListBox();
            this.Objects_Box = new System.Windows.Forms.ListBox();
            this.Regions_Box = new System.Windows.Forms.ListBox();
            this.Fields_Box = new System.Windows.Forms.ListBox();
            this.Ctrl_press = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
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
            this.FieldView.Location = new System.Drawing.Point(0, 27);
            this.FieldView.Name = "FieldView";
            this.FieldView.Size = new System.Drawing.Size(605, 535);
            this.FieldView.StencilBits = ((byte)(0));
            this.FieldView.TabIndex = 0;
            this.FieldView.Load += new System.EventHandler(this.FieldView_Load);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveMetricMapToolStripMenuItem,
            this.generatePathMapToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.occupancyGridToolStripMenuItem,
            this.metricMapToolStripMenuItem});
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // occupancyGridToolStripMenuItem
            // 
            this.occupancyGridToolStripMenuItem.Name = "occupancyGridToolStripMenuItem";
            this.occupancyGridToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.occupancyGridToolStripMenuItem.Text = "Occupancy Grid";
            this.occupancyGridToolStripMenuItem.Click += new System.EventHandler(this.occupancyGridToolStripMenuItem_Click);
            // 
            // metricMapToolStripMenuItem
            // 
            this.metricMapToolStripMenuItem.Name = "metricMapToolStripMenuItem";
            this.metricMapToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.metricMapToolStripMenuItem.Text = "Metric Map";
            this.metricMapToolStripMenuItem.Click += new System.EventHandler(this.metricMapToolStripMenuItem_Click);
            // 
            // saveMetricMapToolStripMenuItem
            // 
            this.saveMetricMapToolStripMenuItem.Name = "saveMetricMapToolStripMenuItem";
            this.saveMetricMapToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.saveMetricMapToolStripMenuItem.Text = "Save Metric Map";
            this.saveMetricMapToolStripMenuItem.Click += new System.EventHandler(this.saveMetricMapToolStripMenuItem_Click);
            // 
            // generatePathMapToolStripMenuItem
            // 
            this.generatePathMapToolStripMenuItem.Name = "generatePathMapToolStripMenuItem";
            this.generatePathMapToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.generatePathMapToolStripMenuItem.Text = "Create and Save PRM ";
            this.generatePathMapToolStripMenuItem.Click += new System.EventHandler(this.generatePathMapToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideOccupancyGridToolStripMenuItem,
            this.clearMapToolStripMenuItem1});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // hideOccupancyGridToolStripMenuItem
            // 
            this.hideOccupancyGridToolStripMenuItem.Name = "hideOccupancyGridToolStripMenuItem";
            this.hideOccupancyGridToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.hideOccupancyGridToolStripMenuItem.Text = "Hide Occupancy Grid";
            this.hideOccupancyGridToolStripMenuItem.Click += new System.EventHandler(this.hideOccupancyGridToolStripMenuItem_Click);
            // 
            // clearMapToolStripMenuItem1
            // 
            this.clearMapToolStripMenuItem1.Name = "clearMapToolStripMenuItem1";
            this.clearMapToolStripMenuItem1.Size = new System.Drawing.Size(187, 22);
            this.clearMapToolStripMenuItem1.Text = "Clear Map";
            this.clearMapToolStripMenuItem1.Click += new System.EventHandler(this.clearMapToolStripMenuItem1_Click);
            // 
            // Delete
            // 
            this.Delete.Location = new System.Drawing.Point(699, 27);
            this.Delete.Name = "Delete";
            this.Delete.Size = new System.Drawing.Size(75, 23);
            this.Delete.TabIndex = 2;
            this.Delete.Text = "Delete";
            this.Delete.UseVisualStyleBackColor = true;
            this.Delete.Click += new System.EventHandler(this.Delete_Click);
            // 
            // Add
            // 
            this.Add.Location = new System.Drawing.Point(615, 27);
            this.Add.Name = "Add";
            this.Add.Size = new System.Drawing.Size(75, 23);
            this.Add.TabIndex = 3;
            this.Add.Text = "Add";
            this.Add.UseVisualStyleBackColor = true;
            this.Add.Click += new System.EventHandler(this.Add_Click);
            // 
            // Walls_Label
            // 
            this.Walls_Label.AutoSize = true;
            this.Walls_Label.Location = new System.Drawing.Point(612, 62);
            this.Walls_Label.Name = "Walls_Label";
            this.Walls_Label.Size = new System.Drawing.Size(33, 13);
            this.Walls_Label.TabIndex = 5;
            this.Walls_Label.Text = "Walls";
            // 
            // Objects_Label
            // 
            this.Objects_Label.AutoSize = true;
            this.Objects_Label.Location = new System.Drawing.Point(612, 185);
            this.Objects_Label.Name = "Objects_Label";
            this.Objects_Label.Size = new System.Drawing.Size(43, 13);
            this.Objects_Label.TabIndex = 7;
            this.Objects_Label.Text = "Objects";
            // 
            // Regions_Label
            // 
            this.Regions_Label.AutoSize = true;
            this.Regions_Label.Location = new System.Drawing.Point(612, 308);
            this.Regions_Label.Name = "Regions_Label";
            this.Regions_Label.Size = new System.Drawing.Size(46, 13);
            this.Regions_Label.TabIndex = 9;
            this.Regions_Label.Text = "Regions";
            // 
            // Fields_Label
            // 
            this.Fields_Label.AutoSize = true;
            this.Fields_Label.Location = new System.Drawing.Point(612, 431);
            this.Fields_Label.Name = "Fields_Label";
            this.Fields_Label.Size = new System.Drawing.Size(34, 13);
            this.Fields_Label.TabIndex = 11;
            this.Fields_Label.Text = "Fields";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // Walls_Box
            // 
            this.Walls_Box.FormattingEnabled = true;
            this.Walls_Box.HorizontalScrollbar = true;
            this.Walls_Box.Location = new System.Drawing.Point(615, 78);
            this.Walls_Box.Name = "Walls_Box";
            this.Walls_Box.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Walls_Box.Size = new System.Drawing.Size(159, 95);
            this.Walls_Box.TabIndex = 12;
            this.Walls_Box.SelectedIndexChanged += new System.EventHandler(this.Walls_Box_SelectedIndexChanged);
            // 
            // Objects_Box
            // 
            this.Objects_Box.FormattingEnabled = true;
            this.Objects_Box.HorizontalScrollbar = true;
            this.Objects_Box.Location = new System.Drawing.Point(615, 201);
            this.Objects_Box.Name = "Objects_Box";
            this.Objects_Box.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Objects_Box.Size = new System.Drawing.Size(159, 95);
            this.Objects_Box.TabIndex = 13;
            this.Objects_Box.SelectedIndexChanged += new System.EventHandler(this.Objects_Box_SelectedIndexChanged);
            // 
            // Regions_Box
            // 
            this.Regions_Box.FormattingEnabled = true;
            this.Regions_Box.HorizontalScrollbar = true;
            this.Regions_Box.Location = new System.Drawing.Point(615, 324);
            this.Regions_Box.Name = "Regions_Box";
            this.Regions_Box.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Regions_Box.Size = new System.Drawing.Size(159, 95);
            this.Regions_Box.TabIndex = 14;
            this.Regions_Box.SelectedIndexChanged += new System.EventHandler(this.Regions_Box_SelectedIndexChanged);
            // 
            // Fields_Box
            // 
            this.Fields_Box.FormattingEnabled = true;
            this.Fields_Box.HorizontalScrollbar = true;
            this.Fields_Box.Location = new System.Drawing.Point(615, 447);
            this.Fields_Box.Name = "Fields_Box";
            this.Fields_Box.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple;
            this.Fields_Box.Size = new System.Drawing.Size(159, 95);
            this.Fields_Box.TabIndex = 15;
            this.Fields_Box.SelectedIndexChanged += new System.EventHandler(this.Fields_Box_SelectedIndexChanged);
            // 
            // Ctrl_press
            // 
            this.Ctrl_press.AutoSize = true;
            this.Ctrl_press.Location = new System.Drawing.Point(482, 37);
            this.Ctrl_press.Name = "Ctrl_press";
            this.Ctrl_press.Size = new System.Drawing.Size(111, 13);
            this.Ctrl_press.TabIndex = 16;
            this.Ctrl_press.Text = "Hold Ctrl to move map";
            // 
            // MainGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.Ctrl_press);
            this.Controls.Add(this.Fields_Box);
            this.Controls.Add(this.Regions_Box);
            this.Controls.Add(this.Objects_Box);
            this.Controls.Add(this.Walls_Box);
            this.Controls.Add(this.Fields_Label);
            this.Controls.Add(this.Regions_Label);
            this.Controls.Add(this.Objects_Label);
            this.Controls.Add(this.Walls_Label);
            this.Controls.Add(this.Add);
            this.Controls.Add(this.Delete);
            this.Controls.Add(this.FieldView);
            this.Controls.Add(this.menuStrip1);
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "MainGUI";
            this.Text = "Map Editor";
            this.SizeChanged += new System.EventHandler(this.MainGUI_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainGUI_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainGUI_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Tao.Platform.Windows.SimpleOpenGlControl FieldView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem occupancyGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem metricMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveMetricMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem generatePathMapToolStripMenuItem;
        private System.Windows.Forms.Button Delete;
        private System.Windows.Forms.Button Add;
        private System.Windows.Forms.Label Walls_Label;
        private System.Windows.Forms.Label Objects_Label;
        private System.Windows.Forms.Label Regions_Label;
        private System.Windows.Forms.Label Fields_Label;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem hideOccupancyGridToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearMapToolStripMenuItem1;
        private System.Windows.Forms.ListBox Walls_Box;
        private System.Windows.Forms.ListBox Objects_Box;
        private System.Windows.Forms.ListBox Regions_Box;
        private System.Windows.Forms.ListBox Fields_Box;
        private System.Windows.Forms.Label Ctrl_press;
    }
}

