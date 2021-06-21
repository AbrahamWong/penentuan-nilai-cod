using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunnelTrigger : MonoBehaviour
{
    private SimulationController simulationController;
    private Funnel funnel;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        // topkek
        funnel = transform.parent.parent.parent.gameObject.GetComponent<Funnel>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        funnel.getFunnelTrigger(other);
    }
}
