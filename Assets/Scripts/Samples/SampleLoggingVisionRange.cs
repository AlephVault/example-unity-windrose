using UnityEngine;
using WindRose.Behaviours.Entities.Objects;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using System.Collections;

[RequireComponent(typeof(Watcher))]
class SampleLoggingVisionRange : MonoBehaviour
{
    private bool stillInside = false;
    private int x;
    private int y;

    private MapObject underlyingObject;

    void Awake()
    {
        underlyingObject = GetComponent<MapObject>();
        GetComponent<Watcher>().onWatcherReady.AddListener(delegate ()
        {
            Invoke("StartChatListener", 0.5f);
        });
    }

    void StartChatListener()
    {
        TriggerVisionRange visionRange = GetComponent<Watcher>().RelatedVisionRange;
        visionRange.onMapTriggerExit.AddListener((obj, platform, x, y) =>
        {
            stillInside = false;
        });
        visionRange.onMapTriggerMoved.AddListener((obj, platform, px, py) =>
        {
            if (!stillInside)
            {
                x = px;
                y = py;

                underlyingObject.RunInteraction(Gotcha);
                stillInside = true;
            }
        });
    }

    IEnumerator Gotcha(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(
            string.Format("Gotcha! Saw you at ({0}, {1}) of my sight", x, y)
        ).Wait().End());
    }
}
