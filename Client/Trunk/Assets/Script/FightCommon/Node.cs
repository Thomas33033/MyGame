using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using behaviac;

namespace FightCommom
{
    public enum _ListState { Unassigned, Open, Close };

    public class Node 
    {
        public int ID;
        public Vector3 pos;
        public Node[] neighbourNode;
        public float[] neighbourCost;
        public Node parent;

        public bool walkable;
       
        public float tempScoreG = 0;
        public float scoreG;
        public float scoreH;
        public float scoreF;

        public _ListState listState = _ListState.Unassigned;

        public Node() { }

        public Node(NodeData nodeData)
        {
            this.ID = nodeData.ID;
            this.pos = Tools.ToVector3(nodeData.pos);
            this.walkable = nodeData.walkable;
        }
        
        public Node(Vector3 position, int id)
        {
            pos = position;
            ID = id;
            this.walkable = true;
        }

        public void SetNeighbour(List<Node> arrNeighbour, List<float> arrCost)
        {
            neighbourNode = arrNeighbour.ToArray();
            neighbourCost = arrCost.ToArray();
        }

        public void ProcessNeighbour(Node node)
        {
            ProcessNeighbour(node.pos);
        }

        public void ProcessNeighbour(Vector3 pos)
        {
            for (int i = 0; i < neighbourNode.Length; i++)
            {
                if (neighbourNode[i].listState == _ListState.Unassigned)
                {
                    neighbourNode[i].scoreG = scoreG + neighbourCost[i];
                    neighbourNode[i].scoreH = Vector3.Distance(neighbourNode[i].pos, pos);
                    neighbourNode[i].UpdateScoreF();
                    neighbourNode[i].parent = this;
                }
                else if (neighbourNode[i].listState == _ListState.Open)
                {
                    tempScoreG = scoreG + neighbourCost[i];
                    if (neighbourNode[i].scoreG > tempScoreG)
                    {
                        neighbourNode[i].parent = this;
                        neighbourNode[i].scoreG = tempScoreG;
                        neighbourNode[i].UpdateScoreF();
                    }
                }
            }
        }

        void UpdateScoreF()
        {
            scoreF = scoreG + scoreH;
        }


        public float Distance(Node node)
        {
            return Vector3.Distance(this.pos, node.pos);
        }
    }

    public class NodeData {
        public int ID;
        public string pos;
        public bool walkable;
        public NodeData(Node node)
        {
            this.ID = node.ID;
            this.pos = Tools.Vector3ToString(node.pos);
            this.walkable = node.walkable;
        }
    }

    [System.Serializable]
    public class PlaneConnection : MonoBehaviour
    {
        public Transform plane1;
        public Transform plane2;
        public bool isConnected = false;
        public bool isAligned = false;
        public Vector3[] overlapPoint;
    }


}
