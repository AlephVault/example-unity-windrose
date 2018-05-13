using UnityEngine;
using WindRose.Behaviours;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using System.Collections;

[RequireComponent(typeof(Watcher))]
class SampleLoggingVisionRange : MonoBehaviour
{
    private bool stillInside = false;
    private int x;
    private int y;

    [SerializeField]
    private InteractiveInterface ui;

    // Use this for initialization
    void OnWatcherReady()
    {
        Invoke("StartChatListener", 0.5f);
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
                ui.RunInteraction(Gotcha);
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
