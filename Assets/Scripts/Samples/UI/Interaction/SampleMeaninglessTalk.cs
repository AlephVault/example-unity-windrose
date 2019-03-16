using System.Collections;
using UnityEngine;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Behaviours.Entities.Objects.CommandExchange.Talk;

[RequireComponent(typeof(InteractionLauncher))]
[RequireComponent(typeof(TalkReceiver))]
class SampleMeaninglessTalk : MonoBehaviour
{
    InteractionLauncher interactionLauncher;

    void Awake()
    {
        GetComponent<TalkReceiver>().onTalkReceived.AddListener(delegate (GameObject sender)
        {
            interactionLauncher.InteractionTab.RunInteraction(MeaninglessInteraction);
        });
    }

    void Start()
    {
        interactionLauncher = GetComponent<InteractionLauncher>();
    }

    IEnumerator MeaninglessInteraction(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write("I refuse to believe the end of our world will arrive in such a beautiful day.").NewlinePrompt(true).Wait(0.5f).End());
    }
}

