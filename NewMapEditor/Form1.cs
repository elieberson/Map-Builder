using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using DEASL.Core.Mathematics;
using DEASL.Core.Mathematics.Shapes;
using DEASL.Core.Rendering;
using DEASL.Components.CommonRenderables;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using DEASL.Core.GeneralStructures;
using PRM;

namespace NewMapEditor
{
    public partial class MainGUI : Form
    {
        //Renderers
        Renderer rend;
        OccupancyGridRenderable ocgridRend;
        PointRenderable firstPointRend = new PointRenderable();
        PathRenderable newPolyRend;
        PolygonRenderable selectedPolyRend;
        PolygonRenderable wallsRend;
        PolygonRenderable objectsRend;
        PolygonRenderable regionsRend;
        PolygonRenderable fieldsRend;

        //Item Holders
        Field mainField;            //Main field that holds all of the items
        Dictionary<string, Polygon> subFields;  //Holds all the polygons created by the fields item
        List<Polygon> selectedPoly; //Selected item polygons
        IRenderTool originalTool;   //Original navigation tool in rend
        ClickTool polyTool;         //ClickTool used for drawing polygons

        //State Holders
        Polygon newPoly;    //Current polygon being drawn
        string newName;     //Current polygon's name
        types drawType;     //Current polygon's type
        bool accRobot;      //Whether or not robot size was considered for last item added
        bool ocgridShow;    //whether or not to show the occupancy grid
        
        //Works with shifting
        bool shift;         //whether or not we are currently shifting

        //Constants
        double robotR = 0.35;
        double wallR = 0.05;

        public MainGUI()
        {
            InitializeComponent();
            rend = new Renderer(FieldView);
            ocgridRend = new OccupancyGridRenderable("Grid SLAM Map", Color.Red);
            firstPointRend = new PointRenderable();
            firstPointRend.Show = false;
            newPolyRend = new PathRenderable();
            wallsRend = new PolygonRenderable(Color.Brown, true);
            objectsRend = new PolygonRenderable(Color.Blue, true);
            regionsRend = new PolygonRenderable(Color.Yellow, 0.5f, true);
            fieldsRend = new PolygonRenderable(Color.Gray);
            selectedPolyRend = new PolygonRenderable(Color.Black);
            selectedPolyRend.LineThickness = 2F;
            rend.AddRenderable(ocgridRend);
            rend.AddRenderable(firstPointRend);
            rend.AddRenderable(newPolyRend);
            rend.AddRenderable(wallsRend);
            rend.AddRenderable(objectsRend);
            rend.AddRenderable(regionsRend);
            rend.AddRenderable(fieldsRend);
            rend.AddRenderable(selectedPolyRend);

            mainField = new Field("main");
            subFields = new Dictionary<string, Polygon>();
            selectedPoly = new List<Polygon>();

            originalTool = rend.Tool;
            polyTool = new ClickTool();
            polyTool.clicked += new EventHandler<ClickedEventArgs>(FieldViewClick);

            newPoly = new Polygon();
            accRobot = false;
            ocgridShow = true;

            Ctrl_press.Hide();
        }

        #region splitting
        /// <summary>
        /// Grabs all the objects, regions, and walls inside the field polygon 
        /// and stores them in a new Field.
        /// </summary>
        /// <param name="field">The name and shape of the area desired for the 
        /// new field.</param>
        /// <returns>A new Field that is a subsection of mainField</returns>
        private Field split(KeyValuePair<string, Polygon> field)
        {
            Field subField = new Field(field.Key);
            subField.shape = field.Value;
            foreach (KeyValuePair<string, Polygon> f in subFields)
            {
                foreach (Vector2 v in f.Value)
                {
                    if (checkInside(v))
                    {
                        Error inside = new Error(errorTypes.SaveInside, f.Key);
                        inside.ShowDialog();
                        inside.Dispose();
                        return null;
                    }
                }
            }
            foreach (KeyValuePair<string, List<Polygon>> w in mainField.walls)
            {
                List<Polygon> subWalls = new List<Polygon>();
                foreach (Polygon p in w.Value)
                {
                    List<Polygon> sub = splitHelp(field.Value, p);
                    if (sub.Count != 0)
                    {
                        subWalls.AddRange(sub);
                    }
                }
                if (subWalls.Count != 0)
                {
                    if (w.Key.Contains(" : "))
                    {
                        int pos = w.Key.IndexOf(" : ");
                        subField.addWall(w.Key.Substring(0, pos) + " : " + field.Key, subWalls);
                    }
                    else
                    {
                        subField.addWall(w.Key + " : " + field.Key, subWalls);
                    }
                }
            }
            foreach (KeyValuePair<string, List<Polygon>> o in mainField.objects)
            {
                List<Polygon> subObjects = new List<Polygon>();
                foreach (Polygon p in o.Value)
                {
                    List<Polygon> sub = splitHelp(field.Value, p);
                    if (sub.Count != 0)
                    {
                        subObjects.AddRange(sub);
                    }
                }
                if (subObjects.Count != 0)
                {
                    if (o.Key.Contains(" : "))
                    {
                        int pos = o.Key.IndexOf(" : ");
                        subField.addObject(o.Key.Substring(0, pos) + " : " + field.Key, subObjects, 0);
                    }
                    else
                    {
                        subField.addObject(o.Key + " : " + field.Key, subObjects, 0);
                    }
                }
            }
            foreach (KeyValuePair<string, List<Polygon>> r in mainField.regions)
            {
                List<Polygon> subRegions = new List<Polygon>();
                foreach (Polygon p in r.Value)
                {
                    List<Polygon> sub = splitHelp(field.Value, p);
                    if (sub.Count != 0)
                    {
                        subRegions.AddRange(sub);
                    }
                }
                if (subRegions.Count != 0)
                {
                    if (r.Key.Contains(" : "))
                    {
                        int pos = r.Key.IndexOf(" : ");
                        subField.addRegion(r.Key.Substring(0, pos) + " : " + field.Key, subRegions);
                    }
                    else
                    {
                        subField.addRegion(r.Key + " : " + field.Key, subRegions);
                    }
                }
            }
            return subField;
        }

        /// <summary>
        /// Does the actuall splitting of each item. split needs to apply
        /// splitHelp to each individual item so easier to have a helper method.
        /// </summary>
        /// <param name="field">Shape of new Field</param>
        /// <param name="item">Specific item it is splitting</param>
        /// <returns>List of Polygons</returns>
        private List<Polygon> splitHelp(Polygon field, Polygon item)
        {
            List<Polygon> sub = new List<Polygon>();
            Polygon itemMini = mainField.CreateThickItem(item, -0.00001);
            for (int i = 0; i < item.Count; i++)
            {
                Vector2[] interV = new Vector2[0];

                bool A = field.IsInside(itemMini[i]);
                DEASL.Core.Mathematics.Shapes.LineSegment current = new DEASL.Core.Mathematics.Shapes.LineSegment();
                DEASL.Core.Mathematics.Shapes.LineSegment currentMini = 
                    new DEASL.Core.Mathematics.Shapes.LineSegment(itemMini[(i + 1) % item.Count], itemMini[i]);
                bool B = field.DoesIntersect(currentMini);
                if (A)
                {
                    addCorrect(sub, new Vector2[] {item[i]}, field);
                }
                bool interDifferent = false;
                if (B)
                {
                    current = new DEASL.Core.Mathematics.Shapes.LineSegment(item[(i + 1) % item.Count], item[i]);
                    field.Intersect(current, out interV);
                }
                if (B && A)
                {
                    foreach (Vector2 v in interV)
                    {
                        interDifferent |= item[i].DistanceTo(v) < 0.0001 || 
                            item[(i + 1) % item.Count].DistanceTo(v) < 0.0001;
                    }
                }
                if (B && ! interDifferent)
                {
                    if (interV.Length > 1 && !current.UnitVector.ApproxEquals(new  
                        DEASL.Core.Mathematics.Shapes.LineSegment(
                        interV[interV.Length - 1],interV[0]).UnitVector, 0.00001))
                    {
                        interV = reverse(interV);
                    }
                    addCorrect(sub, interV, field);
                }
            }
            return sub;
        }

        /// <summary>
        /// Goes through all the vectors that are currently being checked and calles addCorrectHelp
        /// </summary>
        /// <param name="addTo">List of polygons that are collecting the split parts</param>
        /// <param name="addFrom">List of all vectors to add to polygon</param>
        /// <param name="field">Field shape</param>
        private void addCorrect(List<Polygon> addTo, Vector2[] addFrom, Polygon field)
        {
            foreach (Vector2 v in addFrom)
            {
                addCorrectHelp(addTo, v, field);
            }
        }

        /// <summary>
        /// Adds the vectors in the correct order and to the correct polygon in addTo. Called from addCorrect.
        /// </summary>
        /// <param name="addTo">List of polygons that are collecting the split parts</param>
        /// <param name="addFrom">List of all vectors to add to polygon</param>
        /// <param name="field">Field shape</param>
        private void addCorrectHelp(List<Polygon> addTo, Vector2 addFrom, Polygon field)
        {

            bool addNew = true;
            for (int i = 0; i < addTo.Count; i++)
            {
                List<Polygon> tempPL = addTo.Select(p => new Polygon(p.points)).ToList();
                addNew = false;

                tempPL[i].Add(addFrom);
                if (tempPL[i].Count == 2)
                {
                    Vector2 v = tempPL[i][1] - tempPL[i][0];
                    DEASL.Core.Mathematics.Shapes.LineSegment line = new DEASL.Core.Mathematics.Shapes.
                        LineSegment(tempPL[i][1] - 0.00001 * v, tempPL[i][0] + 0.00001 * v);
                    if (field.IsInside(line))
                    {
                        addTo[i].Add(addFrom);
                        break;
                    }
                }
                else
                {
                    tempPL[i] = mainField.CreateThickItem(tempPL[i], -0.00001);
                    bool noIntersect = true;
                    for(int j = 0; j < tempPL[i].Count; j++)
                    {
                        DEASL.Core.Mathematics.Shapes.LineSegment line = new DEASL.Core.Mathematics.Shapes.
                        LineSegment(tempPL[i][j], tempPL[i][(j + 1) % tempPL[i].Count]);
                        if (field.DoesIntersect(line))
                        {
                            noIntersect = false;
                            break;
                        }
                    } 
                    if (noIntersect && field.IsInside(tempPL[i]))
                    {
                        addTo[i].Add(addFrom);
                        break;
                    }
                }
                addNew = true;
            }
            if (addNew)
            {
                addTo.Add(new Polygon(new Vector2[] { addFrom }));
            }
        }

        /// <summary>
        /// Reverses an array list of elements and returns as an array list.
        /// </summary>
        /// <param name="v">List of vectors</param>
        /// <returns>Reverse List of vectors</returns>
        public Vector2[] reverse(Vector2[] v)
        {
            Vector2[] res = new Vector2[v.Length];
            for (int i = 0; i < v.Length; i++)
            {
                res[i] = v[v.Length - i - 1];
            }
            return res;
        }
        #endregion

        /// <summary>
        /// Checks whether a point is inside of another item on the field.
        /// Primary use is to control where users create new subfields.
        /// </summary>
        /// <param name="location">The location of the point</param>
        /// <returns>Boolean of whether or not it is inside another item</returns>
        private bool checkInside(Vector2 location)
        {
        bool isInside = false;
            foreach (List<Polygon> w in mainField.walls.Values)
            {
                foreach (Polygon p in w)
                {
                    isInside |= p.IsInside(location);
                    if (isInside)
                    {
                        break;
                    }
                }
            }
            foreach (List<Polygon> o in mainField.objects.Values)
            {
                foreach (Polygon p in o)
                {
                    isInside |= p.IsInside(location);
                    if (isInside)
                    {
                        break;
                    }
                }
            }
            foreach (List<Polygon> r in mainField.regions.Values)
            {
                foreach (Polygon p in r)
                {
                    isInside |= p.IsInside(location);
                    if (isInside)
                    {
                        break;
                    }
                }
            }
            return isInside;
        }

        /// <summary>
        /// This creates the newPoly list and updates the newPoly renderable. The way 
        /// it works for walls is different than compared to other types. Also calls
        /// check inside to make sure every new point added for abstract wall is not
        /// inside of another shape. This is to prevent errors.
        /// </summary>
        /// <param name="location">The location of the click. Where the next point
        /// of the polygon is located.</param>
        private void AddPolygon()
        {
            if(drawType!= types.walls)
            {
                List<Polygon> comp = new List<Polygon>();
                switch (drawType)
                {
                    case types.objects:     //adding an object
                        if (accRobot)   //if considering robot
                        {
                            mainField.addObject(newName, newPoly, robotR);
                        }
                        else
                        {
                            mainField.addObject(newName, newPoly, 0);
                        }
                        foreach (List<Polygon> p in mainField.objects.Values)
                        {
                            comp.AddRange(p);
                        }
                        Objects_Box.Items.Add(newName);
                        objectsRend.Polygons = comp;
                        break;
                    case types.regions:     //adding a region
                        mainField.addRegion(newName, newPoly);
                        foreach (List<Polygon> p in mainField.regions.Values)
                        {
                            comp.AddRange(p);
                        }
                        regionsRend.Polygons = comp;
                        Regions_Box.Items.Add(newName);
                        break;
                    case types.fields:     //adding a field
                        subFields.Add(newName, newPoly);
                        fieldsRend.Polygons = subFields.Values.ToList<Polygon>();
                        Fields_Box.Items.Add(newName);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets a point on another subField that is closest to input location.
        /// Distance must be less than range to be considered. Snaps to corner by 1.5 
        /// times more than 
        /// </summary>
        /// <param name="location">Closest point from this location</param>
        /// <param name="range">Maximum distance looking for</param>
        /// <returns>CLosest point</returns>
        private Vector2 getNearest(Vector2 location, double range, List<Polygon> subFields)
        {
            Vector2 minVP = location;
            Vector2 minVL = location;
            double minDP = Double.MaxValue;
            double minDL = Double.MaxValue;
            double dis;
            foreach (Polygon p in subFields)
            {
                foreach (Vector2 v in p)
                {
                    dis = location.DistanceTo(v);
                    if (dis < range && dis < minDP)
                    {
                        minDP = dis;
                        minVP = v;
                    }
                }
                DEASL.Core.Mathematics.Shapes.LineSegment l =
                    p.ShortestLineToOther(new Polygon(new Vector2[] { location }));
                dis = l.Length;
                if (dis < range && dis < minDL)
                {
                    minDL = dis;
                    minVL = l.P0;
                }
            }
            return (minDP < minDL * 1.5)? minVP : minVL;
        }

        /// <summary>
        /// Reset the main parameters so another item can be drawn.
        /// </summary>
        private void resetParameters()
        {
            newPoly = new Polygon();
            newPolyRend.UpdatePoints(newPoly.points);
            firstPointRend.Show = false;
            rend.Tool = originalTool;
            Add.Text = "Add";
        }

        #region clicks/resize
        /// <summary>
        /// This occurs when the renderer's tool is set to clickTool. It gets the
        /// location of the mouseclick relative to the rederer.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FieldViewClick(object sender, ClickedEventArgs e)
        {
            Vector2 location = e.Location;
            if (drawType == types.fields)
            {
                location = getNearest(location, 0.3, subFields.Values.ToList());
                if (checkInside(location))
                {
                    Error inside = new Error(errorTypes.PointInside);
                    inside.ShowDialog();
                    inside.Dispose();
                    return;
                }
            }
            bool intersects = false;
            Vector2 v1 = location - newPoly[newPoly.Count - 1];
            DEASL.Core.Mathematics.Shapes.LineSegment line1 =
                new DEASL.Core.Mathematics.Shapes.LineSegment(location, newPoly[newPoly.Count - 1] + 0.00001 * v1);
            for (int i = 1; i < newPoly.Count; i++)
            {
                Vector2 temp;
                DEASL.Core.Mathematics.Shapes.LineSegment ls = new DEASL.Core.Mathematics.
                    Shapes.LineSegment(newPoly[i], newPoly[i - 1]);
                if (ls.Intersect(line1, out temp))
                {
                    intersects = true;
                    break;
                }
            }
            if (intersects)
            {
                Error intersect = new Error(errorTypes.Intersect);
                intersect.ShowDialog();
                intersect.Dispose();
            }
            else
            {
                newPoly.Add(location);
                firstPointRend.Coordinates2D = newPoly.points[0];
                firstPointRend.Show = true;
                newPolyRend.UpdatePoints(newPoly.points);
            }
        }

        /// <summary>
        /// When Add is clicked, it switches to Done. A popup of AddPoly appears asking for
        /// what type of new item is desired. Then the actual drawing is commenced.
        /// If one wishes to stop before finishing a object, region, or field, click Stop and
        /// the shape will be deleted.
        /// If one wishes to stop creating walls, then click Stop and that set of walls will remain.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Add_Click(object sender, EventArgs e)
        {
            if (Add.Text == "Add")  //Button said add so new polygon
            {
                AddPoly popup = new AddPoly(accRobot, mainField.walls.Keys.ToList<string>(),
                    mainField.objects.Keys.ToList<string>(), mainField.regions.Keys.ToList<string>(),
                    subFields.Keys.ToList<string>());
                popup.ShowDialog();
                accRobot = popup.accRobot;
                if (popup.finished)
                {
                    Add.Text = "Stop";
                    rend.Tool = polyTool;
                    newName = popup.name;
                    drawType = popup.drawType;
                    Ctrl_press.Show();
                }
                popup.Dispose();    
            }
            else        //Button said Stop so stop current job.
            {
                if (drawType == types.walls && newPoly.Count > 1)
                {
                    double thickness = accRobot ? robotR : wallR;
                    mainField.addWall(newName, newPoly.points, thickness);
                    List<Polygon> comp = new List<Polygon>();
                    foreach (List<Polygon> p in mainField.walls.Values)
                    {
                        comp.AddRange(p);
                    }
                    wallsRend.Polygons = comp;
                    Walls_Box.Items.Add(newName);
                }
                else if (newPoly.Count > 2)
                {
                        bool intersects = false;
                        Vector2 v2 = newPoly[0] - newPoly[newPoly.Count - 1];
                        DEASL.Core.Mathematics.Shapes.LineSegment line2 = new DEASL.Core.Mathematics.Shapes.
                            LineSegment(newPoly[0] - 0.00001 * v2, newPoly[newPoly.Count - 1] + 0.00001 * v2);

                        for (int i = 1; i < newPoly.Count; i++)
                        {
                            Vector2 temp;
                            DEASL.Core.Mathematics.Shapes.LineSegment ls = new DEASL.Core.Mathematics.
                                Shapes.LineSegment(newPoly[i], newPoly[i - 1]);
                            if (ls.Intersect(line2, out temp))
                            {
                                intersects = true;
                                break;
                            }
                        }
                        if (intersects)
                        {
                            Error intersect = new Error(errorTypes.Intersect);
                            intersect.ShowDialog();
                            intersect.Dispose();
                        }
                        else
                        {
                            AddPolygon();
                        }
                }
                resetParameters();
                Ctrl_press.Hide();
            }
        }

        /// <summary>
        /// When clicked, deletes all the polygons selected from the boxes on the
        /// side from mainField and the polygon renderables dictionaries. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Delete_Click(object sender, EventArgs e)
        {
            for (int i = Walls_Box.Items.Count - 1; i >= 0; i--)    //deletes walls from mainField and box
            {
                if (Walls_Box.GetSelected(i))
                {
                    mainField.walls.Remove((string)Walls_Box.Items[i]);
                    Walls_Box.Items.RemoveAt(i);
                }
            }
            for (int i = Objects_Box.Items.Count - 1; i >= 0; i--)  //deletes objects from mainField and box
            {
                if (Objects_Box.GetSelected(i))
                {
                    mainField.objects.Remove((string)Objects_Box.Items[i]);
                    Objects_Box.Items.RemoveAt(i);
                }
            }
            for (int i = Regions_Box.Items.Count - 1; i >= 0; i--)  //deletes regions from mainField and box
            {
                if (Regions_Box.GetSelected(i))
                {
                    mainField.regions.Remove((string)Regions_Box.Items[i]);
                    Regions_Box.Items.RemoveAt(i);
                }
            }
            for (int i = Fields_Box.Items.Count - 1; i >= 0; i--)   //deletes fields from subFields and box
            {
                if (Fields_Box.GetSelected(i))
                {
                    subFields.Remove((string)Fields_Box.Items[i]);
                    Fields_Box.Items.RemoveAt(i);
                }
            }
            //Remove from renderables
            List<Polygon> comp = new List<Polygon>();
            foreach (List<Polygon> p in mainField.walls.Values)
            {
                comp.AddRange(p);
            }
            wallsRend.Polygons = comp;
            comp = new List<Polygon>();
            foreach (List<Polygon> p in mainField.objects.Values)
            {
                comp.AddRange(p);
            }
            objectsRend.Polygons = comp;
            comp = new List<Polygon>();
            foreach (List<Polygon> p in mainField.regions.Values)
            {
                comp.AddRange(p);
            }
            regionsRend.Polygons = comp;
            fieldsRend.Polygons = subFields.Values.ToList<Polygon>();
        }

        /// <summary>
        /// Clears all items on the map. Used as a refresh.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void clearMapToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            mainField.walls.Clear();
            mainField.objects.Clear();
            mainField.regions.Clear();
            subFields.Clear();
            selectedPoly.Clear();

            ocgridRend.Data = new OccupancyGrid2D(0,0,0,0);
            wallsRend.Polygons = subFields.Values.ToList<Polygon>();
            objectsRend.Polygons = subFields.Values.ToList<Polygon>();
            regionsRend.Polygons = subFields.Values.ToList<Polygon>();
            fieldsRend.Polygons = subFields.Values.ToList<Polygon>();
            selectedPolyRend.Polygons = selectedPoly;

            Walls_Box.Items.Clear();
            Objects_Box.Items.Clear();
            Regions_Box.Items.Clear();
            Fields_Box.Items.Clear();
        }

        /// <summary>
        /// Control viewing the occupancy grid in the background.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hideOccupancyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ocgridShow = !ocgridShow;
            ocgridRend.Show = ocgridShow;
            if (ocgridShow)
            {
                hideOccupancyGridToolStripMenuItem.Text = "Hide Occupancy Grid";
            }
            else
            {
                hideOccupancyGridToolStripMenuItem.Text = "Show Occupancy Grid";
            }
        }

        /// <summary>
        /// Ensures that all objects stay at their correct position when window is resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGUI_SizeChanged(object sender, EventArgs e)
        {
            int w = MainGUI.ActiveForm.Width;
            int h = MainGUI.ActiveForm.Height;
            FieldView.Width = w - 195;
            FieldView.Height = h - 65;
            Add.Location = new Point(w - 185, 27);
            Delete.Location = new Point(w - 101, 27);
            int boxH = (h - 224) / 4;
            Walls_Label.Location = new Point(w - 188, 62);
            Walls_Box.Location = new Point(w - 185, 78);
            Walls_Box.Height = boxH;
            Objects_Label.Location = new Point(w - 188, Walls_Box.Location.Y + boxH + 13);
            Objects_Box.Location = new Point(w - 185, Objects_Label.Location.Y + 16);
            Objects_Box.Height = boxH;
            Regions_Label.Location = new Point(w - 188, Objects_Box.Location.Y + boxH + 13);
            Regions_Box.Location = new Point(w - 185, Regions_Label.Location.Y + 16);
            Regions_Box.Height = boxH;
            Fields_Label.Location = new Point(w - 188, Regions_Box.Location.Y + boxH + 13);
            Fields_Box.Location = new Point(w - 185, Fields_Label.Location.Y + 16);
            Fields_Box.Height = boxH;
            Ctrl_press.Location = new Point(FieldView.Width - 123, 37);
        }

        /// <summary>
        /// Controls the highlighting of items when they have been selected/deselected from
        /// the boxes on the side.
        /// </summary>
        private void SelectChange()
        {
            selectedPoly.Clear();
            Polygon p = new Polygon();
            List<Polygon> pl = new List<Polygon>();
            foreach (string w in Walls_Box.SelectedItems)
            {
                mainField.walls.TryGetValue(w, out pl);
                selectedPoly.AddRange(pl);
            }
            foreach (string o in Objects_Box.SelectedItems)
            {
                mainField.objects.TryGetValue(o, out pl);
                selectedPoly.AddRange(pl);
            }
            foreach (string r in Regions_Box.SelectedItems)
            {
                mainField.regions.TryGetValue(r, out pl);
                selectedPoly.AddRange(pl);
            }
            foreach (string f in Fields_Box.SelectedItems)
            {
                subFields.TryGetValue(f, out p);
                selectedPoly.Add(p);
            }
            selectedPolyRend.Polygons = selectedPoly;
        }

        /// <summary>
        /// When selected items in Walls_Box change. Simply calls
        /// SelectedChange helper to do the work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Walls_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChange();
        }

        /// <summary>
        /// When selected items in Walls_Box change. Simply calls
        /// SelectedChange helper to do the work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Objects_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChange();
        }

        /// <summary>
        /// When selected items in Walls_Box change. Simply calls
        /// SelectedChange helper to do the work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Regions_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChange();
        }

        /// <summary>
        /// When selected items in Walls_Box change. Simply calls
        /// SelectedChange helper to do the work.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fields_Box_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectChange();
        }
        #endregion

        #region Loading/Opening/Saving
        private void FieldView_Load(object sender, EventArgs e)
        {
            ocgridRend.ShowGrid = false;
            rend.OnFormShown();
        }

        /// <summary>
        /// This saves all the metric maps into seperate xml files stored in the 
        /// folder specified by the user. This will call the split function to
        /// seperate the mainField into the smaller parts defined by subfields. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveMetricMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (subFields.Count == 0)
            {
                Error noFields = new Error(errorTypes.NoFields);
                noFields.ShowDialog();
                noFields.Dispose();
                return;
            }
            saveMetricMap();
        }

        /// <summary>
        /// Prompts for the free points in each subField and then saves the field.
        /// This is different from just saving the playField map because it also 
        /// stores the freePoints in the Field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void generatePathMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (subFields.Count == 0)
            {
                Error noFields = new Error(errorTypes.NoFields);
                noFields.ShowDialog();
                noFields.Dispose();
                return;
            }
            List<Field> fields = new List<Field>();
            foreach (KeyValuePair<string, Polygon> p in subFields)
            {
                Field f = split(p);
                if (f == null)
                {
                    return;
                }
                fields.Add(f);
            }
            PathMapGenerator path = new PathMapGenerator(fields);
            path.ShowDialog();
            if (path.complete)
            {
                save(path.fields, path.PRMs, path.pm, true);
            }
            path.Dispose();
        }

        /// <summary>
        /// Saves the metric map. Used by saveMetricMap and generatePathMap.
        /// This is if the splitting has not already occured.
        /// </summary>
        private void saveMetricMap()
        {
            List<Field> saveFiles = new List<Field>();
            foreach (KeyValuePair<string, Polygon> field in subFields)
            {
                Field f = split(field);
                if (f == null)
                {
                    return;
                }
                saveFiles.Add(f);
            }
            save(saveFiles, new List<PRMAlgorithm>(), new PathMap(), false);
        }

        /// <summary>
        /// Saves the metric map, pathMap and PRM. If one only wants to save the Metric Map, saveP is false.
        /// </summary>
        /// <param name="fields">The list of split fields to save</param>
        /// <param name="PRMs">The list of PRMs to save</param>
        /// <param name="PM">The PathMap to save</param>
        /// <param name="saveP">Whether or not to save the PRM and PathMap</param>
        private void save(List<Field> fields, List<PRMAlgorithm> PRMs, PathMap PM, bool saveP)
        {
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                System.Xml.Serialization.XmlSerializer writeField = new System.Xml.Serialization.XmlSerializer(typeof(Field));
                System.Xml.Serialization.XmlSerializer writePRM = new System.Xml.Serialization.XmlSerializer(typeof(PRMAlgorithm));
                System.Xml.Serialization.XmlSerializer writePM = new System.Xml.Serialization.XmlSerializer(typeof(PathMap));
                string filePath = this.saveFileDialog1.FileName;
                System.IO.Directory.CreateDirectory(filePath);
                if (saveP)
                {
                    StreamWriter files = new StreamWriter(filePath + "\\" + "PathMap.xml");
                    writePM.Serialize(files, PM);
                    files.Close();
                }
                for (int i = 0; i < fields.Count; i++)
                {
                    StreamWriter file = new StreamWriter(filePath + "\\" + fields[i].name + ".xml");
                    writeField.Serialize(file, fields[i]);
                    if (saveP)
                    {
                        file = new StreamWriter(filePath + "\\" + fields[i].name + "PRM.xml");
                        writePRM.Serialize(file, PRMs[i]);
                    }
                    file.Close();
                }
                
            }
        }

        /// <summary>
        /// Opens metric map, fills mainField, and renders all Polygons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void metricMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.openFileDialog1.Multiselect = true;
            this.openFileDialog1.Filter = "Xml File | *.xml";
            DialogResult result = this.openFileDialog1.ShowDialog();
            int sameTracker = subFields.Count;
            if (result == DialogResult.OK)
            {
                foreach (String path in openFileDialog1.FileNames)
                {
                    try
                    {
                        Field f = Field.FromFile(path);
                        if (subFields.Keys.Contains<string>(f.name))
                        {
                            Error err = new Error(errorTypes.MultiName);
                            err.ShowDialog();
                            err.Dispose();
                            return;
                        }

                        shift = false;
                        //Shift shifter = new Shift(path, rend);
                        //shifter.ShowDialog();
                        //if (shifter.shift)
                        //{
                        //    shift = true;
                        //}
                        ////shifter.Dispose();
                        if (!shift)
                        {
                            subFields.Add(f.name, f.shape);
                            Fields_Box.Items.Add(f.name);
                            foreach (KeyValuePair<string, List<Polygon>> w in f.walls)
                            {
                                mainField.walls.Add(w.Key, w.Value);
                                Walls_Box.Items.Add(w.Key);
                            }
                            foreach (KeyValuePair<string, List<Polygon>> o in f.objects)
                            {
                                mainField.objects.Add(o.Key, o.Value);
                                Objects_Box.Items.Add(o.Key);
                            }
                            foreach (KeyValuePair<string, List<Polygon>> r in f.regions)
                            {
                                mainField.regions.Add(r.Key, r.Value);
                                Regions_Box.Items.Add(r.Key);
                            }
                            fieldsRend.Polygons = subFields.Values.ToList<Polygon>();
                            List<Polygon> comp = new List<Polygon>();
                            foreach (List<Polygon> p in mainField.walls.Values)
                            {
                                comp.AddRange(p);
                            }
                            wallsRend.Polygons = comp;
                            comp = new List<Polygon>();
                            foreach (List<Polygon> p in mainField.objects.Values)
                            {
                                comp.AddRange(p);
                            }
                            objectsRend.Polygons = comp;
                            comp = new List<Polygon>();
                            foreach (List<Polygon> p in mainField.regions.Values)
                            {
                                comp.AddRange(p);
                            }
                            regionsRend.Polygons = comp;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        Error open = new Error(errorTypes.OpenFail, path);
                        open.ShowDialog();
                        open.Dispose();
                    }
                }
            }
            if (subFields.Count != sameTracker)
            {
                Vector2 centerP = new Vector2(0, 0);
                foreach (Polygon shape in subFields.Values)
                {
                    centerP += shape.Center;
                }
                centerP /= subFields.Count;
                PoseYPR point = new PoseYPR(centerP.X, centerP.Y, rend.CamOrtho.CameraPosition.z, rend.CamOrtho.CameraPosition.yaw,
                    rend.CamOrtho.CameraPosition.pitch, rend.CamOrtho.CameraPosition.roll);
                rend.CamOrtho.CameraPosition = point;
            }
        }

        /// <summary>
        /// Opens a an occupancy grid in the form of a binary serialization.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void occupancyGridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            this.openFileDialog1.Filter = "Binary File | *.bin";
            DialogResult result = this.openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                IFormatter formatter = new BinaryFormatter();
                Stream stream = new FileStream(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                OccupancyGrid2D grid = (OccupancyGrid2D)formatter.Deserialize(stream);
                stream.Close();
                ocgridRend.Data = grid;
                ocgridShow = true;
                ocgridRend.Show = ocgridShow;
                hideOccupancyGridToolStripMenuItem.Text = "Hide Occupancy Grid";
            }
        }
        #endregion

        #region Keypress
        /// <summary>
        /// Change to originalTool if Ctrl key is held.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainGUI_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey && Add.Text != "Add")
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
        private void MainGUI_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey && Add.Text != "Add")
            {
                rend.Tool = polyTool;
                Ctrl_press.Show();
            }
        }
        #endregion
    }
}