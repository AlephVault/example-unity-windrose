using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            public class SparseStackList
            {
                /**
                 * A sparse stack list is just a list of stacks, managed in a special
                 *   way to not break indices.
                 * 
                 * When an element is added to the list, it will check for any null
                 *   position in the list to occupy. If any is found, it is occupied
                 *   by the element (which, btw, is a "stack").
                 * 
                 * When an element is removed, it will be replaced by null.
                 */

                public class Exception : Types.Exception
                {
                    public Exception(string message) : base(message) {}
                }

                private List<Stacks.Stack> stacks;

                public SparseStackList()
                {
                    stacks = new List<Stacks.Stack>();
                }

                public int Count
                {
                    get
                    {
                        return stacks.Count;
                    }
                }

                public Stacks.Stack this[int index]
                {
                    get
                    {
                        return stacks[index];
                    }
                }

                public IEnumerable<Stacks.Stack> Stacks()
                {
                    return stacks.AsEnumerable();
                }

                public int Add(Stacks.Stack stack)
                {
                    // Search for the first empty place, and occupy
                    int length = stacks.Count();
                    for(int index = 0; index < length; index++)
                    {
                        if (stacks[index] == null)
                        {
                            stacks[index] = stack;
                            return index;
                        }
                    }

                    // Or append the element and occupy a new place
                    stacks.Add(stack);
                    return length; //== the new/last index
                }

                public void Remove(int index)
                {
                    // The index must be valid for the list
                    int length = stacks.Count();
                    if (index >= length || index < 0)
                    {
                        throw new Exception("Invalid index: out of bounds");
                    }

                    // On idempotent case, we return
                    if (stacks[index] != null)
                    {
                        // Otherwise, we enter and clean
                        stacks[index] = null;

                        // And finally, trim trailing null values
                        while (stacks[length - 1] == null)
                        {
                            stacks.RemoveAt(length - 1);
                            length--;
                        }
                    }
                }
            }
        }
    }
}
