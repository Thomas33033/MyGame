using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Game.Fight;

public class Entity: TimeSchedule
{
    public static int uidCount = 1;
    public int uid = 0;
    public Entity()
    {
        this.uid = ++Entity.uidCount;
    }

    /// <summary>
    /// 请重写该方法
    /// </summary>
    /// <returns></returns>
    public virtual EEntityType GetEntityType() {
        return EEntityType.None;
    }
}

public class EntityData
{ 
}

public class Point
{
    public int x;
    public int y;
    public int z;
}

