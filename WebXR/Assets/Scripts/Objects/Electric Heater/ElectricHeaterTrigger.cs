using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElectricHeaterTrigger : GameInteractables
{
    [SerializeField] private string interactableContainedName;
    private GameInteractables interactableContained;

    // Start is called before the first frame update
    void Start()
    {
        interactableContainedName = particleContained;
    }

    // Update is called once per frame
    void Update()
    {
        if (interactableContained != null)
        {
            interactableContained.setTemperature(interactableContained.getTemperature() + 0.01f);
            interactableContained.GetComponent<TextMeshPro>().SetText(interactableContained.getTemperature().ToString());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        interactableContained =  other.GetComponentInParent<GameInteractables>();
        interactableContainedName = interactableContained.name;

        interactableContained.gameObject.AddComponent<TextMeshPro>();
        TextMeshPro tmp = interactableContained.GetComponent<TextMeshPro>();
        tmp.fontSize = 0.5f;
        tmp.alignment = TextAlignmentOptions.Center;
    }

    private void OnTriggerExit(Collider other)
    {
        interactableContainedName = "";

        Destroy(interactableContained.GetComponent<TextMeshPro>());
        interactableContained = null;
    }
}
