using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPipetteTrigger : MonoBehaviour
{
    private bool insideTrigger = false;
    private string collideWith;
    private GamePourable volumetric;
    private VolumePipette volumePipette;

    // Start is called before the first frame update
    void Start()
    {
        volumePipette = gameObject.GetComponent<VolumePipette>();
    }

    // Update is called once per frame
    void Update()
    {
        if (insideTrigger)
        {
            switch (collideWith)
            {
                case "Sulfuric Acid Bottle":
                    gameObject.GetComponent<VolumePipette>().IncreaseFill("bpip");
                    break;

                case "Volumetric 500_S":
                    gameObject.GetComponent<VolumePipette>().PourToVolumetric(volumetric);
                    break;

                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string collideName = other.transform.name;
        if (collideName.Equals("Sulfuric Acid Bottle"))
        {
            insideTrigger = true;
            collideWith = collideName;

            ArrayList substance = new ArrayList();
            substance.Add("h2so4");
            volumePipette.setParticleContained(substance);
        }
        else if (collideName.Equals("Volumetric 500_S"))
        {
            insideTrigger = true;
            collideWith = collideName;
            volumetric = other.transform.GetComponent<Volumetric500>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string collideName = other.transform.name;
        if (collideName.Equals("Sulfuric Acid Bottle") || collideName.Equals("Volumetric 500_S"))
        {
            insideTrigger = false;
            collideWith = "";
        }
    }
}
