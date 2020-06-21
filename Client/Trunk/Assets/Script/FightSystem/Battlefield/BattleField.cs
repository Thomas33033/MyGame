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
            for (int i = 0; i < this.nodeGraph.Length; i++) {
                dicNodeGraph.Add(nodeGraph[i].ID, nodeGraph[i]);
            }
        }

        public void AddFightObject(FightObject fightObject)
        {
            fightObject.SetBattleField(this);
            listObject.Add(fightObject);
        }

        //public MapGrid GetRandomHex()
        //{
        //    return listHex[UnityEngine.Random.Range(0, listHex.Count)];
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

       
        internal void AddRole(Role v, int position)
        {
            if (dicNodeGraph[position].walkable == true)
            {
                v.position = dicNodeGraph[position];
                v.SetBattleField(this);

                int nodeId = 0;
                for (int i = 0; v.costNodes != null && i < v.costNodes.Length; i++)
                {
                    nodeId = v.costNodes[i];
                    dicNodeGraph[nodeId].walkable = false;
                    FightSceneRender.Instance.battleFieldRender.platform.RefreshColor(nodeId);
                }

                listRoles.Add(v);
                v.Init();
            }
            else
            {
                Debug.LogError("该节点阻塞，不能创建角色");
               // AddRole(v, 0);
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
        public List<Role> GetEnemy(Role attacker, Node node, int targetType, int rangeType, int range, int width = 0)
        {
            if (rangeType == 0)
                return GetEnemy(attacker, targetType, range);

            if (rangeType == 3)
                node = attacker.position;

            List<Role> list = new List<Role>();

            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].StatusCheck(RoleStatus.Unselected) && targetType == 1)
                    continue;

                if (targetType == 1)
                {
                    if (attacker.teamId == listRoles[i].teamId)
                    {
                        continue;
                    }
                }
                else if (targetType == 2)
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
                else if (targetType == 3)
                {
                    if (attacker != listRoles[i])
                    {
                        continue;
                    }
                }
                else if (targetType == 4)
                {
                    if (attacker.teamId != listRoles[i].teamId)
                    {
                        continue;
                    }
                }

                if (node.Distance(listRoles[i].position) > range)
                {
                    continue;
                }

                list.Add(listRoles[i]);
            }

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
        public List<Role> GetEnemy(Role attacker, int targetType, int range)
        {
            List<Role> list = new List<Role>();

            if (targetType == 3)
            {
                list.Add(attacker);
                return list;
            }

            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].StatusCheck(RoleStatus.Unselected) && targetType == 1)
                    continue;
                if (targetType == 1)
                {
                    if (attacker.teamId == listRoles[i].teamId)
                    {
                        continue;
                    }
                }
                else if (targetType == 2)
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
                else if (targetType == 3)
                {
                    if (attacker != listRoles[i])
                    {
                        continue;
                    }
                }
                else if (targetType == 4)
                {
                    if (attacker.teamId != listRoles[i].teamId)
                    {
                        continue;
                    }
                }

                if (range != 0 && listRoles[i].position.Distance(attacker.position) > range)
                {
                    continue;
                }

                list.Add(listRoles[i]);
            }

            //if (list.Count > 0)
            //{
            //    list.Sort(ListTargetSortHandler);
            //}

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

        public List<Vector3> RoleMove(Role fightRole, Node grid)
        {
            fightRole.position.walkable = true;
            fightRole.position = grid;
            //grid.walkable = false;
            return PathFinder.GetPath(fightRole.position.pos, grid.pos, this.nodeGraph);
        }

        public bool CheckMove(Node grid)
        {
            return grid.walkable;
        }

        public List<Vector3> GetMoveHex(Node start, Node end, bool checkWeapon)
        {
            List<Vector3> results = new List<Vector3>();
            GetPath(start, end, checkWeapon, results);
            return results;
        }

        private List<Node> templist = new List<Node>();

        internal List<Node> GetAround(Node node, int range = 1)
        {
            templist.Clear();
            int tx = node.x;
            int ty = node.y;
            int id = 0;
            for (int x = -range; x <= range; x++)
            {
               for (int y = -range; y <= range; y++)
                {
                    id = (tx + x) * node.width + (ty + y);

                    if (dicNodeGraph[id].walkable)
                    {
                        templist.Add(dicNodeGraph[id]);
                        FightSceneRender.Instance.battleFieldRender.platform.SetNodeState(id, ENodeColor.CanBuild);
                    }
                }
            }

            return templist;
        }


        public void GetPath(Node start, Node end, bool checkWeapon, List<Vector3> results)
        {
            results = PathFinder.GetPath(start.pos, end.pos, this.nodeGraph);
        }

        internal void Die(Role fightRole)
        {
            fightRole.position.walkable = false;
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