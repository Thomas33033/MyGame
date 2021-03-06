﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace FightCommom
{
    public enum _ListState { Unassigned, Open, Close };

    public class Node 
    {
        public int Id;
        public Vector3 pos;
        public IntVector2 gridPos;
        public Vector3 worldPos;
        public Node[] neighbourNode;
        public float[] neighbourCost;
        public Node parent;

        private bool _walkable;

        private int weight;

        public ENodeType state = ENodeType.Empty;

        public bool walkable {
            get { return _walkable; }
            set {_walkable = value; }
        }

        public float tempScoreG = 0;
        public float scoreG;
        public float scoreH;
        public float scoreF;

        public _ListState listState = _ListState.Unassigned;

        public Node() { }

        public Node(NodeData nodeData)
        {
            this.Id = nodeData.ID;
            this.pos = Tools.ToVector3(nodeData.pos);
            this._walkable = nodeData.walkable;
        }
        
        public Node(Vector3 position, int id)
        {
            pos = position;
            this.gridPos = new IntVector2((int)pos.x,(int)pos.z);
            Id = id;
            this._walkable = true;
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

        public void SetWeight(int i)
        {
            weight = i;
        }

        public int GetWeight()
        {
            return weight;
        }

        public float Distance(Node node)
        {
            float distance = Vector3.Distance(this.pos, node.pos);
            return distance;
        }

        public Vector3 GetWorldPosition()
        {
            return worldPos;
        }


        public void SetNodeState(ENodeType state)
        {
            this.state = state;
        }

        public Vector2 getFlowFieldVector()
        {
            return Vector2.zero;
        }
    }

    public class NodeData {
        public int ID;
        public string pos;
        public bool walkable;
        public NodeData(Node node)
        {
            this.ID = node.Id;
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
