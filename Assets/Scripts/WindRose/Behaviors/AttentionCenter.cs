using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        public class AttentionCenter : MonoBehaviour
        {
            public Camera followerCamera;
            public uint cameraDistance = 10;

            void Update()
            {
                if (followerCamera)
                {
                    followerCamera.orthographic = true;
                    followerCamera.transform.position = new Vector3(
                        gameObject.transform.position.x,
                        gameObject.transform.position.y,
                        gameObject.transform.position.z - cameraDistance
                    );
                    followerCamera.transform.LookAt(gameObject.transform);
                }
            }
        }
    }
}
