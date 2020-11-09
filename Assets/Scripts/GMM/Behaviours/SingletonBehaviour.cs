using UnityEngine;
using System.Collections;
using GMM.Types;

namespace GMM
{
    namespace Behaviours
    {
        /// <summary>
        ///   <para>
        ///     A singleton class to serve as base type of singleton behaviours.
        ///       Only one instance can be created for each singleton type and
        ///       this is checked / enforced in the Awake() method, and released
        ///       in the OnDestroy() method.
        ///   </para>
        ///   <para>
        ///     Your children classes must be autorreferential (e.g. <c>public
        ///       class Foo : SingletonBehaviour&lt;Foo&gt;</c>).
        ///   </para>
        /// </summary>
        /// <typeparam name="T">The autorreferential type to ensure uniqueness</typeparam>
        public abstract class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
        {
            /// <summary>
            ///   The only instance allowed of this singleton.
            /// </summary>
            public static T Instance;

            protected virtual void Awake()
            {
                if (Instance == null)
                {
                    Instance = this as T;
                }
                else
                {
                    Destroy(gameObject);
                    throw new SingletonException(string.Format("An instance of a Singleton<{0}>-derived class ({1}) is already created", typeof(T).FullName, GetType().FullName));
                }
            }

            protected virtual void OnDestroy()
            {
                if (Instance == this) Instance = null;
            }
        }
    }
}