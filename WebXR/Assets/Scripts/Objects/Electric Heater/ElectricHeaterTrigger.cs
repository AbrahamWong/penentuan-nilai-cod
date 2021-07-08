using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ElectricHeaterTrigger : GameInteractables
{
    [SerializeField] private string interactableContainedName;
    private GameObject textObject;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        interactableContainedName = particleContained.ToString();

        text.text = "";
    }

    // Update is called once per frame
    // Not implementing function from GameInteractable.Update(), use new;
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<GameInteractables>() == null) return;

        GameInteractables interactableContained =  other.GetComponent<GameInteractables>();
        interactableContainedName = interactableContained.name;

        StartCoroutine("raiseTemperature", interactableContained);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<GameInteractables>() == null) return;

        interactableContainedName = "";

        StopCoroutine("raiseTemperature");
        Debug.Log("Electric Heater: Coroutine Ended;");
        // Destroy(textObject);
    }

    IEnumerator raiseTemperature(GameInteractables interactables)
    {
        while (true)
        {
            interactables.setTemperature(interactables.getTemperature() + 0.1f);
            // tmp.SetText(
            //     "Massa ditampung = " + (Mathf.Round(interactables.getWeightContained() * 100) / 100).ToString() + 
            //     "\nTemperatur = " + interactables.getTemperature().ToString());

            // https://stackoverflow.com/questions/4774820/how-to-find-the-child-class-name-from-base-class
            if (interactables.GetType() == typeof(Erlenmeyer300) && interactables.getTemperature() >= 70)
            {
                Erlenmeyer300 erlenmeyer = interactables.GetComponent<Erlenmeyer300>();
                erlenmeyer.startLerpingDown();
            }

            if (interactables.GetComponent<GamePourable>() != null)
            {
                Debug.Log("Electric Heater: name => " + interactables.GetComponent<GamePourable>().name);
                Debug.Log("Electric Heater: type => " + interactables.GetType());
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
}
