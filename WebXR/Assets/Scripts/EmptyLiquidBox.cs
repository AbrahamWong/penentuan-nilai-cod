using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyLiquidBox : MonoBehaviour
{
    private GamePourable pourable = null;

    private void OnTriggerEnter(Collider other)
    {
        pourable = other.GetComponent<GamePourable>() == null ? pourable : other.GetComponent<GamePourable>();
        if (pourable != null)
        {
            pourable.getParticleContained().Clear();
            pourable.ReduceFill("empty");

            Debug.Log("EmptyLiquidBox: particle is " + pourable.getParticleInString() + " - with .Count = " + pourable.getParticleContained().Count);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.GetComponent<GamePourable>() == null) return;

        pourable = null;
    }
}
