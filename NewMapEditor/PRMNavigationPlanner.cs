using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DEASL.Core.GeneralStructures;
using DEASL.Core.Mathematics;
using DEASL.Core.Mathematics.Shapes;
using DEASL.Components.Mapping;
using DEASL.Core.Functional.IPathPlanner;

namespace PRM
{
    public class PRMNavigationPlanner
    {
        private Dictionary<string, PRMAlgorithm> allPRMs; 
        private string currentField; 
        private List<Vector2> currentPath; 
        private bool moving; 
        private Dictionary<Vector2, Node> pathDictionary;
        private Vector2 currentPosition;
        private PathMap pathMap;
        private List<PathMapNode> pathMapNodes; 


        // Constructor to create the path map
        public PRMNavigationPlanner(List<PRMAlgorithm> prms, string cf)
        {

            allPRMs = new Dictionary<string, PRMAlgorithm>();
            currentPath = new List<Vector2>(); 
            pathDictionary = new Dictionary<Vector2,Node>();
            currentField = cf; 

            //Add all the fields and their prms. 
            foreach (PRMAlgorithm p in prms)
            {
                string name = p.PRMmap.name;
                allPRMs.Add(name, p);
            }

            pathMapNodes = new List<PathMapNode>();
            List<string> visitedRooms = new List<string>(); 

            //find connection pair for each room pair. 
            foreach (string f1 in allPRMs.Keys)
            {
                PRMAlgorithm prm1 = allPRMs[f1];
                foreach (string f2 in allPRMs.Keys)
                {
                    if (!visitedRooms.Contains(f2) && f1 != f2)
                    {
                        PRMAlgorithm prm2 = allPRMs[f2];
                        Tuple<Node, Node> connection = PRMAlgorithm.mergePRM(prm1, prm2);
                        if (connection != null)
                        {
                            PathMapNode pmn = new PathMapNode(connection.Item1.coord, connection.Item1.fieldLocation, connection.Item2.coord, connection.Item2.fieldLocation);
                            pathMapNodes.Add(pmn);
                        }
                    }


                    
                }
                
                visitedRooms.Add(f1); 

            }

            //check list of conenctions
            foreach (PathMapNode pnode1 in pathMapNodes)
            {
                foreach(PathMapNode pnode2 in pathMapNodes)
                {
                    if (pnode1 != pnode2)
                    {
                        if (pnode1.v1.Item2 == pnode2.v1.Item2)
                        {
                            pnode1.addConnection(pnode2, 1, pnode2.v1.Item1);
                        }
                        else if (pnode1.v2.Item2 == pnode2.v1.Item2)
                        {
                            pnode1.addConnection(pnode2, 2, pnode2.v1.Item1);

                        }
                        else if (pnode1.v1.Item2 == pnode2.v2.Item2)
                        {
                            pnode1.addConnection(pnode2, 1, pnode2.v2.Item1); ;
                        }
                        else if (pnode1.v2.Item2 == pnode2.v2.Item2)
                        {
                            pnode1.addConnection(pnode2, 2, pnode2.v2.Item1);

                        }
                    }
                }

            }
            pathMap = new PathMap(pathMapNodes); 
        }

        //Get the created Path Map
        public PathMap getPathMap()
        {
            return pathMap;
        }

        //Constructor for when path map is already made
        public PRMNavigationPlanner(List<PRMAlgorithm> prms, PathMap pm, string cf)
        {

            allPRMs = new Dictionary<string, PRMAlgorithm>(); 
            pathDictionary = new Dictionary<Vector2, Node>();
            pathMap = pm;
            pathMapNodes = pm.getNodes(); 
            currentField = cf; 


            //Add all the fields and their prms. 
            foreach (PRMAlgorithm p in prms)
            {
                string name = p.PRMmap.name;
                allPRMs.Add(name, p);
            }
        }

        // Everytime a Waypoint is achieved
        public void pointAcheived()
        {
            Node node = pathDictionary[currentPath[0]];
            currentPath.RemoveAt(0);
            if (node.fieldLocation != null)
            {
                currentField = node.fieldLocation;
            }
        }

        //update every time pose is updated
        public void UpdatePosition(Vector2 position)
        {
            currentPosition = position;
        }

        private PathMapNode getConnection(string start, string end)
        {
            foreach (PathMapNode pmn in pathMapNodes)
            {
                if ((pmn.v1.Item2 == start) && (pmn.v2.Item2 == end))
                    return pmn;
                else if ((pmn.v2.Item2 == start) && (pmn.v1.Item2 == end))
                    return pmn;
            }

            return null;
        }

        //Go to a field
        public List<Vector2> planField(string goalField)
        {
            List<string> fieldOrder = pathMap.path(currentField, goalField);//will use the PathMap field
            fieldOrder.Reverse();
            fieldOrder.Add(currentField);
            fieldOrder.Reverse(); 
            List<Node> nodePath = new List<Node>();
            currentPath = new List<Vector2>();
            pathDictionary.Clear(); 
            
            //For the field order, go to the exit point and entry point for the next room. 
            for (int i = 1; i<fieldOrder.Count; i++)
            {
                 Vector2 fieldExit;//shoudl be point in the pathnode thing
                Vector2 nextFieldEnter;//Path node thing
                PathMapNode roomConnection = getConnection(fieldOrder[i - 1], fieldOrder[i]);
                if (roomConnection.v1.Item2 == fieldOrder[i - 1])
                {
                    fieldExit = roomConnection.v1.Item1;
                    nextFieldEnter = roomConnection.v2.Item1;
                }
                else
                {
                    fieldExit = roomConnection.v2.Item1;
                    nextFieldEnter = roomConnection.v1.Item1;
                }
               
                //find path to get to the exit point for the next room
                List<Node> fieldPath = planPoint(fieldExit, fieldOrder[i-1]);
                foreach (Node n in fieldPath)
                {
                    nodePath.Add(n);
                }
                //Do not need PRM to go from room to room. 
                if(i+1==fieldOrder.Count)
                    nodePath.Add(new Node(nextFieldEnter.X, nextFieldEnter.Y,false,fieldOrder[i])); 
            }

            foreach (Node n in nodePath)
            {
                if (!pathDictionary.Keys.Contains(n.coord))
                {
                    pathDictionary.Add(n.coord, n);
                    currentPath.Add(n.coord);
                }
            }
            return currentPath; 
        }

        //plan a path to goalPoint. goalPoint must be within current field
        public List<Vector2> planPoint(Vector2 goalPoint)
        {
            List<Node> nodePath = planPoint(goalPoint, currentField); 
            currentPath = new List<Vector2>();
            pathDictionary.Clear(); 
            foreach (Node n in nodePath)
            {
                pathDictionary.Add(n.coord, n);
                currentPath.Add(n.coord); 

            }
            return currentPath ; 

        }


        private List<Node> planPoint(Vector2 goalPoint, string pointField)
        {
            List<Node> nodePath = new List<Node>();
            PRMAlgorithm prm = allPRMs[pointField];
            bool reachable; 
            nodePath = prm.DijkstraSearchNode(currentPosition, goalPoint, out reachable);
            if (!reachable)
                return null; 
            return nodePath; 
        }



    }
}
