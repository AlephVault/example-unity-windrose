using System.Collections;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            /// <summary>
            ///   This interactor has no UI interaction. This one is used when you need to display
            ///     text without expecting any action from the user (not even a click on "ok" or
            ///     "continue" button).
            /// </summary>
            public class NullInteractor : Interactor
            {
                protected override IEnumerator Input(InteractiveMessage interactiveMessage)
                {
                    yield break;
                }
            }
        }
    }
}
