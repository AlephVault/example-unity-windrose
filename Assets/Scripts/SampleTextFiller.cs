using System.Collections;
using UnityEngine;
using WindRose.Behaviors.UI;
using WindRose.Behaviors.UI.Interactors;

public class SampleTextFiller : MonoBehaviour
{
    const string INTRO = "An unknown evil lurks in the outerspace and threatens our universe and God knows what other worlds.";
    const string QUESTION = "Our last hope is a once-heard legend to become true. Would you join our quest?";
    const string THANKYOU = "We are glad to hear! If our world survives, I will ensure your name will be remembered for centuries.";
    const string FUCKOFF = "Then go home and eat a bag of d*cks, you f*cking lame. Our world is doomed.";

    private InteractiveInterface ui;

    // Use this for initialization
    // THIS IS JUST AN EXAMPLE and not a real-life one. This Start method will be run as a coroutine
    //   just to give time to the interactive interfact to initialize.
    IEnumerator Start ()
    {
        ui = WindRose.Utils.Layout.RequireComponentInChildren<InteractiveInterface>(gameObject);
        yield return new WaitForSeconds(0.5f);
        ui.RunInteraction(StartSampleMessages);
	}

    IEnumerator StartSampleMessages(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        ButtonsInteractor yesnoInteractor = (ButtonsInteractor) manager["yesno-input"];
        NullInteractor nullInteractor = (NullInteractor) manager["null-input"];
        yield return yesnoInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
            new InteractiveMessage.Prompt(INTRO), new InteractiveMessage.Prompt(QUESTION)
        });
        if (yesnoInteractor.Result == "yes")
        {
            yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
                new InteractiveMessage.Prompt(THANKYOU)
            });
        }
        else
        {
            yield return nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
                new InteractiveMessage.Prompt(FUCKOFF)
            });
        }
    }
}
