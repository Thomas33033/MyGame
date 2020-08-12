using FightCommom;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fight
{
    /// <summary>
    /// 战场，保存场景格子信息
    /// 负责：角色寻路路劲，目标搜索
    /// </summary>
    public class BattleField
    {
        public int id;

        public string teamId;

        public int row;
        public int column;


        public List<Role> listRoles;

        public List<FightObject> listObject;


        public List<FightReport> listReport;

        public BattleComposite composite;

        public List<FightAttackData> listAttacks;

        public Node[] nodeGraph;

        public Dictionary<int, Node> dicNodeGraph;

        public bool isEnd;

        public BattleField(int id, string teamId, BattleFieldData battleFieldData )
        {
            this.id = id;
            this.teamId = teamId;
            listRoles = new List<Role>();
            listReport = new List<FightReport>();
            listObject = new List<FightObject>();
            listAttacks = new List<FightAttackData>();
            dicNodeGraph = new Dictionary<int, Node>();

            this.nodeGraph = battleFieldData.nodeGraph;
            this.row = battleFieldData.row;
            this.column = battleFieldData.column;

            for (int i = 0; i < this.nodeGraph.Length; i++) {
                dicNodeGraph.Add(nodeGraph[i].Id, nodeGraph[i]);
            }
        }

        public void AddFightObject(FightObject fightObject)
        {
            fightObject.SetBattleField(this);
            listObject.Add(fightObject);
        }

        //public MapGrid GetRandomHex()
        //{
        //    return listHex[RandomTools.Range(0, listHex.Count)];
        //}


        internal void AddAttackData(FightAttackData fightAttackData)
        {
            listAttacks.Add(fightAttackData);
        }

        //internal List<Role> GetAroundRoles(MapGrid hex, int distance)
        //{
        //    List<Role> list = new List<Role>();
        //    for (int i = 0; i < listRoles.Count; i++)
        //    {
        //        if (listRoles[i].position.Subtract(hex).Length() <= distance)
        //        {
        //            list.Add(listRoles[i]);
        //        }
        //    }
        //    return list;
        //}

        internal List<Role> GetAllRoles()
        {
            return new List<Role>(listRoles);
        }

        public bool CanBuild(int position)
        {
            return dicNodeGraph[position].walkable;
        }

        internal void AddRole(Role v, int position)
        {
            if (dicNodeGraph[position].walkable == true)
            {
                v.node = dicNodeGraph[position];
                v.SetBattleField(this);

                int nodeId;
                for (int i = 0; v.costNodes != null && i < v.costNodes.Length; i++)
                {
                    nodeId = v.costNodes[i];
                    dicNodeGraph[nodeId].walkable = false;
                }

                listRoles.Add(v);
                v.Init();
            }
            else
            {
                Debug.LogError("该节点阻塞，不能创建角色");
            }
        }

        public void GetReport(ref List<FightReport> list)
        {
            for (int i = 0; i < listRoles.Count; i++)
            {
                listRoles[i].reportComp.GetReport(ref listReport);
                if (listRoles[i].state == BattleState.Die)
                {
                    listRoles.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < listObject.Count; i++)
            {
                if (listObject[i].state == BattleState.Die)
                {
                    listObject.RemoveAt(i);
                    i--;
                }
            }

            list.AddRange(this.listReport);
            this.listReport.Clear();
        }

        public List<Role> GetEnemy(int myTeamId)
        {
            List<Role> list = new List<Role>();
            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].teamId != myTeamId)
                {
                    list.Add(listRoles[i]);
                }
            }
            return list;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="o">起点</param>
        /// <param name="targetType">1敌人 2队友 3自己 4己方</param>
        /// <param name="targetNum"></param>
        /// <param name="rangeType">0 1圆形 2直线 3自身周围 4扇形</param>
        /// <param name="range">远</param>
        /// <param name="width">宽度</param>
        /// <returns></returns>
        public List<Role> GetEnemy(Role attacker, Node node, ESearchTargetType targetType, int rangeType, int range, int width = 0)
        {
            List<Role> list = ScanForTarget(attacker, targetType, range);
            if (rangeType == 0)
                return list;

            if (rangeType == 3)
                node = attacker.node;


            if (list.Count > 0 && rangeType == 2 && range > 0)
            {
                //MapGrid p = o;
                //MapGrid sub = o.Subtract(attacker.position);
                //while (true && sub.Length() > 0)
                //{
                //    if (dicMapData.ContainsKey(p) == false)
                //        break;

                //    p = p.Add(sub);
                //}

                //List<MapGrid> listPosition = FractionalHex.HexLinedraw(attacker.position, p);
                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (listPosition.IndexOf(list[i].position) == -1)
                //    {
                //        list.RemoveAt(i);
                //        i--;
                //    }
                //}
            }
            else if (list.Count > 0 && rangeType == 4 && range > 0)
            {
                //MapGrid d = o.Subtract(attacker.position);
                //int[] tempArr = new int[] { d.q, d.r, d.s };
                //int[] arr = new int[] { 0, 0, 0 };
                //bool flag = false;

                //for (int i = 0; i < tempArr.Length; i++)
                //{
                //    if (flag == false && Mathf.Abs(tempArr[i]) == Mathf.Min(Mathf.Abs(tempArr[0]), Mathf.Abs(tempArr[1]), Mathf.Abs(tempArr[2])))
                //    {
                //        arr[i] = 0;
                //        flag = true;
                //    }
                //    else if (tempArr[i] > 0)
                //    {
                //        arr[i] = 1;
                //    }
                //    else
                //    {
                //        arr[i] = -1;
                //    }
                //}

                //d = new MapGrid(arr[0], arr[1], arr[2]);

                //int index = MapGrid.directions.IndexOf(d);

                //List<MapGrid> listPosition = new List<MapGrid>();
                //MapGrid temp = attacker.position;
                //for (int i = 0; i < range; i++)
                //{
                //    listPosition.Add(temp.Add(MapGrid.directions[index]));
                //    listPosition.Add(temp.Add(MapGrid.directions[(index + 1) % MapGrid.directions.Count]));
                //    listPosition.Add(temp.Add(MapGrid.directions[(index + MapGrid.directions.Count - 1) % MapGrid.directions.Count]));
                //    temp = temp.Add(MapGrid.directions[index]);
                //}

                //for (int i = 0; i < list.Count; i++)
                //{
                //    if (listPosition.IndexOf(list[i].position) == -1)
                //    {
                //        list.RemoveAt(i);
                //        i--;
                //    }
                //}
            }

            //if (list.Count > 0)
            //{
            //    list.Sort(ListTargetSortHandler);
            //}

            return list;
        }

        /// <param name="targetType">1敌人 2队友 3自己 4己方</param>
        public List<Role> ScanForTarget(Role attacker, ESearchTargetType targetType, int range)
        {
            List<Role> list = new List<Role>();

            if (targetType == ESearchTargetType.Self)
            {
                list.Add(attacker);
                return list;
            }

            for (int i = 0; i < listRoles.Count; i++)
            {
      
                if (targetType == ESearchTargetType.Enemy)
                {
                    if (listRoles[i].StatusCheck(RoleStatus.Unselected))
                    {
                        continue;
                    }
                    else if (attacker.teamId == listRoles[i].teamId)
                    {
                        continue;
                    }
                }
                else if (targetType == ESearchTargetType.Teammate)
                {
                    if (attacker.teamId != listRoles[i].teamId)
                    {
                        continue;
                    }
                    else if (attacker == listRoles[i])
                    {
                        continue;
                    }
                }
                else if (targetType == ESearchTargetType.Self)
                {
                    if (attacker != listRoles[i])
                    {
                        continue;
                    }
                }
                else if (targetType == ESearchTargetType.OtherTeam)
                {
                    if (attacker.teamId != listRoles[i].teamId)
                    {
                        continue;
                    }
                }

                if (range != 0 && listRoles[i].node.Distance(attacker.node) > range)
                {
                    continue;
                }

                list.Add(listRoles[i]);
            }

            return list;
        }

        public List<Role> GetRole(RoleType roleType)
        {
            List<Role> list = new List<Role>();
            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].type == roleType)
                {
                    list.Add(list[i]);
                }
            }
            return list;
        }

        public List<Role> GetRoles(int teamId)
        {
            List<Role> list = new List<Role>();
            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].teamId == teamId)
                {
                    list.Add(listRoles[i]);
                }
            }
            return list;
        }

        public Role GetRole(int id)
        {
            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].id == id)
                {
                    return listRoles[i];
                }
            }
            return null;
        }

        public void Update(float nowTime)
        {
            for (int i = 0; i < listAttacks.Count; i++)
            {
                if (isEnd)
                    break;

                FightAttackData fightAttackData = listAttacks[i];
                if (fightAttackData.time <= nowTime)
                {
                    fightAttackData.Execute();
                    listAttacks.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < listObject.Count; i++)
            {
                if (isEnd)
                    break;
                if (listObject[i].state != BattleState.Die)
                    listObject[i].Update(nowTime);
            }

            for (int i = 0; i < listRoles.Count; i++)
            {
                if (isEnd)
                    break;
                if (listRoles[i].state != BattleState.Die)
                    listRoles[i].Update(nowTime);
            }
        }

        public void RoleMove(Role fightRole, Node grid)
        {
            fightRole.node.walkable = true;
            
            fightRole.node = grid;

            fightRole.node.walkable = false;
        }

        public bool CheckMove(Node grid)
        {
            return grid.walkable;
        }

        public List<Node> GetMoveNode(Node start, Node end, bool checkWeapon)
        {
            return PathFinder.GetPath(start, end, this.nodeGraph);
        }

        private List<Node> templist = new List<Node>();


        internal int GetGridRange(int size)
        {
            return size / 2 ;
        }

        internal List<Node> GetAround(Node node, int _range, int targetSize)
        {
            
           
            templist.Clear();
            int tx = (int) node.pos.x;
            int ty = (int) node.pos.z;
            int id;
            int range = _range + GetGridRange(targetSize);
            
            int gx, gy;
            for (int x = -range; x <= range; x++)
            {
               for (int y = -range; y <= range; y++)
                {

                    gx = (tx + x);
                    gy = (ty + y);

                    if (gx < 0 || gy < 0 || gx > this.column || gy > row)
                    {
                        continue;
                    }

                    id = gx * this.column + gy;

                    if (dicNodeGraph.ContainsKey(id))
                    {
                        if (dicNodeGraph[id].walkable)
                        {
                            templist.Add(dicNodeGraph[id]);
                            FightSceneRender.Instance.battleFieldRender.platform.SetNodeState(id, ENodeColor.CanBuild);
                        }
                    }
                }
            }

            return templist;
        }


        internal void Die(Role fightRole)
        {
            fightRole.node.walkable = true;
            int nodeId;
            for (int i = 0; i < fightRole.costNodes.Length; i++)
            {
                nodeId = fightRole.costNodes[i];
                dicNodeGraph[nodeId].walkable = true;
            }

            composite.Die(fightRole.teamId, fightRole.id);
        }


        //获取当前单元格所有可行邻居格
        private void GetNeighbortPoint(Node point, bool checkWeapon)
        {
            //neighborCells.Clear();

            //for (int i = 0; i < Hex.directions.Count; i++)
            //{
            //    Hex hex = point.Coordinate.Neighbor(i);
            //    if (CheckMove(hex, checkWeapon) && closeList.Contains(hex) == false)
            //    {
            //        neighborCells.Add(hex);
            //    }
            //}
        }
    }
}