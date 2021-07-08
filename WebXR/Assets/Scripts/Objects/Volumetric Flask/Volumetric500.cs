using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Volumetric500 : GamePourable
{
    // Fill dari beaker (500ml) memiliki rentang nilai 0.04 (500ml) s/d -0.06 (kosong)
    // Fill dari labu ukur (500ml) memiliki rentang nilai 0 (500ml) s/d -0.11 (kosong)
    [SerializeField] private float fillInMililiter;
    private Coroutine co;

    protected override void Start()
    {
        // https://answers.unity.com/questions/63317/access-a-child-from-the-parent-or-other-gameobject.html
        // https://answers.unity.com/questions/851056/how-can-i-find-object-childs-child-or-child-in-chi.html
        rend = gameObject.transform.Find("MeshContainer/volumetric_500/Filling").GetComponent<Renderer>();

        normalXAngle = 0.6959127;
        normalZAngle = 0.6959127;
        maxFill = 0f;
        minFill = -0.11f;

        weightContained = 0;    // 0ml => test 100ml
        capacity = 500;         // 500ml

        base.Start();
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        
        co = StartCoroutine(coroutineCheckPreRequisites());
        cheatCode();
    }

    private void cheatCode()
    {
        if(!name.Contains("CHEAT")) return;
        weightContained = 500;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        isFull = true;
        particleContained.Clear();
        Debug.Log("CHEAT: " + GetType().ToString());
    
        switch (name)
        {
            case "CHEAT1":
                particleContained.Add("h2c2o4");
                break;
    
            case "CHEAT2":
                particleContained.Add("kmno4");
                break;
            
            case "CHEAT3":
                weightContained = 611.11f;
                rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
                capacity = 611.11f;
                particleContained.Add("h2so4");
                break;
            
            default:
                break;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
        {
            // Find nearest object with burette
            GameInteractables interactableNearby = simulationController.GetClosestInteractable(transform);
            if (rend.material.GetFloat(rendererFillReference) <= minFill || interactableNearby == null) return;

            else if (interactableNearby.GetType() == typeof(Funnel))
                simulationController.OnPouringInteractable(this, 
                    interactableNearby.GetComponent<Funnel>().getAttachedIntearctable().GetComponent<GamePourable>());

            simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(transform));
        }
        fillInMililiter = EstimateFillInML();
    }

    IEnumerator coroutineCheckPreRequisites()
    {
        while (!simulationController.isExperimentDone())
        {
            Debug.Log("CO: " + name + "'s Coroutine still running");
            
            if (fillInMililiter >= capacity)
            {
                switch (particleContained[0])
                {
                    // Prasyarat 1 terpenuhi: 0.01N Asam Oksalat
                    case "h2c2o4":
                        if (simulationController.getPrerequisiteStatus(0)) break;
                        simulationController.setPrerequisiteStatus(0, true);
                        break;

                    // Prasyarat 2 terpenuhi: 0.01N Kalium Permanganat
                    case "kmno4":
                        if (simulationController.getPrerequisiteStatus(1)) break;
                        simulationController.setPrerequisiteStatus(1, true);
                        break;

                    // Prasyarat 3 terpenuhi: 8N Asam Sulfat
                    case "h2so4":
                        if (simulationController.getPrerequisiteStatus(2) || capacity < 600) break;
                        simulationController.setPrerequisiteStatus(2, true);
                        break;

                    // Belum ada prasyarat yang dipenuhi
                    default:
                        break;
                }
            }

            yield return new WaitForSeconds(1f);
        }

        yield return null;
    }

    // Karena capacity asli = 500ml, dan total kapasitas yang dibutuhkan untuk membuat
    // H2SO4 8N adalah 611.11ml, maka perlu memperbesar kapasitas labu ukur
    public void increaseCapacity()
    {
        capacity = 611.11f;
        isFull = false;
    }
}
