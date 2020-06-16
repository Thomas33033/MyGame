using System.Collections.Generic;
using UnityEngine;

namespace Fight
{
    public class TagComponent : BaseComponent
    {
        public List<FightTagData> listTags;

        public TagComponent(Role role) : base(role)
        {
            listTags = new List<FightTagData>();
        }

        public void TagCreate(string v)
        {
            this.listTags = new List<FightTagData>();

            if (string.IsNullOrEmpty(v))
                return;

            string[] arr = v.Split(';');

            for (int i = 0; i < arr.Length; i++)
            {
                string[] t = arr[i].Split(',');
                if (t.Length > 1)
                {
                    listTags.Add(new FightTagData(t[0], int.Parse(t[1])));
                }
                else
                {
                    Debug.LogError("Error TagCreate 格式不匹配：" + v);
                }
            }
        }

        public bool TagHas(string name, int type = 0)
        {
            for (int i = 0; i < listTags.Count; i++)
            {
                if ((type == 0 || listTags[i].type == type) && listTags[i].name == name)
                {
                    return true;
                }
            }

            return false;
        }
    }
}