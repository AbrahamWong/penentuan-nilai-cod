using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillBox : MonoBehaviour
{
    private SimulationController simulationController;
    private GamePourable pourable = null;
    private int timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        timer = 0;
        pourable = other.GetComponent<GamePourable>() == null ? pourable : other.GetComponent<GamePourable>();
        if (pourable != null) pourable.getParticleContained().Clear();
    }
    private void OnTriggerStay(Collider other)
    {
        if (pourable != null && (timer + 1) % 5 == 0)
        {
            simulationController.RefillPourable(pourable);
        }

        timer++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.GetComponent<GamePourable>() == null) return;

        pourable = null;
    }
}
