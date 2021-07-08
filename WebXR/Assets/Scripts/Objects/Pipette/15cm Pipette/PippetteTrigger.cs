using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PippetteTrigger : MonoBehaviour
{
    private Pippette15 pipette;

    private void Start()
    {
        pipette = gameObject.GetComponent<Pippette15>();
    }

    // Falloff
    public int count = 0;
    private void OnTriggerEnter(Collider other)
    {
        GameInteractables interactables = other.GetComponent<GameInteractables>();
        Debug.Log("Pipette: other.name => " + other.name + ", interactable isNull? " + (interactables == null));
        if (interactables == null) return;
        
        Debug.Log("Pipette: interactable => " + interactables.name);

        // Real function
        pipette.setTempParticle(interactables.getParticleContained() != null ? interactables.getParticleContained()[0].ToString() : "");
        pipette.setUsability(true);

        // Falloff func
        GamePourable pourable = other.GetComponent<GamePourable>();
        Debug.Log("Pipette: pourable => " + pourable.name);
        if (pourable != null)
        {
            pipette.setPourableOnContact(pourable);
            switch (count % 2)
            {
                case 0:
                    pipette.StopTriggerAction();
                    break;

                case 1:
                    pipette.StartTriggerAction();
                    break;
            }
            count++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        pipette.setPourableOnContact(null);
        // gameObject.GetComponentInParent<Pippette15>().setUsability(false);
    }
}
