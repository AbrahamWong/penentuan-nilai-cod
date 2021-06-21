using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : GameInteractables
{
    private GameInteractables attachedTo;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (attachedTo == null) return;
        if (Vector3.Distance(attachedTo.transform.position, gameObject.transform.position) > 0.3f)
        {
            transform.SetParent(transform.root);
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            attachedTo = null;
        }
    }

    public void getFunnelTrigger (Collider other)
    {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
        
        // Cek collider yang mana yang menyentuh corong
        switch (other.transform.parent.name)
        {
            case "Lab Watch Glass":
                simulationController.getWatchGlass().emptyChemicals();
                break;

            case "Volumetric 500":
                GameObject volumetric500 = transform.root.Find("Volumetric 500").gameObject;

                transform.SetParent(volumetric500.transform.Find("MeshContainer/volumetric_500"));
                transform.rotation = new Quaternion(0, 0, 0, 0);
                transform.localPosition = new Vector3(0f, 0.23f, 0f);

                attachedTo = volumetric500.GetComponent<GameInteractables>();

                // https://forum.unity.com/threads/freeze-multiple-rigid-body-constraints.141450/
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                break;

            default:
                Debug.Log("Objek tidak terdaftar untuk trigger Funnel");
                rb.useGravity = true;
                break;
        }
    }
}
