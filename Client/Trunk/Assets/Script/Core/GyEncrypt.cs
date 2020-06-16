using System.Security.Cryptography;
using System;
using System.Text;
using System.IO;


/// <summary>
/// 加密解密相关的类;
/// </summary>
public static class GyEncrypt
{

    #region start DES
    public static string C_PRIVATE_KEY = string.Empty; //8 bit
    public static byte[] C_KEYS = new byte[8];

    /// <summary>
    /// DES:Data Encryption Standard,可逆加密算法.加密;
    /// </summary>
    /// <returns>返回加密后的数据字串.;</returns>
    /// <param name="encryptString">需要加密的字串.;</param>
    /// <param name="encryptKey">PRIVATE_KEY</param>
    public static string EncryptDES(string encryptString, string encryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = C_KEYS;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }

    public static string EncryptDES1(string encryptString, string encryptKey, byte[] keys)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
            byte[] rgbIV = keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }

    /// <summary>
    /// DES:Data Encryption Standard,可逆加密算法.解密;
    /// </summary>
    /// <returns>返回解密后的数据字串.;</returns>
    /// <param name="decryptString">需要解密的字串.;</param>
    /// <param name="decryptKey">PRIVATE_KEY</param>
    public static string DecryptDES(string decryptString, string decryptKey)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey.Substring(0, 8));
            byte[] rgbIV = C_KEYS;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }
    }
    #endregion end DES

    #region start EncryptToMD5
    /// <summary>
    /// MD5不可逆算法;
    /// </summary>
    /// <returns>加密后的字符串;</returns>
    /// <param name="src">Source.</param>
    public static string EncryptToMD5(string src)
    {
        try
        {
            StringBuilder sbr = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(src);
            byte[] md5Data = md5.ComputeHash(data);
            md5.Clear();
            for (int i = 0; i < md5Data.Length; i++)
            {
                sbr.Append(md5Data[i].ToString("x").PadLeft(2, '0'));
            }
            return sbr.ToString();
        }
        catch
        {
            return src;
        }
    }
    #endregion end EncryptToMD5

    #region start EncryptToSHA1

    /// <summary>
    /// 获取由SHA1加密的字符串;
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string EncryptToSHA1(string str)
    {
        SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
        byte[] str1 = Encoding.UTF8.GetBytes(str);
        byte[] str2 = sha1.ComputeHash(str1);
        sha1.Clear();
        (sha1 as IDisposable).Dispose();
        return Convert.ToBase64String(str2);
    }
    #endregion end EncryptToSHA1


    #region start Base64

    /// <summary>
    /// Base64加密;
    /// </summary>
    /// <param name="codeName">加密采用的编码方式;</param>
    /// <param name="source">待加密的明文;</param>
    /// <returns></returns>
    public static string EncodeBase64(Encoding encode, string source)
    {
        string result = "";
        byte[] bytes = encode.GetBytes(source);
        try
        {
            result = Convert.ToBase64String(bytes);
        }
        catch
        {
            result = source;
        }
        return result;
    }

    /// <summary>
    /// Base64加密，采用utf8编码方式加密;
    /// </summary>
    /// <param name="source">待加密的明文;</param>
    /// <returns>加密后的字符串;</returns>
    public static string EncodeUTF8Base64(string source)
    {
        return EncodeBase64(Encoding.UTF8, source);
    }

    /// <summary>
    /// Base64解密;
    /// </summary>
    /// <param name="codeName">解密采用的编码方式，注意和加密时采用的方式一致;</param>
    /// <param name="source">待解密的密文;</param>
    /// <returns>解密后的字符串;</returns>
    public static string DecodeBase64(Encoding encode, string source)
    {
        string result = "";
        byte[] bytes = Convert.FromBase64String(source);
        try
        {
            result = encode.GetString(bytes);
        }
        catch
        {
            result = source;
        }
        return result;
    }

    /// <summary>
    /// Base64解密，采用utf8编码方式解密;
    /// </summary>
    /// <param name="result">待解密的密文;</param>
    /// <returns>解密后的字符串;</returns>
    public static string DecodeUTF8Base64(string result)
    {
        return DecodeBase64(Encoding.UTF8, result);
    }

    #endregion end Base64


    public static byte[] C_KEY_BANK =
    {
        0x00,0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,0x0A,0x0B,0x0C,0x0D,0x0E,0x0F,
        0x10,0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,0x1A,0x1B,0x1C,0x1D,0x1E,0x1F,
        0x20,0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,0x2A,0x2B,0x2C,0x2D,0x2E,0x2F,
        0x30,0x31,0x32,0x33,0x34,0x35,0x36,0x37,0x38,0x39,0x3A,0x3B,0x3C,0x3D,0x3E,0x3F,
        0x40,0x41,0x42,0x43,0x44,0x45,0x46,0x47,0x48,0x49,0x4A,0x4B,0x4C,0x4D,0x4E,0x4F,
        0x50,0x51,0x52,0x53,0x54,0x55,0x56,0x57,0x58,0x59,0x5A,0x5B,0x5C,0x5D,0x5E,0x5F,
        0x60,0x61,0x62,0x63,0x64,0x65,0x66,0x67,0x68,0x69,0x6A,0x6B,0x6C,0x6D,0x6E,0x6F,
        0x70,0x71,0x72,0x73,0x74,0x75,0x76,0x77,0x78,0x79,0x7A,0x7B,0x7C,0x7D,0x7E,0x7F,
        0x80,0x81,0x82,0x83,0x84,0x85,0x86,0x87,0x88,0x89,0x8A,0x8B,0x8C,0x8D,0x8E,0x8F,
        0x90,0x91,0x92,0x93,0x94,0x95,0x96,0x97,0x98,0x99,0x9A,0x9B,0x9C,0x9D,0x9E,0x9F,
        0xA0,0xA1,0xA2,0xA3,0xA4,0xA5,0xA6,0xA7,0xA8,0xA9,0xAA,0xAB,0xAC,0xAD,0xAE,0xAF,
        0xB0,0xB1,0xB2,0xB3,0xB4,0xB5,0xB6,0xB7,0xB8,0xB9,0xBA,0xBB,0xBC,0xBD,0xBE,0xBF,
        0xC0,0xC1,0xC2,0xC3,0xC4,0xC5,0xC6,0xC7,0xC8,0xC9,0xCA,0xCB,0xCC,0xCD,0xCE,0xCF,
        0xD0,0xD1,0xD2,0xD3,0xD4,0xD5,0xD6,0xD7,0xD8,0xD9,0xDA,0xDB,0xDC,0xDD,0xDE,0xDF,
        0xE0,0xE1,0xE2,0xE3,0xE4,0xE5,0xE6,0xE7,0xE8,0xE9,0xEA,0xEB,0xEC,0xED,0xEE,0xEF,
        0xF0,0xF1,0xF2,0xF3,0xF4,0xF5,0xF6,0xF7,0xF8,0xF9,0xFA,0xFB,0xFC,0xFD,0xFE,0xFF
    };
}

