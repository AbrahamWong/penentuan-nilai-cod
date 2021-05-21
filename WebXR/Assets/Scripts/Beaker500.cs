using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beaker500 : MonoBehaviour
{
    SimulationController simulationController;
    [SerializeField] Renderer rend;

    // Start is called before the first frame update
    void Start()
    {
        // Memanggil komponen dari objek lain
        // https://forum.unity.com/threads/how-can-i-reference-to-a-component-of-another-gameobject.280451/
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();

        // Memanggil komponen dari objek lain dari nama objek.
        // Karena bersifat sementara, ganti cara ini ketika banyak objek terlibat.
        // https://forum.unity.com/threads/getting-a-gameobject-by-name.777797/
        rend = GameObject.Find("Beaker Filling").GetComponent<Renderer>();
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

        if (transform.rotation.normalized.z > 0.4378937)
        {
            // Debug.Log(transform.rotation.eulerAngles.x + ", " + transform.rotation.eulerAngles.z);

            // Referensikan gameObject dari sebuah objek secara langsung: 
            // https://answers.unity.com/questions/36109/get-the-gameobject-that-is-connected-to-the-script.html
            simulationController.OnPouringInteractable(gameObject, simulationController.GetClosestInteractable(gameObject));

            Debug.Log(rend.material.GetFloat("_BeakerFill"));
            rend.material.SetFloat("_BeakerFill", rend.material.GetFloat("_BeakerFill") - 0.0001f);
        }
    }
}
