using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEASL.Core.GeneralStructures;
using DEASL.Core.Mathematics;
using DEASL.Core.Mathematics.Shapes;
using DEASL.Components.Mapping;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace PRM
{
    public class PathMapNode : IComparable<PathMapNode>
    {

        public Tuple<Vector2, string> v1;  //First point and which room it is in
        public Tuple<Vector2, string> v2;  //Second point and which room it is in
        public Dictionary<PathMapNode, double> connections;    //Connection to all the other PathMapNodes that are in the either v1 or v2's room

        //used for dijkstra's search
        public double distance;
        public bool visited;
        public PathMapNode previous;

        /// <summary>
        /// Constructor for a PathMapNode.
        /// </summary>
        /// <param name="v1">Postion of first point</param>
        /// <param name="name1">Room name of which first point is in</param>
        /// <param name="v2">Postion of second point</param>
        /// <param name="name2">Room name of which second point is in</param>
        public PathMapNode(Vector2 v1, string name1, Vector2 v2, string name2)
        {
            this.v1 = new Tuple<Vector2, string>(v1, name1);
            this.v2 = new Tuple<Vector2, string>(v2, name2);
            connections = new Dictionary<PathMapNode, double>();
        }

        public void addConnection(PathMapNode pmNode, int roomNum, Vector2 point)
        {
            if (roomNum == 1)
            {
                double dis = v1.Item1.DistanceTo(point);
                connections.Add(pmNode, dis);
            }

            else if (roomNum == 2)
            {
                double dis = v2.Item1.DistanceTo(point);
                connections.Add(pmNode, dis);
            }

        }


        /// <summary>
        /// Empty constructor for use in path search
        /// </summary>
        public PathMapNode()
        {
            connections = new Dictionary<PathMapNode, double>();
            v1 = new Tuple<Vector2, string>(new Vector2(), "");
            v2 = new Tuple<Vector2, string>(new Vector2(), "");
        }

        /// <summary>
        /// The implementation of the comparable interface. Basically compares doubles while
        /// also sometimes considering strings. 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PathMapNode other)
        {
            if (distance == other.distance && v1.Item2 == other.v1.Item2 &&
                v2.Item2 == other.v2.Item2)
            {
                return 0;
            }
            return (distance > other.distance) ? 1 : -1;
        }
    }

    public class PathMap : IXmlSerializable
    {
        public List<PathMapNode> map;

        /// <summary>
        /// Constructor with no arguments. Used for the xml serializer.
        /// </summary>
        public PathMap()
        {
            map = new List<PathMapNode>();
        }

        /// <summary>
        /// Constructor for PathMap
        /// </summary>
        /// <param name="map">The lsits of nodes in its map</param>
        public PathMap(List<PathMapNode> map)
        {
            this.map = map;
        }

        public List<PathMapNode> getNodes()
        {
            return map;
        }

        /// <summary>
        /// Returns the list string of rooms needed to pass through to get to goal room.
        /// Does not include initial room.
        /// </summary>
        /// <param name="startField">Field robot is in</param>
        /// <param name="endField">Field the robot wants to be in</param>
        /// <returns>List of field path</returns>
        public List<string> path(string startField, string endField)
        {
            return path(startField, new Vector2(), false, endField);
        }

        /// <summary>
        /// Returns the list string of rooms needed to pass through to get to goal room.
        /// Does not include initial room. This one considers the position of the robot. 
        /// </summary>
        /// <param name="startField">Field robot is in</param>
        /// /// <param name="startPoint">Point of robot initially</param>
        /// <param name="endField">Field the robot wants to be in</param>
        /// <returns>List of field path</returns>
        public List<string> path(string startField, Vector2 startPoint, string endField)
        {
            return path(startField, startPoint, true, endField);
        }

        //// <summary>
        /// Returns the list string of rooms needed to pass through to get to goal room.
        /// Does not include initial room. This is the general algorithm and is used by both
        /// of the public path methods. It is a version of Dijkstra's graph search algorithm.
        /// </summary>
        /// <param name="startField">Field robot is in</param>
        /// /// <param name="startPoint">Point of robot initially</param>
        /// /// <param name="usePoint">Whether or not to use the startPoint</param>
        /// <param name="endField">Field the robot wants to be in</param>
        /// <returns>List of field path</returns>
        private List<string> path(string startField, Vector2 startPoint, bool usePoint, string endField)
        {
            SortedSet<PathMapNode> queue = new SortedSet<PathMapNode>();
            PathMapNode current = new PathMapNode();
            List<string> path = new List<string>();
            foreach (PathMapNode m in map)
            {
                if (m.v1.Item2 == startField)
                {
                    double dis = (usePoint) ? startPoint.DistanceTo(m.v1.Item1) : 0;
                    current.connections.Add(m, dis);
                }
                else if (m.v2.Item2 == startField)
                {
                    double dis = (usePoint) ? startPoint.DistanceTo(m.v2.Item1) : 0;
                    current.connections.Add(m, dis);
                }

                m.distance = double.MaxValue;
                m.visited = false;
            }
            bool first = true;
            current.distance = 0;
            current.previous = null;
            queue.Add(current);
            while (queue.Count > 0)
            {
                PathMapNode f = queue.First();
                if (!first)
                {
                    if (f.v1.Item2 == endField || f.v2.Item2 == endField)
                    {
                        string name = endField;
                        while (f.previous != null)
                        {
                            path.Add(name);
                            if (f.v1.Item2 != name)
                            {
                                name = f.v1.Item2;
                            }
                            else
                            {
                                name = f.v2.Item2;
                            }
                            f = f.previous;
                        }
                        path.Reverse();
                        break;
                    }
                }
                queue.Remove(f);
                f.visited = true;
                foreach (KeyValuePair<PathMapNode, double> node in f.connections)
                {
                    double dis = f.distance + node.Value;
                    if (dis < node.Key.distance)
                    {
                        node.Key.distance = dis;

                        node.Key.previous = f;
                        if (queue.Contains(node.Key))
                        {
                            queue.Remove(node.Key);
                            queue.Add(node.Key);
                        }
                        if (!node.Key.visited)
                        {
                            queue.Add(node.Key);
                        }
                    }
                }
                first = false;
            }
            return path;
        }

        #region serialize

        /// <summary>
        /// Creates a new Field from an xml file.
        /// </summary>
        /// <param name="filename">string path to the xml file</param>
        /// <returns>The new Field</returns>
        public static PathMap FromFile(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PathMap));
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            PathMap pm = (PathMap)serializer.Deserialize(fs);
            fs.Close();
            return pm;
        }

        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer xmlString = new XmlSerializer(typeof(string));
            XmlSerializer xmldouble = new XmlSerializer(typeof(double));
            XmlSerializer xmlVector = new XmlSerializer(typeof(Vector2));

            reader.Read();
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("PMR"))      //if name is element
                {
                    reader.ReadStartElement("PMR");
                    reader.ReadStartElement("v1");
                    Vector2 p1 = (Vector2)xmlVector.Deserialize(reader);
                    string n1 = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                    reader.ReadStartElement("v2");
                    Vector2 p2 = (Vector2)xmlVector.Deserialize(reader);
                    string n2 = (string)xmlString.Deserialize(reader);
                    reader.ReadEndElement();
                    PathMapNode p = new PathMapNode(p1, n1, p2, n2);
                    reader.ReadEndElement();
                    map.Add(p);
                }
                if (reader.IsStartElement("connections"))
                {
                    reader.ReadStartElement("connections");
                    Vector2 p1 = (Vector2)xmlVector.Deserialize(reader);
                    Vector2 p2 = (Vector2)xmlVector.Deserialize(reader);
                    foreach (PathMapNode pmn in map)
                    {
                        if (p1 == pmn.v1.Item1 && p2 == pmn.v2.Item1)
                        {
                            while (reader.IsStartElement("link"))
                            {
                                reader.ReadStartElement("link");
                                Vector2 pp1 = (Vector2)xmlVector.Deserialize(reader);
                                Vector2 pp2 = (Vector2)xmlVector.Deserialize(reader);
                                double length = (double)xmldouble.Deserialize(reader);
                                reader.ReadEndElement();
                                foreach (PathMapNode pmnAttach in map)
                                {
                                    if (pp1 == pmnAttach.v1.Item1 && pp2 == pmnAttach.v2.Item1)
                                    {
                                        pmn.connections.Add(pmnAttach, length);
                                        break;
                                    }
                                }
                            }
                            break;
                        }
                    }
                    reader.ReadEndElement();
                }
            }
        }

        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer xmlString = new XmlSerializer(typeof(string));
            XmlSerializer xmldouble = new XmlSerializer(typeof(double));
            XmlSerializer xmlVector = new XmlSerializer(typeof(Vector2));

            foreach (PathMapNode pmn in map)
            {
                writer.WriteStartElement("PMR");
                writer.WriteStartElement("v1");   //First tuple
                xmlVector.Serialize(writer, pmn.v1.Item1);
                xmlString.Serialize(writer, pmn.v1.Item2);
                writer.WriteEndElement();

                writer.WriteStartElement("v2");   //First tuple
                xmlVector.Serialize(writer, pmn.v2.Item1);
                xmlString.Serialize(writer, pmn.v2.Item2);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
            foreach (PathMapNode pmn in map)
            {
                writer.WriteStartElement("connections");
                xmlVector.Serialize(writer, pmn.v1);
                xmlVector.Serialize(writer, pmn.v2);
                foreach (KeyValuePair<PathMapNode, double> con in pmn.connections)
                {
                    writer.WriteStartElement("link");
                    xmlVector.Serialize(writer, con.Key.v1);
                    xmlVector.Serialize(writer, con.Key.v2);
                    xmldouble.Serialize(writer, con.Value);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }


        }
        #endregion
    }
}