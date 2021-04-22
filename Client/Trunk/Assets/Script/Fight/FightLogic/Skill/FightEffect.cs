using System.Collections.Generic;

namespace Fight
{
    /// <summary>
    /// 技能效果触发类型
    /// </summary>
    public enum TriggerType
    {
        None = 0,
        AttackBefore = 1,  //角色普攻前
        AttackAfter = 2,   //角色普攻后
        AttackDataAttackerHitBefore = 3,  //伤害命中前
        AttackDataExecuted = 4,   //伤害命中
        Time = 6,  //按时间触发
        Skill = 10,  //技能释放后
        AttackDataTargetHurtBefore = 20,  //角色受到伤害前
        AttackDataTargetDamageBefore = 21,  //角色受到Damage伤害计算前
        AttackDataTargetHurtAfter = 22,  //角色受到伤害后
        AttackDataAttackerDamageBefore = 23, //角色伤害计算之前
        Buff = 30,  //buff执行之后
        BuffStack = 31, //buff叠满之后
        AttackDataAttackerHit = 40, //角色伤害命中之后
        AttackDataTargetHit = 41,   //角色收到伤害命中之后
        Kill = 50,   //角色击杀后
        PrepareFight = 60,  //准备战斗
        Init = 61,   //角色初始化
        AttackDataTargetCureBefore = 70, //角色受到伤害治疗之前
        AttackDataAttackerCureHit = 71,  //角色Damage治疗命中后
        AttackDataTargetCureHit = 72,    //角色受到Damage治疗命中后
        AttackDataAttackerCureBefore = 73, //角色伤害治疗计算之前
        AuraAdd = 80,  //光环添加角色时
        AuraRemove = 81,  //光环移除角色时
        AuraEnter = 82,   //角色受到光环时
        AuraExit = 83,  //角色离开光环时
        AuraHas = 84,   //角色有光环时
        AuraNone = 85,  //光环没有角色时
        AuraCreate = 86, //角色开启光环时
        AuraEnd = 87,   //角色关闭光环时
        Die = 90,    //角色死亡
        Dying = 91,  //角色即将死亡

        ShieldCreated = 100,
        TargetChange = 110,  //目标切换
        MpMax = 120,         //怒气满时
        OutControl = 130,  //失去控制
    }

    public class FightEffect
    {
        public Role role { get => box.attacker; }
        //寄宿对象
        public Role lodger { get => box.lodger; }

        public FightEffectBox box;

        public List<FightEffectAction> listAction;

        public float lastTime;

        public float cd { get => effectInfo.cd; }

        public List<Role> listTargets;

        public List<FightAttackData> listAttackData;

        public int[] arrValues;

        private int _index;

        private bool _needUpdating;

        public bool needUpdating
        {
            set
            {
                _needUpdating = value;
            }
            get
            {
                return _needUpdating || isChannelling;
            }
        }

        public bool isChannelling;

        public int triggerType { get => effectInfo.trigger; }

        public int triggerSpace { get => effectInfo.space; }

        public FightEffectInfo effectInfo;

        public FightSkillEffectData effectData;

        public int level;

        private bool _isRunning;

        public FightEffect(FightEffectBox box, FightEffectInfo effectInfo, FightSkillEffectData effectData, int level)
        {
            this.box = box;
            this.effectInfo = effectInfo;
            this.effectData = effectData;
            this.level = level;
            listTargets = new List<Role>();

            listAction = new List<FightEffectAction>();
            for (int i = 0; i < effectInfo.actionInfos.Length; i++)
            {
                listAction.Add(FightEffectAction.GetAction(effectInfo.actionInfos[i], effectData));
            }

            listAttackData = new List<FightAttackData>();
        }

        public void Break()
        {
            isChannelling = false;
            _isRunning = false;
        }

        internal void Update(float nowTime)
        {
            if (needUpdating)
            {
                if (_index < listAction.Count)
                    listAction[_index].Update(nowTime);
            }
        }

        public void AddAttackData(FightAttackData fightAttackData)
        {
            listAttackData.Add(fightAttackData);
        }

        private FightAttackDataBase _triggerData;

        public bool Trigger(TriggerType type, FightAttackDataBase triggerData = null)
        {
            if (_isRunning)
                return false;

            if (triggerType != (int)type)
                return false;

            return Execute(triggerData);
        }

        public void AttackExecute()
        {
            for (int i = 0; i < listAttackData.Count; i++)
            {
                box.AddAttackData(listAttackData[i]);
            }
            listAttackData.Clear();
        }

        public void Done()
        {
            lastTime = role.Time;
            _triggerData = null;

            AttackExecute();

            _isRunning = false;
        }

        public void Reset()
        {
            _index = -1;
            for (int i = 0; i < listAction.Count; i++)
            {
                listAction[i].Reset();
            }
            listAttackData.Clear();
            listTargets.Clear();
        }

        public bool DoNext()
        {
            _index++;

            if (_index >= listAction.Count)
            {
                Done();
                return true;
            }

            //UnityEngine.Debug.Log("@Execute Action " + listAction[_index].ToString());
            if (listAction[_index].Execute(this, _triggerData) == false)
            {
                _isRunning = false;
                return false;
            }

            if (needUpdating == false)
            {
                return DoNext();
            }
            else
            {
                AttackExecute();
            }

            return true;
        }

        public bool Execute(FightAttackDataBase triggerData = null)
        {
            Reset();
            _triggerData = triggerData;
            _isRunning = true;

            if (role.target != null)
            {
                listTargets.Add(role.target);
            }
            return DoNext();
        }

        //public void Update(float nowTime)
        //{
        //    if (isUpdating)
        //    {
        //        listAction[_index].Update(nowTime);
        //    }
        //}

        internal void ToStep(int step)
        {
            _index = step - 1;
        }

        public void ClearTargets()
        {
            listTargets.Clear();
        }

        public void AddTargets(List<Role> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AddTarget(list[i]);
            }
        }

        public void AddTarget(Role target)
        {
            if (listTargets.IndexOf(target) == -1)
            {
                listTargets.Add(target);
            }
        }

        public int[] GetTargetIds()
        {
            int[] targetIds = new int[listTargets.Count];
            for (int j = 0; j < listTargets.Count; j++)
            {
                targetIds[j] = listTargets[j].id;
            }
            return targetIds;
        }
    }
}