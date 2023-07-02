using System;
using AlephVault.Unity.GabTab.Authoring.Behaviours;
using AlephVault.Unity.GabTab.Authoring.Behaviours.Interactors;
using UnityEngine;
using UnityEngine.UI;

public class TextOption
{
    public readonly string Key;
    public readonly string Text;
    public readonly bool Available;

    public TextOption(string key, string text, bool available = true)
    {
        Key = key;
        Text = text;
        Available = available;
    }

    public override string ToString()
    {
        return string.Format("[{0}:{1}]", Key, Text);
    }
}

public class TextOptionListInteractor : ListInteractor<TextOption>
{
    protected override void RenderItem(TextOption source, GameObject destination, bool isSelectable, SelectionStatus selectionStatus)
    {
        Text text = destination.GetComponentInChildren<Text>();
        Image image = destination.GetComponentInChildren<Image>();
        image.color = (selectionStatus == SelectionStatus.NO) ? Color.white : Color.blue;
        text.color = isSelectable ? Color.black : Color.red;
        text.text = source.Text;
    }

    protected override void ValidateSelectedItem(TextOption item, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
    {
        if (!item.Available)
        {
            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Write(string.Format("{0} is not available.\n", item.Text)).Wait().End());
        }
    }
}