using UnityEngine;

/// <summary>Main fermée = rouge · mi-ouverte = orange · ouverte = vert (avec hystérésis).</summary>
[RequireComponent(typeof(TrafficLight))]
public class HandTrafficLightController : MonoBehaviour
{
    [SerializeField] HandOpenness hand;
    [SerializeField] float closedBelow = .3f, openAbove = .7f, margin = .04f;

    [SerializeField] float spiderHold = .6f;   // le geste doit être perdu aussi longtemps avant de quitter Spider
    float lastSpiderTime;
    TrafficLight light_;
    Signal state = Signal.Amber;
    public Signal State => state;

    void Start() { light_ = GetComponent<TrafficLight>(); light_.Set(state); }

    void Update()
    {
        if (hand == null || !hand.IsTracked) return;
        var f = hand.Fingers;   // 0 pouce, 1 index, 2 majeur, 3 annulaire, 4 auriculaire
        bool spider = f[0] > .6f && f[1] > .6f && f[4] > .6f && f[2] < .4f && f[3] < .4f;
        if (spider) { lastSpiderTime = Time.time; if (state != Signal.Spider) { state = Signal.Spider; light_.Set(state); } return; }
        if (state == Signal.Spider && Time.time - lastSpiderTime < spiderHold) return;
        if (state == Signal.Spider) { state = Signal.Amber; light_.Set(state); }   // sortie de Spider : on repart de l'orange

        float o = hand.Value;
        Signal next = state;
        switch (state)
        {
            case Signal.Red:   next = o > closedBelow + margin ? (o > openAbove ? Signal.Green : Signal.Amber) : Signal.Red; break;
            case Signal.Green: next = o < openAbove - margin ? (o < closedBelow ? Signal.Red : Signal.Amber) : Signal.Green; break;
            default:           next = o < closedBelow - margin ? Signal.Red : o > openAbove + margin ? Signal.Green : Signal.Amber; break;
        }
        if (next != state) { state = next; light_.Set(state); }
    }
}
