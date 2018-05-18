using System.Collections;
using UnityEngine;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using WindRose.Behaviours.Objects;
using WindRose.Behaviours.Objects.CommandExchange;

[RequireComponent(typeof(InteractionLauncher))]
[RequireComponent(typeof(TalkReceiver))]
class SampleMeaninglessTalk : MonoBehaviour
{
    InteractionLauncher interactionLauncher;

    void Start()
    {
        interactionLauncher = GetComponent<InteractionLauncher>();
    }

    void OnTalkCommandReceived(GameObject sender)
    {
        interactionLauncher.InteractionTab.RunInteraction(MeaninglessInteraction);
    }

    IEnumerator MeaninglessInteraction(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write("I refuse to believe the end of our world will arrive in such a beautiful day.").NewlinePrompt(true).Wait(0.5f).End());
    }
}

