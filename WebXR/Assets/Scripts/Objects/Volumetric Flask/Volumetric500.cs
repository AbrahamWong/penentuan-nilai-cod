using System.Collections;
using UnityEngine;

public class Volumetric500 : GamePourable
{
    protected override void Start()
    {
        // https://answers.unity.com/questions/63317/access-a-child-from-the-parent-or-other-gameobject.html
        // https://answers.unity.com/questions/851056/how-can-i-find-object-childs-child-or-child-in-chi.html
        rend = gameObject.transform.Find("MeshContainer/volumetric_500/Filling").GetComponent<Renderer>();

        normalXAngle = 0.6959127;
        normalZAngle = 0.6959127;
        maxFill = 0f;
        minFill = -0.101f;

        weightContained = 0;    // 0ml => test 100ml
        capacity = 500;         // 500ml

        base.Start();
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        
        StartCoroutine(coroutineCheckPreRequisites());
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
                offsetWeight = 0.225f;
                break;
    
            case "CHEAT2":
                particleContained.Add("kmno4");
                offsetWeight = 0.158f;
                break;
            
            case "CHEAT3":
                weightContained = 611.11f;
                offsetWeight = 111.11f;
                rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
                capacity = 611.11f;
                particleContained.Add("h2so4");
                break;
            
            default:
                break;
        }
    }


    private new void OnTriggerEnter(Collider other)
    {
        pouredObject = other.GetComponent<GamePourable>();
        if (pouredObject == null)
        {
            pouredObject = other.GetComponent<Funnel>().getAttachedIntearctable().gameObject.GetComponent<GamePourable>();
            if (pouredObject == null) return;
        }

        else if (pouredObject.GetComponent<Volumetric500>() != null || pouredObject.GetComponent<Burette>() != null) return;

        okToPour = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (okToPour)
        {
            if (rend.material.GetFloat(rendererFillReference) <= minFill ||
                pouredObject.transform.position.y > transform.position.y) return;

            if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
            {
                simulationController.OnPouringInteractable(this, pouredObject);
                Debug.Log("Volumetric500: " + name + " menuangkan ke " + pouredObject.name);
            }
                
        }
    }


    IEnumerator coroutineCheckPreRequisites()
    {
        while (!simulationController.isExperimentDone())
        {   
            if (weightContained >= capacity)
            {
                switch (particleContained[0])
                {
                    // Prasyarat 1 terpenuhi: 0.01N Asam Oksalat
                    case "h2c2o4":
                        if (simulationController.getPrerequisiteStatus(0) || 
                            !isFloatEqual(simulationController.PrerequisiteIngredientDictionary["h2c2o4"], offsetWeight, 4)) break;
                        simulationController.setPrerequisiteStatus(0, true);
                        simulationController.PlayAudioByName("ok_final");
                        break;

                    // Prasyarat 2 terpenuhi: 0.01N Kalium Permanganat
                    case "kmno4":
                        if (simulationController.getPrerequisiteStatus(1) ||
                            !isFloatEqual(simulationController.PrerequisiteIngredientDictionary["kmno4"], offsetWeight, 4)) break;
                        simulationController.setPrerequisiteStatus(1, true);
                        simulationController.PlayAudioByName("ok_final");
                        break;

                    // Prasyarat 3 terpenuhi: 8N Asam Sulfat
                    case "h2so4":
                        if (simulationController.getPrerequisiteStatus(2) || capacity < 600 ||
                            !isFloatEqual(simulationController.PrerequisiteIngredientDictionary["h2so4"], offsetWeight, 0)) break;
                        simulationController.setPrerequisiteStatus(2, true);
                        simulationController.PlayAudioByName("ok_final");
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

    private bool isFloatEqual (float a, float b, int power)
    {
        float roundTo = Mathf.Pow(10, power);
        float A = Mathf.Round(a * roundTo) / roundTo;
        float B = Mathf.Round(b * roundTo) / roundTo;
        Debug.Log(A + " <= A || B => " + B);
        return A == B;
    }

    // Karena capacity asli = 500ml, dan total kapasitas yang dibutuhkan untuk membuat
    // H2SO4 8N adalah 611.11ml, maka perlu memperbesar kapasitas labu ukur
    public void increaseCapacity()
    {
        capacity = 611.11f;
        isFull = false;
    }
}
