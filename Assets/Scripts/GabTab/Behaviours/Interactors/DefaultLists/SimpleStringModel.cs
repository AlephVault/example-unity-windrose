using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GabTab
{
    namespace Behaviours
    {
        namespace Interactors
        {
            namespace DefaultLists
            {
                /// <summary>
                ///   One of the selectable items. It will have a code-friendly key,
                ///     a user-friendly label, and a flag telling whether it will
                ///     be enabled (selectable) or not.
                /// </summary>
                public class SimpleStringModel
                {
                    public readonly string Key;
                    public readonly string Value;
                    public bool Enabled;

                    public SimpleStringModel(string key, string value, bool enabled = true)
                    {
                        Key = key;
                        Value = value;
                        Enabled = enabled;
                    }
                }
            }
        }
    }
}
