using UnityEngine;
using System.Collections;
using Mirror;

namespace NetworkedSamples
{
    namespace Behaviours
    {
        [RequireComponent(typeof(SamplePlayer))]
        public class SamplePlayerControl : NetworkBehaviour
        {
            private SamplePlayer samplePlayer;

            private void Awake()
            {
                samplePlayer = GetComponent<SamplePlayer>();                
            }

            [Command]
            public void Pick()
            {
                if (samplePlayer.Bag) samplePlayer.Bag.Pick(out _);
            }

            [Command]
            public void Drop(int position)
            {
                if (samplePlayer.Bag) samplePlayer.Bag.Drop(position);
            }

            [Command]
            public void Right()
            {
                if (samplePlayer.CurrentCharacter)
                {
                    samplePlayer.CurrentCharacter.MapObject.StartMovement(WindRose.Types.Direction.RIGHT);
                }
            }

            [Command]
            public void Up()
            {
                if (samplePlayer.CurrentCharacter)
                {
                    samplePlayer.CurrentCharacter.MapObject.StartMovement(WindRose.Types.Direction.UP);
                }
            }

            [Command]
            public void Left()
            {
                if (samplePlayer.CurrentCharacter)
                {
                    samplePlayer.CurrentCharacter.MapObject.StartMovement(WindRose.Types.Direction.LEFT);
                }
            }

            [Command]
            public void Down()
            {
                if (samplePlayer.CurrentCharacter)
                {
                    samplePlayer.CurrentCharacter.MapObject.StartMovement(WindRose.Types.Direction.DOWN);
                }
            }
        }
    }
}