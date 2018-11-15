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
            public class Pack
            {
                /**
                 * A pack is just a list of stacks, managed in a special way to not
                 *   break indices.
                 * 
                 * When an element is added to the list, it will check for any null
                 *   position in the list to occupy. If any is found, it is occupied
                 *   by the element (which, btw, is a "stack").
                 * 
                 * When an element is removed, it will be replaced by null.
                 */

                public class PackException : Exception
                {
                    public PackException(string message) : base(message) {}
                }

                public abstract class PackHeld
                {
                    /**
                     * This is the base class for stacks. By doing this, we are
                     *   providing a Pack member that can only by changed by this
                     *   class. 
                     */

                    public Pack Pack
                    {
                        get; private set;
                    }
                }

                [SerializeField]
                private List<Stacks.Stack> stacks = new List<Stacks.Stack>();

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

                public int Add(Stacks.Stack stack)
                {
                    // Idempotent case - return where the stack is currently located
                    if (stack.Pack == this)
                    {
                        return stacks.IndexOf(stack);
                    }

                    // On already-assigned stacks, explode
                    if (stack.Pack != null)
                    {
                        throw new PackException("Stack is already added to a pack");
                    }

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
                        throw new PackException("Invalid index: out of bounds");
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
