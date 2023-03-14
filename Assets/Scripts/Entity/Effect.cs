using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect
{
    public enum EffectType
    {
        Poison, Burn, Paralysis, Regeneration, Rage, Protection, MagicalVision, Weakness, Vulnerability
    }
    private EffectType effectType;
    private int leftTurn;

    public EffectType _EffectType { get => effectType; set => effectType = value; }
    public int LeftTurn { get => leftTurn; set => leftTurn = value; }

    public string EffectName()
    {
        switch (effectType)
        {
            case EffectType.Poison:
                return "독";
            case EffectType.Burn:
                return "연소";
            case EffectType.Paralysis:
                return "마비";
            case EffectType.Regeneration:
                return "재생";
            case EffectType.Rage:
                return "분노";
            case EffectType.Protection:
                return "보호";
            case EffectType.MagicalVision:
                return "마법시야";
            case EffectType.Weakness:
                return "약화";
            case EffectType.Vulnerability:
                return "방어감소";
            default:
                return "?";
        }
    }
}
