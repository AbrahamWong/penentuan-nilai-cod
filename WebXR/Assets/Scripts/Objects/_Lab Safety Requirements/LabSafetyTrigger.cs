using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabSafetyTrigger : GameInteractables
{
    public GameObject safetyObject;

    protected override void Start()
    {
        base.Start();
        text.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cek untuk trigger agar hanya dilakukan oleh TrackedAlias
        if (other.name.Equals("ExampleAvatar"))
        {
            switch (safetyObject.name)
            {
                case "Item Lab Coat":
                    simulationController.usingLabCoat = true;
                    Destroy(gameObject);
                    break;

                case "Item Gloves":
                    simulationController.usingGloves = true;
                    Destroy(gameObject);
                    break;

                case "Item Respirator":
                    simulationController.usingRespirator = true;
                    Destroy(gameObject);
                    break;

                case "Item Safety Glass":
                    simulationController.usingGlasses = true;
                    Destroy(gameObject);
                    break;

                default:
                    break;
            }
        }
    }
}
