using UnityEngine;
using GameMeanMachine.Unity.GabTab.Authoring.Behaviours;
using GameMeanMachine.Unity.GabTab.Authoring.Behaviours.Interactors;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.Entities.Objects.CommandExchange.Talk;
using GameMeanMachine.Unity.WindRose.Authoring.Behaviours.UI;
using System.Threading.Tasks;

[RequireComponent(typeof(TalkReceiver))]
[RequireComponent(typeof(HUDLinker))]
public class SampleMeaninglessTalk : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TalkReceiver>().onTalkReceived.AddListener(delegate (GameObject sender)
        {
            gameObject.GetComponent<HUDLinker>().RunInteraction(MeaninglessInteraction);
        });
    }

    private async Task MeaninglessInteraction(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write("I refuse to believe the end of our world will arrive in such a beautiful day.").NewlinePrompt(true).Wait(0.5f).End());
    }
}

