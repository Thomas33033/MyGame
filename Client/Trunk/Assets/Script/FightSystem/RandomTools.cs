using System;

public class RandomTools
{
    private static Random random;

    public static void SetRandomSeed(int seed)
    {
        //设置随机数种子
        random = new System.Random(seed);
    }

    /// <summary>
    /// 随机整型
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static int Range(int min, int max)
    {
        return random.Next(min, max);
    }

    /// <summary>
    /// 随机浮点数
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static float Range(float min, float max)
    {
        var r = random.NextDouble();
        return (float)(r * (max - min) + min);
    }
}