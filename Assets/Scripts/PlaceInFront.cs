using System.Collections;
using UnityEngine;

/// <summary>Au lancement, place le feu dans le champ de vision, face à l'utilisateur.</summary>
public class PlaceInFront : MonoBehaviour
{
    [SerializeField] float distance = 1.5f;
    [SerializeField] float headDrop = -.10f;   // comme un écran d'ordinateur : le centre de la tête du feu arrive un peu au-dessus des yeux

    IEnumerator Start()
    {
        // Laisse au casque le temps de fournir une vraie position de tête.
        yield return new WaitForSeconds(1f);
        Place();
    }

    public void Place()
    {
        var cam = Camera.main;
        if (cam == null) return;
        var fwd = cam.transform.forward; fwd.y = 0;
        if (fwd.sqrMagnitude < .001f) fwd = Vector3.forward;
        fwd.Normalize();

        var p = cam.transform.position + fwd * distance;
        p.y = cam.transform.position.y - headDrop;
        transform.SetPositionAndRotation(p, Quaternion.LookRotation(-fwd));   // lentilles (+Z) tournées vers vous
    }
}
