using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AfterTitrationBox : MonoBehaviour
{
    [SerializeField] private List<Erlenmeyer300> erlenmeyers = new List<Erlenmeyer300>();
    private int minimalErlenmeyer = 2;
    private TextMeshPro tmp;

    private void Start()
    {
        tmp = GetComponentInChildren<TextMeshPro>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Erlenmeyer300 erlenmeyer = other.GetComponent<Erlenmeyer300>();
        if (erlenmeyer == null || !erlenmeyer.getCODStatus()) return;

        erlenmeyers.Add(erlenmeyer);
        if (erlenmeyers.Count >= minimalErlenmeyer) StartCoroutine(coroutineCheckCODValue());
    }

    IEnumerator coroutineCheckCODValue()
    {
        float titrationsNeeded = 0f; 
        erlenmeyers.ForEach(erlenmeyer => { titrationsNeeded += erlenmeyer.getPermanganateTitrationNeeded(); });
        float mean = titrationsNeeded / (float) erlenmeyers.Count;

        float cod = Formula.CalculateCOD(mean, 0.02715f, 10, 0.01f);
        tmp.text = "Nilai COD = " + cod.ToString();
        GameObject.FindGameObjectWithTag("GameController").GetComponent<SimulationController>().setPrerequisiteStatus(5, true);
        yield return null;
    }
}
