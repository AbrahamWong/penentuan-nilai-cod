using UnityEngine;

public class MeasurementC10 : GamePourable
{
    // Start is called before the first frame update
    protected override void Start()
    {
        // https://answers.unity.com/questions/63317/access-a-child-from-the-parent-or-other-gameobject.html
        // https://answers.unity.com/questions/851056/how-can-i-find-object-childs-child-or-child-in-chi.html
        rend = gameObject.transform.Find("MeshContainer/measuring_cylinder_10ml/Fill").GetComponent<Renderer>();

        normalXAngle = 0.6959127;
        normalZAngle = 0.6959127;
        maxFill = 0.014f;
        minFill = -0.068f;

        weightContained = 0;
        capacity = 10;         // 10ml

        base.Start();
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
    }

    // Update is called once per frame
    private void Update()
    {
        // if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
        // {
        //     if (rend.material.GetFloat(rendererFillReference) <= minFill) return;
        //     simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(transform));
        // }
    }

    private void OnTriggerStay(Collider other)
    {
        if (okToPour)
        {
            if (rend.material.GetFloat(rendererFillReference) <= minFill ||
                pouredObject.transform.position.y > transform.position.y) return;

            if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
                simulationController.OnPouringInteractable(this, pouredObject);
        }
    }
}
