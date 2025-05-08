using System.Net.NetworkInformation;
using UnityEngine;

public static class Easing{
    public static float OutQuint(float t){
        return Mathf.Pow(1 - t, 5);
    }
    public static float InOutQuad(float t){
        return t < 0.5 ? 2 * t * t : 1 - Mathf.Pow(-2 * t + 2, 2) / 2;
    }
    public static float SinYoyo(float t){
        return Mathf.Sin(Mathf.PI*t);
    }
}