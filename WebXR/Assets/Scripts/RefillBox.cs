using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefillBox : MonoBehaviour
{
    private SimulationController simulationController;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        GamePourable pourable = simulationController.GetClosestPourables(transform);
        if (pourable != null)
        {
            simulationController.RefillPourable(pourable);
        }
    }
}
