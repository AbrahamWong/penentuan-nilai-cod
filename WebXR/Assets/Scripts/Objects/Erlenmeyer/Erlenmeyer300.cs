using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Erlenmeyer300 : GamePourable
{
    float eulerXAngle;
    float eulerZAngle;
    [SerializeField] private float fillInMililiter;
    private bool codReadyToCount = false;
    const float permanganateStdNeeded = 4.5f;
    [SerializeField] private float permanganateTitrationNeeded;

    public bool getCODStatus() => codReadyToCount;
    public float getPermanganateTitrationNeeded() => permanganateTitrationNeeded;

    // Start is called before the first frame update
    protected override void Start()
    {
        rend = transform.Find("MeshContainer/erlenmeyer_300/Fill").GetComponent<Renderer>();

        minFill = -0.0605f;
        maxFill = 0.02f;
        normalXAngle = 0.5f;
        normalZAngle = 0.5f;
        eulerXAngle = 60;
        eulerZAngle = 60;
        capacity = 300;     // 300ml

        base.Start();

        weightContained = name.Equals("Air Danau UNAIR") ? 100 : 0;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());

        if (name.Equals("Air Danau UNAIR"))
        {
            particleContained.Clear();
            particleContained.Add("air_danau_unair");
        }

        permanganateTitrationNeeded = Random.Range(4f, 5f);
        StartCoroutine(coroutineCheckExperiments());
    }

    // Update is called once per frame
    private void Update()
    {
        if (transform.rotation.normalized.x > normalXAngle || transform.rotation.normalized.z > normalZAngle 
            || transform.rotation.eulerAngles.x > eulerXAngle || transform.rotation.eulerAngles.z > eulerZAngle )
        {
            if (rend.material.GetFloat(rendererFillReference) <= minFill) return;

            if (simulationController.GetClosestPourables(transform) != null)
            {
                GamePourable closest = simulationController.GetClosestPourables(transform);
                simulationController.OnPouringInteractable(this, closest);
            }
        }

        fillInMililiter = EstimateFillInML();
    }

    IEnumerator coroutineCheckExperiments()
    {
        while (!simulationController.isExperimentDone())
        {
            if (!titrationOnce) 
            {
                yield return null;
                continue;
            }

            // Eksperimen 1: Standarisasi KMnO4
            // Tahapan:
            // 1. Memanaskan 100 ml aquadest dan 5 ml asam sulfat 8 N di dalam erlenmeyer sampai suhu 60oC
            // 2. Setelah suhu 60oC segera tambahkan 10 ml asam oksalat 0,01 N
            // 3. Kemudian mengambil 10 ml larutan tersebut kedalam Erlenmeyer lalu menitrasinya dengan larutan KMnO4(menentukan berapa ml KMnO4)
            // 4. Mencatat berapa volume KMnO4 yang dibutuhkan
            //
            // Erlenmeyer -> onPouring(Beaker::100ml aqua) -> onPipette(5ml H2SO4) -> ElectricHeater->coroutine(raiseTemp=>60)
            //      -> onPouring(MeasurementC10::10ml KMnO4) -> onSUCK(this)
            //      -> [Pindah erlenmeyer] onPipette(this to Erlmenmeyer) -> onTitrate(KMnO4) [4.5 || 4.7]
            if (!name.Equals("Air Danau UNAIR") && temperature >= 60 && titrationFinished) 
            {
                simulationController.setPrerequisiteStatus(3, true);
            }

            // Eksperimen 2: Penentuan Nilai COD
            // Tahapan:
            // 1. Memasukkan *100 ml sampel air danau ITS* ke dalam erlenmeyer 300 ml, kemudian menambahkan *5 ml H2SO4* 8 N
            // 2. Memanaskan larutan tersebut sampai suhu *70oC*.
            // 3. Setelah mencapai suhu 70oC tambahkan *10 ml larutan standart KMnO4 0,01 N* dalam larutan tersebut dan meneruskan pemanasan
            //      sampai mendidih. Pendidihan dilakukan dengan hati-hati selama 10 menit lalu didinginkan pada suhu ruang.
            //      Pada tahap ini, larutan akan berubah warna menjadi bening kembali.
            // 4. Setelah larutan mendidih tambahkan segera *10 ml asam oksalat 0,01 N*.
            // 5. Kemudian menitrasi dengan *KMnO4 0,02175 N* sampai berwarna merah muda.
            //
            // 100ml air danau -> onPipette(5ml H2SO4) -> ElectricHeater->coroutine(raiseTemp=>70) -> onPouring(MeasurementC10::10ml KMnO4)
            //      -> ElectricHeater->coroutine(raiseTemp=>boil) while LerpDown() -> onPouring(MeasurementC10::10ml H2C2O4)
            //      -> onTitrate(KMnO4) [Mathf.Rand(4, 5)]
            else if (temperature >= 80 && titrationFinished && heatLerpOnce)
            {
                simulationController.setPrerequisiteStatus(4, true);
                codReadyToCount = true;
            }

            yield return new WaitForSeconds(.05f);
        }

        yield return null;
    }

    bool titrationOnce = false, titrationFinished = false;

    // Used for lerping up the fill colour, specifically for titration with KMnO4.
    public void startLerpingUp()
    {
        if (titrationOnce) return;
        Debug.Log("Erlenmeyer: testing lerp down.");
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        StartCoroutine(LerpUp());
        titrationOnce = true;
    }

    IEnumerator LerpUp()
    {
        // https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        Color liquidBefore = rend.material.GetColor("_LiquidColor");
        Color surfaceBefore = rend.material.GetColor("_LiquidColor");
        Color liquidPurple = parseHexToColor(LiquidColors.KMNO4_50);
        Color surfacePurple = parseHexToColor(SurfaceColors.KMNO4_50);

        float originalValue = weightContained;
        float titrationProgress = weightContained - originalValue;
        float lerpTitration = name.Equals("Air Danau UNAIR") ? permanganateTitrationNeeded : permanganateStdNeeded;
        while (titrationProgress < lerpTitration)
        {
            Color liquidLerp = Color.Lerp(liquidBefore, liquidPurple, titrationProgress / lerpTitration);
            Color surfaceLerp = Color.Lerp(surfaceBefore, surfacePurple, titrationProgress / lerpTitration);

            Debug.Log("Erlenmeyer: Lerp up. progress = " + titrationProgress + "/" + lerpTitration);

            rend.material.SetColor("_LiquidColor", liquidLerp);
            rend.material.SetColor("_SurfaceColor", surfaceLerp);
            titrationProgress = weightContained - originalValue;

            yield return null;
        }
        
        rend.material.SetColor("_LiquidColor", liquidPurple);
        rend.material.SetColor("_SurfaceColor",surfacePurple);
        titrationFinished = true;
    }

    float heatingProgress = 0; bool heatLerpOnce = false;
    private float lerpFrame = 120;
    public void startLerpingDown()
    {
        if (heatLerpOnce) return;
        Debug.Log("Erlenmeyer: testing lerp down.");
        foreach (string p in particleContained)
        {
            if (p.Equals("kmno4"))
            {
                heatLerpOnce = true;
                if (colorCoroutine != null) StopCoroutine(colorCoroutine);
                StartCoroutine(LerpDown());
                break;
            }
        }
    }

    IEnumerator LerpDown()
    {
        // https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        Color liquidBefore = rend.material.GetColor("_LiquidColor");
        Color surfaceBefore = rend.material.GetColor("_LiquidColor");
        Color liquidNormal = parseHexToColor(LiquidColors.NORMAL);
        Color surfaceNormal = parseHexToColor(SurfaceColors.NORMAL);

        while (heatingProgress < lerpFrame)
        {
            Color liquidLerp = Color.Lerp(liquidBefore, liquidNormal, heatingProgress / lerpFrame);
            Color surfaceLerp = Color.Lerp(surfaceBefore, surfaceNormal, heatingProgress/ lerpFrame);

            rend.material.SetColor("_LiquidColor", liquidLerp);
            rend.material.SetColor("_SurfaceColor", surfaceLerp);
            heatingProgress++;

            yield return new WaitForSeconds(.1f);
        }
        
        rend.material.SetColor("_LiquidColor", liquidNormal);
        rend.material.SetColor("_SurfaceColor", surfaceNormal);
        heatingProgress++;

    }
}
