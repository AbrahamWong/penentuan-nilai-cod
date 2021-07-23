using UnityEngine;

public class LabSafetyTrigger : GameInteractables
{
    public Material gloveMaterial;

    protected override void Start()
    {
        base.Start();
        text.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.transform.parent.CompareTag("VRController")) return;
        switch (gameObject.name)
        {
            case "Item Lab Coat":
                simulationController.usingLabCoat = true;
                gameObject.SetActive(false);
                break;

            case "Item Gloves":
                simulationController.usingGloves = true;
                simulationController.getLeftController().transform.Find("model").GetComponent<Renderer>().material = gloveMaterial;
                simulationController.getRightController().transform.Find("model").GetComponent<Renderer>().material = gloveMaterial;
                gameObject.SetActive(false);
                break;

            case "Item Respirator":
                simulationController.usingRespirator = true;
                gameObject.SetActive(false);
                break;

            case "Item Safety Glass":
                simulationController.usingGlasses = true;
                gameObject.SetActive(false);
                break;

            default:
                break;
        }
    }
}
