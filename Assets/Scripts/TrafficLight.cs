using UnityEngine;

public enum Signal { Red, Amber, Green, Spider }

/// <summary>Construit le feu tricolore en primitives et pilote les 3 lampes.</summary>
public class TrafficLight : MonoBehaviour
{
    static readonly Color[] On  = { new(1f, .16f, .10f), new(1f, .48f, 0f), new(0f, .83f, .41f) };
    static readonly Color[] Off = { new(.23f, .05f, .04f), new(.24f, .13f, .02f), new(.04f, .17f, .10f) };

    [SerializeField] Texture spiderImage;   // image affichée à la place des 3 couleurs
    [SerializeField] AudioClip spiderSound;
    AudioSource audioSrc;
    [System.NonSerialized] Material[] lenses = new Material[3];
    [System.NonSerialized] Light[] glows = new Light[3];

    void Awake() { if (!GetComponent<PlaceInFront>()) gameObject.AddComponent<PlaceInFront>(); Build(); audioSrc = gameObject.AddComponent<AudioSource>(); audioSrc.spatialBlend = 0f; audioSrc.volume = 1f; audioSrc.playOnAwake = false;
        // Si les champs ne sont pas remplis dans l'Inspector, on charge depuis Assets/Resources.
        if (spiderSound == null) spiderSound = Resources.Load<AudioClip>("spider_sound");
        if (spiderImage == null) spiderImage = Resources.Load<Texture>("spider_image");
    }

    /// <summary>Reconstruit le feu (appelable depuis l'éditeur pour le voir sans lancer le jeu).</summary>
    [ContextMenu("Reconstruire le feu")]
    public void Build()
    {
        lenses = new Material[3]; glows = new Light[3];
        for (int c = transform.childCount - 1; c >= 0; c--)
        {
            var child = transform.GetChild(c).gameObject;
            if (Application.isPlaying) Destroy(child); else DestroyImmediate(child);
        }
        var housingMat = Mat(new Color(.17f, .18f, .20f));
        var plateMat = Mat(new Color(.08f, .08f, .09f));

        Part(PrimitiveType.Cylinder, "support", new Vector3(0, .025f, 0), new Vector3(.15f, .025f, .15f), housingMat, transform);
        var head = new GameObject("tete_de_feu").transform;
        head.SetParent(transform, false);
        head.localPosition = new Vector3(0, .54f, 0);
        Part(PrimitiveType.Cube, "boitier", Vector3.zero, new Vector3(.34f, .98f, .26f), housingMat, head);
        Part(PrimitiveType.Cube, "chapeau", new Vector3(0, .5f, 0), new Vector3(.2f, .03f, .12f), housingMat, head);

        float[] ys = { .315f, 0f, -.315f };
        for (int i = 0; i < 3; i++)
        {
            var unit = new GameObject("optique_" + (Signal)i).transform;
            unit.SetParent(head, false);
            unit.localPosition = new Vector3(0, ys[i], .13f);

            var fond = Part(PrimitiveType.Cylinder, "fond", new Vector3(0, 0, .006f), new Vector3(.244f, .006f, .244f), plateMat, unit);
            fond.localRotation = Quaternion.Euler(90, 0, 0);

            lenses[i] = Mat(Off[i]);
            var lens = Part(PrimitiveType.Sphere, "lentille", new Vector3(0, 0, .012f), new Vector3(.2f, .2f, .056f), lenses[i], unit);
            lens.GetComponent<Collider>().enabled = false;

            var glowGo = new GameObject("lueur");
            glowGo.transform.SetParent(unit, false);
            glowGo.transform.localPosition = new Vector3(0, 0, .12f);
            glows[i] = glowGo.AddComponent<Light>();
            glows[i].type = LightType.Point; glows[i].range = 1.2f; glows[i].color = On[i]; glows[i].intensity = 0;
        }
    }

    public void Set(Signal s)
    {
        if (lenses == null || lenses[0] == null) Build();   // références perdues (rechargement des scripts) : on reconstruit
        if (s == Signal.Spider) { ShowSpider(); return; }
        if (audioSrc != null && audioSrc.isPlaying) audioSrc.Stop();
        for (int i = 0; i < 3; i++)
        {
            bool lit = i == (int)s;
            SetTexture(lenses[i], null);
            lenses[i].color = lit ? On[i] : Off[i];
            lenses[i].EnableKeyword("_EMISSION");
            lenses[i].SetColor("_EmissionColor", lit ? On[i] * 2.5f : Color.black);
            glows[i].color = On[i];
            glows[i].intensity = lit ? 1.5f : 0;
        }
    }

    [ContextMenu("Test Spider (jeu en cours)")]
    void TestSpider() => Set(Signal.Spider);

    [ContextMenu("Test Orange (jeu en cours)")]
    void TestAmber() => Set(Signal.Amber);

    void ShowSpider()
    {
        for (int i = 0; i < 3; i++)
        {
            SetTexture(lenses[i], spiderImage);
            lenses[i].color = Color.white;
            lenses[i].EnableKeyword("_EMISSION");
            lenses[i].SetColor("_EmissionColor", Color.white * 0.9f);
            glows[i].color = On[0]; glows[i].intensity = .3f;
        }
        if (spiderSound == null || audioSrc == null) { Debug.LogWarning("Spider : pas de son (spiderSound=" + spiderSound + ")"); return; }
        audioSrc.clip = spiderSound; audioSrc.Play();
        Debug.Log("Spider : lecture du son " + spiderSound.name + " (" + spiderSound.length + " s)");
    }

    static void SetTexture(Material m, Texture t)
    {
        m.SetTexture("_BaseMap", t); m.SetTexture("_MainTex", t);
        m.SetTexture("_EmissionMap", t);
    }

    static Material Mat(Color c)
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var m = new Material(shader);
        m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        return m;
    }

    static Transform Part(PrimitiveType t, string name, Vector3 pos, Vector3 scale, Material m, Transform parent)
    {
        var go = GameObject.CreatePrimitive(t);
        go.name = name;
        go.transform.SetParent(parent, false);
        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.GetComponent<Renderer>().sharedMaterial = m;
        return go.transform;
    }
}
