using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightCalculation : MonoBehaviour
{
    private SimulationController sc;
    private float weightAttached = 0f;

    // Start is called before the first frame update
    void Start()
    {
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject Parent = collision.transform.gameObject;

        weightAttached = Parent.GetComponent<GameInteractables>().EstimateFillInML();
        Debug.Log(Parent.name + " menyentuhku dengan berat " + weightAttached);
    }

    private void OnCollisionExit(Collision collision)
    {
        weightAttached = 0f;
    }
}
