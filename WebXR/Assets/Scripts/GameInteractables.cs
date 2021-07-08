using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

abstract public class GameInteractables : MonoBehaviour
{
    protected SimulationController simulationController;
    [SerializeField] protected float weightContained = 0f;
    [SerializeField] protected ArrayList particleContained;
    [SerializeField] protected float temperature = 22;

    protected TextMeshPro text;

    public TextMeshPro getInteractableText() => text;
    public void setInteractableText(string text) => this.text.text = text;

    // https://stackoverflow.com/questions/53076669/how-to-correctly-inherit-unitys-callback-functions-like-awake-start-and-up
    // --------- Virtual Function ---------
    protected virtual void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        GameObject textObject = new GameObject();
        textObject.transform.SetParent(transform);
        textObject.name = "Teks Sampel";
        textObject.transform.localPosition = new Vector3(0, 0.1f, 0);
        textObject.transform.eulerAngles = Vector3.zero;

        text = textObject.AddComponent<TextMeshPro>();
        text.fontSize = 0.2f;
        text.alignment = TextAlignmentOptions.Center;

        // https://forum.unity.com/threads/modify-the-width-and-height-of-recttransform.270993/
        RectTransform rt = textObject.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0.25f, 0.0625f);
        rt.localRotation = Quaternion.Euler(0, 0, 0);

        text.text = "Massa ditampung = " + (Mathf.Round(weightContained * 100) / 100).ToString() +
            "\nTemperatur = " + temperature.ToString();

        particleContained = new ArrayList
        {
            ""
        };
    }

    public virtual void StartTriggerAction()
    {
    }

    public virtual void StopTriggerAction()
    {
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

    public ArrayList getParticleContained()
    {
        return particleContained;
    }

    public string getParticleInString()
    {
        string particle = "";

        foreach (string p in particleContained)
        {
            particle = particle + " " + p;
        }

        return particle;
    }

    public void setParticleContained(ArrayList particleName)
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
        public const string KMNO4_50 = "#2C5878";
    }

    public static class SurfaceColors
    {
        public const string NORMAL = "#2FD6D1";

        public const string KMNO4_100 = "#500074";
        public const string KMNO4_50 = "#406BA3";
    }

    public static class LiquidTransferSpeed
    {
        public const float pouringSpeed = 0.5f;
        public const float pipetteSpeed = 5;
        public const float bigPipetteSpeed = 0.1f;
        public const float buretteSpeed = 0.005f;
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
