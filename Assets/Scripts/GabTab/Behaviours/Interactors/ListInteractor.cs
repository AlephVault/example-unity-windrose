using System;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /**
             * This interactor allows us to interact with a list of elements. There is no specific
             *   type for the list, since this interactor is a Generic class that should be
             *   overriden each time.
             * 
             * One must always do:
             *   1. Define which class will work as item source. Any serialziable class will do, but
             *        one should use a custom class or struct that holds complex data.
             *   2. Set the `itemDisplays` field in the editor to descendant objects that will serve
             *        as displays of the current data elements. It is recommended that those objects
             *        be an instance of the same prefab. Position will not matter, but perhaps their
             *        structure will.
             *   3. Define (Override) the RenderItem function according to their needs.
             *   4. Provide a way to call Reset, Move, MovePages, ... from the UI, or use the
             *        placeholders for the standard navigation buttons. The same applies for cancel
             *        and continue buttons.
             */
            [RequireComponent(typeof(Image))]
            public abstract class ListInteractor<ListItem> : Interactor
            {
                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Component configuration. These members are static settings. These settings are not
                 *   meant to be changed during the lifecycle of this object since they will determine
                 *   the nature of this list selector.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * In the editor, we will set the items in this arrays to be:
                 * 1. Children/Descendants of the GameObject having this behaviour.
                 *    This is not required, but recommended. Even better if these
                 *      GameObject instances have a Button component attached (and
                 *      preferrably, with no visual transition between states).
                 *    By having a Button component, those objects become selectable
                 *      by default. Otherwise, the user must provide additional
                 *      behaviour or a way to make them react to being clicked.
                 * 2. Instances of the same prefab.
                 *    It is recommended that each item comes from the same prefab
                 *      so their aesthetics will be always the same.
                 * 3. At least one item must be specified! Otherwise an error will
                 *      be raised. Yes: we need at least one item to display.
                 */
                [SerializeField]
                GameObject[] itemDisplays;

                /**
                 * Whether the item rendering will be circular (e.g. if rendering 5 items starting from item
                 *   28th out of 30, the next items will be 20th, 30th, 1st and 2nd), paginated, or paginated
                 *   and clamped.
                 * 
                 * The difference between either is that if your page size (which will be determined by the
                 *   amount of elements in `itemDisplays`) is not a divider of the length of the list to pick
                 *   the data from, there will be (P-R) empty elements, where R is the remainder of the
                 *   division, in the "paginated" case, while in the "clamped" case, no empty elements will
                 *   be shown, but the offset will be clamped to the last P elements of the list.
                 */
                public enum PagingType
                {
                    SNAPPED = 0,
                    CLAMPED = 1,
                    LOOPONG = 2
                }
                [SerializeField]
                private PagingType pagingType = PagingType.SNAPPED;

                /**
                 * Whether multiple elements can be selected or not. Despite your choice here,
                 *   selecting items is mandatory for the "Continue" button to be enabled.
                 */
                [SerializeField]
                private bool multiSelect;

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Related components. They are the UI that will interact with this component. Most of
                 *   the times, this will involve buttons that will provide handlers when being clicked.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * If a "Continue" button is specified, it will be disabled when no element in the list is
                 *   selected. This button, when clicked, will end the interaction and keep the resulting
                 *   selected items.
                 * 
                 * If this button is not set and multiSelect is true, it is an exception: An explicit button
                 *   should exist to close the dialog. Such exception will be triggered when calling "Input".
                 * 
                 * If this button is not set and multiSelect is false, when an item is selected (and is allowed)
                 *   it will count automatically as the final result and the interaction will end. 
                 */
                [SerializeField]
                private Button continueButton;

                /**
                 * If a "Cancel" button is specified, the user can click such button and avoid selecting stuff.
                 * The flow will continue with no item being selected.
                 * 
                 * If a "Cancel" button is not specified, a null or empty list is specified (or all the items
                 *   in the list cannot be selected [do not pass validation]) when "Input" is called, an
                 *   exception will be thrown. For your good fortune, always provide a "Cancel" button.
                 */
                [SerializeField]
                private Button cancelButton;

                /****************************************************************************************
                 * 
                 * Navigation buttons are optional, and will implement Next, Prev, Next Page, Prev Page
                 *   and Reset. The user can opt to not use any or all of these buttons and implement
                 *   the desired logic to invoke those methods in the Start() inherited implementation.
                 * 
                 ****************************************************************************************/

                /**
                 * If a "Next" button is specified, it will gain calling Move(1) on click.
                 * Also, it will be disabled when it cannot move like that.
                 */
                [SerializeField]
                private Button nextButton;

                /**
                 * This is an analogous of the former, but calling Move(-1) instead.
                 */
                [SerializeField]
                private Button prevButton;

                /**
                 * This is analogous of the former, but calling MovePages(1) instead.
                 */
                [SerializeField]
                private Button nextPageButton;

                /**
                 * This is an analogous of the former, but calling MovePages(-1) instead.
                 */
                [SerializeField]
                private Button prevPageButton;

                /**
                 * This is an analogous of the former, but calling Rewind() instead.
                 */
                [SerializeField]
                private Button rewindButton;

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Internal state. This will track data elements that will change but not directly by the
                 *   user's choice but instead indirectly by user's actions.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * Current position of the list when picking elements. Will default to 0 and will reset when we
                 *   assign a new list. This position will be affected according to paging type, paging size, and
                 *   step-by-step navigation.
                 */
                private int position = 0;

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Code interaction data elements. One thing we are allowed to change via code is the
                 *   list of elements to assing, and the selection choices to assign and retrieve. Also
                 *   a validator function that would allow or disallow elements in such list to be
                 *   selected.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * The currently assigned list of items to cycle through.
                 */
                private System.Collections.Generic.List<ListItem> items;

                /**
                 * Sets/gets the list. When setting, this interactor will be reset.
                 */
                public System.Collections.Generic.List<ListItem> Items
                {
                    get
                    {
                        return items;
                    }
                    set
                    {
                        items = value;
                        Reset();
                    }
                }

                /**
                 * The currently selected item(s). The last selected item will be considered by us as the "active" item.
                 * However, it will be up to users to actually take care of such quality.
                 */
                private Support.Types.OrderedSet<ListItem> selectedItems;

                /**
                 * Get/Set the selected items from an array. The last one will be considered active.
                 */
                public ListItem[] SelectedItems
                {
                    get
                    {
                        ListItem[] output = new ListItem[selectedItems.Count];
                        selectedItems.CopyTo(output, 0);
                        return output;
                    }
                    set
                    {
                        selectedItems.Clear();
                        if (value != null && items != null)
                        {
                            if (multiSelect)
                            {
                                foreach (ListItem item in value)
                                {
                                    // Only allowing items in master list.
                                    if (items.Contains(item))
                                    {
                                        selectedItems.Add(item);
                                    }
                                }
                            }
                            else
                            {
                                // Only allowing item in master list, if an item is specified.
                                if (value.Length > 0 && items.Contains(value[value.Length - 1]))
                                {
                                    selectedItems.Add(value[value.Length - 1]);
                                }
                            }
                        }
                        RenderItems();
                    }
                }

                /**
                 * Whether the interaction should continue and we'd be ready to read the selected items,
                 *   or not.
                 */
                private bool HasResult = false;

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Navigation methods. They trigger many rendering events, change the position of the
                 *   navigation and, in case of Reset(), they also release the check marks.
                 * 
                 * Also there are selection methods allowing to select/deselect one/all elements.
                 * 
                 * All these methods but Reset(), may be triggered as a user's action.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * This private method resets the list position to the start.
                 * Also resets the choices, and the fact that it has a result.
                 */
                private void Reset()
                {
                    selectedItems.Clear();
                    HasResult = false;
                    Rewind();
                }

                /**
                 * This protected method resets the navigation position to the
                 *   start.
                 * Derived classes should make use of this method.
                 */
                protected void Rewind()
                {
                    if (items == null) return;
                    position = 0;
                    RenderItems();
                }

                /**
                 * This protected method advances/rollbacks the position by N items.
                 * Derived classes should make use of this method.
                 * 
                 * This method has no effect if the master list is not bigger than the
                 *   displays array.
                 */
                protected void Move(int numItems)
                {
                    if (items == null) return;
                    if (itemDisplays.Length >= items.Count) return;

                    switch(pagingType)
                    {
                        case PagingType.LOOPONG:
                            position = (position + numItems) % items.Count;
                            break;
                        default:
                            position = Support.Utils.Values.Clamp<int>(0, position + numItems, items.Count - 1);
                            break;
                    }
                    RenderItems();
                }

                /**
                 * This protected method advances/rollbacks the position by N pages.
                 * Derived classes should make use of this method.
                 * 
                 * This method has no effect if the master list is not bigger than the
                 *   displays array.
                 */
                protected void MovePages(int numItems)
                {
                    if (items == null) return;
                    if (itemDisplays.Length >= items.Count) return;

                    switch (pagingType)
                    {
                        case PagingType.SNAPPED:
                            // Snapped movement implies:
                            // 1. You can only move N steps backward, where N is the result of position / itemDisplays.Length
                            // 2. You can only move M steps forward, where M is the result of (items.Count - 1 - position) / itemDisplays.Length
                            int min = -(position / itemDisplays.Length);
                            int max = (items.Count - 1 - position) / itemDisplays.Length;
                            position += Support.Utils.Values.Clamp<int>(min, numItems, max) * itemDisplays.Length;
                            break;
                        case PagingType.CLAMPED:
                            position = Support.Utils.Values.Clamp<int>(0, position + numItems * itemDisplays.Length, items.Count - 1);
                            break;
                        case PagingType.LOOPONG:
                            position = (position + numItems * itemDisplays.Length) % items.Count;
                            break;
                    }

                    RenderItems();
                }

                /**
                 * This protected method selects one item by its index.
                 */
                protected void SelectOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (!multiSelect) selectedItems.Clear();
                    if (relative) index = (index + position) % items.Count();
                    selectedItems.Add(items[index]);
                    RenderItems();
                }

                /**
                 * This protected method unselects one item by its index.
                 */
                protected void UnselectOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (relative) index = (index + position) % items.Count();
                    selectedItems.Remove(items[index]);
                    RenderItems();
                }

                /**
                 * This protected method toggles one item by its index.
                 */
                protected void ToggleOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (relative) index = (index + position) % items.Count();
                    ListItem item = items[index];
                    if (selectedItems.Contains(item))
                    {
                        selectedItems.Remove(item);
                    }
                    else
                    {
                        selectedItems.Add(item);
                    }
                    RenderItems();
                }

                /**
                 * This protected method selects all items.
                 */
                protected void SelectAll()
                {
                    if (items == null) return;
                    if (!multiSelect) return;

                    foreach (ListItem item in items)
                    {
                        selectedItems.Add(item);
                    }
                    RenderItems();
                }

                /**
                 * This protected method unselects all items.
                 */
                protected void UnselectAll()
                {
                    if (items == null) return;

                    selectedItems.Clear();
                    RenderItems();
                }

                /**
                 * Utility method to check whether Rewind() will have effect or not.
                 */
                protected bool CanRewind()
                {
                    return items != null;
                }

                /**
                 * Utility method to check whether `Move(numItems)` will have effect or not.
                 */
                protected bool CanMove(int numItems)
                {
                    if (items == null) return false;
                    if (itemDisplays.Length >= items.Count) return false;

                    switch (pagingType)
                    {
                        case PagingType.LOOPONG:
                            return true;
                        default:
                            int newPosition = position + numItems;
                            return 0 <= newPosition && newPosition <= items.Count - 1;
                    }
                }

                /**
                 * Utility method to check whether MovePages(x) will have effect or not.
                 */
                protected bool CanMovePages(int numPages)
                {
                    if (items == null) return false;
                    if (itemDisplays.Length >= items.Count) return false;

                    switch (pagingType)
                    {
                        case PagingType.SNAPPED:
                            int min = -(position / itemDisplays.Length);
                            int max = (items.Count - 1 - position) / itemDisplays.Length;
                            return min <= numPages && numPages <= max;
                        case PagingType.CLAMPED:
                            int newPosition = position + numPages * itemDisplays.Length;
                            return 0 <= newPosition && newPosition <= items.Count - 1;
                        case PagingType.LOOPONG:
                            return true;
                        default:
                            // There is no a distinct case here. This will not happen.
                            return false;
                    }
                }

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Core implementation of this component.
                 * 
                 * TO-DO
                 * 
                 * Will interact like this:
                 * 
                 * do
                 *   1. yield wait until a result if available
                 *   2. check if any selected element is invalid
                 *      Notes: The very checking of the validity should trigger the display of the
                 *        appropriate error messages.
                 * while any selected element is invalid
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * Executes the interaction by looping like this:
                 * 1. Expect a result of selected items.
                 * 2. Expect all of them be valid elements. Otherwise, messages will be reported to
                 *      the console (interactive message). Perhaps even providing heading and trailing
                 *      messages for the overall errors.
                 * 3. Keep valid items.
                 */
                protected override IEnumerator Input(InteractiveMessage message)
                {
                    if (cancelButton == null && !AtLeastOneSelectableItem())
                    {
                        throw new Types.Exception("A list interactor with no cancel button must provide at least one item in the list of selectable items when calling Input(). Otherwise, the interaction will no way of ending");
                    }

                    // Reset the list's position
                    Rewind();

                    // Start this whole loop
                    bool allSelectedItemsAreValid;
                    do
                    {
                        allSelectedItemsAreValid = true;
                        // On each loop we will:
                        // 1. Release the fact that we have a result.
                        // 2. Wait for a result (i.e. a selection).
                        HasResult = false;
                        yield return new WaitUntil(() => HasResult);
                        // 3. Validate the selection.
                        foreach(ListItem item in SelectedItems)
                        {
                            // We will not only iterate and report the validation errors.
                            // We will also report the initial message for validation errors.
                            InteractiveMessage.Prompt[] prompt = null;
                            AfterSelectionValidationErrors((InteractiveMessage.Prompt[] reported) => { prompt = reported; });
                            if (prompt != null)
                            {
                                // First comes the initial message (for the first message only)
                                if (allSelectedItemsAreValid)
                                {
                                    // The condition being true implies this is our first validation error
                                    InteractiveMessage.Prompt[] initialPrompt = null;
                                    AfterSelectionValidationErrors((InteractiveMessage.Prompt[] reported) => { initialPrompt = reported; });
                                    if (initialPrompt != null) yield return message.PromptMessages(initialPrompt);
                                }

                                // Then comes the validation error, and clearing the flag
                                yield return message.PromptMessages(prompt);
                                allSelectedItemsAreValid = false;
                            }
                        }
                        // 4. Report a final message when validation failed.
                        if (!allSelectedItemsAreValid)
                        {
                            InteractiveMessage.Prompt[] finalPrompt = null;
                            AfterSelectionValidationErrors((InteractiveMessage.Prompt[] reported) => { finalPrompt = reported; });
                            if (finalPrompt != null) yield return message.PromptMessages(finalPrompt);
                        }
                        // 5. Repeat until the validation does not fail.
                    }
                    while (!allSelectedItemsAreValid);
                    // At this point, each item in SelectedItems is valid
                }

                /**
                 * Tells whether the current list has at least one selectable element.
                 */
                private bool AtLeastOneSelectableItem()
                {
                    // nulls lists do not have items.
                    if (items == null) return false;

                    // we tell whether there is at least one item satisfying the validator.
                    return items.Where((ListItem item) => ItemIsSelectable(item)).Any();
                }

                /**
                 * Validates an item being selected. It is up to the user to implement this method, or just
                 *   leave it as it is right now: no validation is performed by default.
                 * 
                 * When the user wants to fail a validation, all they must do is to invoke the function being
                 *   passed as second argument.
                 * 
                 * THIS FUNCTION SHOULD NOT HAVE/PRODUCE ANY SIDE EFFECT. The reason: this function will be
                 *   invoked in three different contexts:
                 *   
                 *   1. Initial check on the overall list of elements to see whether at least one is selectable.
                 *   2. Rendering the elements: Checking whether it is selectable will allow us to render the
                 *        element differently.
                 *   3. Actually validating a selection (submitting a result).
                 */
                protected void ValidateSelectedItem(ListItem item, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
                {
                }

                /**
                 * Allows us to report an initial message before the first validation error.
                 * It is actually safe to not implement this one.
                 * 
                 * PLEASE NOTE: The amount or nature of error messages is not known at this point.
                 */
                protected void BeforeSelectionValidationErrors(Action<InteractiveMessage.Prompt[]> reportMessage)
                {
                }

                /**
                 * Allows us to report an initial message after the last validation error.
                 * It is actually safe to not implement this one.
                 */
                protected void AfterSelectionValidationErrors(Action<InteractiveMessage.Prompt[]> reportMessage)
                {
                }

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * The Start method will set the appropriate callbacks for the buttons.
                 * 
                 * This method should be overriden by a descendant class if it needs more behaviour.
                 *   However, remember calling base.Start() beforehand;
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                /**
                 * When starting, it will check whether the type is serializable, and yell if not.
                 * Then it will also check and install buttons:
                 * 1. It is an error to not specify a continue button for a multiSelect list.
                 * 2. If no specifying continue button to a single select list, then clicking an
                 *      element will both select the item AND act as the default behaviour for
                 *      a present continue button.
                 * 3. A default behaviour for both continue and cancel button. It will be the same for both!
                 *    The true difference between both buttons is that the continue button will be disabled
                 *      when no selection is made.
                 * 4. A standard behaviour for standard navigation buttons.
                 */
                protected void Start()
                {
                    if (!typeof(ListItem).IsSerializable)
                    {
                        throw new Types.Exception("The type argument must be serializable for this class to work");
                    }

                    if (itemDisplays.Length < 1)
                    {
                        throw new Types.Exception("The list of item displays must not be empty. Open the Editor and add at least one GameObject");
                    }

                    if (multiSelect && continueButton != null)
                    {
                        throw new Types.Exception("No continue button is specified, and the list has multiSelect=false - There is no way to end the interaction positively");
                    }

                    // Setting click handlers for GameObjects that have buttons
                    // There are two possibilities:
                    // 1. If no continue button (it will also be single-select list)
                    //    -> SelectOne(i, true); HasResult = true;
                    // 2. If continue button
                    //    -> ToggleOne(i, true);
                    if (continueButton != null)
                    {
                        for (int i = 0; i < itemDisplays.Length; i++)
                        {
                            Button button = itemDisplays[i].GetComponent<Button>();
                            if (button)
                            {
                                button.onClick.AddListener(() => ToggleOne(i, true));
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < itemDisplays.Length; i++)
                        {
                            Button button = itemDisplays[i].GetComponent<Button>();
                            if (button)
                            {
                                button.onClick.AddListener(() => {
                                    SelectOne(i, true);
                                    HasResult = true;
                                });
                            }
                        }
                    }

                    // Setting click handlers for standard buttons, if assigned
                    if (nextButton)
                    {
                        nextButton.onClick.AddListener(() => Move(1));
                    }

                    if (nextPageButton)
                    {
                        nextPageButton.onClick.AddListener(() => MovePages(1));
                    }

                    if (prevButton)
                    {
                        prevButton.onClick.AddListener(() => Move(-1));
                    }

                    if (prevPageButton)
                    {
                        prevPageButton.onClick.AddListener(() => MovePages(-1));
                    }

                    if (rewindButton)
                    {
                        rewindButton.onClick.AddListener(() => Rewind());
                    }

                    if (continueButton)
                    {
                        continueButton.onClick.AddListener(() => HasResult = true);
                    }

                    if (cancelButton)
                    {
                        cancelButton.onClick.AddListener(() => HasResult = true);
                    }

                    //Finally, an initial reset.
                    Reset();
                }

                /*****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 * Internal implementations and template methods of this component. Great part of the
                 *   magic will actually go here. These are all related to the rendering of the component.
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************
                 *****************************************************************************************/

                public enum SelectionStatus
                {
                    NO, YES, YES_ACTIVE
                }

                /**
                 * This method re-renders the items accordingly. Is called by Reset, Rewind and Move* methods.
                 * There will be display items not being rendered (And instead being hidden) when no data is
                 *   present for them in the items list.
                 * 
                 * Also standard buttons will be enabled/disabled, and perhaps more custom user implementation
                 *   will be run in the lifecycle given by this method.
                 */
                private void RenderItems()
                {
                    for(int i = 0; i < itemDisplays.Length; i++)
                    {
                        if (position + i < items.Count)
                        {
                            ListItem item = items[position + i];
                            GameObject display = itemDisplays[i];
                            SelectionStatus status = (selectedItems.Last.Equals(item) ? SelectionStatus.YES_ACTIVE : (selectedItems.Contains(item) ? SelectionStatus.YES : SelectionStatus.NO));
                            bool selectable = ItemIsSelectable(item);
                            RenderItem(item, display, selectable, status);
                            itemDisplays[i].SetActive(true);
                        }
                        else
                        {
                            itemDisplays[i].SetActive(false);
                        }
                    }
                    RefreshStandardButtons();
                    RenderExtraDetails();
                }

                /**
                 * Validates an item silently, just to pass the true/false indicator. Nothing is done with the reported error.
                 */
                private bool ItemIsSelectable(ListItem item)
                {
                    bool selectable = true;
                    ValidateSelectedItem(item, (InteractiveMessage.Prompt[] prompt) => { selectable = false; });
                    return selectable;
                }

                /**
                 * This protected method must be implemented! It will render a chunk of
                 *   data elements on the game objects in `itemDisplays`. Each element in
                 *   `itemDisplays` will also be styled depending on whether it is selected
                 *   or not.
                 */
                protected abstract void RenderItem(ListItem source, GameObject destination, bool isSelectable, SelectionStatus selectionStatus);

                /**
                 * Refreshes the standard buttons, whether they may be clicked or not.
                 */
                private void RefreshStandardButtons()
                {
                    if (nextButton != null)
                    {
                        nextButton.interactable = CanMove(1);
                    }

                    if (nextPageButton != null)
                    {
                        nextButton.interactable = CanMovePages(1);
                    }

                    if (prevButton != null)
                    {
                        prevButton.interactable = CanMove(-1);
                    }

                    if (prevPageButton != null)
                    {
                        prevPageButton.interactable = CanMovePages(-1);
                    }

                    if (rewindButton != null)
                    {
                        rewindButton.interactable = CanRewind();
                    }

                    if (continueButton != null)
                    {
                        continueButton.interactable = SelectedItems.Count() > 0;
                    }
                }

                /**
                 * This protected method is optional, at user's criteria.
                 * We could need more stuff to render in our component (e.g. a descriptive text telling "N items selected").
                 * If the user needs to update, enable, or disable components (e.g. custom navigation buttons), they should do
                 *   that in THIS method.
                 */
                protected void RenderExtraDetails()
                {
                    // No implementation. This empty implementation is safe.
                }
            }
        }
    }
}
