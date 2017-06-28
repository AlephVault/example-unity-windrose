using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        public class AttentionCenter : MonoBehaviour
        {
            public Camera followerCamera;

            void Update()
            {
                if (followerCamera)
                {
                    followerCamera.transform.position = new Vector3(
                        gameObject.transform.position.x,
                        gameObject.transform.position.y,
                        followerCamera.transform.position.z
                    );
                }
            }
        }
    }
}
