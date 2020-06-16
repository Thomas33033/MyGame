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

        public Dictionary<MapGrid, int> dicMapData;

        public List<Role> listRoles;

        public List<FightObject> listObject;

        public List<MapGrid> listHex;

        public List<FightReport> listReport;

        public BattleComposite composite;

        public List<FightAttackData> listAttacks;

        public bool isEnd;

        public BattleField(int id, string teamId)
        {
            this.id = id;
            this.teamId = teamId;
            dicMapData = new Dictionary<MapGrid, int>();
            listRoles = new List<Role>();
            listHex = new List<MapGrid>();
            listReport = new List<FightReport>();
            listObject = new List<FightObject>();
            listAttacks = new List<FightAttackData>();

            ////初始化寻路格子
            //for (int i = 3; i > -5; i--)
            //{
            //    for (int j = 1; j > -3; j--)
            //    {
            //        DoubledCoord coord = new DoubledCoord(i, j);
            //        MapGrid grid = coord.RdoubledToCube2();
            //        dicMapData.Add(hex, "");
            //        listHex.Add(hex);
            //    }
            //}
        
            InitA();
        }

        //public void AddFightObject(FightObject fightObject)
        //{
        //    fightObject.SetBattleField(this);
        //    listObject.Add(fightObject);
        //}

        public MapGrid GetRandomHex()
        {
            return listHex[UnityEngine.Random.Range(0, listHex.Count)];
        }


        internal void AddAttackData(FightAttackData fightAttackData)
        {
            listAttacks.Add(fightAttackData);
        }

        internal List<Role> GetAroundRoles(MapGrid hex, int distance)
        {
            List<Role> list = new List<Role>();
            for (int i = 0; i < listRoles.Count; i++)
            {
                if (listRoles[i].position.Subtract(hex).Length() <= distance)
                {
                    list.Add(listRoles[i]);
                }
            }
            return list;
        }

        internal List<Role> GetAllRoles()
        {
            return new List<Role>(listRoles);
        }

        public void AddRole(Role v, int direct)
        {
            for (int i = 0; i < 100; i++)
            {
                MapGrid position = listHex[UnityEngine.Random.Range(0, listHex.Count)];
                if (direct == 1 && position.r < 2)
                {
                    continue;
                }
                else if (direct == -1 && position.r > -1)
                {
                    continue;
                }

                if (dicMapData[position] <= 0)
                {
                    AddRole(v, position);
                    break;
                }
            }
        }

        internal void AddRole(Role v, MapGrid position)
        {
            if (dicMapData[position] <= 0)
            {
                v.position = position;
                v.SetBattleField(this);
                dicMapData[position] = v.id;
                listRoles.Add(v);

                v.Init();
            }
            else
            {
                AddRole(v, 0);
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
        public List<Role> GetEnemy(Role attacker, MapGrid o, int targetType, int rangeType, int range, int width = 0)
        {
            if (rangeType == 0)
                return GetEnemy(attacker, targetType, range);

            if (rangeType == 3)
                o = attacker.position;

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

                if (o.Distance(listRoles[i].position) > range)
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
                MapGrid d = o.Subtract(attacker.position);
                int[] tempArr = new int[] { d.q, d.r, d.s };
                int[] arr = new int[] { 0, 0, 0 };
                bool flag = false;

                for (int i = 0; i < tempArr.Length; i++)
                {
                    if (flag == false && Mathf.Abs(tempArr[i]) == Mathf.Min(Mathf.Abs(tempArr[0]), Mathf.Abs(tempArr[1]), Mathf.Abs(tempArr[2])))
                    {
                        arr[i] = 0;
                        flag = true;
                    }
                    else if (tempArr[i] > 0)
                    {
                        arr[i] = 1;
                    }
                    else
                    {
                        arr[i] = -1;
                    }
                }

                d = new MapGrid(arr[0], arr[1], arr[2]);

                int index = MapGrid.directions.IndexOf(d);

                List<MapGrid> listPosition = new List<MapGrid>();
                MapGrid temp = attacker.position;
                for (int i = 0; i < range; i++)
                {
                    listPosition.Add(temp.Add(MapGrid.directions[index]));
                    listPosition.Add(temp.Add(MapGrid.directions[(index + 1) % MapGrid.directions.Count]));
                    listPosition.Add(temp.Add(MapGrid.directions[(index + MapGrid.directions.Count - 1) % MapGrid.directions.Count]));
                    temp = temp.Add(MapGrid.directions[index]);
                }

                for (int i = 0; i < list.Count; i++)
                {
                    if (listPosition.IndexOf(list[i].position) == -1)
                    {
                        list.RemoveAt(i);
                        i--;
                    }
                }
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

        public void RoleMove(Role fightRole, MapGrid grid)
        {
            dicMapData[fightRole.position] = -1;
            fightRole.position = grid;
            dicMapData[fightRole.position] = fightRole.id;
        }

        public bool CheckMove(MapGrid grid, bool checkWeapon = true)
        {
            if (dicMapData.ContainsKey(grid) == false)
                return false;
            if (checkWeapon)
            {
                //for (int i = 0; i < listCannon.Count; i++)
                //{
                //    if (listCannon[i].position.Equals(hex))
                //        return false;
                //}
            }

            return dicMapData[grid] <= 0;
        }

        public Stack<MapGrid> GetMoveHex(MapGrid start, MapGrid end, bool checkWeapon)
        {
            Stack<MapGrid> results = new Stack<MapGrid>();
            GetPath(start, end, checkWeapon, results);
            return results;
        }

        private List<MapGrid> templist = new List<MapGrid>();

        internal List<MapGrid> GetAround(MapGrid position, int range = 1)
        {
            templist.Clear();

            //for (int x = -range; x <= range; x++)
            //{
            //    for (int y = -range; y <= range; y++)
            //    {
            //        for (int z = -range; z <= range; z++)
            //        {
            //            if (x + y + z == 0)
            //                templist.Add(position.Add(new Hex(x, y, z)));
            //        }
            //    }
            //}

            return templist;
        }

        //private static ObjectPool<HexCellPoint> hexcellPool = null;//使用对象池，避免频繁创建对象

        private List<MapGrid> neighborCells = new List<MapGrid>(6);//可行邻居格最多6个

        //private HashSet<MapGrid> closeList;
        //private Heap<HexCellPoint> openList;

        private void InitA()
        {
            //openList = new Heap<HexCellPoint>(HexCellPoint.Compare, dicMapData.Count);//比全部网格稍少一点
            //closeList = new HashSet<Hex>(Vector2IntComparer.GetComperer());
            //if (hexcellPool == null)
            //    hexcellPool = new ObjectPool<HexCellPoint>(() => new HexCellPoint(), dicMapData.Count);
        }

        public bool GetPath(MapGrid start, MapGrid end, bool checkWeapon, Stack<MapGrid> results)
        {
        //    closeList.Clear();
        //    openList.Clear();

        //    HexCellPoint temp = hexcellPool.GetObject();
        //    temp.Coordinate = start;
        //    temp.H = end.Subtract(start).Length();
        //    temp.Parent = null;
        //    openList.Add(temp);
        //    Vector2IntComparer comparer = Vector2IntComparer.GetComperer();
        //    HexCellPoint temp2 = null, temp2OriParent;
        //    int oidx;
        //    while (openList.Count > 0)
        //    {
        //        temp = openList.RemoveTop();
        //        GetNeighbortPoint(temp, checkWeapon);
        //        for (int i = 0; i < neighborCells.Count; i++)
        //        {
        //            if (comparer.Equals(end, neighborCells[i])) //找到终点
        //            {
        //                temp2 = hexcellPool.GetObject();
        //                temp2.Parent = temp;
        //                temp2.Coordinate = end;
        //                goto ok;
        //            }
        //            if (!OpenListContains(neighborCells[i], out oidx))
        //            {
        //                temp2 = hexcellPool.GetObject();
        //                temp2.Parent = temp;
        //                temp2.Coordinate = neighborCells[i];
        //                temp2.CaculateG();
        //                temp2.H = Distance(end, neighborCells[i]);
        //                openList.Add(temp2);
        //            }
        //            else //开启列表包含该点，检测是否需要更换父节点
        //            {
        //                temp2 = openList[oidx];
        //                temp2OriParent = temp2.Parent;
        //                int orig = temp2.G;
        //                temp2.Parent = temp;
        //                temp2.CaculateG();
        //                if (temp2.G >= orig)  //原本的G值更小，恢复原状
        //                {
        //                    temp2.Parent = temp2OriParent;
        //                    temp2.G = orig;
        //                }
        //                else
        //                    openList.MoveUp(oidx);
        //            }
        //        }
        //        closeList.Add(temp.Coordinate);
        //        // openList.ReBuildHeap();//重建堆，效率反而更低
        //    }
        //    return false;
        ////return null; //其实也可以不返回null，这样就可以来到离不可达点周围最近的某个点
        //ok:
        //    while (temp2.Parent != null)//这样就不会把起点也放进去
        //    {
        //        results.Push(temp2.Coordinate);
        //        hexcellPool.RecircleObject(temp2);
        //        temp2 = temp2.Parent;
        //    }
            return true;
            //return results;
        }

        internal void Die(Role fightRole)
        {
            dicMapData[fightRole.position] = -1;
            composite.Die(fightRole.teamId, fightRole.id);
        }


        private int Distance(MapGrid end, MapGrid start)
        {
            return end.Subtract(start).Length();
        }

        //获取当前单元格所有可行邻居格
        private void GetNeighbortPoint(HexCellPoint point, bool checkWeapon)
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

        private bool OpenListContains(MapGrid grid, out int idx)
        {
            //Vector2IntComparer comparer = Vector2IntComparer.GetComperer();
            //for (int i = 0; i < openList.Count; i++)
            //{
            //    if (comparer.Equals(openList[i].Coordinate, coor))
            //    {
            //        idx = i;
            //        return true;
            //    }
            //}
            idx = -1;
            return false;
        }
    }

    public class HexCellPoint
    {
        public MapGrid Coordinate;
        public HexCellPoint Parent;

        /// <summary>
        /// 离起点的距离
        /// </summary>
        public int G;

        /// <summary>
        /// 离终点的距离
        /// </summary>
        public int H;

        /// <summary>
        /// 计算当前点到起点的距离
        /// </summary>
        public void CaculateG()
        {
            G = Parent != null ? Parent.G + 1 : 0; //对于六边形地图来说，只加1就可以了
        }

        public int F { get { return G + H; } }

        public static bool Compare(HexCellPoint p1, HexCellPoint p2)
        {
            int p1f = p1.F, p2f = p2.F;
            if (p1f == p2f)
                return p1.H < p2.H;
            return p1f < p2f;
        }
    }
}