using UnityEngine;
using System.Collections;

namespace Fight
{
    //战斗标签
    public class FightTagData
    {
        public string name;

        public int type;

        public FightTagData(string v1, int v2)
        {
            this.name = v1;
            this.type = v2;
        }
    }
}