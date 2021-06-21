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
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(transform.rotation.normalized.x + " ayy lmao " + transform.rotation.normalized.z);

        // Karena beaker hanya memiliki satu arah untuk menuangkan cairan dengan benar, batasi logika penuangan hanya pada
        // mulut beaker (Rotasi Z positif)
        // Karena berdasarkan observasi, sudut yang memungkinkan perpindahan cairan secara realistis adalah
        // (x atau z) < -54 dan (x atau z) > 52
        // Nilai normal pada sudut N derajat untuk Z+ adalah 0.4378937, dimana nilai normal memiliki nilai 0 - 1 dan 
        // merepresentasikan rotasi sebuah benda berdasarkan perpindahan pada putaran, tanpa memedulikan arah putaran.

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
            rend.material.SetFloat("_BeakerFill", rend.material.GetFloat("_BeakerFill") + liquidSpeed);
        }
        else if (type.Equals("suck"))
        {
            rend.material.SetFloat("_BeakerFill", rend.material.GetFloat("_BeakerFill") + 0.0005f);
        }

        
    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("pour"))
        {
            rend.material.SetFloat("_BeakerFill", rend.material.GetFloat("_BeakerFill") - liquidSpeed);
        }
        else if (type.Equals("suck"))
        {
            rend.material.SetFloat("_BeakerFill", rend.material.GetFloat("_BeakerFill") - 0.0005f);
        }
    }
}
