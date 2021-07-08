using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumePipette : GamePourable
{
    [SerializeField] private float fillInMililiter;
    private TextMeshPro fillValue;

    // Start is called before the first frame update
    protected override void Start()
    {
        rend = gameObject.transform.Find("MeshContainer/volume_pipette_25ml/Fill").GetComponent<Renderer>();

        maxFill = 0.1f;
        minFill = -0.2f;
        capacity = 25;      // 25ml

        base.Start(); 
        rend.material.SetFloat(rendererFillReference, minFill);
        weightContained = EstimateFillInML();

        fillValue = transform.Find("MeshContainer/volume_pipette_25ml/Teks Volume/Volume").GetComponent<TextMeshPro>();
        fillValue.text = weightContained.ToString();

        text.text = "";
    }

    // Update is called once per frame
    private void Update()
    {
        weightContained = EstimateFillInML();
        fillInMililiter = EstimateFillInML();

        // https://answers.unity.com/questions/50391/how-to-round-a-float-to-2-dp.html
        fillValue.text = (Mathf.Round(weightContained * 100f) / 100f).ToString();
    }

    public void PourToVolumetric(GamePourable volumetric)
    {
        if (weightContained < 0.01f) return;

        if (volumetric.GetType() == typeof(Volumetric500)) volumetric.GetComponent<Volumetric500>().increaseCapacity();
        simulationController.onPouringWithBigPipette(this, volumetric);
    }
}