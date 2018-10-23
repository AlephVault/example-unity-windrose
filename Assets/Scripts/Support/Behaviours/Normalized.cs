using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Support
{
    namespace Behaviours
    {
        class Normalized : MonoBehaviour
        {
            /**
             * This behaviour resets the transform to avoid surprises.
             */
            private void Awake()
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
