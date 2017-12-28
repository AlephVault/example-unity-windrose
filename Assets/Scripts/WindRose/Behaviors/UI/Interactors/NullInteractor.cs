using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Behaviors
    {
        namespace UI
        {
            namespace Interactors
            {
                public class NullInteractor : Interactor
                {
                    /**
                     * A null interactor does nothing. When the messages end displaying, the interactor
                     *   terminates. One would prefer the simple "End"/"Continue" terminator (i.e. a
                     *   button or game input with those semantics), however, this one serves also for
                     *   a testing purpose.
                     */
                    protected override IEnumerator Input(InteractiveMessage interactiveMessage)
                    {
                        yield break;
                    }
                }
            }
        }
    }
}
