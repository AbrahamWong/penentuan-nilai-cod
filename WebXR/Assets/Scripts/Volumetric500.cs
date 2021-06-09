using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volumetric500 : GameInteractables
{
    // Fill dari beaker (500ml) memiliki rentang nilai 0.04 (500ml) s/d -0.06 (kosong)
    // Fill dari labu ukur (500ml) memiliki rentang nilai 0 (500ml) s/d -0.11 (kosong)
    [SerializeField] private float fillInMililiter;

    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        // https://answers.unity.com/questions/63317/access-a-child-from-the-parent-or-other-gameobject.html
        // https://answers.unity.com/questions/851056/how-can-i-find-object-childs-child-or-child-in-chi.html
        rend = gameObject.transform.Find("MeshContainer/volumetric_500/Filling").GetComponent<Renderer>();

        normalXAngle = 0.6959127;
        normalZAngle = 0.6959127;
        maxFill = 0f;
        minFill = -0.11f;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(transform.rotation.normalized.x + " ayy lmao " + transform.rotation.normalized.z);

        if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
        {
            if (rend.material.GetFloat("_VolumetricFill") <= minFill) return;
            simulationController.OnPouringInteractable(this, simulationController.GetClosestInteractable(this));
        }
        fillInMililiter = EstimateFillInML();
    }

    public override float EstimateFillInML()
    {
        return ((rend.material.GetFloat("_VolumetricFill") - minFill) / (maxFill - minFill)) * 500;
    }

    public override void IncreaseFill(string type)
    {
        if (type.Equals("pour"))
        {
            rend.material.SetFloat("_VolumetricFill", rend.material.GetFloat("_VolumetricFill") + 0.0001f);
        }
        else if (type.Equals("suck"))
        {
            rend.material.SetFloat("_VolumetricFill", rend.material.GetFloat("_VolumetricFill") + 0.0005f);
        }


    }

    public override void ReduceFill(string type)
    {
        if (type.Equals("pour"))
        {
            rend.material.SetFloat("_VolumetricFill", rend.material.GetFloat("_VolumetricFill") - 0.0001f);
        }
        else if (type.Equals("suck"))
        {
            rend.material.SetFloat("_VolumetricFill", rend.material.GetFloat("_VolumetricFill") - 0.0005f);
        }
    }
}
