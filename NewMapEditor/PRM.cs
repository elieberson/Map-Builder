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
using NewMapEditor;

namespace PRM
{
    /// <summary>
    /// This class is the algorithm to make a Probabilistic Road Map (PRM) and to find the shortest
    /// path between two points. 
    /// </summary>
    public class PRMAlgorithm : IXmlSerializable
    {
        public Graph PRMGraph;
        //public PlayfieldMap PRMmap;
        public Field PRMmap; 
        public Vector2 mapPoint;
        public List<Polygon> obstacles;
        public double maxDistance; 
        public int numPRMpoints;
        public double obstacleRatio;
        private string fieldName; 

        /// <summary>
        /// Constructor for the PRM. Initializes the obstacles
        /// </summary>
        public PRMAlgorithm()
        {
            obstacles = new List<Polygon>();
        }

        /// <summary>
        /// Main Constructor for PRMAlgorthim. Creates PRM
        /// </summary>
        /// <param name="map"></param> Playfield Map of the region
        /// <param name="mp"></param>User defined central point in the free space of map
        /// <param name="radius"></param> Radius of the segway
        /// <param name="pointNum"></param> Number of points to be generated in the PRM
        /// <param name="or"></param> User defined estimated ratio of obstacleArea:OveralArea
        public PRMAlgorithm(Field map, double radius, int pointNum)
        {
            PRMmap = map;
            fieldName = map.name; 
            mapPoint = PRMmap.freePoint;
            
            numPRMpoints = pointNum;
            obstacleRatio = PRMmap.AreaRatio();
            //obstacles = PRMmap.GetWallPolygons();
            obstacles = PRMmap.blocks();
            obstacles = BloatObstacles(obstacles, radius);
            obstacles.Add(PRMmap.shape); 
            maxDistance = calculateMaxDis();
            createPRM();
            obstacles.Remove(PRMmap.shape); 
        }

        //for debugging
        public List<Vector2> getNodeVector2()
        {
            List<Vector2> nodesv2 = new List<Vector2>();
            foreach(Node n in PRMGraph.AllNodes)
            {
                nodesv2.Add(n.coord); 
            }
            return nodesv2;
        }

        #region serialization
        /// <summary>
        /// Serialize PRM to XML
        /// </summary>
        /// <param name="filename"></param> Save path for the PRM XML file
        public void serializePRM(string filename)
        {

            MemoryStream ms = new MemoryStream(10000);
            XmlSerializer serializer = new XmlSerializer(typeof(PRMAlgorithm));
            serializer.Serialize(ms, this);
            FileStream fs = null;
            if (File.Exists(filename))
            {
                fs = new FileStream(filename, FileMode.Truncate, FileAccess.Write);
            }
            else
            {
                fs = new FileStream(filename, FileMode.Create, FileAccess.Write);
            }

            ms.WriteTo(fs);
            fs.Close();
        }
        /// <summary>
        /// Read XML file of a PRM
        /// </summary>
        /// <param name="filename"></param> Path of XML file to read
        /// <returns></returns> PRM read from file

        public static PRMAlgorithm readPRM(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(PRMAlgorithm));
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            PRMAlgorithm returnPRM = (PRMAlgorithm)serializer.Deserialize(fs);
            fs.Close();
            return returnPRM;
        }

        #endregion 


        #region Create Original PRM

        /// <summary>
        /// Given the numPRMpoints and the obstacleRatio, calculate an approriate maximum distance an edge can be. 
        /// </summary>
        /// <returns></returns>maximumDistance
        /// 
        private void createPRM()
        {
            PRMGraph = new Graph();
            createPRMpoints();
            createkEdges();

            if (PRMGraph.AllNodes.Count > numPRMpoints)
            {
                int numberToRemove = PRMGraph.AllNodes.Count - numPRMpoints;
                removeExtraPoints(numberToRemove);
            }
            else if (PRMGraph.AllNodes.Count < numPRMpoints)
            {
                int numToAdd = numPRMpoints - PRMGraph.AllNodes.Count;
                addMorePoints(numToAdd);
            }
        }
        private double calculateMaxDis()
        {
            //double yint = Math.Abs(PRMmap.bottomLeftPoint.Y - PRMmap.topRightPoint.Y);
            //double xint = Math.Abs(PRMmap.bottomLeftPoint.X - PRMmap.topRightPoint.X);
            double area = Math.Abs(PRMmap.shape.Area); 
            //double freeSpace = xint * yint * (1 - obstacleRatio);
            double freeSpace = area * (1 - obstacleRatio); 
            return freeSpace/(double)numPRMpoints*4;
        }

        /// <summary>
        /// Randomly Generate points throughout map. Use collision detection to see if they are within the free space and remove those that are not. 
        /// If the number of points generated in the free space are less than numPRMpoints, generate more points.
        /// If the number of points generated in the free space are more than numPRMpoints, remove points.
        /// </summary>
        private void createPRMpoints()
        {
            //Create Random Points
            Polygon shape = PRMmap.shape;
            List<Vector2> points = PRMmap.shape.points;
            double smallestx = points[0].X;
            double smallesty= points[0].Y; 
             double largestx = points[0].X;
            double largesty= points[0].Y; 

            foreach (Vector2 p in points)
            {
                if (p.X < smallestx)
                    smallestx = p.X;
                if (p.Y < smallesty)
                    smallesty = p.Y;
                if (p.X > largestx)
                    largestx = p.X;
                if (p.Y > largesty)
                    largesty = p.Y; 
            }

            Vector2 botLeftPt = new Vector2(smallestx, smallesty);
            Vector2 topRightPt = new Vector2(largestx, largesty); 
            //Vector2 botLeftPt = PRMmap.bottomLeftPoint;
            //Vector2 topRighPt = PRMmap.topRightPoint;
            double yint = Math.Abs(botLeftPt.Y - topRightPt.Y);
            double xint = Math.Abs(botLeftPt.X - topRightPt.X);
            int nPts = (int) (numPRMpoints/(1-obstacleRatio)*1.5); // create more initial points given the percentage of map is obstacles


            Random random = new Random();
            for (int i = 0; i < nPts; i++)
            {
                PRMGraph.AddVertex(new Node((random.NextDouble() * xint + botLeftPt.X), (random.NextDouble() * yint + botLeftPt.Y), false, fieldName));
            }


            List<Node> badPoints = IsIntersecting(PRMGraph.AllNodes, mapPoint);

            foreach (Node bad in badPoints)
            {
                PRMGraph.removeVertex(bad); 
            }


        }

        /// <summary>
        /// For each point in the PRM, find the neighboring points that are within maximumDistance. Created edges between these points. 
        /// </summary>
        private void createkEdges()
        {
            List<Node> graphNodes = PRMGraph.AllNodes;
            List<int> kNNIndices;
            List<double> kNNDists;
            for (int i = 0; i < PRMGraph.getNodeNum(); i++)
            {

                getDistanceNeighbors(graphNodes, graphNodes[i], maxDistance, out kNNIndices, out kNNDists);
                for (int j = 0; j < kNNIndices.Count; j++)
                {
                    PRMGraph.AddConnection(graphNodes[i].VertexID, graphNodes[kNNIndices[j]].VertexID); 
                    //PRMGraph.AddEdge(new Edge(graphNodes[i], graphNodes[kNNIndices[j]], kNNDists[j]));

                }
            }
        }

        /// <summary>
        /// If the number of points generated in the free space (in createPRM()) are less than numPRMpoints, generate more points.
        /// New points are generated around the points that have the least amount of connected edges. 
        /// </summary>
        /// <param name="numToAdd"></param>
        private void addMorePoints(int numToAdd)
        {
            //Find points with least connected Edges
            List<Node> closeToAdd = new List<Node>(numToAdd); //should all be zeros
            //List<double> connectedEdges = new List<double>(numToAdd);
            List<int> connections = new List<int>(numToAdd); 

            List<Node> graphNodes = PRMGraph.AllNodes;
            bool fill = false;
            for (int i = 0; i < graphNodes.Count; i++)
            {
                Node tempNode = graphNodes[i];

                if (!fill)
                {
                    closeToAdd.Add(tempNode);
                    //connectedEdges.Add(tempNode.numConnectedEdges);
                    connections.Add(PRMGraph.getConnections(tempNode.VertexID).Count); 
                    if (closeToAdd.Count == numToAdd)
                        fill = true;

                }

                else if (tempNode.numConnectedEdges < connections.Max())
                {
                    //int maxIndex = connectedEdges.IndexOf(connectedEdges.Max());
                    //connectedEdges.Remove(connectedEdges.Max());
                    //connectedEdges.Add(tempNode.numConnectedEdges);

                    int maxIndex = connections.IndexOf(connections.Max());
                    connections.Remove(connections.Max());
                    connections.Add(PRMGraph.getConnections(tempNode.VertexID).Count); 

                    closeToAdd.Remove(closeToAdd[maxIndex]);
                    closeToAdd.Add(tempNode);

                }
            }

            List<Vector2> addedPts = new List<Vector2>();

            while (addedPts.Count < numToAdd)
            {
                foreach (Node n in closeToAdd)
                {
                    addedPts.Add(surveyAndAdd(n));
                    if (addedPts.Count == numToAdd)
                        break;
                }
            }


        }

        /// <summary>
        /// Given a point, create a random point within a radius of maxDistance. 
        /// Generate points until one is found that is in the free space. 
        /// </summary>
        /// <param name="centerPt"></param>
        /// <returns></returns>
        private Vector2 surveyAndAdd(Node centerPt)
        {
            Random random = new Random();
            bool added = false;
            double radius = maxDistance; //guarantee that the point will be generated to be an edge of centerPt
            List<Node> checkNode = new List<Node>();
            Vector2 testvec = new Vector2();
            while (!added)
            {
                double theta = random.NextDouble() * 2 * Math.PI;
                double rad = random.NextDouble() * radius / 2;
                testvec = new Vector2((centerPt.coord.X + rad * Math.Cos(theta)), (centerPt.coord.Y + rad * Math.Sin(theta)));
                added = addPoint(testvec.X, testvec.Y);

            }
            return testvec;

        }

        /// <summary>
        /// If the number of points generated in the free space (in createPRM()) are more than numPRMpoints, remove points.
        /// Points with the most number of connected edges are removed. The connected edges are also removed. 
        /// </summary>
        /// <param name="numToRemove"></param>
        private void removeExtraPoints(int numToRemove)
        {
            List<Node> toRemove = new List<Node>(numToRemove); //should all be zeros
            //List<double> connectedEdges = new List<double>(numToRemove);
            List<int> connections = new List<int>(numToRemove); 

            List<Node> graphNodes = PRMGraph.AllNodes;
            bool fill = false;
            for (int i = 0; i < graphNodes.Count; i++)
            {
                Node tempNode = graphNodes[i];

                if (!fill)
                {
                    toRemove.Add(tempNode);
                    //connectedEdges.Add(tempNode.numConnectedEdges);
                    connections.Add(PRMGraph.getConnections(tempNode.VertexID).Count); 
                    if (toRemove.Count == numToRemove)
                        fill = true;

                }

                else if (tempNode.numConnectedEdges > connections.Max())
                {

                    int maxIndex = connections.IndexOf(connections.Min());
                    connections.Remove(connections.Max());
                    connections.Add(PRMGraph.getConnections(tempNode.VertexID).Count);

                    toRemove.Remove(toRemove[maxIndex]);
                    toRemove.Add(tempNode);
                }
            }

            foreach (Node n in toRemove)
                removePoint(n, false, null);


        }

        #endregion 

        #region Helper Methods

        /// <summary>
        /// Bloat obstacles to accomodate the robot. 
        /// </summary>
        /// <param name="unbloated"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private List<Polygon> BloatObstacles(List<Polygon> unbloated, double r)
        {
            List<Polygon> bloated = new List<Polygon>();

            foreach (Polygon p in unbloated)
                bloated.Add(Polygon.ConvexMinkowskiConvolution(p, Polygon.VehiclePolygonWithRadius(r)));

            return bloated;
        }

        /// <summary>
        /// Collision Detection for using all the obstacles in the PRMmap
        /// </summary>
        /// <param name="points"></param> points to check
        /// <param name="freePoint"></param> central point that is in the map's free space. 
        /// <returns></returns> Points that are not in the free space.
        private List<Node> IsIntersecting(List<Node> points, Vector2 freePoint)
        {
            return IsIntersecting(this.obstacles, points, freePoint); 
        }

        /// <summary>
        /// Collision Detection given a new set of obstacles. 
        /// </summary>
        /// <param name="boundaries"></param> new set of obstacles
        /// <param name="points"></param>points to check
        /// <param name="freePoint"></param>central point that is in the map's free space. 
        /// <returns></returns> Points that are not in the free space
        private List<Node> IsIntersecting(List<Polygon> boundaries, List<Node> points, Vector2 freePoint)
        {
            
            List<Node> pointsToMove = new List<Node>();
            List<int> lengths = new List<int>();
            for (int i = 0; i <= points.Count - 1; i++)
            {
                LineSegment line = new LineSegment(points[i].coord, freePoint);

                for (int k = 0; k < boundaries.Count; k++)
                {
                    Vector2[] pts;
                    boundaries[k].Intersect(line, out pts);
                    lengths.Add(pts.Length);
                    if (pts.Length%2==1)
                    {
                        pointsToMove.Add(points[i]);

                        break;
                    }
                }

            }
            return pointsToMove;
        }



        // To remove each point
        //1) find all edges that contain that point and remove
        // 2) remove from nodes. 
        
        /// <summary>
        /// After the PRM is created, remove a point and its associated edges
        /// Also used as a help method in removePoints()
        /// </summary>
        /// <param name="point"></param> point to be removed
        /// <param name="obstacle"></param> if used in removePoints()
        /// <param name="newOb"></param> new obstacle
        private void removePoint(Node point, bool obstacle, List<Polygon> newOb)
        {
            List<Node> graphNodes = PRMGraph.AllNodes; 
           // List<Edge> graphEdges = PRMGraph.AllEdges;
            //List<Edge> edgesToRemove = new List<Edge>(); 
            string pID = point.VertexID; 
 
            //foreach (Edge e in PRMGraph.AllEdges )
            //{
            //    if (e.PointA == point || e.PointB == point)
            //        edgesToRemove.Add(e);
            //    else if (obstacle&&!IsPathClear(newOb, e.PointA.coord, e.PointB.coord))
            //        edgesToRemove.Add(e);

            //}

            //this could take a while. 
            List<KeyValuePair<string, List<string>>> connectionDict = PRMGraph.nodeConnections.ToList(); 
            foreach(KeyValuePair<string, List<string>> kp in connectionDict)
            {
                string origin = kp.Key; 
                Vector2 a = PRMGraph.nodeRef[kp.Key].coord;
                List<string> connects = kp.Value.ToList(); 
                foreach(string i in connects)
                {
                   Vector2 b = PRMGraph.nodeRef[i].coord;
                    if(pID ==i)
                        PRMGraph.removeConnection(origin, i); 
                    else if(obstacle && !IsPathClear(newOb, a, b))
                        PRMGraph.removeConnection(origin, i); 

                        
                }
            }

            //foreach (Edge e in edgesToRemove)
            //    PRMGraph.removeEdge(e);

            PRMGraph.removeVertex(point); 
       
        }




       

        //returns index of neighbors within maxDistance

        /// <summary>
        /// Get the neighbors of a given point within maxDistance
        /// </summary>
        /// <param name="origin"></param> Given point
        /// <param name="kNN"></param> Index within PRMGraph of the neighboring points
        /// <param name="kNNDist"></param> distance of the neighboring points to the origin points
        public void getDistanceNeighbors(List<Node> allNodes, Node origin, double maxD,  out List<int> kNN, out List<double> kNNDist)
        {
            kNN = new List<int>(); //should all be zeros
            kNNDist = new List<double>();
            double originx = origin.coord.X;
            double originy = origin.coord.Y;
            List<Node> graphNodes = allNodes;
            for (int i = 0; i < graphNodes.Count; i++)
            {
                Node tempNode = graphNodes[i];
                double distance = Math.Sqrt(Math.Pow((tempNode.coord.X - originx), 2) + Math.Pow((tempNode.coord.Y - originy), 2));

                bool clearPath = IsPathClear(this.obstacles, origin.coord, graphNodes[i].coord);
                // don't include nodes that are the same as the origin
                //If distance is less than maxDistance, add to list

                if (clearPath && (distance < maxD) && distance != 0)
                {
                    kNN.Add(i);
                    kNNDist.Add(distance); 
                }


            }
        }

        /// <summary>
        /// Get the k nearest neighbors given a point
        /// </summary>
        /// <param name="origin"></param>Given point
        /// <param name="k"></param>How many neighbors to return 
        /// <param name="kNN"></param> Index within PRMGraph of the neighboring points
        /// <param name="kNNDist"></param> distance of the neighboring points to the origin points
        private void getKNearestNeighbors(Node origin, int k, out List<int> kNN, out List<double> kNNDist)
        {
            kNN = new List<int>(k); //should all be zeros
            kNNDist = new List<double>(k);
            double originx = origin.coord.X;
            double originy = origin.coord.Y;
            List<Node> graphNodes = PRMGraph.AllNodes;
            double[] distances = new double[graphNodes.Count];
            bool fill = false; 
            for (int i = 0; i < graphNodes.Count; i++)
            {
                Node tempNode = graphNodes[i];
                double distance = Math.Sqrt(Math.Pow((tempNode.coord.X - originx), 2) + Math.Pow((tempNode.coord.Y - originy), 2));
                distances[i] = distance;
                //automatically fill first k
                bool clearPath = IsPathClear(obstacles,origin.coord, graphNodes[i].coord);
                if (!fill && clearPath)
                {
                    kNN.Add(i);
                    kNNDist.Add(distance);
                    
                }

                // don't include nodes that are the same as the origin
                // find farthest element and replace with new distance if closer
                //hopefully this saves memory/time because avoiding sorting an array because we only want k elements of that array
                else if (clearPath && distance < kNNDist.Max() && distance != 0)
                {
                    
                    int maxIndex = kNNDist.IndexOf(kNNDist.Max());
                    kNN[maxIndex] = i;
                    kNNDist[maxIndex] = distance;
                }

                if (kNN.Count == k)
                    fill = true; 


            }
        }


        /// <summary>
        /// Collision Detection from one point to another
        /// </summary>
        /// <param name="obs"></param> obstacles to avoid
        /// <param name="start"></param> start point
        /// <param name="end"></param>end point
        /// <returns></returns> If the path is clear
        private bool IsPathClear(List<Polygon> obs, Vector2 start, Vector2 end)
        {
            
            Vector2[] temp;
            LineSegment line = new LineSegment(start, end);

            foreach (Polygon p in obs)
            {
                if (p.Intersect(line, out temp))
                    return false;
            }

            return true;
        }

        #endregion

        #region Public Methods
       /// <summary>
       /// After PRM is created, add an addition point to the PRM
       /// </summary>
       /// <param name="x"></param> x coordinate of new point
       /// <param name="y"></param>y coordinate of new point
       /// <returns></returns> If adding the point was successful
        public bool addPoint(double x, double y)
        {
            bool added = false;
            Node newNode = new Node(x, y, false, fieldName);
            List<Node> tempCheck = new List<Node>();
            tempCheck.Add(newNode);
            List<Node> tempList = new List<Node>();
            tempList = IsIntersecting(tempCheck, mapPoint);

            if (tempList.Count == 0)
            {
                PRMGraph.AddVertex(newNode); 
                List<Node> graphNodes = PRMGraph.AllNodes;
                List<int> kNNIndices;
                List<double> kNNDists;
                getDistanceNeighbors(graphNodes, newNode,maxDistance, out kNNIndices, out kNNDists);

                for (int j = 0; j < kNNIndices.Count; j++)
                {
                    //PRMGraph.AddEdge(new Edge(newNode, graphNodes[kNNIndices[j]], kNNDists[j]));
                   // PRMGraph.AddEdge(new Edge(graphNodes[kNNIndices[j]], newNode, kNNDists[j]));
                    PRMGraph.AddConnection(newNode.VertexID, graphNodes[kNNIndices[j]].VertexID);
                    PRMGraph.AddConnection(graphNodes[kNNIndices[j]].VertexID, newNode.VertexID); 
                }

                //PRMGraph.AddVertex(newNode);
                added = true;
            }

            return added;

        }



        /// <summary>
        /// Given a new obstacle in the map, remove the conflicting points and edges. 
        /// </summary>
        /// <param name="newObstacle"></param> new Obstacle
        public void removePoints(Polygon newObstacle)
        {
            obstacles.Add(newObstacle);
            List<Polygon> newOb = new List<Polygon>();
            newOb.Add(newObstacle);
            List<Node> ptsToRemove = new List<Node>();
            ptsToRemove = IsIntersecting(newOb, PRMGraph.AllNodes, mapPoint);
            foreach (Node n in ptsToRemove)
                removePoint(n, true, newOb);

        }

        /// <summary>
        /// Graph Search used to find the shortest path from one point to another
        /// </summary>
        /// <param name="startPt"></param> start point
        /// <param name="goalPt"></param> end point
        /// <param name="reachable"></param> If the path is obtainable
        /// <returns></returns> List of points that create the shortest path
        public List<Vector2> DijkstraSearch(Vector2 startPt, Vector2 goalPt, out bool reachable)
        {
            //PRMGraph.Reset(); 
            List<Vector2> solutionPoints = new List<Vector2>();
            reachable = true; 
            Node startNode; 
            Node goalNode; 
        
            //Find closest Node on graph of start and goal 
            List<int> kNN = new List<int>(1); 
            List<double> kNND = new List<double>(1);
            getKNearestNeighbors(new Node(startPt.X, startPt.Y, false,PRMmap.name), 1, out kNN, out kNND);
            if (kNN.Count == 0)
            {
                reachable = false; 
                return solutionPoints;
            }
            else 
                startNode = PRMGraph.AllNodes[kNN[0]];

            PRMGraph.SourceVertex = startNode;
            kNN.Clear();
            kNND.Clear();
            getKNearestNeighbors(new Node(goalPt.X, goalPt.Y, false, PRMmap.name), 1, out kNN, out kNND);
            if (kNN.Count == 0)
            {
                reachable = false;
                return solutionPoints;
            }
            else
                goalNode = PRMGraph.AllNodes[kNN[0]];
            

            reachable =  PRMGraph.CalculateShortestPath(goalNode);
            List<Node> allNodes = PRMGraph.nodeRef.Values.ToList(); 

            if (reachable)
            {

                goalNode = allNodes[kNN[0]];//reget the goal node after calculations have been made

                //Calculate the shortest path for each node

                List<Node> solutionNodes = PRMGraph.RetrieveShortestPath(goalNode);

                //Add nodes to a resulting list of Vector2s
                solutionPoints.Add(startPt);


                foreach (Node currentNode in solutionNodes)
                {
                    solutionPoints.Add(currentNode.coord);
                }

                solutionPoints.Add(goalPt);
            }


            return solutionPoints; 
        }

        public List<Node> DijkstraSearchNode(Vector2 startPt, Vector2 goalPt, out bool reachable)
        {
            //PRMGraph.Reset(); 
            List<Node> solutionPoints = new List<Node>();
            reachable = true;
            Node startNode;
            Node goalNode;

            //Find closest Node on graph of start and goal 
            List<int> kNN = new List<int>(1);
            List<double> kNND = new List<double>(1);
            getKNearestNeighbors(new Node(startPt.X, startPt.Y, false, fieldName), 1, out kNN, out kNND);
            if (kNN.Count == 0)
            {
                reachable = false;
                return solutionPoints;
            }
            else
                startNode = PRMGraph.AllNodes[kNN[0]];

            PRMGraph.SourceVertex = startNode;
            kNN.Clear();
            kNND.Clear();
            getKNearestNeighbors(new Node(goalPt.X, goalPt.Y, false, fieldName), 1, out kNN, out kNND);
            if (kNN.Count == 0)
            {
                reachable = false;
                return solutionPoints;
            }
            else
                goalNode = PRMGraph.AllNodes[kNN[0]];


            reachable = PRMGraph.CalculateShortestPath(goalNode);
            List<Node> allNodes = PRMGraph.nodeRef.Values.ToList();

            if (reachable)
            {

                goalNode = allNodes[kNN[0]];//reget the goal node after calculations have been made

                //Calculate the shortest path for each node

                List<Node> solutionNodes = PRMGraph.RetrieveShortestPath(goalNode);

                //Add nodes to a resulting list of Vector2s
                solutionPoints.Add(new Node(startPt.X, startPt.Y, false, PRMmap.name));


                foreach (Node currentNode in solutionNodes)
                {
                    solutionPoints.Add(currentNode);
                }

                solutionPoints.Add(new Node(goalPt.X, goalPt.Y, false, fieldName));
            }


            return solutionPoints;
        }

        public void recreatePRM()
        {
            createPRM();
        }

        public static Tuple<Node, Node> mergePRM(PRMAlgorithm PRM1, PRMAlgorithm PRM2)
        {
            PRMAlgorithm newPRM = new PRMAlgorithm(); 
            newPRM.mapPoint = PRM1.mapPoint; 
            newPRM.numPRMpoints = PRM1.numPRMpoints + PRM2.numPRMpoints;
            newPRM.obstacles = new List<Polygon>(); 
            newPRM.maxDistance = PRM1.maxDistance;
            newPRM.obstacleRatio = 0;
            List<Polygon> polygons = PRM2.obstacles; 
            for( int p = 0; p<polygons.Count; p++)
            {
               newPRM.obstacles.Add(polygons[p]); 
            }

            polygons = PRM1.obstacles;
            for (int p = 0; p < polygons.Count; p++)
            {
                newPRM.obstacles.Add(polygons[p]);
            }
            newPRM.PRMGraph = new Graph();
            List<Node> graphNodes1 = PRM1.PRMGraph.AllNodes;
            List<Node> graphNodes2 = PRM2.PRMGraph.AllNodes;

            //add all nodes to dictionary!
            foreach (Node n in graphNodes1)
            {
                newPRM.PRMGraph._listOfNodes.Add(n); 
                newPRM.PRMGraph.nodeRef.Add(n.VertexID, n);
                newPRM.PRMGraph.nodeConnections.Add(n.VertexID, PRM1.PRMGraph.nodeConnections[n.VertexID]);
            }

            foreach (Node n in graphNodes2)
            {
                newPRM.PRMGraph._listOfNodes.Add(n); 
                newPRM.PRMGraph.nodeRef.Add(n.VertexID, n);
                newPRM.PRMGraph.nodeConnections.Add(n.VertexID, PRM2.PRMGraph.nodeConnections[n.VertexID]);
            }
            
            
            List<int> kNNIndices;
            List<double> kNNDists;
            Dictionary<double, Tuple<Node, Node>> mergedConnections = new Dictionary<double,Tuple<Node,Node>>(); 

            //For each of the nodes in the first PRM, check against the nodes in the second PRM
            for (int i = 0; i < graphNodes1.Count; i++)
            {

                newPRM.getDistanceNeighbors(graphNodes2, graphNodes1[i], PRM1.maxDistance,  out kNNIndices, out kNNDists);
                for (int j = 0; j < kNNIndices.Count; j++)
                {
                    mergedConnections.Add(kNNDists[j], new Tuple<Node, Node>(graphNodes1[i], graphNodes2[kNNIndices[j]]));

                }
            }

            if (mergedConnections.Count ==0)
                return null; 

            //return the smallest connecting nodes between the two
            double minDistance = mergedConnections.Keys.Min(); 

            

            return mergedConnections[minDistance]; 

        }


        #endregion 

        #region XML

        System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema()
        {
            return null; 
        }

         void IXmlSerializable.ReadXml(XmlReader reader)
        {
            XmlSerializer graphSerializer = new XmlSerializer(typeof(Graph));
            XmlSerializer doubleSerializer = new XmlSerializer(typeof(double));
            XmlSerializer mapSerializer = new XmlSerializer(typeof(Field));
            XmlSerializer vector2Serializer = new XmlSerializer(typeof(Vector2));
            XmlSerializer polygonSerializer = new XmlSerializer(typeof(Polygon));
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read(); // read root tag
            if (wasEmpty)
            {
                return;
            }


            while (reader.NodeType != XmlNodeType.EndElement)
            {
                if (reader.IsStartElement("PRMGraph"))
                {
                    reader.ReadStartElement("PRMGraph");
                    PRMGraph = (Graph)graphSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }

                else if (reader.IsStartElement("numPRMpoints"))
                {
                    reader.ReadStartElement("numPRMpoints");
                    numPRMpoints = (int)intSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }

                else if (reader.IsStartElement("PRMmap"))
                {
                    reader.ReadStartElement("PRMmap");
                    PRMmap = (Field)mapSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }


                else if (reader.IsStartElement("mapPoint"))
                {
                    reader.ReadStartElement("mapPoint");
                    mapPoint = (Vector2)vector2Serializer.Deserialize(reader);
                    reader.ReadEndElement();
                }

                else if (reader.IsStartElement("obstacles"))
                {
                    if (reader.IsEmptyElement)
                    {
                        reader.ReadStartElement("obstacles");
                    }
                    else
                    {
                        reader.ReadStartElement("obstacles");
                        while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
                        {

                            reader.ReadStartElement("obstacle");
                            Polygon p = (Polygon)polygonSerializer.Deserialize(reader);
                            reader.ReadEndElement();
                            obstacles.Add(p);

                        }
                        reader.ReadEndElement();
                    }
                }
                else if (reader.IsStartElement("maxDistance"))
                {
                    reader.ReadStartElement("maxDistance");
                    maxDistance = (double)doubleSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
                else if (reader.IsStartElement("obstacleRatio"))
                {
                    reader.ReadStartElement("obstacleRatio");
                    obstacleRatio = (double)doubleSerializer.Deserialize(reader);
                    reader.ReadEndElement();
                }
            }
            reader.ReadEndElement();
 
           

        }


         void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            XmlSerializer graphSerializer = new XmlSerializer(typeof(Graph));
            XmlSerializer doubleSerializer = new XmlSerializer(typeof(double));
            XmlSerializer mapSerializer = new XmlSerializer(typeof(Field));
            XmlSerializer vector2Serializer = new XmlSerializer(typeof(Vector2));
            XmlSerializer polygonSerializer = new XmlSerializer(typeof(Polygon));
            XmlSerializer intSerializer = new XmlSerializer(typeof(int));

            writer.WriteStartElement("PRMGraph");
            graphSerializer.Serialize(writer, PRMGraph);
            writer.WriteEndElement();

            writer.WriteStartElement("obstacleRatio");
            doubleSerializer.Serialize(writer, obstacleRatio);
            writer.WriteEndElement();

            writer.WriteStartElement("numPRMpoints");
            intSerializer.Serialize(writer, numPRMpoints);
            writer.WriteEndElement();


            writer.WriteStartElement("PRMmap");
            mapSerializer.Serialize(writer, PRMmap);
            writer.WriteEndElement();

            writer.WriteStartElement("mapPoint");
            vector2Serializer.Serialize(writer, mapPoint);
            writer.WriteEndElement();

            writer.WriteStartElement("obstacles");
            foreach (Polygon p in obstacles)
            {

                writer.WriteStartElement("obstacle");
                polygonSerializer.Serialize(writer, p);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();

            writer.WriteStartElement("maxDistance");
            doubleSerializer.Serialize(writer, maxDistance);
            writer.WriteEndElement();
 
        }

        #endregion 
    }
}
