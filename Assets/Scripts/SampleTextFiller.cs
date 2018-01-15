using System.Collections;
using UnityEngine;
using GabTab.Behaviours;
using GabTab.Behaviours.Interactors;
using Support.Utils;

public class SampleTextFiller : MonoBehaviour
{
    const string INTRO = "An unknown evil lurks in the outerspace and threatens our universe and God knows what other worlds.";
    const string QUESTION = "Our last hope is a once-heard legend to become true. Would you join our quest?";
    const string YOURNAME = "I'm glad to hear that! What is your name?";
    const string MISSING = "Well, let me call you \"{0}\".";
    const string ZODIAC_QUESTION = "What is your zodiacal sign?";
    const string ZODIAC_ANSWER = "Ok. Your sign is {0}.";
    const string THANKYOU = "{0}, if our world survives I will ensure your name will be remembered for centuries.";
    const string FUCKOFF = "Then go home and eat a bag of d*cks, you f*cking lame. Our world is doomed.";

    private InteractiveInterface ui;

    // Use this for initialization
    // THIS IS JUST AN EXAMPLE and not a real-life one. This Start method will be run as a coroutine
    //   just to give time to the interactive interfact to initialize.
    IEnumerator Start ()
    {
        ui = Layout.RequireComponentInChildren<InteractiveInterface>(gameObject);
        yield return new WaitForSeconds(0.5f);
        ui.RunInteraction(StartSampleMessages);
	}

    IEnumerator StartSampleMessages(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        ButtonsInteractor yesnoInteractor = (ButtonsInteractor)manager["yesno-input"];
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        TextInteractor textInteractor = (TextInteractor)manager["string-input"];
        ZodiacListInteractor zodiacInteractor = (ZodiacListInteractor)manager["zodiac-input"];
        yield return yesnoInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
            new InteractiveMessage.Prompt(INTRO), new InteractiveMessage.Prompt(QUESTION)
        });
        if (yesnoInteractor.Result == "yes")
        {
            textInteractor.PlaceholderPrompt = "Enter your name ...";
            yield return textInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
                new InteractiveMessage.Prompt(YOURNAME)
            });
            string name;
            if (textInteractor.Result == true)
            {
                name = textInteractor.Content;
            }
            else
            {
                name = "Anonymous";
                yield return nullInteractor.RunInteraction(interactiveMessage, new[] {
                    new InteractiveMessage.Prompt(string.Format(MISSING, name), false)
                });
            }
            yield return zodiacInteractor.RunInteraction(interactiveMessage, new[] {
                new InteractiveMessage.Prompt(ZODIAC_QUESTION)
            });
            yield return nullInteractor.RunInteraction(interactiveMessage, new[] {
                new InteractiveMessage.Prompt(string.Format(ZODIAC_ANSWER, zodiacInteractor.SelectedItems[0].Text), true)
            });
            yield return nullInteractor.RunInteraction(interactiveMessage, new[] {
                new InteractiveMessage.Prompt(string.Format(THANKYOU, name), true)
            });
        }
        else
        {
            yield return nullInteractor.RunInteraction(interactiveMessage, new[] {
                new InteractiveMessage.Prompt(FUCKOFF)
            });
        }
    }
}
