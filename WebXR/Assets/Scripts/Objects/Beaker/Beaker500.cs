using UnityEngine;

public class Beaker500 : GamePourable
{
    protected override void Start()
    {
        rend = gameObject.transform.Find("MeshContainer/beaker_500/Beaker Filling").GetComponent<Renderer>();

        normalZAngle = 0.4378937;
        maxFill = 0.04f;
        minFill = -0.063f;
        capacity = 500; // 500ml

        base.Start();
        weightContained = 0;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
    }

    // Update is called once per frame
    private void Update()
    {
        // Debug.Log("Beaker: " + transform.rotation.normalized);
        // if (transform.rotation.normalized.z > normalZAngle && transform.rotation.eulerAngles.z > 53)
        // if ((53 < transform.rotation.eulerAngles.z) && (transform.rotation.eulerAngles.z < 90))
        // {
        //     // Jika kosong, abaikan kemiringan, karena isi beaker habis.
        //     if (rend.material.GetFloat(rendererFillReference) <= minFill) return ;
        // 
        //     // Referensikan gameObject dari sebuah objek secara langsung: 
        //     // https://answers.unity.com/questions/36109/get-the-gameobject-that-is-connected-to-the-script.html
        //     simulationController.OnPouringInteractable(this, simulationController.GetClosestPourables(transform));
        //     
        // }
    }

    private new void OnTriggerEnter(Collider other)
    {
        pouredObject = other.GetComponent<GamePourable>();
        if (pouredObject == null)
        {
            Debug.Log("Beaker: pouredObject = null, check: isFunnelHit: " + (other.GetComponent<Funnel>() != null));
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

            if ((53 < transform.rotation.eulerAngles.z) && (transform.rotation.eulerAngles.z < 90))
                simulationController.OnPouringInteractable(this, pouredObject);
        }
    }
}
