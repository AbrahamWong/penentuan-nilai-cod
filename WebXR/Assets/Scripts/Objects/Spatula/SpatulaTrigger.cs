using UnityEngine;

public class SpatulaTrigger : MonoBehaviour
{
    const float weight = 0.1f;
    private bool hasMaterial = false;
    private SimulationController sc;
    private GameObject h2c2o4, kmno4;

    void Start()
    {
        sc = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
        h2c2o4 = transform.Find("MeshContainer/spatula/Taburan H2C2O4").gameObject;
        kmno4 = transform.Find("MeshContainer/spatula/Taburan KMnO4").gameObject;
        h2c2o4.SetActive(false);
        kmno4.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        string name = other.name;
        Debug.Log(name + ", hasMaterial: " + hasMaterial);

        switch (hasMaterial)
        {
            // Memiliki material: Spatula sedang menampung bahan kimia.
            case true:
                switch (name)
                {
                    case "Lab Watch Glass":
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
                        break;

                    case "Oxalic Acid Bottle":
                        // Kembalikan asam oksalat ke dalam botol
                        if (h2c2o4.activeInHierarchy)
                        {
                            h2c2o4.SetActive(false);
                            hasMaterial = false;
                        }
                        break;

                    case "Potassium Permanganate Bottle":
                        // Kembalikan asam oksalat ke dalam botol
                        if (kmno4.activeInHierarchy)
                        {
                            kmno4.SetActive(false);
                            hasMaterial = false;
                        }
                        break;

                    default:
                        break;
                }
                break;

            // Tidak memiliki material: Spatula tidak menampung bahan kimia.
            case false:
                switch (name)
                {
                    // Mengambil asam oksalat dari botol
                    case "Oxalic Acid Bottle":
                        h2c2o4.SetActive(true);
                        hasMaterial = true;
                        return;

                    // Mengambil kalium permanganat dari botol
                    case "Potassium Permanganate Bottle":
                        kmno4.SetActive(true);
                        hasMaterial = true;
                        return;

                    // Mengambil apapun dari kaca arloji
                    case "Lab Watch Glass":
                        sc.getWatchGlass().reduceChemicals();
                        return;

                    default:
                        return;
                }
        }
    }
}
