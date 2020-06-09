using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseStat
{
    public List<StatBonus> IntChange { get; set; }
    public int BaseValue { get; set; }
    public string StatName { get; set; }
    public string StatDescription { get; set; }
    public int FinalValue { get; set; }

    public BaseStat(int baseValue, string statName, string statDescription)
    {
        this.IntChange = new List<StatBonus>();
        this.BaseValue = baseValue;
        this.StatName = statName;
        this.StatDescription = statDescription;
    }

    public void ApplyStatBonus(StatBonus statBonus)
    {
        this.IntChange.Add(statBonus);
    }
    public void RemoveStatBonus(StatBonus statBonus)
    {
        this.IntChange.Remove(statBonus);
    }
    public int GetFineValue()
    {
        this.IntChange.ForEach(x => this.FinalValue += x.BonusInt);
        FinalValue += BaseValue;
        return FinalValue;
    }
}
