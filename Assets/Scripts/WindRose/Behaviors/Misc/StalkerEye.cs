using UnityEngine;

namespace WindRose
{
    namespace Behaviors
    {
        namespace Misc
        {
            [RequireComponent(typeof(Camera))]
            public class StalkerEye : MonoBehaviour
            {
                private Camera camera;
                public GameObject target;
                public uint cameraDistance = 10;

                private void Start()
                {
                    camera = GetComponent<Camera>();
                }

                void Update()
                {
                    camera.orthographic = true;
                    if (target)
                    {
                        transform.position = target.transform.position - new Vector3(0, 0, cameraDistance);
                        transform.LookAt(target.transform);
                    }
                }
            }
        }
    }
}
