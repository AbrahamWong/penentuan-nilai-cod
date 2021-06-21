﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatulaTrigger : MonoBehaviour
{
    // Berat bahan yang dibutuhkan yang perlu diambil dengan spatula tidak
    // lebih dari 2 gr.
    // Larutan asam oksalat 0.01N butuh 0.225gr H2C2O4.
    // Larutan kalium permanganat 0.01n butuh 0.630gr KMnO4
    const float weight = 0.1f;
    private bool hasMaterial = false;
    private SimulationController sc;
    private GameObject h2c2o4, kmno4;

    // Start is called before the first frame update
    void Start()
    {
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
        h2c2o4 = transform.Find("lab_spatula/Taburan H2C2O4").gameObject;
        kmno4 = transform.Find("lab_spatula/Taburan KMnO4").gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        string name = other.transform.parent.name;
        Debug.Log(name + ", hasMaterial: " + hasMaterial);

        switch (hasMaterial)
        {
            // Memiliki material: Spatula sedang menampung bahan kimia.
            case true:
                if (name.Equals("Lab Watch Glass"))
                {
                    GameObject h2c2o4 = gameObject.transform.Find("lab_spatula/Taburan H2C2O4").gameObject;
                    GameObject kmno4 = gameObject.transform.Find("lab_spatula/Taburan KMnO4").gameObject;
                    
                    if (h2c2o4.activeInHierarchy)
                    {
                        sc.getWatchGlass().addChemicals(weight, "h2c2o4");
                        h2c2o4.SetActive(false);
                    }
                    else if (kmno4.activeInHierarchy)
                    {

                        kmno4.SetActive(false);
                        sc.getWatchGlass().addChemicals(weight, "kmno4");

                    }

                    hasMaterial = false;
                    return;
                }

                else
                {
                    Debug.Log("Collider bernama " + name + " tidak terdaftar untuk event trigger spatula");
                    return;
                }

            // Tidak memiliki material: Spatula tidak menampung bahan kimia.
            case false:
                switch (name)
                {
                    // Mengambil asam oksalat dari botol
                    case "Oxalic Acid Bottle":
                        // Munculkan "objek" bahan kimia berbentuk bubuk yang sebelumnya dinonaktifkan.
                        // Untuk testing, digunakan objek Sphere.
                        Debug.Log("Asam oksalat diambil sebanyak 0.1gr.");
                        h2c2o4.SetActive(true);
                        hasMaterial = true;
                        return;

                    // Mengambil kalium permanganat dari botol
                    case "Potassium Permanganate Bottle":
                        // Munculkan "objek" bahan kimia berbentuk bubuk yang sebelumnya dinonaktifkan.
                        // Untuk testing, digunakan objek Sphere.
                        Debug.Log("Kalium permanganat diambil sebanyak 0.1gr.");
                        kmno4.SetActive(true);
                        hasMaterial = true;
                        return;

                    // Mengambil apapun dari kaca arloji
                    case "Lab Watch Glass":
                        Debug.Log("Mengambil dari kaca arloji sebanyak 0.05gr.");
                        sc.getWatchGlass().reduceChemicals();
                        return;

                    default:
                        Debug.Log("Collider bernama " + name + " tidak terdaftar untuk event trigger spatula");
                        return;
                }
        }
    }
}
