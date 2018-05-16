using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Oriented))]
        [RequireComponent(typeof(EventDispatcher))]
        public class Watcher : MonoBehaviour
        {
            /**
             * Installs a TriggerVisionRange for this component.
             */

            // see TriggerVisionRange
            [SerializeField]
            private uint visionSize = 0;

            // see TriggerVisionRange
            [SerializeField]
            private uint visionLength = 0;

            private TriggerVisionRange relatedVisionRange;
            public TriggerVisionRange RelatedVisionRange { get { return relatedVisionRange; } }

            void Start()
            {
                EventDispatcher eventDispatcher = GetComponent<EventDispatcher>();
                Oriented oriented = GetComponent<Oriented>();
                GameObject aNewGameObject = new GameObject("WatcherVisionRange");
                Support.Utils.Layout.AddComponent<BoxCollider2D>(aNewGameObject);
                relatedVisionRange = Support.Utils.Layout.AddComponent<TriggerVisionRange>(aNewGameObject, new System.Collections.Generic.Dictionary<string, object>()
                {
                    { "relatedEventDispatcher", eventDispatcher },
                    { "direction", oriented.orientation },
                    { "visionSize", visionSize },
                    { "visionLength", visionLength }
                });
                SendMessage("OnWatcherReady");
            }

            void OnDestroy()
            {
                try
                {
                    if (relatedVisionRange.gameObject != null) Destroy(relatedVisionRange.gameObject);
                }
                catch (MissingReferenceException)
                {
                    // It doesn't matter if this exception is fired when destroying this crap.
                    // This means that somehow the reference failed despite evaluating as not-null.
                    // This means that it does not exist, and was destroyed beforehand.
                }
            }
        }
    }
}