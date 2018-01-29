using UnityEngine;

namespace CamJam
{
    namespace Behaviours
    {
        [RequireComponent(typeof(Camera))]
        public class StalkerEye : MonoBehaviour
        {
            private Camera camera;
            public GameObject target;
            public uint cameraDistance = 10;

            private void Awake()
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
