using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEASL.Core.GeneralStructures;
using DEASL.Core.Mathematics;
using DEASL.Core.Mathematics.Shapes;
using System.IO;
using System.Xml;
using System.Xml.Serialization;



namespace PRM
{
    // http://dijkstra-csharp.sourceforge.net/


    #region Node
    /// <summary>
    /// A node in the graph that represents a point in the map. keeps track of properties used for dijkstras. 
    /// These properties can be reset in Reset()
    /// </summary>
    public class Node : IXmlSerializable
    {
        public const double INFINITY = -1;
        public Vector2 loc;
        private bool _deadend;
        private bool _visited;
        private static int VertexIDSequencer = 0;
        private int _vertexID;
        public string idString; 
        private double _aggregateCost;
        public int _numConnectedEdges;
        public string lowestNode;
        public string fieldLocation; 


        #region Properties

        public string VertexID
        {
            get
            {
                return idString;
            }
        }

        public double AggregateCost
        {
            get
            {
                return _aggregateCost;
            }
            set
            {
                _aggregateCost = value;
            }
        }

        public Vector2 coord
        {
            get
            {
                return loc;
            }

        }



        public bool Deadend
        {
            get
            {
                return _deadend;
            }

            set
            {
                _deadend = value;
            }
        }

        public bool Visited
        {
            get
            {
                return _visited;
            }
            set
            {
                _visited = value;
            }
        }
        public int numConnectedEdges
        {
            get
            {
                return _numConnectedEdges;
            }
            set
            {
                _numConnectedEdges = value;
            }
        }


        #endregion

        /// <summary>
        /// Constructor for Node 
        /// </summary>
        public Node() {
            _aggregateCost = INFINITY;
            _deadend = false;
            _visited = false;
            lowestNode = ""; 
        }
        /// <summary>
        /// Main Constructor for Node
        /// </summary>
        /// <param name="x"></param> x coordinate
        /// <param name="y"></param> y coordinate
        /// <param name="deadend"></param> if Deadend
        public Node(double x, double y, bool deadend, string name)
        {
            _visited = false;
            loc = new Vector2(x, y);
            _deadend = deadend;
            _aggregateCost = INFINITY;
            _vertexID = ++VertexIDSequencer;
            idString = name + _vertexID.ToString();
            fieldLocation = name; 
            lowestNode = "";
        }


        #region XML
         System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

         void IXmlSerializable.ReadXml(XmlReader reader)
        {
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read(); // read root tag
            if (wasEmpty)
            {
                return;
            }

            XmlSerializer intSerializer = new XmlSerializer(typeof(int));
            XmlSerializer stringSerializer = new XmlSerializer(typeof(string));
            XmlSerializer vector2Serializer = new XmlSerializer(typeof(Vector2));




            while (reader.NodeType != XmlNodeType.EndElement)
            {

                if (reader.IsStartElement("loc"))
                {
                    reader.ReadStartElement("loc");
                    loc = (Vector2)vector2Serializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("idString"))
                {
                    reader.ReadStartElement("idString");
                    idString = (string)stringSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("_numConnectedEdges"))
                {
                    reader.ReadStartElement("_numConnectedEdges");
                    _numConnectedEdges = (int)intSerializer.Deserialize(reader);
                    reader.ReadEndElement();

                }

                else if (reader.IsStartElement("fieldLocation"))
                {
                    reader.ReadStartElement("fieldLocation");
                    fieldLocation = (string)stringSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
 


            }
            reader.ReadEndElement(); 
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));
            XmlSerializer stringSerializer = new XmlSerializer(typeof(string));
            XmlSerializer vector2Serializer = new XmlSerializer(typeof(Vector2));

            writer.WriteStartElement("loc");
            vector2Serializer.Serialize(writer, loc);
            writer.WriteEndElement();

            writer.WriteStartElement("idString");
            stringSerializer.Serialize(writer, idString);
            writer.WriteEndElement();

            writer.WriteStartElement("_numConnectedEdges");
            intSerializer.Serialize(writer, _numConnectedEdges);
            writer.WriteEndElement();

            writer.WriteStartElement("fieldLocation");
            stringSerializer.Serialize(writer, fieldLocation);
            writer.WriteEndElement();

        }
        #endregion 
    }

    #endregion

    #region Priority Queue
    /// <summary>
    /// Priority Queue to Find the shortest path. Node with the lowest cost gets pushed to the front
    /// </summary>
    public class PriorityQueue
    {
        public List<Node> q;

        public PriorityQueue()
        {
            q = new List<Node>();
        }

        public void Push(Node node)
        {


            if (q.Count == 0)
            {
                q.Add(node);
                return;
            }

            // If the Queue doesnt contain the node
            // The queue is made up of pointers so if the node gets changed (aka cost and visited) outside of the queue, it will 
            //also update inside the queue
            if(!q.Contains(node))
            {

                for (int i = 0; i < q.Count; i++)
                {
                    if (q[i].AggregateCost > node.AggregateCost)
                    {
                        q.Insert(i, node);
                        return;
                    }
                }

            q.Add(node);
            return;
            }

            
        }

        public Node Pop()
        {
            Node top = q.ElementAt(0);
            q.RemoveAt(0);
            return top;
        }
    }

    #endregion 

    #region Graph
    /// <summary>
    /// Graph used in PRMAlgorithm
    /// </summary>
    public class Graph : IXmlSerializable
    {
        public Node _sourceNode;
        public List<Node> _listOfNodes;
        //public List<Edge> _listOfEdges;
        public Dictionary<string, List<string>> nodeConnections; //Dictionary of the node and its connecting nodes
        public Dictionary<string, Node> nodeRef; //Node reference dictionary (by vertexID)

        #region Properties

        public int getNodeNum()
        {
            return _listOfNodes.Count;
        }

        public List<Node> AllNodes
        {
            get { return _listOfNodes; }
            set { _listOfNodes = value; }
        }


        // Read-Write properties
        public Node SourceVertex
        {
            get
            {
                return _sourceNode;
            }
            set
            {
                // SourceVertex is only valid if it is found in the graph.
                // Do not make any changes otherwise.
                for (int i = 0; i < _listOfNodes.Count; i++)
                {
                    if (_listOfNodes[i] == value)
                    {
                        _sourceNode = value;
                        break;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// main Constructor for graph. 
        /// </summary>
        public Graph()
        {
           // _listOfEdges = new List<Edge>();
            _listOfNodes = new List<Node>();
            nodeConnections = new Dictionary<string, List<string>>();
            nodeRef = new Dictionary<string, Node>(); 
            

            _sourceNode = null; //_targetNode = null;

            //_totalCost = -1;
            //_optimalTraversal = new List<Node>();
        }

       /// <summary>
       /// Adds a connection to nodeConnections
       /// </summary>
       /// <param name="nodeID"></param> nodeConnections key
       /// <param name="connectionID"></param> value to be added to nodeID's list of connections
        public void AddConnection(string nodeID, string connectionID)
        {
            List<string> connections = nodeConnections[nodeID];
            connections.Add(connectionID); 
            nodeConnections[nodeID] = connections; 
        }

        /// <summary>
        /// Remove a connection from nodeConnections
        /// </summary>
        /// <param name="nodeID"></param>nodeConnections key
        /// <param name="connectionID"></param>value to be removed from nodeID's list of connections
        public void removeConnection(string nodeID, string connectionID)
        {
            List<string> connections = nodeConnections[nodeID];
            connections.Remove(connectionID);
            nodeConnections[nodeID] = connections;

        }

        
        /// <summary>
        /// Get Connections of a given node that have not already been visited
        /// </summary>
        /// <param name="nodeId"></param>given node
        /// <returns></returns> List of unvisited Connections
        public List<string> getConnections(string nodeId)
        {
            List<string> connections = new List<string>();
            foreach (string i in nodeConnections[nodeId])
            {
                if (nodeRef.Keys.Contains(i) && !nodeRef[i].Visited)
                    connections.Add(i);
            }
            return connections; 
        }


        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="node"></param>
        public void AddVertex(Node node)
        {
            _listOfNodes.Add(node);
            nodeConnections.Add(node.VertexID, new List<string>());
            nodeRef.Add(node.VertexID, node); 

            // Reset stats due to a change to the graph.
            this.Reset();
        }

        public Node getNode(string ID)
        {
            return nodeRef[ID]; 
        }

        /// <summary>
        /// Remove node from graph
        /// </summary>
        /// <param name="node"></param>
        public void removeVertex(Node node)
        {
            _listOfNodes.Remove(node);
            nodeConnections.Remove(node.VertexID);
            nodeRef.Remove(node.VertexID); 
            this.Reset(); 
        }



        /// <summary>
        /// Dijkstra Initialization
        /// </summary>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public bool CalculateShortestPath(Node targetNode)
        {
            bool reachable = true;

            if (_sourceNode == null) // || _targetNode == null)
            {
                return false;
            }

            // Algorithm starts here

            // Reset stats
            this.Reset();

            // Set the cost on the source node to 0 and flag it as visited
            _sourceNode.AggregateCost = 0;


            // if the targetnode is not the sourcenode
            // if (_targetNode.AggregateCost == Node.INFINITY) {
            // Start the traversal across the graph
            reachable = PerformCalculationForAllNodes(targetNode);
            //}


            //_totalCost = _targetNode.AggregateCost;



            return reachable;
        }



        /// <summary>
        /// After Dijkstra calculations are complete: retrieve the shortest path ffrom sourceNOde to targetNode
        /// </summary>
        /// <param name="targetNode"></param> target
        /// <returns></returns> shortestpath
        public List<Node> RetrieveShortestPath(Node targetNode)
        {
            List<Node> shortestPath = new List<Node>();

            if (targetNode == null)
            {
                throw new InvalidOperationException("Target node is null.");
            }
            else
            {
                Node currentNode = targetNode;

                shortestPath.Add(currentNode);

                while (currentNode.lowestNode != "")
                {
                    currentNode = nodeRef[currentNode.lowestNode];
                    Console.WriteLine("L{0}", currentNode.VertexID);
                    shortestPath.Add(currentNode);
                }
            }

            // reverse the order of the nodes, because we started from target node first
            shortestPath.Reverse();

            return shortestPath;
        }


        /// <summary>
        /// Resets the graph (to be done before each path calculation)
        /// </summary>
        public void Reset()
        {
            // reset visited flag and reset the aggregate cost on all nodes
            //for (int i = 0; i < _listOfNodes.Count; i++)
            //{
            //    // The current node is now considered visited
            //    //_listOfNodes[i].Visited = false;
            //    //_listOfNodes[i].AggregateCost = Node.INFINITY;
            //    //_listOfNodes[i].EdgeWithLowestCost = null;
            //    //_listOfNodes[i].Deadend = false;


            //}

            List<string> ids = nodeRef.Keys.ToList();
            foreach (string i in ids)
            {
                nodeRef[i].Visited = false;
                nodeRef[i].AggregateCost = Node.INFINITY;
                nodeRef[i].lowestNode = "";
                nodeRef[i].Deadend = false;
            }

        }


        /// <summary>
        /// Perform the Calculations for Dijkstras
        /// </summary>
        /// <param name="targetNode"></param>
        /// <returns></returns> if the Path to the target node is possible
        private bool PerformCalculationForAllNodes(Node targetNode)
        {
            Node currentNode = _sourceNode;
            bool reachable = true;
            // Start by marking the source node as visited
            currentNode.Visited = true;
            int count = 0;
            bool finish = false;
            Node previousBest = currentNode;
            PriorityQueue queue = new PriorityQueue();
            queue.Push(currentNode); 
            do
            {
                count++;

                //Node nextBestNode = null;


                    //"Visit" the node with the lowest cost
                    Node visitedNode = queue.Pop();
                    nodeRef[visitedNode.VertexID].Visited = true;

                    // If the visited node is the target node, then we have found the shortest path
                    if (visitedNode.VertexID == targetNode.VertexID)
                    {
                        finish = true;
                        break;
                    }

                
                   // Console.WriteLine("{0}", visitedNode.VertexID);

                    // find all connected nodes that have not already been visited(popped from the queue)
                    List<string> connectedEdges = getConnections(visitedNode.VertexID); 
                    if (connectedEdges.Count == 0)
                    {
                        visitedNode.Deadend = true;
                    }
                    else
                    {
                        //find the cost of each connected node and update its node that results in the lowest cost

                        for (int j = 0; j < connectedEdges.Count; j++)
                        {

                            string connectedEdgeID = connectedEdges[j]; 
                            Node n = nodeRef[connectedEdgeID];
                            double distance = Math.Sqrt(Math.Pow((nodeRef[connectedEdgeID].coord.X - visitedNode.coord.X), 2) + Math.Pow((nodeRef[connectedEdgeID].coord.Y - visitedNode.coord.Y), 2));

                            if (nodeRef[connectedEdgeID].AggregateCost == Node.INFINITY
                                || (visitedNode.AggregateCost + distance) < nodeRef[connectedEdgeID].AggregateCost)
                            {
                                nodeRef[connectedEdgeID].AggregateCost = visitedNode.AggregateCost + distance;

                                // update the pointer to the edge with the lowest cost in the other node
                                nodeRef[connectedEdgeID].lowestNode = visitedNode.VertexID;
                            }

                            //put connected nodes on the queue
                            queue.Push((nodeRef[connectedEdgeID])); 

                        }


                    }

                    // If the queue empties that means we've visited all possible nodes and the path is not possible
                    if (queue.q.Count == 0)
                    {
                        finish = true;
                        reachable = false;
                    }

            } while (!finish); // Loop until  either queue is empty or visits targetNode
            return reachable;
        }


        #region XML
        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null; 
        }





        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            XmlSerializer nodeSerializer = new XmlSerializer(typeof(Node));
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));
            XmlSerializer stringSerializer = new XmlSerializer(typeof(string));


            bool wasEmpty = reader.IsEmptyElement;
            reader.Read(); // read root tag
            if (wasEmpty)
            {
                return;
            }

            while (reader.NodeType != XmlNodeType.EndElement)
            {

                if (reader.IsStartElement("_listofNodes"))
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("_listofNodes");
                    }
                    else
                    {
                        reader.ReadStartElement("_listofNodes");
                        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                        {

                            reader.ReadStartElement("node");
                            Node n = (Node)nodeSerializer.Deserialize(reader);
                            reader.ReadEndElement();
                            _listOfNodes.Add(n);


                        }
                        reader.ReadEndElement();

                    }
                }

                else if (reader.IsStartElement("nodeRef"))
                {
                    #region read nodeRef
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("nodeRef");
                    }
                    else
                    {
                        reader.ReadStartElement("nodeRef");

                        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                        {

                            reader.ReadStartElement("key"); //<key>
                            string key = (string)stringSerializer.Deserialize(reader); //<string>...</string>
                            reader.ReadEndElement(); //</key>

                            reader.ReadStartElement("value"); //<value>
                            Node node = (Node)nodeSerializer.Deserialize(reader); //<Polygon>...</Polygon>
                            reader.ReadEndElement(); //</value>


                            nodeRef.Add(key, node);

                        }

                        reader.ReadEndElement();
                    }

                    #endregion

                }

                else if (reader.IsStartElement("nodeConnections"))
                {
                    #region read nodeConnections
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("nodeConnections");
                    }
                    else
                    {
                        reader.ReadStartElement("nodeConnections");

                        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                        {

                            reader.ReadStartElement("key"); //<key>
                            string key = (string)stringSerializer.Deserialize(reader); //<string>...</string>
                            reader.ReadEndElement(); //</key>

                            if (reader.IsEmptyElement)
                            {
                                reader.ReadStartElement("value");
                            }
                            else
                            {

                                reader.ReadStartElement("value"); //<value>
                                List<string> connectedN = new List<string>();

                                while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                                {
                                    reader.ReadStartElement("nodeID"); //<key>
                                    string nodei = (string)stringSerializer.Deserialize(reader); 
                                    reader.ReadEndElement(); //</value>

                                    connectedN.Add(nodei);
                                }


                                nodeConnections.Add(key, connectedN);
                                reader.ReadEndElement();
                            }

                        }

                        reader.ReadEndElement();
                    }
                    #endregion
                }
            }
            reader.ReadEndElement(); 

        }



        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XmlSerializer nodeSerializer = new XmlSerializer(typeof(Node));
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));
            XmlSerializer stringSerializer = new XmlSerializer(typeof(string));


            writer.WriteStartElement("_listofNodes"); //All Nodes
            foreach (Node n in _listOfNodes)
            {

                writer.WriteStartElement("node"); //node
                nodeSerializer.Serialize(writer, n);
                writer.WriteEndElement(); //node
            }
            writer.WriteEndElement(); //All Nodes

            writer.WriteStartElement("nodeRef"); //nodeRef
            foreach (string key in nodeRef.Keys)
            {
                Node n = nodeRef[key];

                writer.WriteStartElement("key");//key
                stringSerializer.Serialize(writer, key);
                writer.WriteEndElement(); //</key>

                writer.WriteStartElement("value"); //val
                nodeSerializer.Serialize(writer, n);
                writer.WriteEndElement(); //val

            }
            writer.WriteEndElement();//nodeRef

            writer.WriteStartElement("nodeConnections");//nodeConnections
            foreach (string key in nodeConnections.Keys)
            {

                List<string> connections = nodeConnections[key];

                writer.WriteStartElement("key");//key
                stringSerializer.Serialize(writer, key);
                writer.WriteEndElement(); //</key>

                writer.WriteStartElement("value");//value

                foreach (string i in connections)
                {

                    writer.WriteStartElement("nodeID");//nodeID
                    stringSerializer.Serialize(writer, i);
                    writer.WriteEndElement();//nodeID
                }

                writer.WriteEndElement();//value

            }
            writer.WriteEndElement();//nodeConnections

        }
        #endregion 
    }

    #endregion 
}
