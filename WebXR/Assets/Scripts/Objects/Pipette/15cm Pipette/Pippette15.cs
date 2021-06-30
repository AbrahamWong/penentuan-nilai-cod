using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pippette15 : GamePourable
{
    [SerializeField] private float fillInMililiter;
    [SerializeField] private bool hasFill = false;
    
    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
        rend = gameObject.transform.Find("MeshContainer/pipet_15_cm/Isi").GetComponent<Renderer>();

        maxFill = 0.15f;
        minFill = -0.15f;

        rend.material.SetFloat("_PipetteFill", minFill);
        weightContained = EstimateFillInML();
        if (weightContained > 0) hasFill = true;
    }

    private void Update()
    {
        weightContained = EstimateFillInML();
        fillInMililiter = EstimateFillInML();
    }

    public void CallTriggerEnterFromChild(Collider other)
    {
        if (hasFill)
        {
            simulationController.onPouringWithPipette(this, simulationController.GetClosestPourables(this));
        }
        else
        {
            simulationController.onSuckingWithPipette(this, simulationController.GetClosestPourables(this));
        }
    }

    public override void IncreaseFill(string type)
    {
        if (type.Equals("suck"))
        {
            rend.material.SetFloat("_PipetteFill", maxFill);
            hasFill = true;
        }
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("suck"))
        {
            rend.material.SetFloat("_PipetteFill", minFill);
            hasFill = false;
        }
        
    }

    public override float EstimateFillInML()
    {
        // Asumsi dia hanya bisa menghisap 10ml larutan
        return (rend.material.GetFloat("_PipetteFill") - minFill) / (maxFill - minFill) * 10;
    }
}
