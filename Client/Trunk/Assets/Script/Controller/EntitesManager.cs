using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Fight;
public class EntitesManager : Singleton<EntitesManager>
{

    //等待销毁列表
    public List<Tower> waitDeleteTowerList = new List<Tower>();
    public List<Monster> waitDeleteMonsterList = new List<Monster>();

    
    //存储游戏中所有的角色 每个角色均有一个唯一ID
    public Dictionary<int, CharacterBase> EntityMap = new Dictionary<int, CharacterBase>();
    public Dictionary<EEntityType, List<CharacterBase>> EntitylistMap = new Dictionary<EEntityType, List<CharacterBase>>();


    public List<CharacterBase> GetList(EEntityType type)
    {
        if (EntitylistMap.ContainsKey(type))
        {
            return EntitylistMap[type];
        }
        return null;
    }

    private List<EEntityType> keyList = new List<EEntityType>();
    public List<EEntityType> GetTypeList()
    {
        keyList.Clear();
        foreach (var key in this.EntitylistMap.Keys)
        {
            keyList.Add(key);
        }
        return keyList;
    }


    private void AddCharactor(CharacterBase entity)
    {
    
        if (!this.EntityMap.ContainsKey(entity.uid))
        {
            this.EntityMap[entity.uid] = entity;
            EEntityType type = entity.GetEntityType();
            if (!this.EntitylistMap.ContainsKey(type))
            {
                this.EntitylistMap[type] = new List<CharacterBase>();
            }
            this.EntitylistMap[type].Add(entity);
        }
        else
        {
            Debug.LogError("警告: 系统创建角色时ID重复");
        }
    }

    public Tower CreateTower(uint configId)
    {
        Tower tower = new Tower();
        NpcData _data = new NpcData();
        _data.InitData(configId);
        tower.OnInit(_data);
        this.AddCharactor(tower);
        return tower;
    }

    public CharacterBase CreateCharactor(EEntityType type, CharacterData data)
    {
        CharacterBase entity = null;
        switch (type)
        { 
            case EEntityType.Monster:
                entity = new Monster();
                break;
            case EEntityType.Tower:
                 entity = new Tower();
                break;
            case EEntityType.Npc:

                break;
            case EEntityType.Player:

                break;
        }
        if (entity != null)
        {
            entity.OnInit(data);
            this.AddCharactor(entity);
        }

        return entity;
    }


    public Tower CreateTower(NpcData _data)
    {
        CharacterBase entity = this.CreateCharactor(EEntityType.Tower, _data);
        if(entity != null)
        {
            return entity as Tower;
        }
        else
        {
            Debug.LogError("创建失败");
        }
        return  null;
    }

    public Monster CreateMonster(s_MonsterData _data)
    {
        CharacterBase entity = this.CreateCharactor(EEntityType.Monster, _data);
        if (entity != null)
        {
            return entity as Monster;
        }
        else
        {
            Debug.LogError("创建失败");
        }
        return null;
    }

    public void RemoveCharactor(int uid)
    {
        CharacterBase entity = this.EntityMap[uid];
        if (entity != null)
        {
            this.EntityMap.Remove(uid);
            List<CharacterBase> list = this.EntitylistMap[entity.GetEntityType()];
            if (list != null)
            {
                list.Remove(entity);
            }
            
            if (entity.GetEntityType() == EEntityType.Monster)
            { 
                this.waitDeleteMonsterList.Add(entity as Monster);
            }
            else if (entity.GetEntityType() == EEntityType.Tower)
            {
                this.waitDeleteTowerList.Add(entity as Tower);
            }
            //......


        }
       

    }

    public CharacterBase GetEntity(int uId)
    {
        if (EntityMap.ContainsKey(uId))
        {
            return EntityMap[uId];
        }
        else
        {
            string str = string.Format("can't find eid {0} int entityMap", uId);
            Debug.LogError(str);
            return null;
        }

    }

    public CharacterBase GetTower(GameObject go)
    {
        foreach (var v in this.EntityMap)
        {
            if (v.Value != null 
                && v.Value.GetEntityType() == EEntityType.Tower 
                && v.Value.ModelObj == go)
            {
                return v.Value;
            }
        }
        return null;
    }

    public CharacterBase GetMonster(GameObject go)
    {
        foreach (var v in this.EntityMap)
        {
            if (v.Value != null
                && v.Value.GetEntityType() == EEntityType.Monster
                && (v.Value.ModelObj == go))
            {
                return v.Value;
            }
        }
        return null;
    }


    public void OnUpdate(float dt)
    {
        foreach (var v in this.EntityMap)
        {
            if (v.Value != null )
            {
                if (v.Value.canDelete)
                {
                    this.RemoveCharactor(v.Value.uid);
                }
                else
                {
                    v.Value.OnUpdate(dt);
                }
            }
        }

        if (this.waitDeleteMonsterList.Count > 0)
        {
            for (int i = 0; i < this.waitDeleteMonsterList.Count; i++)
            {
                this.waitDeleteMonsterList[i].OnDestroy();
            }
            //通知处理器 该怪物已经被销毁
            this.waitDeleteMonsterList.Clear();
        }

        if (this.waitDeleteTowerList.Count > 0)
        {
            for (int i = 0; i < this.waitDeleteTowerList.Count; i++)
            {
                this.waitDeleteTowerList[i].OnDestroy();
            }
            //通知处理器 该塔已经被销毁
            this.waitDeleteTowerList.Clear();
        }

    }




    public void DeleteMonster(Monster monster)
    {
        if (monster != null)
        {
            if (!monster.IsLive())
            {
                this.waitDeleteMonsterList.Add(monster);
            }
            else
            {
                Debug.LogError("Monster 没有死亡不能被删除");
            }
           
        }
    }

    public void DeleteTower(Tower tower)
    {
        if (tower != null )
        {
            if (!tower.IsLive())
            {
                this.waitDeleteTowerList.Add(tower);
            }
            else
            {
                Debug.LogError("Monster 没有死亡不能被删除");
            }
            
        }
    }


}
