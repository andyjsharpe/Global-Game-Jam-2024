using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NormalDistribution
{
    public static float MakeNormal(float value, float standardDeviation, float mean)
    {
        var denominator = standardDeviation * Mathf.Sqrt(2 * Mathf.PI);
        var ratio = (value - mean) / standardDeviation;
        var final = Mathf.Exp(-Mathf.Pow(ratio, 2)/2) / denominator;
        return final;
    }

    public static float NormalizedRandom(float startInclusive, float endInclusive, int amount)
    {
        var value = Random.Range(startInclusive, endInclusive);
        //var mean = (endInclusive + startInclusive) / 2;
        //var standardDeviation = (endInclusive - startInclusive) / 3.0f;
        //return Mathf.Clamp(MakeNormal(value, standardDeviation, mean), startInclusive, endInclusive);
        return Mathf.Pow(value, 1 + 2 * amount);
    }
}
