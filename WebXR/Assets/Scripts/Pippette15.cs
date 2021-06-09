using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pippette15 : GameInteractables
{
    private bool canFill = true, first = true;

    [SerializeField] private float fillInMililiter;

    public bool isFillable() { return canFill; }
    public void setFillable(bool cf) { canFill = cf; }

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
        rend = gameObject.transform.Find("MeshContainer/pipet_15_cm/Isi").GetComponent<Renderer>();

        maxFill = 0.15f;
        minFill = -0.15f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CallTriggerEnterFromChild(Collider other)
    {
        if (first)
        {
            first = false;
            canFill = false; 
            return ;
        }
        else
        {
            canFill = false;
            simulationController.onPouringWithPipetteInteractable(this, simulationController.GetClosestInteractable(this));
        }
    }

    public void CallTriggerExitFromChild(Collider other)
    {
        canFill = true;
        simulationController.onSuckingWithPipetteInteractable(this, simulationController.GetClosestInteractable(this));
    }

    public override void IncreaseFill(string type)
    {
        if (type.Equals("suck"))
        {
            rend.material.SetFloat("_PipetteFill", maxFill);
        }
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("suck"))
        {
            rend.material.SetFloat("_PipetteFill", minFill);
        }
        
    }

    public override float EstimateFillInML()
    {
        // Asumsi dia hanya bisa menghisap 10ml larutan
        return ((rend.material.GetFloat("_PipetteFill") - minFill) / (maxFill - minFill)) * 10;
    }
}
