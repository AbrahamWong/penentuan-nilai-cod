using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Funnel : GameInteractables
{
    private GameInteractables attachedTo;
    private Rigidbody rb;

    public GameInteractables getAttachedIntearctable() => attachedTo;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        text.text = "";
    }

    // Update is called once per frame
    // Not implementing GameInteractables.Update()
    private void Update()
    {
        if (attachedTo == null) return;
        if (
            // Vector3.Distance(attachedTo.transform.position, gameObject.transform.position) > 0.3f || 
            Mathf.Abs(attachedTo.transform.position.x - transform.position.x) > 0.08f || attachedTo.transform.position.z - transform.position.z > 0.08f)
        {
            transform.SetParent(transform.root);
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
            attachedTo = null;
        }
    }

    public void getFunnelTrigger (Collider other)
    {
        // Berarti interaksi dengan objek yang bukan GameInteractables
        // Misal: meja.
        Debug.Log("Funnel: trigger " + other.name);
        if (other.gameObject.GetComponent<GameInteractables>() == null) return;
        Debug.Log("Funnel: hit " + other.name);

        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;

        // Cek collider yang mana yang menyentuh corong
        switch (other.transform.name)
        {
            case "Lab Watch Glass":
                WatchGlass wg = simulationController.getWatchGlass();

                ArrayList particle = new ArrayList();
                particle.Add(wg.particleInside);
                attachedTo.setParticleContained(particle);

                Debug.Log("Containment: particle contains = \"" + wg.particleInside + "\"");
                Debug.Log("Containment: particle contained = \"" + attachedTo.getParticleContained()[0] + "\"");
                wg.emptyChemicals();
                break;

            // Semua labu ukur
            case "Volumetric 500":
            case "Volumetric 500 (1)":
            case "Volumetric 500_S":
                GameObject volumetric500 = other.gameObject;

                transform.SetParent(volumetric500.transform.Find("MeshContainer/volumetric_500"));
                transform.rotation = new Quaternion(0, 0, 0, 0);
                transform.localPosition = new Vector3(0f, 0.22f, 0f);

                attachedTo = volumetric500.GetComponent<GameInteractables>();

                // https://forum.unity.com/threads/freeze-multiple-rigid-body-constraints.141450/
                rb.isKinematic = true;
                // rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                break;

            case "buret":
                GameObject burette = other.gameObject;

                transform.SetParent(burette.transform);
                transform.localEulerAngles = new Vector3(-90, 0, 180);
                transform.localPosition = new Vector3(0.0f, 0.0f, 0.626f);

                attachedTo = burette.GetComponent<GameInteractables>();

                rb.isKinematic = true;
                rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
                break;

            default:
                Debug.Log("Objek tidak terdaftar untuk trigger Funnel");
                rb.useGravity = true;
                break;
        }
    }
}
