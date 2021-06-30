using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaker500 : GamePourable
{
    // Fill dari beaker (500ml) memiliki rentang nilai 0.04 (500ml) s/d -0.06 (kosong)
    // Fill dari labu ukur (500ml) memiliki rentang nilai 0 (500ml) s/d -0.11 (kosong)
    [SerializeField] private float fillInMililiter;

    void Start()
    {
        // Memanggil komponen dari objek lain
        // https://forum.unity.com/threads/how-can-i-reference-to-a-component-of-another-gameobject.280451/
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        rend = gameObject.transform.Find("MeshContainer/beaker_500/Beaker Filling").GetComponent<Renderer>();

        normalZAngle = 0.4378937;
        maxFill = 0.04f;
        minFill = -0.06f;

        weightContained = EstimateFillInML();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.rotation.normalized.z > normalZAngle)
        {
            // Jika kosong, abaikan kemiringan, karena isi beaker habis.
            if (rend.material.GetFloat("_BeakerFill") <= minFill) return ;

            // Referensikan gameObject dari sebuah objek secara langsung: 
            // https://answers.unity.com/questions/36109/get-the-gameobject-that-is-connected-to-the-script.html
            simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(this));
            
        }

        fillInMililiter = EstimateFillInML();
    }

    public override float EstimateFillInML()
    {
        return ((rend.material.GetFloat("_BeakerFill") - minFill) / (maxFill - minFill)) * 500;
    }

    public override void IncreaseFill(string type)
    {
        if (type.Equals("pour"))
        {
            weightContained += 50;
            rend.material.SetFloat("_BeakerFill", getFillMaterialPercentage());
        }
        else if (type.Equals("suck"))
        {
            weightContained += 10;
            rend.material.SetFloat("_BeakerFill", getFillMaterialPercentage());
        }
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("pour"))
        {
            weightContained -= 50;
            rend.material.SetFloat("_BeakerFill", getFillMaterialPercentage());
        }
        else if (type.Equals("suck"))
        {
            weightContained -= 10;
            rend.material.SetFloat("_BeakerFill", getFillMaterialPercentage());
        }
    }

    public float getFillMaterialPercentage()
    {
        float fillPercentage = weightContained / 500;

        return fillPercentage * (maxFill - minFill) + minFill;
    }
}
