using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            namespace Interactors
            {
                /**
                 * This class serves as a base for an interactor.
                 * An interactor is, at a glance, just a game object (which will have
                 *   many children game objects serving as buttons and other UI elements)
                 *   that will be integrated with the lifecycle of an interactive interface
                 *   to actually communicate to the user.
                 * 
                 * An interactor is a component in a game object. It is only useful when
                 *   the game object belongs to a hierarchy that starts with an interactive
                 *   interface.
                 * 
                 * A typical interaction will have one or more texts being displayed in
                 *   an interactive message. After the messages are all shown, the interactive
                 *   part (e.g. buttons) will be displayed. This will all be handled by the
                 *   interactive interface.
                 * 
                 * This component becomes relevant when talking about the interactive parts
                 *   being displayed.
                 */
                abstract class Interactor : MonoBehaviour
                {
                    public delegate void InteractorPreparer(Interactor component);

                    /**
                     * A preparer function can be used to prepare the component status before even displaying the game object.
                     * If specified, it will be executed, allowing the user to initialize component properties before the
                     *   interaction starts.
                     * 
                     * The actual implementation is in the Input() method, which must be overriden by the user.
                     */
                    public Coroutine RunInteraction(InteractorPreparer preparer = null)
                    {
                        return StartCoroutine(WrappedInteraction(preparer));
                    }

                    private IEnumerator WrappedInteraction(InteractorPreparer preparer = null)
                    {
                        if (gameObject.active)
                        {
                            throw new Types.Exception("Cannot run the interaction: A previous interaction is already running");
                        }

                        if (preparer != null)
                        {
                            preparer(this);
                        }
                        gameObject.SetActive(true);
                        yield return StartCoroutine(Input());
                        gameObject.SetActive(false);
                    }

                    protected abstract IEnumerator Input();
                }
            }
        }
    }
}
