using UnityEngine;

namespace SoundAround
{
    namespace Behaviours
    {
        [RequireComponent(typeof(AudioSource))]
        public class AudioLoop : MonoBehaviour
        {
            /**
             * Taken from https://github.com/Gkxd/Rhythmify/blob/master/Assets/Rhythmify_Scripts/MusicWrapper.cs
             */

            public float loopAt;
            public float loopTo;
            public bool relativeToFrequency;
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
