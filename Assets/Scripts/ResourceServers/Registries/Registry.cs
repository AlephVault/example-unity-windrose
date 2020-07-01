﻿using UnityEngine;

namespace ResourceServers
{
    namespace Registries
    {
        /// <summary>
        ///   Resource registries exist, typically, once
        ///     per game and can retrieve arbitrary game
        ///     resources. Such resources will typically
        ///     be scriptable objects, or even GameObject
        ///     instances.
        /// </summary>
        public abstract class Registry : ScriptableObject
        {
            /// <summary>
            ///   <para>
            ///     Finds an object (of arbitrary type) given
            ///     its path. Paths will typically be of form
            ///     "/foo/bar/baz/qoo/1", but this will only
            ///     depend on the registry version (e.g. V2
            ///     uses /package/space/list/id).
            ///   </para>
            ///   <para>
            ///     
            ///   </para>
            /// </summary>
            /// <param name="path">The resource path</param>
            /// <returns></returns>
            public abstract Object Find(string path);
        }
    }
}
