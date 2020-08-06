using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace GMM
{
    namespace Behaviours
    {
        /// <summary>
        ///   This behaviour just ensures the local position of its transform is 0,
        ///     its local scale is 1, and its local rotation is the identity (i.e. no
        ///     further rotation).
        /// </summary>
        public class Normalized : MonoBehaviour
        {
            private void Awake()
            {
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
