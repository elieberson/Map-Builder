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
    public enum errorTypes {PointInside, SaveInside, MultiName, Intersect, NoFields, OpenFail};

    public partial class Error : Form
    {
        public Error(errorTypes errorType)
        {
            InitializeComponent();
            if (errorType == errorTypes.PointInside)
            {
                HideAll();
                Error_PointInside.Show();
            }
            else if (errorType == errorTypes.SaveInside)
            {
                HideAll();
                Error_SaveInside.Show();
            }
            else if (errorType == errorTypes.MultiName)
            {
                HideAll();
                Error_Multi.Show(); 
            }
            else if (errorType == errorTypes.Intersect)
            {
                HideAll();
                Error_BadPoly.Show();
            }
            else if (errorType == errorTypes.NoFields)
            {
                HideAll();
                Error_NoFields.Show();
            }
        }

        public Error(errorTypes errorType, string file)
        {
            InitializeComponent();
            HideAll();
            
            if (errorType == errorTypes.OpenFail)
            {
                int pos = file.LastIndexOf('\\');
                file = file.Substring(pos + 1, file.Length - pos - 5); 
                if (file.Length > 17)
                {
                    file = file.Substring(0, 15) + " ... "; 
                }
                Error_Open.Text = "Failed to open: " + file + ".xml";
                Error_Open.Location = new Point((358 - Error_Open.Width) / 2, Error_Open.Location.Y);
                Error_Open.Show();
            }
            else if (errorType == errorTypes.SaveInside)
            {
                if (file.Length > 12)
                {
                    file = file.Substring(0, 10) + " ...";
                }
                Error_SaveInside.Text = file + " has a point in another object";
                Error_SaveInside.Location = new Point((358 - Error_SaveInside.Width) / 2, Error_SaveInside.Location.Y);
                Error_SaveInside.Show();
            }
        }

        /// <summary>
        /// Hides all Error Messages
        /// </summary>
        private void HideAll()
        {
            Error_SaveInside.Hide();
            Error_Multi.Hide();
            Error_PointInside.Hide();
            Error_BadPoly.Hide();
            Error_NoFields.Hide();
            Error_Open.Hide();
        }

        private void Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
