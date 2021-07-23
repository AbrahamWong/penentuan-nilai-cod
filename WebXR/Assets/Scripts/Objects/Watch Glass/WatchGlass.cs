using System.Collections.Generic;
using UnityEngine;

public class WatchGlass : GameInteractables
{
    [SerializeField] protected float decreaseFactor = 0.025f;
    protected List<Transform> potassiumPermanganates, oxalicAcids;
    protected string activeObjectName = "";

    public string particleInside { get; set; }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        text.text = "";

        Transform kmno4Parent = transform.Find("MeshContainer/watch_glass/KMnO4");
        Transform h2c2o4Parent = transform.Find("MeshContainer/watch_glass/H2C2O4");

        potassiumPermanganates = new List<Transform>();
        oxalicAcids = new List<Transform>();
        foreach (Transform t in kmno4Parent.gameObject.GetComponentsInChildren<Transform>(true)) 
            if (t != kmno4Parent) potassiumPermanganates.Add(t);
        foreach (Transform t in h2c2o4Parent.gameObject.GetComponentsInChildren<Transform>(true)) 
            if (t != h2c2o4Parent) oxalicAcids.Add(t);
        
        Debug.Log("WatchGlass: pottasiumPermanganates => " + potassiumPermanganates[0].name + potassiumPermanganates[1].name);
    }

    // Penggunaan ArrayList daripada array Transform[]
    // https://www.tutorialsteacher.com/csharp/csharp-arraylist
    // private ArrayList createdObjects = new ArrayList();
    public void addChemicals(float weight, string chemType)
    {
        weightContained += weight;

        // https://stackoverflow.com/questions/20147879/switch-case-can-i-use-a-range-instead-of-a-one-number
        switch (weightContained)
        {
            case float n when (n >= 0.5f):
                activateObject(chemType.Equals("h2c2o4") ? oxalicAcids : potassiumPermanganates, 5);
                break;
            case float n when (n >= 0.4f):
                activateObject(chemType.Equals("h2c2o4") ? oxalicAcids : potassiumPermanganates, 4);
                break;
            case float n when (n >= 0.3f):
                activateObject(chemType.Equals("h2c2o4") ? oxalicAcids : potassiumPermanganates, 3);
                break;
            case float n when (n >= 0.2f):
                activateObject(chemType.Equals("h2c2o4") ? oxalicAcids : potassiumPermanganates, 2);
                break;
            case float n when (n >= 0.1f):
                activateObject(chemType.Equals("h2c2o4") ? oxalicAcids : potassiumPermanganates, 1);
                break;
            default:
                break;
        }

        particleInside = chemType;
    }

    public void reduceChemicals()
    {
        if (weightContained < 0f) return;

        weightContained = weightContained - decreaseFactor < 0f ? 0f : weightContained - decreaseFactor;
        if (weightContained <= 0.15f && weightContained > 0.14f) weightContained = 0.158f;

        activateObject(activeObjectName.Substring(activeObjectName.Length - 1) == "O" ? oxalicAcids : potassiumPermanganates, 
            (int)(Mathf.Round(weightContained * 1000) / 100));

    }

    public void emptyChemicals()
    {
        weightContained = 0f;

        activateObject(potassiumPermanganates, 0);
        activateObject(oxalicAcids, 0);
    }

    private void activateObject(List<Transform> gameObjects, int number)
    {
        if (number > gameObjects.Count)
        {
            gameObjects[gameObjects.Count - 1].gameObject.SetActive(true);
            return;
        }

        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(i == number - 1 ? true : false);
            activeObjectName = i == number ? gameObjects[i].name : activeObjectName.Equals("") ? "" : activeObjectName;
        }

        Debug.Log(activeObjectName);
    }
}
