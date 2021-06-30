using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class GameInteractables : MonoBehaviour
{
    protected SimulationController simulationController;
    protected float weightContained = 0f;
    protected string particleContained = "";
    protected float temperature = 22;

    private void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // --------- HanSetter and Gretter ---------

    public float getWeightContained()
    {
        return weightContained;
    }

    public void setWeightContained(float weight)
    {
        weightContained = weight;
    }

    public string getParticleContained()
    {
        return particleContained;
    }

    public void setParticleContained(string particleName)
    {
        particleContained = particleName;
    }

    public float getTemperature()
    {
        return Mathf.Round(temperature * 100) / 100;
    }

    public void setTemperature(float temp)
    {
        temperature = temp;
    }

    // --------- Static classes ---------

    public static class LiquidColors
    {
        public const string NORMAL = "#2AB0AC";

        public const string KMNO4_100 = "#2E0043";
        public const string KMNO4_75 = "#2D2C5D";
        public const string KMNO4_50 = "#2C5878";
        public const string KMNO4_25 = "#2B8492";
    }

    public static class SurfaceColors
    {
        public const string NORMAL = "#2FD6D1";

        public const string KMNO4_100 = "#500074";
        public const string KMNO4_75 = "#48368B";
        public const string KMNO4_50 = "#406BA3";
        public const string KMNO4_25 = "#37A1BA";
    }

    // --------- Functions ---------
    public Color parseHexToColor(string hex)
    {
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
        {
            return color;
        }

        return Color.white;
    }
}
