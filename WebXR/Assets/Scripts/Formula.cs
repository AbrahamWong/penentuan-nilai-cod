using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Formula
{
    static float BM = 158;
    static float equivalency = 5;
    static float coefficient = 1000;
    public static float CalculateNormal(float volumeA, float normalityA, float volumeB) => volumeA * normalityA / volumeB;

    public static float CalculateCOD(float volumeKMnO4, float normalityKMnO4, float volumeH2C2O4, float normalityH2C2O4)
        => ((10 + volumeKMnO4) * normalityKMnO4 - (volumeH2C2O4 * normalityH2C2O4)) * (BM / equivalency) * coefficient / 100;
}
