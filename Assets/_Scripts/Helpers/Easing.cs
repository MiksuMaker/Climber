using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Easing
{
    public enum Type
    {
        inOutBack, inOutExpo, inOutBackExpoHybrid, inExpo, outQuart, easeInCirc,
    }

    static public float EaseType(float x, Type type)
    {
        switch (type)
        {
            case Type.inOutBack: return EaseInOutBack(x);
            case Type.inOutExpo: return EaseInOutExpo(x);
            case Type.inOutBackExpoHybrid: return EaseInOutBackExpoHybrid(x);
            case Type.inExpo: return EaseInExpo(x);
            case Type.outQuart: return EaseOutQuart(x);
            case Type.easeInCirc: return EaseInCirc(x);
            default: return 0;
        }
    }

    static public float EaseInOutBack(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? ((2 * x) * (2 * x) * ((c2 + 1) * 2 * x - c2)) / 2
          : ((2 * x - 2) * (2 * x - 2) * ((c2 + 1) * (x * 2 - 2) + c2) + 2) / 2;
    }

    static public float EaseInOutExpo(float x)
    {
        return x == 0 ? 0 :
                            x == 1 ? 1 : x < 0.5 ? Mathf.Pow(2, 20 * x - 10) / 2 : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
    }

    static public float EaseInOutBackExpoHybrid(float x)
    {
        float c1 = 1.70158f;
        float c2 = c1 * 1.525f;

        return x < 0.5
          ? ((2 * x) * (2 * x) * ((c2 + 1) * 2 * x - c2)) / 2
          // Second half is from Expo
          : (2 - Mathf.Pow(2, -20 * x + 10)) / 2;
    }

    static public float EaseInExpo(float x)
    {
        return x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);
    }

    static public float EaseOutQuart(float x)
    {
        return 1 - ((1 - x) * (1 - x) * (1 - x) * (1 - x));
    }

    static public float EaseInCirc(float x)
    {
        return 1 - Mathf.Sqrt(1 - (x * x));
    }
}
