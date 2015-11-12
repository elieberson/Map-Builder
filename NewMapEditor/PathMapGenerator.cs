using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DEASL.Core.Rendering;
using DEASL.Components.CommonRenderables;
using DEASL.Core.GeneralStructures;
using DEASL.Core.Mathematics;
using DEASL.Core.Mathematics.Shapes;
using System.Xml.Serialization;
using System.IO;
using PRM;

namespace NewMapEditor
{
    public partial class PathMapGenerator : Form
    {
        //Renderers
        Renderer rend;
        PolygonRenderable wallsRend;
        PolygonRenderable objectsRend;
        PolygonRenderable regionsRend;
        PolygonRenderable fieldsRend;
        PointRenderable pointRend;
        List<PathRenderable> lines;

        //Value holders
        Field f;
        int count;
        Vector2 point;

        //Clicktools
        ClickTool polyTool;
        IRenderTool originalTool;
        
        //return
        public bool complete;
        public List<Field> fields;
        public List<PRMAlgorithm> PRMs;
        public PathMap pm;
       

        /// <summary>
        /// Constructor for the path map generator GUI. Takes the subfield list which is
        /// created by the split function in the main form.
        /// </summary>
        /// <param name="fields">List of subfields</param>
        public PathMapGenerator(List<Field> fields)
        {
            InitializeComponent();

            rend = new Renderer(FieldView);
            wallsRend = new PolygonRenderable(Color.Brown, true);
            objectsRend = new PolygonRenderable(Color.Blue, true);
            regionsRend = new PolygonRenderable(Color.Yellow, 0.5f, true);
            fieldsRend = new PolygonRenderable(Color.Gray);
            pointRend = new PointRenderable();
            pointRend.Show = false;
            lines = new List<PathRenderable>();
            rend.AddRenderable(wallsRend);
            rend.AddRenderable(objectsRend);
            rend.AddRenderable(regionsRend);
            rend.AddRenderable(fieldsRend);
            rend.AddRenderable(pointRend);

            originalTool = rend.Tool;
            polyTool = new ClickTool();
            polyTool.clicked += new EventHandler<ClickedEventArgs>(FieldViewClick);
            rend.Tool = polyTool;
            complete = false;
            PRMs = new List<PRMAlgorithm>();
 
            rend.OnFormShown();

            this.fields = fields;
            f = fields[0];
            count = 1;
            LoadNext();
        }

        /// <summary>
        /// Loads the next field into view. Centers it and waits for click.
        /// Also readjusts text at top.
        /// </summary>
        /// <param name="f">Field to display</param>
        /// <param name="n">Current subfields number</param>
        /// <param name="total">Total number of subFields</param>
        private void LoadNext()
        {
            reset();
            Number_Label.Text = "Define central free point in " + f.name + ": " + count + "/" + fields.Count;
            List<Polygon> shape = new List<Polygon>();
            shape.Add(f.shape);
            fieldsRend.Polygons = shape;
            List<Polygon> comp = new List<Polygon>();
            foreach (List<Polygon> p in f.walls.Values)
            {
                comp.AddRange(p);
            }
            wallsRend.Polygons = comp;
            comp = new List<Polygon>();
            foreach (List<Polygon> p in f.objects.Values)
            {
                comp.AddRange(p);
            }
            objectsRend.Polygons = comp;
            comp = new List<Polygon>();
            foreach (List<Polygon> p in f.regions.Values)
            {
                comp.AddRange(p);
            }
            regionsRend.Polygons = comp;
            PoseYPR point = new PoseYPR(f.shape.Center.X, f.shape.Center.Y, rend.CamOrtho.CameraPosition.z, 
                rend.CamOrtho.CameraPosition.yaw, rend.CamOrtho.CameraPosition.pitch, rend.CamOrtho.CameraPosition.roll);
            rend.CamOrtho.CameraPosition = point;
        }

        /// <summary>
        /// This occurs when the renderer's tool is set to clickTool. It gets the
        /// location of the mouseclick relative to the rederer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldViewClick(object sender, ClickedEventArgs e)
        {
            if (!complete)
            {
                bool outside = !f.shape.IsInside(e.Location);
                if (outside)
                {
                    reset();
                    Error_OutsideField.Show();
                }
                bool inside = false;
                foreach (Polygon p in f.blocks())
                {
                    if (p.IsInside(e.Location))
                    {
                        inside = true;
                        break;
                    }
                }
                if (!outside && inside)
                {
                    reset();
                    Error_InsideItem.Show();
                }
                if (!outside && !inside)
                {
                    reset();
                    point = e.Location;
                    pointRend.Coordinates2D = e.Location;
                    pointRend.Show = true;
                    OK.Enabled = true;
                }
            }
            else
            {
                bool outside = true;
                bool inside = false;
                foreach (Field s in fields)
                {
                    if (s.shape.IsInside(e.Location))
                    {
                        outside = false;
                        f = s;
                    }
                    foreach (Polygon p in s.blocks())
                    {
                        if (p.IsInside(e.Location))
                        {
                            inside = true;
                            break;
                        }
                    }
                }
                if (outside)
                {
                    reset();
                    OK.Enabled = true;
                    Error_OutsideField.Show();
                }
                else if (inside)
                {
                    reset();
                    OK.Enabled = true;
                    Error_InsideItem.Show();
                }
                else
                {
                    PRMs[fields.IndexOf(f)].addPoint(e.Location.X, e.Location.Y);
                    PointRenderable p = new PointRenderable(e.Location);
                    rend.AddRenderable(p);
                    pm = new PRMNavigationPlanner(PRMs, "").getPathMap();
                    foreach (PathRenderable pr in lines)
                    {
                        rend.RemoveRenderable(pr);
                    }
                    foreach (PathMapNode pmn in pm.map)
                    {
                        PathRenderable line = new PathRenderable(new List<Vector2>() { pmn.v1.Item1, pmn.v2.Item1 });
                        line.DisplayColor = Color.Blue;
                        lines.Add(line);
                        rend.AddRenderable(line);
                    }
                }
            }
        }
 
        /// <summary>
        /// Resets the Gui for next field
        /// </summary>
        private void reset()
        {
            Error_InsideItem.Hide();
            Error_OutsideField.Hide();
            OK.Enabled = false;
            pointRend.Show = false;
        }

        /// <summary>
        /// Change to originalTool if Ctrl key is held.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathMapGenerator_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                rend.Tool = originalTool;
                Ctrl_press.Hide();
            }
        }

        /// <summary>
        /// Change back to polyTool if Ctrl Key is let go.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathMapGenerator_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                rend.Tool = polyTool;
                Ctrl_press.Show();
            }
        }

        /// <summary>
        /// Click to confirm position of the free point. If more fields, call to
        /// load next field. If no more, render the path map. Then close. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, EventArgs e)
        {
            if (complete)
            {
                this.Hide();
                return;
            }
            f.DefineFreePoint(point);
            PRMs.Add(new PRMAlgorithm(f, 0.35, (int)(f.AreaDifference() / 0.2)));
            if (++count <= fields.Count)
            {
                f = fields[count - 1];
                LoadNext();
            }
            else              //if all points found render all lines   
            {
                reset();
                Number_Label.Text = "Path Map and PRM";
                OK.Text = "Save";
                Vector2 center = new Vector2(0, 0);
                List<Polygon> fieldsComp = new List<Polygon>();
                List<Polygon> wallsComp = new List<Polygon>();
                List<Polygon> objectsComp = new List<Polygon>();
                List<Polygon> regionsComp = new List<Polygon>();
                foreach (Field ff in fields)
                {
                    fieldsComp.Add(ff.shape);
                    TextRenderable name = new TextRenderable(ff.name, new PointF((float)ff.shape.Center.X, (float)ff.shape.Center.Y), Color.Black);
                    rend.AddRenderable(name);
                    center += ff.shape.Center;
                    foreach (List<Polygon> p in ff.walls.Values)
                    {
                        wallsComp.AddRange(p);
                    }
                    foreach (List<Polygon> p in ff.objects.Values)
                    {
                        objectsComp.AddRange(p);
                    }
                    foreach (List<Polygon> p in ff.regions.Values)
                    {
                        regionsComp.AddRange(p);
                    }
                }
                fieldsRend.Polygons = fieldsComp;
                wallsRend.Polygons = wallsComp;
                objectsRend.Polygons = objectsComp;
                regionsRend.Polygons = regionsComp;
                center /= fields.Count;
                rend.CamOrtho.CameraPosition = new PoseYPR(center.X, center.Y, rend.CamOrtho.CameraPosition.z, rend.CamOrtho.CameraPosition.yaw,
                    rend.CamOrtho.CameraPosition.pitch, rend.CamOrtho.CameraPosition.roll);
                OK.Enabled = true;
                complete = true;

                pm = new PRMNavigationPlanner(PRMs, "").getPathMap();

                foreach (PRMAlgorithm prm in PRMs)
                {
                    foreach (Vector2 v in prm.getNodeVector2())
                    {
                        PointRenderable pointr = new PointRenderable(v);
                        rend.AddRenderable(pointr);
                    }
                }
                foreach (PathMapNode pmn in pm.map)
                {
                    PathRenderable line = new PathRenderable(new List<Vector2>() { pmn.v1.Item1, pmn.v2.Item1 });
                    line.DisplayColor = Color.Blue;
                    lines.Add(line);
                    rend.AddRenderable(line);
                }
            }
        }

        /// <summary>
        /// Makes sure the elements on the page are aligned correctly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PathMapGenerator_SizeChanged(object sender, EventArgs e)
        {
            int w = this.Width;
            int h = this.Height;
            FieldView.Width = w - 16;
            FieldView.Height = h - 79;
            Ctrl_press.Location = new Point(FieldView.Width - 123, 51);
            OK.Location = new Point(w - 103, 9);
        }
    }
}
