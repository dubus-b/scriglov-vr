using System.Text;
using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>Panneau de debug : version + état de la main droite. Suit le poignet, sinon reste devant vous.</summary>
public class HandDebug : MonoBehaviour
{
    const string Version = "scriglov VR\ntest handtracking 1\nepitech forward";

    HandOpenness hand;
    HandTrafficLightController ctrl;
    TextMesh text;
    XRHandSubsystem subsystem;
    readonly StringBuilder sb = new StringBuilder();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap() => new GameObject("HandDebug").AddComponent<HandDebug>();

    void Start()
    {
        text = gameObject.AddComponent<TextMesh>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        GetComponent<MeshRenderer>().sharedMaterial = text.font.material;
        text.fontSize = 64;
        text.characterSize = .0025f;
        text.anchor = TextAnchor.LowerCenter;
        text.alignment = TextAlignment.Center;
        text.color = Color.white;
    }

    void Update()
    {
        if (hand == null) hand = FindFirstObjectByType<HandOpenness>();
        if (ctrl == null) ctrl = FindFirstObjectByType<HandTrafficLightController>();
        var cam = Camera.main;
        if (cam == null || text == null) return;

        sb.Clear();
        sb.AppendLine(Version);
        bool tracked = hand != null && hand.IsTracked;
        sb.AppendLine(tracked ? "main droite : suivie" : "main droite : NON suivie");
        if (tracked)
        {
            sb.AppendLine("ouverture " + hand.Value.ToString("0.00"));
            var f = hand.Fingers;
            sb.AppendLine($"P {f[0]:0.00}  I {f[1]:0.00}  M {f[2]:0.00}  A {f[3]:0.00}  Au {f[4]:0.00}");
        }
        if (ctrl != null) sb.Append("feu : " + ctrl.State);
        text.text = sb.ToString();

        Vector3 pos;
        if (tracked && TryWrist(out var wrist, cam)) pos = wrist + Vector3.up * .06f;
        else pos = cam.transform.position + cam.transform.forward * .8f + Vector3.down * .3f;
        transform.position = pos;
        var dir = pos - cam.transform.position;
        if (dir.sqrMagnitude > .0001f) transform.rotation = Quaternion.LookRotation(dir);   // texte lisible, face à la caméra
    }

    bool TryWrist(out Vector3 world, Camera cam)
    {
        world = default;
        if (subsystem == null || !subsystem.running)
        {
            var list = new System.Collections.Generic.List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(list);
            subsystem = list.Count > 0 ? list[0] : null;
            if (subsystem == null) return false;
        }
        if (!subsystem.rightHand.GetJoint(XRHandJointID.Palm).TryGetPose(out var pose)) return false;
        var space = cam.transform.parent;   // les poses sont dans l'espace de suivi
        world = space != null ? space.TransformPoint(pose.position) : pose.position;
        return true;
    }
}
