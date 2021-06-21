using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WeightCalculation : MonoBehaviour
{
    private SimulationController simulationController;
    private float weightAttached = 0f;
    private GameObject bigNumber, smallNumber;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        GameObject[] scaleTexts = GameObject.FindGameObjectsWithTag("ScaleText");
        foreach (var text in scaleTexts)
        {
            if (text.name == "Berat Diatas Koma")
                bigNumber = text;
            if (text.name == "Berat Dibawah Koma")
                smallNumber = text;
        }
    }

    // Update is called once per frame
    private GameObject objectAboveScale;
    private bool hasObjectAbove = false;
    void Update()
    {
        if (hasObjectAbove)
        {
            weightAttached = objectAboveScale.GetComponent<GameInteractables>().getWeightContained();
            PrintValue(weightAttached);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        objectAboveScale = collision.transform.gameObject;

        weightAttached = objectAboveScale.GetComponent<GameInteractables>().getWeightContained();
        PrintValue(weightAttached);
        hasObjectAbove = true;
    }

    private void OnCollisionExit(Collision collision)
    {
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

        bigNumber.GetComponent<TextMeshProUGUI>().SetText(bigger.ToString());
        smallNumber.GetComponent<TextMeshProUGUI>().SetText(smaller.ToString());
    }
}
