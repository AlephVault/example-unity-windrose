using System.Collections;
using UnityEngine;
using WindRose.Behaviors.UI;
using WindRose.Behaviors.UI.Interactors;

public class SampleTextFiller : MonoBehaviour
{
    const string LOREM = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed nec mattis tellus. Nulla pellentesque rutrum est eu porttitor. Phasellus dapibus blandit mauris at iaculis.";
    const string IPSUM = "Aliquam consequat, tellus non consequat sollicitudin, ligula dui pellentesque ipsum, eget lobortis urna turpis ac augue. Nullam sed faucibus eros. Fusce vitae ex sapien. Sed in massa eget tellus ultricies aliquam. Maecenas a euismod ante, vitae molestie arcu.";

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
        Debug.Log("SampleTextFiller::StartSimpleMessages");
        yield return manager["null-input"].RunInteraction(interactiveMessage, new InteractiveMessage.Prompt[] {
            new InteractiveMessage.Prompt(LOREM), new InteractiveMessage.Prompt(IPSUM)
        });
    }
}
