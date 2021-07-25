using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Authentication
            {
                using Types;

                /// <summary>
                ///   A login delegate is a task-returning function (typically, an
                ///   asynchronous one) that returns a result and an arbitrary account
                ///   id (suitable for this particular)
                /// </summary>
                /// <param name="stream"></param>
                /// <returns></returns>
                public delegate Task<Tuple<Response, object>> LoginDeletate(System.IO.Stream stream);
            }
        }
    }
}
