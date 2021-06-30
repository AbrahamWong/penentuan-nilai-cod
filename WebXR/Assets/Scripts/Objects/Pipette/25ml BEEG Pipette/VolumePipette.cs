using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumePipette : GamePourable
{
    [SerializeField] private float fillInMililiter;
    private TextMeshPro fillValue;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
        rend = gameObject.transform.Find("MeshContainer/volume_pipette_25ml/Fill").GetComponent<Renderer>();

        maxFill = 0.1f;
        minFill = -0.2f;

        rend.material.SetFloat("_VolumePipetteFill", minFill);
        weightContained = EstimateFillInML();

        fillValue = transform.Find("MeshContainer/volume_pipette_25ml/Teks Volume/Volume").GetComponent<TextMeshPro>();
        fillValue.text = weightContained.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        weightContained = EstimateFillInML();
        fillInMililiter = EstimateFillInML();

        // https://answers.unity.com/questions/50391/how-to-round-a-float-to-2-dp.html
        fillValue.text = (Mathf.Round(weightContained * 100f) / 100f).ToString();
    }

    public void PourToVolumetric(GamePourable volumetric)
    {
        simulationController.onPouringWithBigPipette(this, volumetric);
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("bpip") && weightContained > 0f)
        {
            weightContained -= 0.01f;
            rend.material.SetFloat("_VolumePipetteFill", getFillMaterialPercentage());
        }
    }

    public override void IncreaseFill(string type)
    {
        if (type.Equals("bpip") && weightContained < 25f)
        {
            weightContained += 0.00075f;
            rend.material.SetFloat("_VolumePipetteFill", getFillMaterialPercentage());
        }
    }

    public override float EstimateFillInML()
    {
        // Asumsi dia hanya bisa menghisap 25ml larutan
        return (rend.material.GetFloat("_VolumePipetteFill") - minFill) / (maxFill - minFill) * 25;
    }

    public float getFillMaterialPercentage()
    {
        float fillPercentage = weightContained / 25;

        return fillPercentage * (maxFill - minFill) + minFill;
    }
}
