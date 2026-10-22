using UnityEngine;

/// <summary>Fond de caméra transparent : permet au passthrough de la Quest d'apparaître derrière la scène.</summary>
public static class PassthroughBackground
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Apply()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0, 0, 0, 0);
    }
}
