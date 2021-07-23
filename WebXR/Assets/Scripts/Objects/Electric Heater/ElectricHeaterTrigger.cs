using System.Collections;
using UnityEngine;

public class ElectricHeaterTrigger : GameInteractables
{
    protected override void Start()
    {
        base.Start();
        text.text = "";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponent<GameInteractables>() == null) return;
        GameInteractables interactableContained =  other.GetComponent<GameInteractables>();
        StartCoroutine("raiseTemperature", interactableContained);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.GetComponent<GameInteractables>() == null) return;
        StopCoroutine("raiseTemperature");
    }

    IEnumerator raiseTemperature(GameInteractables interactables)
    {
        while (true)
        {
            if (interactables.getTemperature() < 100f)
            {
                interactables.setTemperature(interactables.getTemperature() + 0.1f);
                StartCoroutine(simulationController.updateInteractableText(interactables));
            }

            // https://stackoverflow.com/questions/4774820/how-to-find-the-child-class-name-from-base-class
            if (interactables.GetType() == typeof(Erlenmeyer300) && interactables.getTemperature() >= 70)
            {
                Erlenmeyer300 erlenmeyer = interactables.GetComponent<Erlenmeyer300>();
                erlenmeyer.startLerpingDown();
            }

            yield return new WaitForSeconds(.05f);
        }
    }
}
