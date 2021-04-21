using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using AlephVault.Unity.Support.Utils;
using AlephVault.Unity.Support.Generic.Authoring.Types;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /// <summary>
            ///   <para>
            ///     Paging type implies the pagination technique used by the ListIterator. We can illustrate the differences by considering the following example:
            ///   </para>
            ///   
            ///   <para>
            ///     You have 13 elements, 3 displays, and you are after 3 moves to NEXT page (displaying items 9 10 and 11), and you go NEXT one more time.
            ///   </para>
            ///   
            ///   <para>
            ///     <list type="bullet">
            ///       <item>
            ///         <term>Snapped</term>
            ///         <description>You will get <c>items[12] nothing nothing</c> rendered, and you can't move further.</description>
            ///       </item>
            ///       <item>
            ///         <term>Looping</term>
            ///         <description>You will get <c>items[12] items[0] items[1]</c> rendered, and you can still move further: next page being rendered will involve items 2, 3, and 4.</description>
            ///       </item>
            ///       <item>
            ///         <term>Clamped</term>
            ///         <description>You will get <c>items[10] items[11] items[12]</c> rendered, and you can't move further. If you move back from there, you will render elements 7, 8, and 9 instead of the original 9, 10, and 11. After two more back movements you will render elements 1, 2, and 3. One more back will leave you at elements 0, 1, and 2.</description>
            ///       </item>
            ///     </list> 
            ///   </para>
            /// </summary>
            public enum PagingType
            {
                SNAPPED = 0,
                CLAMPED = 1,
                LOOPONG = 2
            }

            /// <summary>
            ///   An enumeration telling whether an object is unselected (NO), selected (YES) or
            ///     selected and also active (YES_ACTIVE).
            /// </summary>
            public enum SelectionStatus
            {
                NO, YES, YES_ACTIVE
            }

            /**
             * This interactor allows us to interact with a list of elements. There is no specific
             *   type for the list, since this interactor is a Generic class that should be
             *   overriden each time.
             * 
             * One must always do:
             *   1. Define which class will work as item source. Any class will do, but one should
             *        use a custom class or struct that holds complex data.
             *   2. Set the `itemDisplays` field in the editor to descendant objects that will serve
             *        as displays of the current data elements. It is recommended that those objects
             *        be an instance of the same prefab. Position will not matter, but perhaps their
             *        structure will.
             *   3. Define (Override) the RenderItem function according to their needs.
             *   4. Provide a way to call Reset, Move, MovePages, ... from the UI, or use the
             *        placeholders for the standard navigation buttons. The same applies for cancel
             *        and continue buttons.
             */
            /// <summary>
            ///   This input is a highly customizable list selector input. It will map list models
            ///     and selection values to their view in the UI and prompt the user for their
            ///     choice(s).
            /// </summary>
            /// <typeparam name="ListItem">The type acting as data model for sources</typeparam>
            /// <remarks>
            ///   This interactor is an abstract class. You must implement a subclass appropriately.
            /// </remarks>
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

                /// <summary>
                ///   Edit this variable in the Inspector to tell which objects will reflect the items
                ///     acting as displays of in-page choice items.
                /// </summary>
                /// <remarks>
                ///   <para>
                ///     At least one element must be set here, or an error will raise.
                ///   </para>
                ///   <para>
                ///     It is recommended that all items are instances of the same prefab.
                ///   </para>
                ///   <para>
                ///     It is recommended that all items are UI elements and, in particular, button
                ///       elements. Otherwise, you'll have to ensure another way they are selected
                ///       (toggled).
                ///   </para>
                /// </remarks>
                [SerializeField]
                GameObject[] itemDisplays;

                /// <summary>
                ///   The paging type. You can edit this member in the Inspector to customize your list.
                ///     See <see cref="PagingType"/> for an explanation. 
                /// </summary>
                [SerializeField]
                private PagingType pagingType = PagingType.SNAPPED;

                /// <summary>
                ///   Edit this member in the Inspector to control whether your listing allows multiple
                ///     selection or not. If multiple selection, remember that automatic selection &
                ///     commit of selected item is not available and will produce an error.
                /// </summary>
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

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a continue
                ///       button (thus terminating the interaction and choosing one or more elements).
                ///   </para>
                ///   <para>
                ///     This button will be disabled as long as there are no elements selected.
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button continueButton;

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a cancel
                ///       button (thus cancelling the interaction with no elements).
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button cancelButton;

                /****************************************************************************************
                 * 
                 * Navigation buttons are optional, and will implement Next, Prev, Next Page, Prev Page
                 *   and Reset. The user can opt to not use any or all of these buttons and implement
                 *   the desired logic to invoke those methods in the Start() inherited implementation.
                 * 
                 ****************************************************************************************/

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a "move
                ///       to next item" button.
                ///   </para>
                ///   <para>
                ///     If a button is specified, it will gain calling Move(1) on click.
                ///     Also, it will be disabled when it cannot move like that.
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button nextButton;

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a "move
                ///       to prev item" button.
                ///   </para>
                ///   <para>
                ///     If a button is specified, it will gain calling Move(-1) on click.
                ///     Also, it will be disabled when it cannot move like that.
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button prevButton;

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a "move
                ///       to next page" button.
                ///   </para>
                ///   <para>
                ///     If a button is specified, it will gain calling MovePage(1) on click.
                ///     Also, it will be disabled when it cannot move like that.
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button nextPageButton;

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a "move
                ///       to prev page" button.
                ///   </para>
                ///   <para>
                ///     If a button is specified, it will gain calling MovePage(-1) on click.
                ///     Also, it will be disabled when it cannot move like that.
                ///   </para>
                /// </summary>
                [SerializeField]
                private Button prevPageButton;

                /// <summary>
                ///   <para>
                ///     Edit this member in the Inspector to tell which <b>Button</b> will work as a "rewind"
                ///       button.
                ///   </para>
                ///   <para>
                ///     If a button is specified, it will gain calling Rewind() on click.
                ///     Also, it will be disabled when it cannot move like that.
                ///   </para>
                /// </summary>
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
                 *   list of elements to assing, and the selection choices to assign and retrieve.
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
                private OrderedSet<ListItem> selectedItems = new OrderedSet<ListItem>();

                /// <summary>
                ///   Gets/sets the items being selected. When you set this property, the last element in
                ///     the list you assign will be considered the "active" one.
                /// </summary>
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

                /// <summary>
                ///   Sets the widget to a page starting with the item 0.
                /// </summary>
                protected void Rewind()
                {
                    if (items == null) return;
                    position = 0;
                    RenderItems();
                }

                /// <summary>
                ///   Moves the renderer by N items. See <see cref="PagingType"/>
                ///     to get details of the behaviour here.
                /// </summary>
                /// <remarks>
                ///   By default invoked by Next/Prev buttons.
                /// </remarks>
                /// <param name="numItems">The said N value, which could be positive or negative.</param>
                protected void Move(int numItems)
                {
                    if (items == null) return;
                    if (itemDisplays.Length >= items.Count) return;

                    switch(pagingType)
                    {
                        case PagingType.LOOPONG:
                            numItems %= items.Count;
                            position = (items.Count + position + numItems) % items.Count;
                            break;
                        default:
                            position += numItems;
                            if (numItems < 0)
                            {
                                position = (position < 0) ? 0 : position;
                            }
                            else
                            {
                                position = (position > items.Count - itemDisplays.Length) ? items.Count - itemDisplays.Length : position;
                            }
                            break;
                    }
                    RenderItems();
                }

                /// <summary>
                ///   Moves the renderer by N pages. See <see cref="PagingType"/>
                ///     to get details of the behaviour here.
                /// </summary>
                /// <remarks>
                ///   By default invoked by Next Page / Prev Page buttons.
                /// </remarks>
                /// <param name="numItems">The said N value, which could be positive or negative.</param>
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
                            position += Values.Clamp<int>(min, numItems, max) * itemDisplays.Length;
                            break;
                        case PagingType.CLAMPED:
                            position = Values.Clamp<int>(0, position + numItems * itemDisplays.Length, items.Count - itemDisplays.Length);
                            break;
                        case PagingType.LOOPONG:
                            numItems = (numItems * itemDisplays.Length) % items.Count;
                            position = (items.Count + position + numItems) % items.Count;
                            break;
                    }

                    RenderItems();
                }

                /// <summary>
                ///   Selects one element, clearing others if single-select list is chosen.
                /// </summary>
                /// <remarks>
                ///   By default assigned to item's button components' click handlers when single-select listing is
                ///     chosen as configuration and no continue button is present.
                /// </remarks>
                /// <param name="index">Index to select.</param>
                /// <param name="relative">Whether the index is relative to the current position or not (true, by default, for the buttons).</param>
                protected void SelectOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (!multiSelect) selectedItems.Clear();
                    if (relative) index = (index + position) % items.Count();
                    selectedItems.Add(items[index]);
                    RenderItems();
                }

                /// <summary>
                ///   Unselects one element.
                /// </summary>
                /// <param name="index">Index to unselect.</param>
                /// <param name="relative">Whether the index is relative to the current position or not (true, by default, for the buttons).</param>
                protected void UnselectOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (relative) index = (index + position) % items.Count();
                    selectedItems.Remove(items[index]);
                    RenderItems();
                }

                /// <summary>
                ///   Selects one element (clearing others if single-select list is chosen) if not selected,
                ///     or unselects one element, if it is selected.
                /// </summary>
                /// <remarks>
                ///   By default assigned to item's button components' click handlers when multi-select listing is
                ///     chosen as configuration or the continue button is pressed.
                /// </remarks>
                /// <param name="index">Index to toggle.</param>
                /// <param name="relative">Whether the index is relative to the current position or not (true, by default, for the buttons).</param>
                protected void ToggleOne(int index, bool relative = false)
                {
                    if (items == null) return;

                    if (!multiSelect) selectedItems.Clear();
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

                /// <summary>
                ///   Selects all elements, if multi-select list is chosen.
                /// </summary>
                /// <remarks>
                ///   It leave silently on single-select lists.
                /// </remarks>
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

                /// <summary>
                ///   Unselects all elements.
                /// </summary>
                protected void UnselectAll()
                {
                    if (items == null) return;

                    selectedItems.Clear();
                    RenderItems();
                }

                /// <summary>
                ///   Tells whether this list's renderer can move to position 0.
                /// </summary>
                protected bool CanRewind()
                {
                    return items != null;
                }

                /// <summary>
                ///   Tells whether this list's renderer can move a certain offset of elements.
                /// </summary>
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
                            return (numItems < 0) && (0 <= newPosition) || (numItems > 0) && (newPosition <= items.Count - itemDisplays.Length);
                    }
                }

                /// <summary>
                ///   Tells whether this list's renderer can move a certain offset of pages.
                /// </summary>
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
                            return (numPages < 0) && (position > 0) || (numPages > 0) && (position < items.Count - itemDisplays.Length);
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
                 * Core implementation of this component. Interacts like this:
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

                /// <summary>
                ///   <para>
                ///     The main loop here interacts with the list and the paging buttons, rendering them
                ///       appropriately considering which page or offset this UI is located at.
                ///   </para>
                ///   <para>
                ///     Depending on the configuration, it will allow selecting one or more items and, after
                ///       the continue button was clicked (or upon its absence and being a single-select
                ///       list, an element was chosen), you can retrieve the selected items via
                ///       <see cref="SelectedItems"/> property. If the interaction was cancelled, you will
                ///       get an empty array in that property.
                ///   </para>
                /// </summary>
                /// <param name="interactiveMessage">
                ///   An instance of <see cref="InteractiveInterface"/>, first referenced by the instance of
                ///     <see cref="InteractiveInterface"/> that ultimately triggered this interaction. 
                /// </param>
                /// <returns>An enumerator to be run inside a coroutine.</returns>
                protected override async Task Input(InteractiveMessage message)
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
                        while (!HasResult)
                        {
                            await Tasks.Blink();
                        }
                        System.Collections.Generic.List<InteractiveMessage.Prompt> prompt = new System.Collections.Generic.List<InteractiveMessage.Prompt>();
                        ValidateSelection(SelectedItems, (InteractiveMessage.Prompt[] reported) => prompt.AddRange(reported));
                        if (prompt.Count > 0) {
                            allSelectedItemsAreValid = false;
                            await message.PromptMessages(prompt.ToArray());
                        }
                        // 4. Repeat until the validation does not fail.
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

                /// <summary>
                ///   Validates items being selected. By default, this implies validating every item separately. The user can freely
                ///     overwrite this method, and create a bypass to report the messages in a different way (e.g.by adding more
                ///     messages to be prompted).
                /// </summary>
                /// <remarks>If you override this one, you may need to invoke the base method.</remarks>
                /// <param name="selectedItems">The items to validate.</param>
                /// <param name="reportInvalidMessage">
                ///   Callback to invoke when you want to fail the validation. You can send several prompts there.
                /// </param>
                protected virtual void ValidateSelection(ListItem[] selectedItems, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
                {
                    foreach (ListItem item in selectedItems)
                    {
                        ValidateSelectedItem(item, reportInvalidMessage);
                    }
                }

                /**
                 * Validates an item being selected. It is up to the user to implement this method, or just
                 *   leave it as it is right now: no validation is performed by default.
                 * 
                 * When the user wants to fail a validation, all they must do is to invoke the function being
                 *   passed as second argument.
                 * 
                 * THIS METHOD SHOULD NOT HAVE/PRODUCE ANY SIDE EFFECT. The reason: this function will be
                 *   invoked in three different contexts:
                 *   
                 *   1. Initial check on the overall list of elements to see whether at least one is selectable.
                 *   2. Rendering the elements: Checking whether it is selectable will allow us to render the
                 *        element differently.
                 *   3. Actually validating a selection (submitting a result).
                 */
                /// <summary>
                ///   <para>
                ///     Validates an item being selected.It is up to the user to implement this method, or just
                ///       leave it as it is right now: no validation is performed by default.
                ///   </para>
                ///   <para>
                ///     When the user wants to fail a validation, all they must do is to invoke the function being
                ///       passed as second argument.
                ///   </para>
                /// </summary>
                /// <remarks>
                ///   <para>
                ///     THIS METHOD SHOULD NOT HAVE/PRODUCE ANY SIDE EFFECT.The reason: this function will be
                ///       invoked in three different contexts: initial check that determines that at least one item
                ///       is valid (and thus selectable), element render (the user may know how to render an invalid
                ///       element differently), and validating when submitting a result.
                ///   </para>
                /// </remarks>
                /// <param name="item">The list item being validated.</param>
                /// <param name="reportInvalidMessage">
                ///   Callback to invoke when you want to fail the validation. You can send several prompts there.
                /// </param>
                protected virtual void ValidateSelectedItem(ListItem item, Action<InteractiveMessage.Prompt[]> reportInvalidMessage)
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
                 * When starting, will also check and install buttons:
                 * 1. It is an error to not specify a continue button for a multiSelect list.
                 * 2. If no specifying continue button to a single select list, then clicking an
                 *      element will both select the item AND act as the default behaviour for
                 *      a present continue button.
                 * 3. A default behaviour for both continue and cancel button. Both will end the interaction!
                 *    The true difference between both buttons is that the continue button will be disabled
                 *      when no selection is made, and the cancel button also releases any selection. leaving
                 *      it empty.
                 * 4. A standard behaviour for standard navigation buttons.
                 */
                protected virtual void Start()
                {
                    if (itemDisplays.Length < 1)
                    {
                        throw new Types.Exception("The list of item displays must not be empty. Open the Editor and add at least one GameObject");
                    }

                    if (multiSelect && continueButton == null)
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
                                int currentIndex = i;
                                button.onClick.AddListener(() => ToggleOne(currentIndex, true));
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
                                int currentIndex = i;
                                button.onClick.AddListener(() => {
                                    SelectOne(currentIndex, true);
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
                        cancelButton.onClick.AddListener(() => {
                            SelectedItems = new ListItem[0];
                            RenderItems();
                            HasResult = true;
                        });
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
                        int endIndex = pagingType == PagingType.LOOPONG ? (position + i) % items.Count : position + i;
                        if (endIndex < items.Count)
                        {
                            ListItem item = items[endIndex];
                            GameObject display = itemDisplays[i];
                            SelectionStatus status = (selectedItems.Count > 0 && selectedItems.Last.Equals(item) ? SelectionStatus.YES_ACTIVE : (selectedItems.Contains(item) ? SelectionStatus.YES : SelectionStatus.NO));
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

                /// <summary>
                ///   <para>
                ///     This method is abstract - you are forced to implement this one because you are the one who will know
                ///       the structure of the rendering items (the prefab you choose) and the model. So, since you get an
                ///       item of an arbitrary type, and a render target of an arbitrary structure, you have to tell how
                ///       does the target renders according to the source item.
                ///   </para>
                ///   <para>
                ///     You will also be given more data: whether the element is selectable ("valid in particular"), and whether the
                ///       element is currently unselected, selected, or selected and also active (last selection).
                ///   </para>
                /// </summary>
                /// <param name="source">The source model object to update the target according to.</param>
                /// <param name="destination">The destination rendering object to update.</param>
                /// <param name="isSelectable">A flag telling whether the element is valid to be selected.</param>
                /// <param name="selectionStatus">A value telling about the selection.</param>
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
                        nextPageButton.interactable = CanMovePages(1);
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

                /// <summary>
                ///   You can override this behaviour to add more rendering steps. In this case,
                ///     these steps apply to the general UI and not to particular items.
                /// </summary>
                protected virtual void RenderExtraDetails()
                {
                    // No implementation. This empty implementation is safe.
                }
            }
        }
    }
}
