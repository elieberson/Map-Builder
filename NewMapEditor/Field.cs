using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using System.Xml.Serialization;

using DEASL.Core.Mathematics.Shapes;
using DEASL.Core.Mathematics;
using System.IO;

namespace NewMapEditor
{
    [Serializable]
    public class Field : IXmlSerializable
    {
        public string name;                         //name of field
        public Polygon shape;                       //shape of field
        public Dictionary<string, List<Polygon>> walls;   //walls of the field.
        public Dictionary<string, List<Polygon>> objects; //physical objects in the field
        public Dictionary<string, List<Polygon>> regions; //regions in the field
        public Vector2 freePoint;           //free point used for PRM
        public bool freePointDefined;     //whether or not freePoint has been defined

        public Field()
        {
            shape = new Polygon();
            walls = new Dictionary<string, List<Polygon>>();
            objects = new Dictionary<string, List<Polygon>>();
            regions = new Dictionary<string, List<Polygon>>();
            freePointDefined = false;
        }

        /// <summary>
        /// Constructor to initialize a field without any items
        /// </summary>
        /// <param name="name">Name of field</param>
        public Field(string name)
        {
            this.name = name;
            this.shape = new Polygon();
            walls = new Dictionary<string, List<Polygon>>();
            objects = new Dictionary<string, List<Polygon>>();
            regions = new Dictionary<string, List<Polygon>>();
            freePointDefined = false;
        }

        /// <summary>
        /// Constructor to initialize a field with all of its items.
        /// Walls are already generated with a thickness.
        /// </summary>
        /// <param name="name">Name of field</param>
        /// <param name="walls">Wall of the field</param>
        /// <param name="objects">Objects inside field</param>
        /// <param name="regions">Regions inside field</param>
        public Field(string name, Polygon shape, Dictionary<string, List<Polygon>> walls,
            Dictionary<string, List<Polygon>> objects, Dictionary<string, List<Polygon>> regions)
        {
            this.shape = shape;
            this.name = name;
            this.walls = walls;
            this.objects = objects;
            this.regions = regions;
            freePointDefined = false;
        }

        /// <summary>
        /// Defines the freePoint attribute that is not always used.
        /// Used for PRM.
        /// </summary>
        /// <param name="point">Free point</param>
        public void DefineFreePoint(Vector2 point)
        {
            freePoint = point;
            freePointDefined = true;
        }

        /// <summary>
        /// Only walls and objects are physical items in a room. This fucntion will
        /// tell you all the areas in a room that are no pass throughable.  Use if 
        /// bloating required. Must also specify the thickness of the walls.
        /// </summary>
        /// <param name="thickness">Thickness of the walls</param>
        /// <returns>List of physical walls and objects</returns>
        public List<Polygon> blocks(double thickness)
        {
            List<Polygon> res = new List<Polygon>();
            foreach (List<Polygon> w in walls.Values)
            {
                foreach (Polygon p in w)
                {
                    res.Add(CreateThickItem(p, thickness));
                }
            }
            foreach (List<Polygon> o in objects.Values)
            {
                foreach (Polygon p in o)
                {
                    res.Add(CreateThickItem(p, thickness));
                }
            }
            return res;
        }

        /// <summary>
        /// Only walls and objects are physical items in a room. This fucntion will
        /// tell you all the areas in a room that are no pass throughable. Use if 
        /// no bloating required. 
        /// </summary>
        /// <returns>List of physical walls and objects</returns>
        public List<Polygon> blocks()
        {
            List<Polygon> res = new List<Polygon>();
            foreach (List<Polygon> w in walls.Values)
            {
                res.AddRange(w);
            }
            foreach (List<Polygon> o in objects.Values)
            {
                res.AddRange(o);
            }
            return res;
        }

        /// <summary>
        /// Triangulates a polygon.
        /// </summary>
        /// <param name="p">Polygon to break down</param>
        /// <returns>List of triangles</returns>
        public List<Polygon> triangulate(Polygon p)
        {
            List<Polygon> tris = new List<Polygon>();
            return triangulateHelper(p, tris);
            //return p.Triangulation();
        }

        /// <summary>
        /// Helper function to triangulate.
        /// </summary>
        /// <param name="poly">Polygon to breakdown</param>
        /// <param name="tris">List of triangles</param>
        /// <returns>List of triangles</returns>
        private List<Polygon> triangulateHelper(Polygon poly, List<Polygon> tris)
        {
            Polygon p = new Polygon(poly);
            if (p.Count <= 3)
            {
                tris.Add(p);
                return tris;
            }
            else
            {
                for (int i = 0; i < p.Count; i++)
                {
                    Vector2 v = p[i+2] - p[i];
                    DEASL.Core.Mathematics.Shapes.LineSegment ls = new DEASL.Core.Mathematics.Shapes.LineSegment(
                        p[i]+0.00001*v, p[i + 2]-0.00001*v);
                    if(!p.DoesIntersect(ls) && p.IsInside(ls))
                    {
                        tris.Add(new Polygon(new Vector2[] {p[i], p[i+1], p[i+2]}));
                        p.RemoveAt(i+1);
                        break;
                    }
                }
                return triangulateHelper(p, tris);
            }
        }

        /// <summary>
        /// Adds another wall element to the walls dictionary. Takes a list of 
        /// vectors that represents walls and creates a inflated version to account
        /// for wall thickness or robot radius.
        /// </summary>
        /// <param name="n">Name of new wall</param>
        /// <param name="w">Vectors representing that wall</param>
        /// <param name="thickness">Thickness of the wall</param>
        public void addWall(string n, List<Vector2> w, double thickness)
        {
            walls.Add(n,CreateThickWalls(w, thickness));
        }

        /// <summary>
        /// Add a list of walls to the field.
        /// </summary>
        /// <param name="n">Name of walls</param>
        /// <param name="w">Set of walls</param>
        public void addWall(string n, List<Polygon> w)
        {
            walls.Add(n, w);
        }

        /// <summary>
        /// Add a new object to the list. Handles bloating and triangualtion.
        /// </summary>
        /// <param name="n">Name of object</param>
        /// <param name="o">Shape of object</param>
        /// <param name="thickness">How much to bloat by.</param>
        public void addObject(string n, Polygon o, double thickness)
        {
            Polygon p = CreateThickItem(o, thickness);
            objects.Add(n, triangulate(p));
        }

        /// <summary>
        /// Add one object that is broken into pieces to the object list.
        /// </summary>
        /// <param name="n">Name of object</param>
        /// <param name="o">Shape of object</param>
        public void addObject(string n, List<Polygon> o, double thickness)
        {
            objects.Add(n, new List<Polygon>());
            foreach (Polygon p in o)
            {
                Polygon bloatP = CreateThickItem(p, thickness);
                objects[n].AddRange(triangulate(bloatP));
            }
        }

        /// <summary>
        /// Adds a region to the regions list. Handles triangulation.
        /// </summary>
        /// <param name="n">Name of region</param>
        /// <param name="r">SHape of region</param>
        public void addRegion(string n, Polygon r)
        {
            regions.Add(n, triangulate(r));
        }

        /// <summary>
        /// Add one region that is broken into pieces to the region list.
        /// </summary>
        /// <param name="n">Name of region</param>
        /// <param name="o">Shape of region</param>
        public void addRegion(string n, List<Polygon> r)
        {
            regions.Add(n, new List<Polygon>());
            foreach (Polygon p in r)
            {
                regions[n].AddRange(triangulate(p));
            }
        }

        /// <summary>
        /// IMPORTANT: This function is currently using (1) not (2). (2) is commented out.
        /// (1) Generates a list of rectangular polygons for each of the vectors in the list w.
        /// (2) Generates thick walls list from the list of walls. This is an outward offset tool. 
        /// </summary>
        /// <returns>List of polygons</returns>
        public List<Polygon> CreateThickWalls(List<Vector2> w, double thickness)
        {
            List<Polygon> thick = new List<Polygon>();
            for (int i = 1; i < w.Count; i++)
            {
                Vector2 paraV = thickness * (w[i] - w[i - 1]).GetNormalizedVector();
                Vector2 perpV = new Vector2(-paraV.Y, paraV.X);
                thick.Add(new Polygon(new Vector2[] { w[i] + paraV + perpV, w[i] + 
                    paraV - perpV, w[i-1] - paraV - perpV, w[i-1] - paraV + perpV}));
            }
            return thick;

            //Vector2[] p = new Vector2[2 * w.Count]; //resulting polygon vectors

            //Vector2 paraV = thickness * (w[1] - w[0]).GetNormalizedVector(); //Gets end points for first vector
            //Vector2 perpV = new Vector2(-paraV.Y, paraV.X);
            //p[0] = w[0] - paraV + perpV;
            //p[2 * w.Count - 1] = w[0] - paraV - perpV;
            //paraV = thickness * (w[w.Count - 1] - w[w.Count - 2]).GetNormalizedVector();    //Gets end points for last vector
            //perpV = new Vector2(-paraV.Y, paraV.X);
            //p[w.Count - 1] = w[w.Count - 1] + paraV + perpV;
            //p[w.Count] = w[w.Count - 1] + paraV - perpV;
            //for (int i = 1; i < w.Count - 1; i++)   //iterates through and creates points
            //{
            //    Vector2 perpV1 = (new Vector2(w[i].Y - w[i + 1].Y, 
            //        w[i + 1].X - w[i].X)).GetNormalizedVector();
            //    Vector2 perpV2 = (new Vector2(w[i - 1].Y - w[i].Y,
            //        w[i].X - w[i - 1].X)).GetNormalizedVector();
            //    Vector2 para = w[i + 1] - w[i];
            //    Vector2 bi = (perpV1 + perpV2).GetNormalizedVector();
            //    double h = thickness/ Math.Sqrt(1 - Math.Pow(bi.Dot(para) / 
            //        (bi.Length * para.Length), 2));
            //    p[i] = w[i] + h * bi;
            //    p[2 * w.Count - 1 - i] = w[i] - h * bi;
            //}
            //return new Polygon(p);
        }

        /// <summary>
        /// Creates an an item that it bloated to consider a certain thickness. 
        /// Primarily used for considering robot thickness. Can also create smaller
        /// items is thickness is <0;
        /// </summary>
        /// <param name="p">Polygon being bloated</param>
        /// <param name="thickness">How to much to bloat by</param>
        /// <returns>Bloated polygon</returns>
        public Polygon CreateThickItem(Polygon p, double thickness)
        {
            Polygon p1 = new Polygon();
            Polygon p2 = new Polygon();
            for (int i = 0; i < p.Count; i++)
            {
                Vector2 perpV1 = (p[i] - p[(i + p.Count - 1) % p.Count]).GetNormalizedVector();
                perpV1 = thickness * (new Vector2(-perpV1.Y, perpV1.X));
                Vector2 perpV2 = (p[(i + 1) % p.Count] - p[i]).GetNormalizedVector();
                perpV2 = thickness * (new Vector2(-perpV2.Y, perpV2.X));
                p1.Add(perpV1 + perpV2 + p[i]);
                p2.Add(-(perpV1 + perpV2) + p[i]);
            }
            if (thickness > 0)
            {
                return (Math.Abs(p1.Area) > Math.Abs(p2.Area)) ? p1 : p2;
            }
            else
            {
                return (Math.Abs(p1.Area) < Math.Abs(p2.Area)) ? p1 : p2;
            }
        }

        /// <summary>
        /// Calculatest the area ratio of physical objects in field 
        /// divided by the total field area. Used for PRM.
        /// </summary>
        /// <returns>Area Ratio</returns>
        public double AreaRatio()
        {
            double pArea = 0;
            foreach (Polygon p in blocks())
            {
                pArea += Math.Abs(p.GetArea());
            }
            return pArea / Math.Abs(shape.GetArea());
        }

        /// <summary>
        /// Gets open area in the field. Total area minus all physical items.
        /// </summary>
        /// <returns>The open area</returns>
        public double AreaDifference()
        {
            double parea = Math.Abs(shape.GetArea());
            foreach (Polygon p in blocks())
            {
                parea -= Math.Abs(p.GetArea());
            }
            return parea;
        }

        #region Serialize

        /// <summary>
        /// Creates a new Field from an xml file.
        /// </summary>
        /// <param name="filename">string path to the xml file</param>
        /// <returns>The new Field</returns>
        public static Field FromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Field));
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            Field f = (Field)serializer.Deserialize(fs);
            fs.Close();
            return f;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer xmlString = new XmlSerializer(typeof(string));
            XmlSerializer xmlPolygon = new XmlSerializer(typeof(Polygon));
            XmlSerializer xmlVector = new XmlSerializer(typeof(Vector2));

            reader.Read();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("name"))      //if name is element
                {
                    reader.ReadStartElement("name");
                    name = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("shape"))
                {
                    reader.ReadStartElement("shape");
                    shape = (Polygon)xmlPolygon.Deserialize(reader);
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("walls"))     //if a wall is the element
                {
                    reader.ReadStartElement("walls");
                    reader.ReadStartElement("key");    //get name of wall
                    string key = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");   //get polygon of wall
                    Polygon val = (Polygon)xmlPolygon.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadEndElement();
                    if (!walls.Keys.Contains<string>(key))
                    {
                        walls.Add(key, new List<Polygon>());    //add new list if list does not already exist
                        
                    }
                    walls[key].Add(val);        //add to the list of walls
                }
                else if (reader.IsStartElement("objects"))     //if a objects is the element
                {
                    reader.ReadStartElement("objects");
                    reader.ReadStartElement("key");    //get name of object
                    string key = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");   //get polygon of object
                    Polygon val = (Polygon)xmlPolygon.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadEndElement();
                    if (!objects.Keys.Contains<string>(key))
                    {
                        objects.Add(key, new List<Polygon>());    //add new list if list does not already exist

                    }
                    objects[key].Add(val); ;    //add to the list of objects
                }
                else if (reader.IsStartElement("regions"))     //if a regions is the element
                {
                    reader.ReadStartElement("regions");
                    reader.ReadStartElement("key");    //get name of regions
                    string key = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("value");   //get polygon of regions
                    Polygon val = (Polygon)xmlPolygon.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadEndElement();
                    if (!regions.Keys.Contains<string>(key))
                    {
                        regions.Add(key, new List<Polygon>());    //add new list if list does not already exist

                    }
                    regions[key].Add(val); ;    //add to the list of objects
                }
                else if (reader.IsStartElement("freePoint"))
                {
                    reader.ReadStartElement("freePoint");
                    freePoint = (Vector2)xmlVector.Deserialize(reader);
                    freePointDefined = true;
                    reader.ReadEndElement();
                }
            }
            reader.ReadEndElement();
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer xmlString = new XmlSerializer(typeof(string));
            XmlSerializer xmlPolygon = new XmlSerializer(typeof(Polygon));
            XmlSerializer xmlVector = new XmlSerializer(typeof(Vector2));

            writer.WriteStartElement("name");   //write name
            xmlString.Serialize(writer, name);
            writer.WriteEndElement();

            writer.WriteStartElement("shape");
            xmlPolygon.Serialize(writer, shape);
            writer.WriteEndElement();

            foreach (KeyValuePair<string, List<Polygon>> w in walls)
            {
                foreach (Polygon p in w.Value)
                {
                    writer.WriteStartElement("walls");        //label type
                    writer.WriteStartElement("key");    //add key
                    xmlString.Serialize(writer, w.Key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");  //add polygon
                    xmlPolygon.Serialize(writer, p);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            foreach (KeyValuePair<string, List<Polygon>> o in objects)
            {
                foreach (Polygon p in o.Value)
                {
                    writer.WriteStartElement("objects");        //label type
                    writer.WriteStartElement("key");    //add key
                    xmlString.Serialize(writer, o.Key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");  //add polygon
                    xmlPolygon.Serialize(writer, p);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            foreach (KeyValuePair<string, List<Polygon>> r in regions)
            {
                foreach (Polygon p in r.Value)
                {
                    writer.WriteStartElement("regions");        //label type
                    writer.WriteStartElement("key");    //add key
                    xmlString.Serialize(writer, r.Key);
                    writer.WriteEndElement();
                    writer.WriteStartElement("value");  //add polygon
                    xmlPolygon.Serialize(writer, p);
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                }
            }
            if (freePointDefined)
            {
                writer.WriteStartElement("freePoint");
                xmlVector.Serialize(writer, freePoint);
                writer.WriteEndElement();
            }        
        }
        #endregion
    }
}
