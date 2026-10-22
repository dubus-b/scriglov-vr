using UnityEditor;
using UnityEngine;

public static class SetupScene
{
    [MenuItem("Feu/Ajouter le feu à la scène")]
    static void Add()
    {
        var go = new GameObject("FeuTricolore");
        go.transform.position = new Vector3(0, 0, 1.6f);   // 1,6 m devant l'origine
        go.transform.localScale = Vector3.one * 1.4f;
        var tl = go.AddComponent<TrafficLight>();
        var tlSo = new SerializedObject(tl);
        tlSo.FindProperty("spiderSound").objectReferenceValue = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Audio/spider.mp3");
        tlSo.FindProperty("spiderImage").objectReferenceValue = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Textures/spider.png");
        tlSo.ApplyModifiedPropertiesWithoutUndo();
        tl.Build();   // visible tout de suite dans la vue Scene
        tl.Set(Signal.Amber);
        var hand = go.AddComponent<HandOpenness>();
        var ctrl = go.AddComponent<HandTrafficLightController>();
        var so = new SerializedObject(ctrl);
        so.FindProperty("hand").objectReferenceValue = hand;
        so.ApplyModifiedPropertiesWithoutUndo();
        Selection.activeGameObject = go;
    }
}
