using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;

/// <summary>Score d'ouverture de la main droite : 0 = poing fermé, 1 = main ouverte.</summary>
public class HandOpenness : MonoBehaviour
{
    // Réglage : rapport distance(poignet→bout) / distance(poignet→phalange proximale).
    // ~1,1 doigt replié, ~1,8 doigt tendu.
    [SerializeField] float closedRatio = 1.1f, openRatio = 1.8f;

    public bool IsTracked { get; private set; }
    public float Value { get; private set; }
    /// <summary>Ouverture par doigt : 0 pouce, 1 index, 2 majeur, 3 annulaire, 4 auriculaire.</summary>
    public readonly float[] Fingers = new float[5];

    XRHandSubsystem subsystem;

    void Update()
    {
        if (subsystem == null || !subsystem.running)
        {
            var list = new List<XRHandSubsystem>();
            SubsystemManager.GetSubsystems(list);
            subsystem = list.Count > 0 ? list[0] : null;
            if (subsystem == null) { IsTracked = false; return; }
        }

        var hand = subsystem.rightHand;
        IsTracked = hand.isTracked;
        if (!IsTracked) return;

        if (!TryPos(hand, XRHandJointID.Wrist, out var wrist)) { IsTracked = false; return; }

        float sum = 0; int n = 0; int fi = 1;
        (XRHandJointID tip, XRHandJointID prox)[] fingers =
        {
            (XRHandJointID.IndexTip,  XRHandJointID.IndexProximal),
            (XRHandJointID.MiddleTip, XRHandJointID.MiddleProximal),
            (XRHandJointID.RingTip,   XRHandJointID.RingProximal),
            (XRHandJointID.LittleTip, XRHandJointID.LittleProximal),
        };
        foreach (var (tipId, proxId) in fingers)
        {
            if (!TryPos(hand, tipId, out var tip) || !TryPos(hand, proxId, out var prox)) continue;
            float ratio = Vector3.Distance(tip, wrist) / Vector3.Distance(prox, wrist);
            float v = Mathf.InverseLerp(closedRatio, openRatio, ratio);
            Fingers[fi++] = v; sum += v; n++;
        }
        // Pouce : bout du pouce → base de l'auriculaire, rapporté à la taille de la paume.
        if (TryPos(hand, XRHandJointID.ThumbTip, out var tt) &&
            TryPos(hand, XRHandJointID.LittleMetacarpal, out var pm) &&
            TryPos(hand, XRHandJointID.MiddleProximal, out var mp))
        {
            float r = Vector3.Distance(tt, pm) / Vector3.Distance(mp, wrist);
            float v = Mathf.InverseLerp(0.6f, 1.1f, r);
            Fingers[0] = v; sum += v; n++;
        }
        if (n > 0) Value = sum / n;
    }

    static bool TryPos(XRHand hand, XRHandJointID id, out Vector3 p)
    {
        if (hand.GetJoint(id).TryGetPose(out var pose)) { p = pose.position; return true; }
        p = default; return false;
    }
}
