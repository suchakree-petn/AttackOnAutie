using Sirenix.OdinInspector;
using UnityEngine;

public static class NumberExtension
{
    public static float ToPercentage(this float value, float max)
    {
        return value / max;
    }

    public static float ToPercentage(this int value, int max)
    {
        return (float)value / max;
    }

    public static bool Compare(float a, Equator equator, float b)
    {
        return equator switch
        {
            Equator.More => a > b,
            Equator.Less => a < b,
            Equator.MoreEqual => a >= b,
            Equator.LessEqual => a <= b,
            _ => false,
        };
    }

    public static int CeilToInt(this float a)
    {
        return Mathf.CeilToInt(a);
    }

    
}

public enum Equator
{
  [LabelText(SdfIconType.ChevronRight)] More,
  [LabelText(SdfIconType.ChevronLeft)] Less,
  [LabelText(SdfIconType.ChevronDoubleRight)] MoreEqual,
  [LabelText(SdfIconType.ChevronDoubleLeft)] LessEqual
}

public enum CompareType{
    And,
    Or
}
