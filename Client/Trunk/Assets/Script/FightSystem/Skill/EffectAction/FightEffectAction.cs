using Boo.Lang;

namespace Fight
{

    public enum FightEffectActionType
    {
        Attack = 1,

        FindTarget = 2,

        AttackBouns = 3,

        Probability = 4,

        ToStep = 5,

        AttackCrit = 6,

        ShieldAdd = 7,

        AreaAttack = 8,

        Channelling = 9,

        AttackRandom = 10,

        FindTargetFromAttackData = 11,

        FindTargetFromEffect = 12,

        FindTargetShip = 13,

        FindTargetCrew = 14,

        FindTargetFilteCur = 15,

        ShareHurt = 20,

        AttackByBuffCount = 21,

        JumpBehindTarget = 22,

        AttackBounsByBuffCount = 23,

        AttackBounsByHp = 24,

        AttackBounsByDamageComHp = 25,

        FilterTargetSkill = 30,

        FilterTargetHpSort = 31,

        FilterTargetHpCompare = 32,

        FilterTargetDistanceSort = 33,

        FilterAttackHurt = 34,

        FilterTargetRandom = 35,

        FilterTag = 36,

        FilterTargetCol = 37,

        FilterRange = 38,

        FilterPositionZone = 39,

        BuffAdd = 40,

        BuffRemove = 41,

        BuffClear = 42,

        AttributeChange = 50,

        Status = 60,

        TimeWait = 70,

        AuraAdd = 80,

        AuraRemove = 81,

        SortTargetAttack = 90,

        SummonRole = 100,

        HpToAttack = 110,

        RunTimes = 120,

        ShieldBouns = 130,

        RoleAttack = 140,

        Kill = 150,

        DamageReflex = 160,

        DamageRefraction = 161,

        Bomb = 170,

        SkillCast = 180,

        SkillRandomCast = 181,

        SkillAdd = 182,

        Cumulative = 190,

        HpLock = 200,

        CheckTagNum = 210,

        CheckTagKindNum = 211,

        CheckTargetNum = 212,

        CheckColNum = 213,

        CheckZone = 214,

        CheckStatus = 215,

        CheckMp = 216,

        CheckDamageType = 217,

        FilterSameCol = 300,

        Or = 400,
    }

    public class FightEffectAction
    {
        //public int[] values { get => info.values; }

        public int[] keys;

        public FightEffectActionInfo info;

        public int actionType { get => info.id; }

        public FightSkillEffectData effectData;

        public FightEffectAction(FightEffectActionInfo info, FightSkillEffectData effectData)
        {
            this.info = info;
            this.effectData = effectData;
            keys = new int[] { };
            List<int> list = new List<int>();
            for (int i = 0; i < info.keys.Length; i++)
            {
                list.Add(int.Parse(info.keys[i]));
            }
            keys = list.ToArray();
        }

        public virtual void Reset()
        {
        }

        public virtual bool Execute(FightEffect fightEffect, FightAttackDataBase something)
        {
            return true;
        }

        public virtual void Update(float nowTime)
        {
        }

        public int GetValue(int index, int level)
        {
            if (level > 0 && info.valuesType[index] > 0)
            {
                switch (info.valuesType[index])
                {
                    case 1:
                        return effectData.value1[level - 1];

                    case 2:
                        return effectData.value2[level - 1];

                    case 3:
                        return effectData.value3[level - 1];
                }
            }
            return info.values[index];
        }

        public static FightEffectAction GetAction(FightEffectActionInfo actionInfo, FightSkillEffectData effectData)
        {
            switch (actionInfo.id)
            {
                case (int)FightEffectActionType.Attack:
                    return new FightEffectActionAttack(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackByBuffCount:
            //        return new FightEffectActionAttackByBuffCount(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackBouns:
            //        return new FightEffectActionAttackBouns(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackCrit:
            //        return new FightEffectActionAttackCrit(actionInfo, effectData);

                case (int)FightEffectActionType.FindTarget:
                    return new FightEffectActionFindTarget(actionInfo, effectData);

            //    case (int)FightEffectActionType.FindTargetFromAttackData:
            //        return new FightEffectActionFindTargetFromAttackData(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetSkill:
            //        return new FightEffectActionFilterTargetSkill(actionInfo, effectData);

            //    //case (int)FightEffectActionType.FilterTargetProfession:
            //    //    return new FightEffectActionFilterTargetProfession(actionInfo, effectData);

            //    case (int)FightEffectActionType.ShareHurt:
            //        return new FightEffectActionShareHurt(actionInfo, effectData);

            //    case (int)FightEffectActionType.FindTargetFromEffect:
            //        return new FightEffectActionFindTargetFromEffect(actionInfo, effectData);

            //    case (int)FightEffectActionType.Probability:
            //        return new FightEffectActionProbability(actionInfo, effectData);

            //    case (int)FightEffectActionType.ToStep:
            //        return new FightEffectActionToStep(actionInfo, effectData);

            //    case (int)FightEffectActionType.BuffAdd:
            //        return new FightEffectActionBuffAdd(actionInfo, effectData);

            //    case (int)FightEffectActionType.BuffRemove:
            //        return new FightEffectActionBuffRemove(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttributeChange:
            //        return new FightEffectActionAttributeChange(actionInfo, effectData);

            //    case (int)FightEffectActionType.ShieldAdd:
            //        return new FightEffectActionShieldAdd(actionInfo, effectData);

            //    case (int)FightEffectActionType.Status:
            //        return new FightEffectActionStatus(actionInfo, effectData);

            //    case (int)FightEffectActionType.AreaAttack:
            //        return new FightEffectActionAreaAttack(actionInfo, effectData);

            //    case (int)FightEffectActionType.TimeWait:
            //        return new FightEffectActionTimeWait(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetHpSort:
            //        return new FightEffectActionFilterTargetHpSort(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetHpCompare:
            //        return new FightEffectActionFilterTargetHpCompare(actionInfo, effectData);

            //    case (int)FightEffectActionType.Channelling:
            //        return new FightEffectActionChannelling(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackRandom:
            //        return new FightEffectActionAttackRandom(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetDistanceSort:
            //        return new FightEffectActionFilterTargetDistanceSort(actionInfo, effectData);

            //    case (int)FightEffectActionType.JumpBehindTarget:
            //        return new FightEffectActionJumpBehindTarget(actionInfo, effectData);

            //    case (int)FightEffectActionType.AuraAdd:
            //        return new FightEffectActionAuraAdd(actionInfo, effectData);

            //    case (int)FightEffectActionType.AuraRemove:
            //        return new FightEffectActionAuraRemove(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterAttackHurt:
            //        return new FightEffectActionFilterAttackHurt(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetRandom:
            //        return new FightEffectActionFilterTargetRandom(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTag:
            //        return new FightEffectActionFilterTag(actionInfo, effectData);

            //    case (int)FightEffectActionType.SortTargetAttack:
            //        return new FightEffectActionSortTargetAttack(actionInfo, effectData);

            //    case (int)FightEffectActionType.SummonRole:
            //        return new FightEffectActionSummonRole(actionInfo, effectData);

            //    case (int)FightEffectActionType.HpToAttack:
            //        return new FightEffectActionHpToAttack(actionInfo, effectData);

            //    case (int)FightEffectActionType.RunTimes:
            //        return new FightEffectActionRunTimes(actionInfo, effectData);

            //    case (int)FightEffectActionType.ShieldBouns:
            //        return new FightEffectActionShieldBouns(actionInfo, effectData);

            //    case (int)FightEffectActionType.RoleAttack:
            //        return new FightEffectActionRoleAttack(actionInfo, effectData);

            //    case (int)FightEffectActionType.Kill:
            //        return new FightEffectActionKill(actionInfo, effectData);

            //    case (int)FightEffectActionType.DamageReflex:
            //        return new FightEffectActionDamageReflex(actionInfo, effectData);

            //    case (int)FightEffectActionType.DamageRefraction:
            //        return new FightEffectActionDamageRefraction(actionInfo, effectData);

            //    case (int)FightEffectActionType.BuffClear:
            //        return new FightEffectActionBuffClear(actionInfo, effectData);

            //    case (int)FightEffectActionType.FindTargetShip:
            //        return new FightEffectActionFindTargetShip(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackBounsByBuffCount:
            //        return new FightEffectActionAttackBounsByBuffCount(actionInfo, effectData);

            //    case (int)FightEffectActionType.Bomb:
            //        return new FightEffectActionBomb(actionInfo, effectData);

            //    case (int)FightEffectActionType.SkillCast:
            //        return new FightEffectActionSkillCast(actionInfo, effectData);

            //    case (int)FightEffectActionType.SkillRandomCast:
            //        return new FightEffectActionSkillRandomCast(actionInfo, effectData);

            //    case (int)FightEffectActionType.Cumulative:
            //        return new FightEffectActionCumulative(actionInfo, effectData);

            //    case (int)FightEffectActionType.HpLock:
            //        return new FightEffectActionHpLock(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterTargetCol:
            //        return new FightEffectActionFilterTargetCol(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckTagNum:
            //        return new FightEffectActionCheckTagNum(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterRange:
            //        return new FightEffectActionFilterRange(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckZone:
            //        return new FightEffectActionCheckZone(actionInfo, effectData);

            //    case (int)FightEffectActionType.SkillAdd:
            //        return new FightEffectActionSkillAdd(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackBounsByHp:
            //        return new FightEffectActionAttackBounsByHp(actionInfo, effectData);

            //    case (int)FightEffectActionType.AttackBounsByDamageComHp:
            //        return new FightEffectActionAttackBounsByDamageComHp(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckTagKindNum:
            //        return new FightEffectActionCheckTagKindNum(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckTargetNum:
            //        return new FightEffectActionCheckTargetNum(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckColNum:
            //        return new FightEffectActionCheckColNum(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterSameCol:
            //        return new FightEffectActionFilterSameCol(actionInfo, effectData);

            //    case (int)FightEffectActionType.FilterPositionZone:
            //        return new FightEffectActionFilterPositionZone(actionInfo, effectData);

            //    case (int)FightEffectActionType.FindTargetFilteCur:
            //        return new FightEffectActionFindTargetFilteCur(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckStatus:
            //        return new FightEffectActionCheckStatus(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckMp:
            //        return new FightEffectActionCheckMp(actionInfo, effectData);

            //    case (int)FightEffectActionType.CheckDamageType:
            //        return new FightEffectActionCheckDamageType(actionInfo, effectData);

            //    case (int)FightEffectActionType.Or:
            //        return new FightEffectActionOr(actionInfo, effectData);
            }
            return null;
        }
    }
}