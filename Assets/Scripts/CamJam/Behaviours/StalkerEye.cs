using System;
using UnityEngine;

namespace CamJam
{
    namespace Behaviours
    {
        public class StalkerEye : CameraJammingEffect
        {
            // The object being followed.
            // If empty, this effect will do nothing.
            [SerializeField]
            private GameObject followed;

            // Units per second. Essentially, speed.
            // If <= 0, the movement will be instantaneous.
            [SerializeField]
            private float speed = 0;

            // Tells whether the camera should remain attached
            //   to the target after reaching it.
            private bool remain = true;

            // Which action will be executed -just once!- when
            //   the movement is ended
            private Action onEnd = null;

            public void Seek(GameObject newFollowed, float? newSpeed = null, bool remainOnEnd = true, Action onMovementEnd = null)
            {
                if (newFollowed)
                {
                    followed = newFollowed;
                    speed = newSpeed.GetValueOrDefault(speed);
                    remain = remainOnEnd;
                    onEnd = onMovementEnd;
                }
                else
                {
                    followed = null;
                }
            }

            protected override void Tick(Camera camera)
            {
                Vector3 position = camera.transform.position;
                Vector3 finalPosition = new Vector3(followed.transform.position.x, followed.transform.position.y, position.z);
                if (followed)
                {
                    if (speed <= 0)
                    {
                        position = finalPosition;
                    }
                    else
                    {
                        position = Vector3.MoveTowards(position, finalPosition, speed * Time.deltaTime);
                    }

                    camera.transform.position = position;

                    if (position == finalPosition)
                    {
                        if (onEnd != null) onEnd();
                        onEnd = null;
                        speed = 0;
                        if (!remain)
                        {
                            followed = null;
                        }
                    }
                }
            }
        }
    }
}
