using System.Collections;
using UnityEngine;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using WindRose.Behaviours.Entities.Objects;
using WindRose.Behaviours.Entities.Objects.CommandExchange.Talk;
using System.Threading.Tasks;

[RequireComponent(typeof(TalkReceiver))]
class SampleMeaninglessTalk : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TalkReceiver>().onTalkReceived.AddListener(delegate (GameObject sender)
        {
            sender.GetComponent<MapObject>().RunInteraction(MeaninglessInteraction);
        });
    }

    private async Task MeaninglessInteraction(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write("I refuse to believe the end of our world will arrive in such a beautiful day.").NewlinePrompt(true).Wait(0.5f).End());
    }
}

