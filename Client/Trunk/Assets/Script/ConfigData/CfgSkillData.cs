using System;
using System.Collections.Generic;

public class CfgSkillData : configBase
{
    public int UId { get; set; }
    public int BaseId { get; set; }
    public int Level { get; set; }
    public string Name { get; set; }
    public string ResName { get; set; }
    public string Icon { get; set; }
    public int BuildType { get; set; }
    public string Pdamage_percent { get; set; }
    public string Pdamage_value { get; set; }
    public string Mdamage_percent { get; set; }
    public string Mdamage_value { get; set; }
    public int Targetnum { get; set; }
    public int Colddown { get; set; }
    public int Attackrange { get; set; }
    public int TargetType { get; set; }
    public string Damagetarget { get; set; }
    public string Trap { get; set; }
    public int Flag { get; set; }
    public string Addbuff { get; set; }
    public string Starbuff { get; set; }
    public string Pasvbuff { get; set; }
    public string Upgradecost { get; set; }
    public int Order { get; set; }
    public string SoundName { get; set; }
    public float SoundStartTime { get; set; }
}
