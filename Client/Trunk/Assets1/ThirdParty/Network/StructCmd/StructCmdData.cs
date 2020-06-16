using Net;
using UnityEngine;
using System.Collections;

#region FL
public class stServerReturnLoginSuccessCmd : StructCmd
{
    public uint dwUserID;
    public uint loginTempID;
    public string pstrIP;
    public ushort wdPort;
    public string account;
    public byte[] key;
    public uint state;

    public stServerReturnLoginSuccessCmd()
    {

    }
    
    public override OctetsStream ReadStruct(OctetsStream os)
    {
        dwUserID = os.unmarshal_uint();
        loginTempID = os.unmarshal_uint();
        pstrIP = os.unmarshal_String(GlobalVar.MAX_IP_LENGTH);
        wdPort = os.unmarshal_ushort();
        account = os.unmarshal_String(GlobalVar.MAX_ACCNAMESIZE);
        key = os.unmarshal_bytes(GlobalVar.MAX_KEY);
        state = os.unmarshal_uint();
        return os;
    }
}


public class stUserVerifyVerCmd : StructCmd
{
    public uint reserve;	//保留字段
    public string version;

    public stUserVerifyVerCmd()
            : base(104,120)
    {

    }

    public override OctetsStream WriteStruct(OctetsStream os)
    {
        os.marshal_uint(reserve);
        os.marshal_string(version, GlobalVar.MAX_VERSION);
        return os;
    }
}

public class stIphoneUserRequestLoginCmd : StructCmd
{
    public ushort userType;
    public string account;                     //玩家数字账号
    public string pstrPassword;	            //<用户密码 
    public ushort game;			            //<游戏类型编号，目前一律添0 
    public ushort zone;				        //<游戏区编号 
    public ushort wdNetType;            		//<网关网络类型，0电信，1网通 
    public ulong uid;                          //用户uid
    public string token;                       //token字符串长度
    public string phone_uuid;                  // 机器唯一uuid;

    public stIphoneUserRequestLoginCmd()
        : base(104, 1)
    {

    }

    public override OctetsStream WriteStruct(OctetsStream os)
    {
        os.marshal_ushort(userType);
        os.marshal_string(account, GlobalVar.MAX_ACCNAMESIZE);
        os.marshal_string("", 33);
        os.marshal_ushort(game);
        os.marshal_ushort(zone);
        os.marshal_ushort(wdNetType);
        os.marshal_ulong(uid);
        os.marshal_string(token, GlobalVar.MAX_NAMESIZE);
        os.marshal_string(phone_uuid, GlobalVar.MAX_NAMESIZE);
        return os;
    }
}


public class stServerReturnLoginFailedCmd : StructCmd
{
    public byte byReturnCode;   /**< 返回的子参数 */
    public string data;				// 对应的字符串错误消息

    public stServerReturnLoginFailedCmd()
    {

    }
    
    public override OctetsStream ReadStruct(OctetsStream os)
    {
        byReturnCode = os.unmarshal_byte();
        data = os.unmarshal_String();
        return os;
    }
}
#endregion

#region GateWay

public class stUserVerifyVerCmd_CS : StructCmd
{
    public uint reserve;	//保留字段
    public string version;

    public stUserVerifyVerCmd_CS()
        : base(CommandID.stUserVerifyVerCmd_CS)
    {

    }

    public override OctetsStream WriteStruct(OctetsStream os)
    {
        os.marshal_uint(reserve);
        os.marshal_string(version, GlobalVar.MAX_VERSION);
        return os;
    }
}

public class stServerReturnLoginFailedCmd_SC : StructCmd
{
    public byte byReturnCode;   /**< 返回的子参数 */

    public stServerReturnLoginFailedCmd_SC()
    {

    }
    
    public override OctetsStream ReadStruct(OctetsStream os)
    {
        byReturnCode = os.unmarshal_byte();
        return os;
    }
}

public class stPhoneInfo : StructData
{
    public string phone_uuid = "";  // 机器唯一码
    public string pushid = "";      // 推送id
    public string phone_model = ""; // 机型
    public string resolution = "";  // 分辨率
    public string opengl = "";      // opengl
    public string cpu = "";
    public string ram = "";
    public string os = "";   // 操作系统

    public override OctetsStream WriteData(OctetsStream ots)
    {
        ots.marshal_string(phone_uuid, 100);
        ots.marshal_string(pushid, 100);
        ots.marshal_string(phone_model, 100);
        ots.marshal_string(resolution, 100);
        ots.marshal_string(opengl, 100);
        ots.marshal_string(cpu, 100);
        ots.marshal_string(ram, 100);
        ots.marshal_string(os, 100);
        return ots;
    }

    public override OctetsStream ReadData(OctetsStream ots)
    {
        return ots;
    }
}

public class stIphoneLoginUserCmd_CS : StructCmd
{
    public uint accid;              //平台返回的ACCID，填写在这儿,用于登录验证,时间戳
    public ushort user_type;	    //登陆类型
    public uint loginTempID;        //平台生成的登录临时ID，用于验证登录有效性,SuperServer/FLClient.cpp中自增生成
    public string account;          //帐号，通行证用户填通行证帐号，非通行证用户填，IPHONE UUID
    public string password;         //非通行证用户填登录密码
    public string szMAC;            // MAC地址
    public string szFlat;           // 平台字符串	
    public stPhoneInfo phone;       // 机器信息
    public byte[] key;             //保存密钥，整个数组用随机数填充

    public stIphoneLoginUserCmd_CS()
        : base(CommandID.stIphoneLoginUserCmd_CS)
    {

    }

    public override OctetsStream WriteStruct(OctetsStream os)
    {
        os.marshal_uint(accid);
        os.marshal_ushort(user_type);
        os.marshal_uint(loginTempID);
        os.marshal_string(account, GlobalVar.MAX_ACCNAMESIZE);
        os.marshal_string(password, GlobalVar.MAX_PASSWORD);
        os.marshal_string(szMAC, GlobalVar.MAX_MAC_ADDR);
        os.marshal_string(szFlat, GlobalVar.MAX_FLAT_LENGTH);      
        os.marshal(phone);
        os.marshal_bytes(key, GlobalVar.MAX_KEY); 
        return os;
    }
}

public class stCreateNewRoleUserCmd_CS : StructCmd
{
    public string strRoleName;
	public byte bySex;
	public uint flatid; // 平台id 用来生成邀请码
    public stPhoneInfo phone; // 机器信息
	public uint heroid; // 选择英雄id

    public stCreateNewRoleUserCmd_CS()
        : base(CommandID.stCreateNewRoleUserCmd_CS)
    {

    }

    public override OctetsStream WriteStruct(OctetsStream os)
    {
        os.marshal_string(strRoleName, GlobalVar.MAX_NAMESIZE);
        os.marshal_byte(bySex);
        os.marshal_uint(flatid);
        os.marshal(phone);
        os.marshal_uint(heroid);
        return os;
    }
}

#endregion

public class stRetErrorOperationUserCmd_SC : StructCmd
{
    public uint cmd_id;   /**< 返回的子参数 */

    public stRetErrorOperationUserCmd_SC()
    {

    }

    public override OctetsStream ReadStruct(OctetsStream os)
    {
        cmd_id = os.unmarshal_uint();
        return os;
    }
}