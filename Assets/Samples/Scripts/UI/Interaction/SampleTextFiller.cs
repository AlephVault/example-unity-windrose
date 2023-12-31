﻿using System.Linq;
using UnityEngine;
using AlephVault.Unity.GabTab.Authoring.Behaviours;
using AlephVault.Unity.GabTab.Authoring.Behaviours.Interactors;
using AlephVault.Unity.WindRose.GabTab.Authoring.Behaviours.UI;
using AlephVault.Unity.Support.Utils;
using System.Threading.Tasks;

[RequireComponent(typeof(HUD))]
public class SampleTextFiller : MonoBehaviour
{
    const string INTRO = "An unknown evil lurks in the outerspace and threatens our universe and God knows what other worlds.";
    const string QUESTION = "Our last hope is a once-heard legend to become true. Would you join our quest?";
    const string YOURNAME = "I'm glad to hear that! What is your name?";
    const string MISSING = "Well, let me call you \"{0}\".";
    const string ZODIAC_QUESTION = "What is your zodiacal sign?";
    const string ZODIAC_ANSWER = "Ok. Your sign is {0}.";
    const string ELEMENTS_QUESTION = "Which elements' powers do you want to pick?";
    const string ELEMENTS_ANSWER = "You are picking elements {0}.";
    const string CHARCLASS_QUESTION = "Which training would you like to take?";
    const string CHARCLASS_ANSWER = "You are picking training for {0}.";
    const string THANKYOU = "{0}, if our world survives I will ensure your name will be remembered for centuries.";
    const string FUCKOFF = "Then go home and eat a bag of d*cks, you f*cking lame. Our world is doomed.";

    private HUD hud;

    void Awake()
    {
        hud = gameObject.GetComponent<HUD>();
    }

    // Use this for initialization
    // THIS IS JUST AN EXAMPLE and not a real-life one. This Start method will be run as a coroutine
    //   just to give time to the interactive interfact to initialize.
    private async void Start()
    {
        // WARNING: If I remove this wait, the map will be loaded, but the sprites (images) will not be
        //          loaded until the Start() methods of the inner Sprite components run. This will
        //          cause the map-pause be executed, but the sprites will not be yet visible since most
        //          of this stuff was run in Awake(), but this method is Start() and the method that
        //          initializes the sprites is also Start() or Update().
        await Tasks.Blink();
        hud.RunInteraction(StartSampleMessages);
	}

    private async Task StartSampleMessages(InteractorsManager manager, InteractiveMessage interactiveMessage)
    {
        ButtonsInteractor yesnoInteractor = (ButtonsInteractor)manager["yesno-input"];
        NullInteractor nullInteractor = (NullInteractor)manager["null-input"];
        TextInteractor textInteractor = (TextInteractor)manager["string-input"];
        ZodiacListInteractor zodiacInteractor = (ZodiacListInteractor)manager["zodiac-input"];
        ElementListInteractor elementsInteractor = (ElementListInteractor)manager["elements-input"];
        CharacterClassListInteractor charClassInteractor = (CharacterClassListInteractor)manager["charclass-input"];
        await yesnoInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(INTRO).Wait().Clear().Write(QUESTION).Wait().End());
        if (yesnoInteractor.Result == "yes")
        {
            textInteractor.PlaceholderPrompt = "Enter your name ...";
            await textInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(YOURNAME).Wait().End());
            string name;
            if (textInteractor.Result == true)
            {
                name = textInteractor.Content;
            }
            else
            {
                name = "Anonymous";
                await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Write(string.Format(MISSING, name)).Wait().End());
            }
            await zodiacInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(ZODIAC_QUESTION).Wait().End());
            await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(string.Format(ZODIAC_ANSWER, zodiacInteractor.SelectedItems[0].Text)).Wait().End());
            await elementsInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(ELEMENTS_QUESTION).Wait().End());
            await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(string.Format(ELEMENTS_ANSWER, string.Join(", ", elementsInteractor.SelectedItems.Select((e) => e.Text).ToArray()))).Wait().End());
            await charClassInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(CHARCLASS_QUESTION).Wait().End());
            await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(string.Format(CHARCLASS_ANSWER, charClassInteractor.SelectedItems[0].Text)).Wait().End());
            await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(string.Format(THANKYOU, name)).Wait().End());
        }
        else
        {
            await nullInteractor.RunInteraction(interactiveMessage, new InteractiveMessage.PromptBuilder().Clear().Write(FUCKOFF).Wait().End());
        }
    }
}
