using Net;
using UnityEngine;
using System.Collections;

public class MessageEncrypt
{
    public static CEncrypt DESEncryptHandle = new CEncrypt(NullCmd.IS_ENCRYPT); // 是否加密，true为加密，false为不加密

    private static CEncrypt.DES_cblock key;

    public static int FillZero(int offset ,ref OctetsStream os)
    {
        int mod = (os.size() - offset) % 8;
        if (0 != mod)
        {
            for (int i = 0; i < (8-mod); i++)
            {
                os.push_back(0);
            }
        }
        if (mod == 0)
        {
            return 0;
        }
        else
        {
            return 8 - mod;
        }
    }

    public static void DESEncrypt(int offset, int length,ref OctetsStream os)
    {
        DESEncryptHandle.encdec_des(os.buffer(), offset, length, true);
    }

    public static void DESDencrypt(ref OctetsStream os)
    {
        DESEncryptHandle.encdec_des(os.buffer(), 0, os.size(), false);
    }

    public static void Init()
    {
        DESEncryptHandle.init();
    }

    public static void SetNewKey(CEncrypt.DES_cblock newkey)
    {
        key = newkey;
    }

    public static void ReSetDESKey()
    {
        lock (DESEncryptHandle)
        {
            DESEncryptHandle.DES_set_key(key);
        }
    }
}
