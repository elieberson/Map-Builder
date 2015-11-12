using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NewMapEditor
{
    //type enumerator
    public enum types { walls = 0, objects = 1, regions = 2, fields = 3, none = -1}

    //Form to add Polygons
    public partial class AddPoly : Form
    {
        public string name;
        public types drawType;
        public bool finished;
        public bool accRobot;

        List<string>[] items;

        /// <summary>
        /// Constructor for AddPoly.
        /// </summary>
        /// <param name="accRobot">Whether or not previous AddPoly had checked this</param>
        /// <param name="field">mainField from GUI. Need this to check for reduntant names</param>
        /// <param name="subfields">List of names of subfields from GUI. Need this to check for reduntant names</param>
        public AddPoly(bool accRobot, List<string> w, List<string> o, List<string> r, List<string> f)
        {
            InitializeComponent();
            drawType = types.none;
            finished = false;
            this.accRobot = accRobot;
            AccRobot.Checked = accRobot;

            items = new List<string>[] { w, o, r, f};

            AccRobot.Hide();
            ErrorName.Hide();
            ErrorType.Hide();
            ErrorName2.Hide();
        }

        private void Wall_Click(object sender, EventArgs e)
        {
            drawType = types.walls;
            AccRobot.Show();
        }

        private void Object_Click(object sender, EventArgs e)
        {
            drawType = types.objects;
            AccRobot.Show();
        }

        private void Region_Click(object sender, EventArgs e)
        {
            drawType = types.regions;
            AccRobot.Hide();
        }

        private void Field_Click(object sender, EventArgs e)
        {
            drawType = types.fields;
            AccRobot.Hide();
        }

        private void AccRobot_Click(object sender, EventArgs e)
        {
            accRobot = !accRobot;
        }

        private void OK_Click(object sender, EventArgs e)
        {
            if (drawType == types.none)
            {
                ErrorName.Hide();
                ErrorName2.Hide();
                ErrorType.Show();
            }
            else if (Name_Box.Text == "")
            {
                ErrorType.Hide();
                ErrorName2.Hide();
                ErrorName.Show();
            }
            else if (Name_Box.Text.Contains("\\") || Name_Box.Text.Contains("/") || Name_Box.Text.Contains(":") || Name_Box.Text.Contains("*") ||
                Name_Box.Text.Contains("?") || Name_Box.Text.Contains("\"") || Name_Box.Text.Contains("<") || Name_Box.Text.Contains(">") ||
                Name_Box.Text.Contains("|"))
            {
                ErrorType.Hide();
                ErrorName.Hide();
                ErrorName2.Show();
            }
            else if(items[(int)drawType].Contains<string>(Name_Box.Text))
            {
                ErrorType.Hide();
                ErrorName.Show();
            }
            else
            {
                name = Name_Box.Text;
                finished = true;
                this.Close();
            }
        }
    }
}
