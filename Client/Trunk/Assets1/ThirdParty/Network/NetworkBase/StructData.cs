using Net;
using UnityEngine;
using System.Collections;

public class StructData
{
    public virtual OctetsStream WriteData(OctetsStream os)
    {
        return os;
    }

    public virtual OctetsStream ReadData(OctetsStream os)
    {
        return os;
    }
}
