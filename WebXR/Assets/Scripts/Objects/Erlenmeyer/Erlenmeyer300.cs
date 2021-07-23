using System.Collections;
using UnityEngine;

public class Erlenmeyer300 : GamePourable
{
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
        capacity = 300;     // 300ml

        base.Start();

        weightContained = name.Equals("Air Danau UNAIR") ? 100 : 0;
        rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());

        if (name.Equals("Air Danau UNAIR"))
        {
            particleContained.Clear();
            particleContained.Add("air_danau_unair");
        }

        // Air Danau UNAIR berdasarkan percobaan dari DTKI, membutuhkan 3 - 4 ml 
        // KMnO4 dalam proses titrasi penentuan nilai COD, dengan nilai COD sekitar 61.1
        permanganateTitrationNeeded = Random.Range(3f, 4f);
        cheatCode();

        StartCoroutine(coroutineCheckExperiments());
    }

    private void cheatCode()
    {
        if (!name.Contains("CHEAT")) return;
        else if (name.Equals("CHEAT4"))
        {
            weightContained = 10;       // 10ml campuran ((air 100ml + asam sulfat 5ml) dipanaskan hingga 60 derajat + 10ml asam oksalat))
            temperature = 61;
            particleContained.Clear();
            particleContained.Add("h2c2o4");
            particleContained.Add("h2so4");
            titrationOnce = true; titrationFinished = true;

            rend.material.SetColor("_LiquidColor", parseHexToColor(LiquidColors.KMNO4_100));
            rend.material.SetColor("_SurfaceColor", parseHexToColor(SurfaceColors.KMNO4_100));

            rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        } 
        else if (name.Equals("CHEAT5"))
        {
            weightContained = 125;       // 10ml campuran ((air 100ml + asam sulfat 5ml) dipanaskan hingga 60 derajat + 10ml asam oksalat))
            temperature = 100;
            particleContained.Clear();
            particleContained.Add("air_danau_unair");
            particleContained.Add("h2c2o4");
            particleContained.Add("h2so4");
            particleContained.Add("kmno4");
            titrationOnce = true; titrationFinished = true;
            heatLerpOnce = true;
            codReadyToCount = true;

            rend.material.SetColor("_LiquidColor", parseHexToColor(LiquidColors.KMNO4_100));
            rend.material.SetColor("_SurfaceColor", parseHexToColor(SurfaceColors.KMNO4_100));
            rend.material.SetFloat(rendererFillReference, getFillMaterialPercentage());
        }
    }

    private void Update()
    {

    }

    private void OnTriggerStay(Collider other)
    {
        if (okToPour)
        {
            if (rend.material.GetFloat(rendererFillReference) <= minFill ||
                pouredObject.transform.position.y > transform.position.y) return;

            if (Mathf.Abs(transform.rotation.normalized.x) > normalXAngle || Mathf.Abs(transform.rotation.normalized.z) > normalZAngle)
                simulationController.OnPouringInteractable(this, pouredObject);
        }
    }

    IEnumerator coroutineCheckExperiments()
    {
        while (!simulationController.isExperimentDone())
        {
            Debug.Log("Erlenmeyer: temperature => " + temperature + ", titrationFinished => " + titrationFinished);
            if (!titrationOnce) yield return null;

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
            else if (!name.Equals("Air Danau UNAIR") 
                // && name.Equals("CHEAT4")
                && temperature >= 60 && titrationFinished) 
            {
                simulationController.setPrerequisiteStatus(3, true);
                simulationController.PlayAudioByName("ok_final");
                break;
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
            else if (name.Equals("Air Danau UNAIR") 
                // || name.Equals("CHEAT5")
                && temperature >= 80 && titrationFinished && heatLerpOnce)
            {
                simulationController.setPrerequisiteStatus(4, true);
                simulationController.PlayAudioByName("ok_final");
                codReadyToCount = true;
                break;
            }

            yield return new WaitForSeconds(.05f);
        }

        yield return null;
    }

    public bool titrationOnce = false, titrationFinished = false;

    // Used for lerping up the fill colour, specifically for titration with KMnO4.
    public void startLerpingUp()
    {
        if (titrationOnce) return;
        Debug.Log("Erlenmeyer: testing lerp up.");
        if (colorCoroutine != null) StopCoroutine(colorCoroutine);
        StartCoroutine(LerpUp());
        titrationOnce = true;
    }

    IEnumerator LerpUp()
    {
        // https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/
        Color liquidBefore = rend.material.GetColor("_LiquidColor");
        Color surfaceBefore = rend.material.GetColor("_LiquidColor");
        Color liquidPurple = parseHexToColor(LiquidColors.KMNO4_100);
        Color surfacePurple = parseHexToColor(SurfaceColors.KMNO4_100);

        float originalValue = weightContained;
        float titrationProgress = weightContained - originalValue;
        float lerpTitration = name.Equals("Air Danau UNAIR") ? permanganateTitrationNeeded : permanganateStdNeeded;
        while (titrationProgress < lerpTitration)
        {
            Color liquidLerp = Color.Lerp(liquidBefore, liquidPurple, titrationProgress / lerpTitration);
            Color surfaceLerp = Color.Lerp(surfaceBefore, surfacePurple, titrationProgress / lerpTitration);

            Debug.Log("Erlenmeyer: Lerp up. progress = " + titrationProgress + " / " + lerpTitration);

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
