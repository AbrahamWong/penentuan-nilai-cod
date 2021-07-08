using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightCalculation : MonoBehaviour
{
    private SimulationController simulationController;
    private float weightAttached = 0f;
    private TextMeshPro bigText, smallText;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        GameObject[] scaleTexts = GameObject.FindGameObjectsWithTag("ScaleText");
        foreach (var text in scaleTexts)
        {
            if (text.name == "Berat Diatas Koma")
            {
                bigText = text.GetComponent<TextMeshPro>();
            }
            else if (text.name == "Berat Dibawah Koma")
            {
                smallText = text.GetComponent<TextMeshPro>();
            }
        }
    }

    // Update is called once per frame
    private GameObject objectAboveScale;
    private GameInteractables interactablesAboveScale;
    private bool hasObjectAbove = false;
    void Update()
    {
        if (hasObjectAbove)
        {
            weightAttached = interactablesAboveScale.getWeightContained();
            PrintValue(weightAttached);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        objectAboveScale = collision.transform.gameObject;
        if (objectAboveScale.GetComponent<GameInteractables>() == null) return;
        interactablesAboveScale = objectAboveScale.GetComponent<GameInteractables>();

        weightAttached = interactablesAboveScale.getWeightContained();
        PrintValue(weightAttached);
        hasObjectAbove = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        objectAboveScale = null;
        interactablesAboveScale = null;

        weightAttached = 0f;
        PrintValue(weightAttached);
        hasObjectAbove = false;
    }

    private void PrintValue(float val)
    {
        // https://docs.microsoft.com/en-us/dotnet/api/system.string.substring?view=net-5.0#System_String_Substring_System_Int32_
        string valString = val.ToString();

        int format = valString.IndexOf(".");
        string bigger, smaller;
        switch (format)
        {
            case -1:
                bigger = valString;
                smaller = "00";

                break;

            default:
                bigger = valString.Substring(0, format);
                smaller = valString.Substring(format + 1);

                break;
        }

        bigText.SetText(bigger.ToString());
        smallText.SetText(smaller.ToString());
    }
}
