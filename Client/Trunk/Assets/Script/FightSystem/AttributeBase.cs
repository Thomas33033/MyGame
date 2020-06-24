using System.Collections.Generic;


namespace Fight
{
    /**
    * 角色属性数据
    **/
    public class AttributeBase
    {
        //基础数据 
        protected AttributeData attribute;

        protected AttributeData attributeBase;

        protected AttributeData attributeBouns;

        public AttributeData AttributeData => attributeBase;

        public virtual float attackCd => 150f / attribute.attackSpeed;
        public virtual int hpMax => attribute.hp;
        public virtual int mpMax => attribute.anger;
        public virtual int crit => attribute.crit;
        public virtual int dodge => attribute.dodge;
        public virtual int hit => attribute.hit;
        public virtual int range => attribute.range;
        public virtual int moveSpeed => attribute.moveSpeed / 30;
        public virtual int physicsAttack => attribute.physicsAttack;
        public virtual int magicAttack => attribute.magicAttack;
        public virtual int magicDefense => attribute.magicDefense;
        public virtual int physicsDefense => attribute.physicsDefense;
        public virtual float damageBouns => attribute.damageBouns / 100f;
        public virtual float damageReduction => attribute.damageReduction / 100f;

        /// <summary>
        /// 血量吸收
        /// </summary>
        public int hpSucking;

        /// <summary>
        /// 护甲穿透
        /// </summary>
        public int defensePenetration;

        /// <summary>
        /// 魔抗穿透
        /// </summary>
        public int magicDefensePenetration;

        /// <summary>
        /// 忽视护甲
        /// </summary>
        public int defenseDestroy;

        /// <summary>
        /// 忽视魔抗
        /// </summary>
        public int magicDefenseDestroy;

        StatusComponent statusComp;
        public AttributeBase(AttributeData attr)
        {
            this.attributeBase = attr;
            statusComp = new StatusComponent();
        }

        //更新数据
        protected virtual void UpdateAttribute()
        {
            this.attribute = this.attributeBase + this.attributeBase * this.attributeBouns;
        }

        //属性改变
        public virtual float AttrChange(string type, int value)
        {
            bool isBouns = type.EndsWith("%");

            if (isBouns)
            {
                type = type.Replace("%", "");
                return DoAttrChange(ref attributeBouns, type, value);
            }

            return DoAttrChange(ref attributeBase, type, value);
        }

        protected virtual float DoAttrChange(ref AttributeData attr, string type, int value)
        {
            switch (type)
            {
                case "anger":
                    attr.anger += value;
                    break;

                case "attack":
                    attr.physicsAttack += value;
                    break;

                case "attackSpeed":
                    attr.attackSpeed += value;
                    break;

                case "crit":
                    attr.crit += value;
                    break;

                case "defense":
                    attr.physicsDefense += value;
                    break;

                case "dodge":
                    attr.dodge += value;
                    break;

                case "magicDefense":
                    attr.magicDefense += value;
                    break;

                case "hit":
                    attr.hit += value;
                    break;

                case "hp":
                    attr.hp += value;
                    break;

                case "range":
                    attr.range += value;
                    break;

                case "moveSpeed":
                    attr.moveSpeed += value;
                    break;

                case "unselected":
                    StatusChange(RoleStatus.Unselected, value);
                    break;

                case "damageReduction":
                    attr.damageReduction += value;
                    break;

                case "damageBouns":
                    attr.damageBouns += value;
                    break;

                case "allDefense":
                    attr.physicsDefense += value;
                    attr.magicDefense += value;
                    break;

                case "undead":
                    StatusChange(RoleStatus.Undead, value);
                    break;

                case "hide":
                    StatusChange(RoleStatus.Hide, value);
                    break;

                case "defensePenetration":
                    defensePenetration += value;
                    break;

                case "magicDefensePenetration":
                    magicDefensePenetration += value;
                    break;

                case "defenseDestroy":
                    defenseDestroy += value;
                    break;

                case "magicDefenseDestroy":
                    magicDefenseDestroy += value;
                    break;

                case "immuneDebuff":
                    StatusChange(RoleStatus.ImmuneDebuff, value);
                    break;

                case "silent":
                    StatusChange(RoleStatus.Silent, value);
                    break;

                case "dizz":
                    StatusChange(RoleStatus.Dizz, value);
                    break;

                case "hpSucking":
                    hpSucking += value;
                    break;
            }
            UpdateAttribute();
            //Debug.Log("attr change id:" + id + " effect:" + type + " bouns:" + value + " value:" + value);
            return value;
        }

        public bool StatusCheck(int status)
        {
            return this.statusComp.StatusCheck(status);
        }

        public bool StatusCheck(RoleStatus status)
        {
            return this.statusComp.StatusCheck(status);
        }

        public virtual void StatusChange(RoleStatus status, int t)
        {
            this.statusComp.StatusChange(status, t);
        }

        public virtual void StatusChange(int status, int t)
        {
            this.statusComp.StatusChange(status, t);
        }
    }

}