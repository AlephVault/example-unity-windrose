using UnityEngine;

namespace SoundAround
{
    namespace Behaviours
    {
        /// <summary>
        ///   This class allos us to create a partial loop on an audio clip. This is interesting in
        ///     most BGMs where they have an introductory part and then the loop.
        /// </summary>
        [RequireComponent(typeof(AudioSource))]
        public class AudioLoop : MonoBehaviour
        {
            /**
             * Taken from https://github.com/Gkxd/Rhythmify/blob/master/Assets/Rhythmify_Scripts/MusicWrapper.cs
             */

            /// <summary>
            ///   Instant that, when the music reaches it, will cause it to loop to the <see cref="loopAt"/>
            ///     instant. See <see cref="relativeToFrequency"/> for details on what an "instant" means here.
            /// </summary>
            [SerializeField]
            private float loopAt;

            /// <summary>
            ///   Instant the music will loop to when reaching the <see cref="loopAt"/> instant. See
            ///     <see cref="relativeToFrequency"/> for details on what an "instant" means here.
            /// </summary>
            [SerializeField]
            private float loopTo;

            /// <summary>
            ///   Whether the <see cref="loopAt"/> and <see cref="loopTo"/> values are expressed in terms
            ///     of seconds or just raw samples.
            /// </summary>
            [SerializeField]
            private bool relativeToFrequency;

            private AudioSource audioSource;

            private void Awake()
            {
                audioSource = GetComponent<AudioSource>();
            }

            public void Update()
            {
                if (loopAt > 0 && loopTo >= 0)
                {
                    float _frequency = audioSource.clip.frequency;
                    int _loopAt = (int)(relativeToFrequency ? loopAt * _frequency : loopAt);
                    int _loopTo = (int)(relativeToFrequency ? loopTo * _frequency : loopTo);

                    if (audioSource.timeSamples > _loopAt)
                    {
                        audioSource.timeSamples = _loopTo;
                    }
                }
            }
        }
    }
}
