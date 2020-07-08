using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class Entity: TimeSchedule
{
    public static int uidCount = 1;
    public int uid = 0;
    public Entity()
    {
        this.uid = ++Entity.uidCount;
    }

    public void OnReset()
    { 
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

