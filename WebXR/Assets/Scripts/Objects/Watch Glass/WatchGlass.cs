using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WatchGlass : GameInteractables
{
    [SerializeField] protected float decreaseFactor = 0.025f;
    protected int iteration = 0;

    public string particleInside { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        simulationController = GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Penggunaan ArrayList daripada array Transform[]
    // https://www.tutorialsteacher.com/csharp/csharp-arraylist
    private ArrayList createdObjects = new ArrayList();
    public void addChemicals(float weight, string chemType)
    {
        weightContained += weight;
        iteration++;
        float fact = 1f + ((float)iteration / 10);

        for (int i = 0; i < 100; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-0.005f, 0.005f) * fact, 
                0.002f * fact, 
                Random.Range(-0.005f, 0.005f) * fact);

            // https://answers.unity.com/questions/586985/how-to-make-an-instantiated-prefab-a-child-of-a-ga.html
            // https://answers.unity.com/questions/886203/change-name-of-instantiate-prefab.html
            string prefab = chemType.Equals("h2c2o4") ? 
                            "Sampel Partikel H2C2O4" : chemType.Equals("kmno4") ? 
                                                       "Sampel Partikel KMnO4" : "Sampel Partikel";
            particleInside = chemType;

            Transform tr = Instantiate(gameObject.transform.Find(prefab), pos + transform.position, Quaternion.identity);
            tr.name = "Dust " + i;
            tr.parent = GameObject.Find("Partikel").transform;
            tr.gameObject.SetActive(true);

            // array[].SetValue(object, index)
            createdObjects.Add(tr);
        }

        Debug.Log("Created Objects Amount: " + createdObjects.Count);
    }

    public void reduceChemicals()
    {
        if (weightContained < 0f || createdObjects[0] == null) return;

        weightContained = weightContained - decreaseFactor < 0f ? 0f : weightContained - decreaseFactor;
        if (weightContained <= 0.15f && weightContained > 0.14f) weightContained = 0.158f;

        int amountBefore = createdObjects.Count;
        for (int i = amountBefore; i > amountBefore - 15; i--)
        {
            if (createdObjects[i - 1] == null) return;

            Debug.Log("Created Objects Amount: " + i);
            Destroy(((Transform) createdObjects[i - 1]).gameObject);
            createdObjects.RemoveAt(i - 1);
        }

    }

    public void emptyChemicals()
    {
        weightContained = 0f;

        foreach (Transform createdObject in createdObjects)
        {
            Destroy(createdObject.gameObject);
        }

        createdObjects.Clear();
        iteration = 0;
    }
}
