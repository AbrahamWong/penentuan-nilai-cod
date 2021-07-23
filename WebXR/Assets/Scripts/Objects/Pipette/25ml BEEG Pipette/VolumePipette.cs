using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VolumePipette : GamePourable
{
    private bool insideTrigger = false, triggerPulled = false;
    private string collideWith;
    private GamePourable volumetric;
    private TextMeshPro fillValue;

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
        // https://answers.unity.com/questions/50391/how-to-round-a-float-to-2-dp.html
        fillValue.text = (Mathf.Round(weightContained * 100f) / 100f).ToString();

        if (insideTrigger)
        {
            switch (collideWith)
            {
                case "Sulfuric Acid Bottle":
                    IncreaseFill(triggerPulled ? "bpit" : "bpip");
                    Debug.Log("VolumePipette: triggerPulled " + triggerPulled);
                    break;

                case "Volumetric 500_S":
                    PourToVolumetric(volumetric);
                    break;

                default:
                    break;
            }
        }
    }
    private new void OnTriggerEnter(Collider other)
    {
        string collideName = other.transform.name;
        if (collideName.Equals("Sulfuric Acid Bottle"))
        {
            insideTrigger = true;
            collideWith = collideName;

            ArrayList substance = new ArrayList();
            substance.Add("h2so4");
            setParticleContained(substance);
        }
        else if (collideName.Equals("Volumetric 500_S"))
        {
            insideTrigger = true;
            collideWith = collideName;
            volumetric = other.transform.GetComponent<Volumetric500>();
        }
    }

    private new void OnTriggerExit(Collider other)
    {
        insideTrigger = false;
        collideWith = "";
    }

    public void PourToVolumetric(GamePourable volumetric)
    {
        if (weightContained < 0.025f || volumetric.GetType() != typeof(Volumetric500)) return;

        volumetric.GetComponent<Volumetric500>().increaseCapacity();
        simulationController.onPouringWithBigPipette(this, volumetric, false);
    }

    public override void StartTriggerAction()
    {
        base.StartTriggerAction();
        if (!collideWith.Equals("Sulfuric Acid Bottle")) return;

        triggerPulled = true;
    }

    public override void StopTriggerAction()
    {
        base.StopTriggerAction();
        triggerPulled = false;
    }
}