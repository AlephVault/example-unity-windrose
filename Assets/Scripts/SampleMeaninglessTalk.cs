using System.Linq;
using System.Collections;
using UnityEngine;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using WindRose.Behaviours.UI;
using WindRose.Behaviours.Objects.CommandExchange;
using Support.Utils;

[RequireComponent(typeof(TalkReceiver))]
class SampleMeaninglessTalk : MonoBehaviour
{
    [SerializeField]
    MapInteractiveInterface mapInteractiveInterface;

    InteractiveInterface interactiveInterface;

    void Start()
    {
        if (mapInteractiveInterface != null)
        {
            interactiveInterface = mapInteractiveInterface.GetComponent<InteractiveInterface>();
        }
    }

    void OnTalkCommandReceived(GameObject sender)
    {
        if (interactiveInterface)
        {
            interactiveInterface.RunInteraction(MeaninglessInteraction);
        }
    }

    IEnumerator MeaninglessInteraction(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write("I refuse to believe the end of our world will arrive in such a beautiful day.").NewlinePrompt(true).Wait(0.5f).End());
    }
}

