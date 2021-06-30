using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigPipetteTrigger : MonoBehaviour
{
    private bool insideTrigger = false;
    private string collideWith;
    private GamePourable volumetric;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (insideTrigger)
        {
            switch (collideWith)
            {
                case "bottle_sulfuric_acid":
                    gameObject.GetComponentInParent<VolumePipette>().IncreaseFill("bpip");
                    break;

                case "volumetric_500":
                    gameObject.GetComponentInParent<VolumePipette>().PourToVolumetric(volumetric);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        string collideName = other.transform.GetChild(0).name;
        if (collideName.Equals("bottle_sulfuric_acid"))
        {
            insideTrigger = true;
            collideWith = collideName;
        }
        else if (collideName.Equals("volumetric_500"))
        {
            insideTrigger = true;
            collideWith = collideName;
            volumetric = other.transform.parent.GetComponent<Volumetric500>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        string collideName = other.transform.GetChild(0).name;
        if (collideName.Equals("bottle_sulfuric_acid") || collideName.Equals("volumetric_500"))
        {
            insideTrigger = false;
            collideWith = "";
        }
    }
}
