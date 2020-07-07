using System.Text;
using System.Collections;
using UnityEngine;
using GMM.Utils;
using System.Threading.Tasks;

namespace GabTab
{
    namespace Behaviours
    {
        /// <summary>
        ///   This component reflects the settings of text speed and explicit wait speed.
        /// </summary>
        /// <remarks>
        ///   <para>
        ///     It also provides methods used by <see cref="InteractiveMessage"/> to yield
        ///       timing coroutines to perform the dirty job for us. Such coroutines are
        ///       started in terms of this object's configuration.
        ///   </para>
        ///   <para>
        ///     You should configure this object's components as well. Recommended settings are:
        ///     <list type="bullet">
        ///       <item>
        ///         <term>Text</term>
        ///         <description>
        ///           This component will ultimately display the text. The recommended settings are:
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Paragraph</term>
        ///               <description>
        ///                 <list type="bullet">
        ///                   <item>
        ///                     <term>Alignment</term>
        ///                     <description>Left and Top</description>
        ///                   </item>
        ///                   <item>
        ///                     <term>Horizontal Overflow</term>
        ///                     <description>Wrap</description>
        ///                   </item>
        ///                   <item>
        ///                     <term>Vertical Overflow</term>
        ///                     <description>Overflow</description>
        ///                   </item>
        ///                 </list> 
        ///               </description>
        ///             </item>
        ///             <item>
        ///               <term>Character</term>
        ///               <description>
        ///                 <list type="bullet">
        ///                   <item>
        ///                     <term>Line Spacing</term>
        ///                     <description>1</description>
        ///                   </item>
        ///                 </list>
        ///               </description>
        ///             </item>
        ///           </list>
        ///         </description>
        ///       </item>
        ///       <item>
        ///         <term>Content Size Fitter</term>
        ///         <description>
        ///           This component is used to scroll appropriately. Recommended:
        ///           <list type="bullet">
        ///             <item>
        ///               <term>Horizontal Fit</term>
        ///               <description>Unconstrained</description>
        ///             </item>
        ///             <item>
        ///               <term>Vertical Fit</term>
        ///               <description>Preferred Size</description>
        ///             </item>
        ///           </list> 
        ///         </description>
        ///       </item>
        ///     </list> 
        ///   </para>
        /// </remarks>
        [RequireComponent(typeof(UnityEngine.UI.Text))]
        [RequireComponent(typeof(UnityEngine.UI.ContentSizeFitter))]
        public class InteractiveMessageContent : MonoBehaviour
        {
            /// <summary>
            ///   Tells the amount of seconds (usually fraction of) to wait
            ///     between each letter being displayed for slow speed.
            /// </summary>
            [SerializeField]
            private float slowTimeBetweenLetters = 0.05f;

            /// <summary>
            ///   Tells the amount of seconds (usually fraction of) to wait
            ///     between each letter being displayed for quick speed.
            /// </summary>
            [SerializeField]
            private float quickTimeBetweenLetters = 0.005f;

            /// <summary>
            ///   Tells the amount of seconds (usually fraction of) to wait
            ///     on each wait being issued for slow speed.
            /// </summary>
            [SerializeField]
            private float slowDelayAfterMessage = 0.5f;

            /// <summary>
            ///   Tells the amount of seconds (usually fraction of) to wait
            ///     on each wait being issued for quick speed.
            /// </summary>
            [SerializeField]
            private float quickDelayAfterMessage = 0.05f;

            /// <summary>
            ///   Tells whether text must be displayed, and waits should be
            ///     performed, at quick speed or slow speed.
            /// </summary>
            public bool QuickTextMovement = false;

            /// <summary>
            ///   This coroutine waits using character-sized time.
            /// </summary>
            /// <returns>A coroutine to start and wait.</returns>
            public async Task CharacterWaiterCoroutine()
            {
                float currentTime = 0;
                float quick = Values.Max(0.00001f, quickTimeBetweenLetters);
                float slow = Values.Max(0.0001f, slowTimeBetweenLetters);
                while (currentTime < (QuickTextMovement ? quick : slow))
                {
                    await Task.Yield();
                    currentTime += Time.deltaTime;
                }
            }

            /// <summary>
            ///   This coroutine waits using an explicit time, which is usually
            ///     bigger than character wait times.
            /// </summary>
            /// <param name="seconds">
            ///   If you specify this, it will wait such amount on slow speed (and such amount divided by 10 on quick speed) instead of the setup wait times.
            /// </param>
            /// <returns>A coroutine to start and wait.</returns>
            public async Task ExplicitWaiter(float? seconds = null)
            {
                if (seconds == null)
                {
                    float currentTime = 0;
                    float quick = Values.Max(0.00001f, quickDelayAfterMessage);
                    float slow = Values.Max(0.0001f, slowDelayAfterMessage);
                    while (currentTime < (QuickTextMovement ? quick : slow))
                    {
                        await Task.Yield();
                        currentTime += Time.deltaTime;
                    }
                }
                else
                {
                    float currentTime = 0;
                    float quick = Values.Max(0.00001f, seconds.Value / 10);
                    float slow = Values.Max(0.0001f, seconds.Value);
                    while (currentTime < (QuickTextMovement ? quick : slow))
                    {
                        await Task.Yield();
                        currentTime += Time.deltaTime;
                    }
                }
            }
        }
    }
}