using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindRose
{
    namespace Types
    {
        namespace Inventory
        {
            namespace Stacks
            {
                public abstract class StackStrategy<T> where T : class
                {
                    /**
                     * A stack strategy will be barely more than a simple
                     *   data bundle: in many cases it will not hold logic
                     *   on its own except for non-side-effected logic that
                     *   would compute data without altering anything.
                     * 
                     * Stack strategies are created by corresponding item
                     *   strategies, and so they will return appropriate
                     *   reference to their creators. It will also hold a
                     *   reference to the stack it is bound to.
                     *   
                     * It has a method to export its settings, as a counterpart
                     *   of the fact that it receives certain arguments in
                     *   its constructor. By default it returns null.
                     */

                    public T ItemStrategy
                    {
                        get; private set;
                    }

                    public Stack Stack
                    {
                        get; private set;
                    }

                    public StackStrategy(T itemStrategy, object argument)
                    {
                        ItemStrategy = itemStrategy;
                        Import(argument);
                    }

                    public void Initialize(Stack stack)
                    {
                        if (Stack == null) Stack = stack;
                    }

                    protected virtual void Import(object argument)
                    {
                    }

                    public virtual object Export()
                    {
                        return null;
                    }
                }
            }
        }
    }
}
