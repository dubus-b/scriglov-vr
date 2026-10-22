using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>Affiche la main droite suivie : une petite sphère par articulation. Se crée tout seul au lancement.</summary>
public class HandVisualizer : MonoBehaviour
{
    [SerializeField] float jointSize = .014f;

    class HandView
    {
        public Transform[] joints = new Transform[XRHandJointID.EndMarker.ToIndex()];
        public Transform root;
    }

    readonly HandView right = new HandView();
    XRHandSubsystem subsystem;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Bootstrap() => new GameObject("HandVisualizer").AddComponent<HandVisualizer>();

    void Update()
    {
        if (subsystem == null || !subsystem.running)
        {
            var list = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(list);
            subsystem = list.Count > 0 ? list[0] : null;
            if (subsystem == null) return;
        }
        Refresh(right, subsystem.rightHand);
    }

    void Refresh(HandView view, XRHand hand)
    {
        if (view.root == null)
        {
            view.root = new GameObject(hand.handedness + "Hand").transform;
            // Les poses des articulations sont exprimées dans l'espace de suivi (parent de la caméra).
            var cam = Camera.main;
            view.root.SetParent(cam != null ? cam.transform.parent : null, false);
        }
        view.root.gameObject.SetActive(hand.isTracked);
        if (!hand.isTracked) return;

        for (int i = XRHandJointID.BeginMarker.ToIndex(); i < XRHandJointID.EndMarker.ToIndex(); i++)
        {
            var id = XRHandJointIDUtility.FromIndex(i);
            if (!hand.GetJoint(id).TryGetPose(out var pose)) continue;
            if (view.joints[i] == null)
            {
                var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                Destroy(s.GetComponent<Collider>());
                s.transform.SetParent(view.root, false);
                s.transform.localScale = Vector3.one * jointSize;
                view.joints[i] = s.transform;
            }
            view.joints[i].SetLocalPositionAndRotation(pose.position, pose.rotation);
        }
    }
}
