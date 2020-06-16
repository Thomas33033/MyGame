using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class GySystemInfo
{

    public enum LevelEnum
    {
        HIGH = 1,
        MIDDLE = 2,
        LOW = 3,
    }

    public static string GetCPU()
    {
        string cpu = "";
#if UNITY_ANDROID
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaClass unityPluginLoader = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin");
                cpu = unityPluginLoader.CallStatic<string>("GetCPU", currentActivity);
            }
            catch (System.Exception e)
            {
                cpu = e.ToString();
            }
#endif
        return string.IsNullOrEmpty(cpu) ? string.Empty : cpu;
    }

    public static int GetLevelInt()
    {
#if UNITY_ANDROID
            if (isInCPUList(GetCPU(), m_highCPUList))
            {
                return (int)LevelEnum.HIGH;
            }
            else if (isInCPUList(GetCPU(), m_middleCPUList))
            {
                return (int)LevelEnum.MIDDLE;
            }
            else if (isInCPUList(GetCPU(), m_lowCPUList))
            {
                return (int)LevelEnum.LOW;
            }
            else if ((SystemInfo.systemMemorySize > 3000) && (SystemInfo.processorCount >= 4) && (SystemInfo.processorFrequency > 2000))
            {
                return (int)LevelEnum.HIGH;
            }
            else if ((SystemInfo.systemMemorySize < 2000) || (SystemInfo.processorFrequency < 1500))
            {
                return (int)LevelEnum.LOW;
            }
#elif UNITY_IOS
        var iOSGen = UnityEngine.iOS.Device.generation;
        if ((int)iOSGen >= (int)UnityEngine.iOS.DeviceGeneration.iPhone6)
        {
            return (int)LevelEnum.HIGH;
        }
#endif

        return (int)LevelEnum.MIDDLE;
    }

    public static bool isInCPUList(string cpuName, List<string> list)
    {
        cpuName = cpuName.ToLower();
        cpuName = cpuName.Replace(" ", "");
        for (int i = 0, count = list.Count; i < count; ++i)
        {
            if (cpuName.Contains(list[i]))
            {
                return true;
            }
        }

        return false;
    }

    private static List<string> m_highCPUList = new List<string>() {
            "骁龙845",
            "exynos9810",
            "骁龙835","msm8998",
            "exynos8895",
            "麒麟970","kirin970",
            "麒麟960","kirin960",
            "骁龙821","msm8996",
            "heliox30",
            "骁龙820",
            "骁龙820降频版",
            "exynos8890",
        };

    private static List<string> m_middleCPUList = new List<string>() {
            "heliop60",
            "骁龙660",
            "heliox27",
            "麒麟955","kirin955",
            "骁龙636",
            "麒麟950","kirin950",
            "骁龙630",
            "骁龙810","msm8994",
            "exynos7420",
            "heliox25",
            "heliox23",
            "骁龙653","msm8976",
            "heliox20",
            "heliop30",
            "骁龙652",
            "heliop25",
            "tegrax1",
            "heliop23",
            "骁龙650","msm8956",
            "heliop20",
            "骁龙808","msm8992",
            "heliox10","mt6795",
            "麒麟935","kirin935",
            "tegrak1",
        };

    private static List<string> m_lowCPUList = new List<string>() {
            "heliop15","mt6755t",
            "exynos5433",
            "heliop10","mt6755",
            "麒麟930","kirin930",
            "麒麟655","kirin655",
            "骁龙626","msm8953",
            "骁龙805","apq8084",
            "麒麟650","kirin650",
            "骁龙625",
            "骁龙801","msm8x74ac",
            "exynos5430",
            "z3590",
            "msm8x74ab",
            "exynos7870",
            "z3580",
            "msm8x74aa",
            "exynos7580",
            "mt6752/m",
            "z3570",
            "exynos5433",
            "mt6753",
            "z3560",
            "mt6750",
            "mt6739",
            "骁龙450",
            "mt6735",
            "exynos5800",
            "z3530",
            "exynos5422",
            "麒麟620","kirin620",
            "骁龙435","msm8940",
            "骁龙617","msm8952",
            "骁龙800","msm8974",
            "exynos5420",
            "mt6595/t",
            "麒麟928","kirin928",
            "z3480",
            "tegra4",
            "骁龙430","msm8937",
            "骁龙616","msm8939v2",
            "exynos5410",
            "mt6592",
            "麒麟925","kirin925",
            "z3460",
            "骁龙427","msm8920",
            "骁龙615","msm8939",
            "麒麟920","kirin920",
            "tegra4i",
            "apq8064",
            "骁龙425","msm8917",
            "骁龙600","apq8064t",
            "exynos5260",
            "mt6582",
            "k3v2+","麒麟910","kirin910",
            "z3580",
            "骁龙212","msm8909v2",
            "骁龙610","msm8936",
            "骁龙210","msm8909",
            "骁龙412","msm8916v2",
            "exynos5250",
            "msm8960t",
            "骁龙208","msm8208",
            "骁龙410","msm8916",
            "exynos4412",
            "atomz3480",
            "msm8260a",
            "msm8660a",
            "msm8960",
            "骁龙200","msm8x12",
            "骁龙400","msm8x30",
            "k3v2e",
            "atomz2580",
            "msm8x10",
            "k3v2",
            "tegra3",
            "z2480",
        };
}

