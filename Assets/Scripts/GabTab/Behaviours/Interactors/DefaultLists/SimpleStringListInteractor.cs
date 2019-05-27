using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            namespace DefaultLists
            {
                /// <summary>
                ///   A simple implementation of list interactor mapping a dictionary of
                ///     string keys to string labels. Optionally, many string keys may
                ///     be selected as "invalid" (they will count as disabled items).
                /// </summary>
                public class SimpleStringListInteractor : ListInteractor<SimpleStringModel>
                {
                    /// <summary>
                    ///   The text color for when the option is enabled.
                    /// </summary>
                    public Color enabledTextColor = Color.black;

                    /// <summary>
                    ///   The background color for when the option is enabled.
                    /// </summary>
                    public Color enabledBackgroundColor = new Color(0, 0, 0, 0);

                    /// <summary>
                    ///   The text color for when the option is disabled.
                    /// </summary>
                    public Color disabledTextColor = Color.gray;

                    /// <summary>
                    ///   The background color for when the option is disabled.
                    /// </summary>
                    public Color disabledBackgroundColor = new Color(0, 0, 0, 0);

                    /// <summary>
                    ///   The text color for when the option is selected.
                    /// </summary>
                    public Color selectedTextColor = Color.blue;

                    /// <summary>
                    ///   The background color for when the option is selected.
                    /// </summary>
                    public Color selectedBackgroundColor = new Color(0, 0, 0, 0);

                    /// <summary>
                    ///   The text for when an invalid option is selected. You can use {0} placeholder
                    ///     to interpolate the label of the selected option.
                    /// </summary>
                    public string InvalidSelectionTemplate = "Invalid option: {0}";

                    /// <summary>
                    ///   This implementation changes text color and background color according to
                    ///     what is told to the component.
                    /// </summary>
                    /// <param name="source">The source <see cref="SimpleStringModel"/></param>
                    /// <param name="destination">The rendering target</param>
                    /// <param name="isSelectable">Whether the item is valid or not</param>
                    /// <param name="selectionStatus">Tells whether the item is selected, unselected, or selected AND active</param>
                    protected override void RenderItem(SimpleStringModel source, GameObject destination, bool isSelectable, SelectionStatus selectionStatus)
                    {
                        Text label = destination.GetComponentInChildren<Text>();
                        Image image = destination.GetComponent<Image>();
                        if (image)
                        {
                            image.color = isSelectable ? (selectionStatus != SelectionStatus.NO ? selectedBackgroundColor : enabledBackgroundColor) : disabledBackgroundColor;
                        }

                        if (label)
                        {
                            label.color = isSelectable ? (selectionStatus != SelectionStatus.NO ? selectedTextColor : enabledTextColor) : disabledTextColor;
                            label.text = source.Value;
                        }
                    }

                    protected override void ValidateSelectedItem(SimpleStringModel item, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
                    {
                        if (!item.Enabled)
                        {
                            reportInvalidMessage(new InteractiveMessage.PromptBuilder().Write(string.Format(InvalidSelectionTemplate, item.Value)).Wait().End());
                        }
                    }
                }
            }
        }
    }
}
