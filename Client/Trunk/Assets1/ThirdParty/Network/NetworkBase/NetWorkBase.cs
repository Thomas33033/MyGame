using Net;

public class NetWorkBase
{


    public virtual void Initialize()
    {
        RegisterMsg();
    }

    public virtual void RegisterMsg()
    {

    }

    public void SendMsg(StructCmd cmd, bool istoself = false)
    {
        NetWorkModule.Instance.Send(cmd, istoself);
    }

    public virtual void Uninitialize()
    {

    }
}