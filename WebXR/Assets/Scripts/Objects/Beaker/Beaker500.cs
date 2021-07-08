using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaker500 : GamePourable
{
    // Fill dari beaker (500ml) memiliki rentang nilai 0.04 (500ml) s/d -0.06 (kosong)
    // Fill dari labu ukur (500ml) memiliki rentang nilai 0 (500ml) s/d -0.11 (kosong)
    [SerializeField] private float fillInMililiter;

    protected override void Start()
    {
        rend = gameObject.transform.Find("MeshContainer/beaker_500/Beaker Filling").GetComponent<Renderer>();

        normalZAngle = 0.4378937;
        maxFill = 0.04f;
        minFill = -0.06f;
        capacity = 500; // 500ml

        base.Start();
        // weightContained = EstimateFillInML();
        weightContained = 0;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.rotation.normalized.z > normalZAngle || transform.rotation.eulerAngles.z > 53)
        {
            Debug.Log(transform.rotation.normalized);
            // Jika kosong, abaikan kemiringan, karena isi beaker habis.
            if (rend.material.GetFloat(rendererFillReference) <= minFill) return ;

            // Referensikan gameObject dari sebuah objek secara langsung: 
            // https://answers.unity.com/questions/36109/get-the-gameobject-that-is-connected-to-the-script.html
            simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(transform));
            
        }

        fillInMililiter = EstimateFillInML();
    }
}
