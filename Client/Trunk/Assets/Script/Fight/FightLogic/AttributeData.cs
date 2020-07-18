using System;
using System.Collections;
using System.Reflection;


namespace Fight
{

    /// <summary>
    /// 玩家基础数据
    /// </summary>
    public struct AttributeData
    {
        //血量
        public int hp;
        //攻击范围
        public int range;
        //移动速度
        public int moveSpeed;
        //愤怒
        public int anger;
        //攻击速度
        public int attackSpeed;
        //暴击
        public int crit;
        //物攻
        public int physicsAttack;
        //魔攻
        public int magicAttack;
        //物理防御
        public int physicsDefense;
        //魔法防御
        public int magicDefense;
        //闪避百分比
        public int dodge;
        //击中百分比
        public int hit;
        //伤害减免
        public int damageReduction;
        //伤害加成
        public int damageBouns;

        static private Type _type = typeof(AttributeData);
        static private Type _typeInt = typeof(int);

        public static AttributeData operator +(AttributeData a, AttributeData b)
        {
            AttributeData result = new AttributeData();
            object obj = result;
            FieldInfo[] memberInfo = _type.GetFields();
            for (int i = 0; i < memberInfo.Length; i++)
            {
                FieldInfo fieldInfo = memberInfo[i];
                if (fieldInfo.FieldType == _typeInt)
                {
                    fieldInfo.SetValue(obj, (int)fieldInfo.GetValue(a) + (int)fieldInfo.GetValue(b));
                }
                else
                {
                    fieldInfo.SetValue(obj, (float)fieldInfo.GetValue(a) + (float)fieldInfo.GetValue(b));
                }
            }
            return (AttributeData)obj;
        }

        public static AttributeData operator *(AttributeData a, float b)
        {
            AttributeData result = new AttributeData();
            object obj = result;
            FieldInfo[] memberInfo = _type.GetFields();
            for (int i = 0; i < memberInfo.Length; i++)
            {
                FieldInfo fieldInfo = memberInfo[i];
                if (fieldInfo.FieldType == _typeInt)
                {
                    fieldInfo.SetValue(obj, (int)((int)fieldInfo.GetValue(a) * b));
                }
                else
                {
                    fieldInfo.SetValue(obj, (float)fieldInfo.GetValue(a) * b);
                }
            }
            return (AttributeData)obj;
        }

        public static AttributeData operator *(AttributeData a, AttributeData b)
        {
            AttributeData result = new AttributeData();
            object obj = result;
            FieldInfo[] memberInfo = _type.GetFields();
            for (int i = 0; i < memberInfo.Length; i++)
            {
                FieldInfo fieldInfo = memberInfo[i];
                if (fieldInfo.FieldType == _typeInt)
                {
                    int v1 = (int)fieldInfo.GetValue(a);
                    int v2 = (int)fieldInfo.GetValue(b);
                    fieldInfo.SetValue(obj, (int)Math.Ceiling(v1 * (v2 / 100f)));
                }
                else
                {
                    float v1 = (float)fieldInfo.GetValue(a);
                    float v2 = (float)fieldInfo.GetValue(b);
                    fieldInfo.SetValue(obj, v1 * v2);
                }
            }
            return (AttributeData)obj;
        }
    }

}

