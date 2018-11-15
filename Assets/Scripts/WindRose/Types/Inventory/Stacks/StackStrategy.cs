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
                     * It has a method to export its values, as a counterpart
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

                    protected object GetKey(Dictionary<string, object> arguments, string key)
                    {
                        string qualifiedKey = GetType().FullName + ":" + key;
                        return (arguments.ContainsKey(qualifiedKey)) ? arguments[qualifiedKey] : arguments[key];
                    }

                    public StackStrategy(T itemStrategy, Dictionary<string, object> arguments)
                    {
                        ItemStrategy = itemStrategy;
                        Import(arguments);
                    }

                    public void Initialize(Stack stack)
                    {
                        Stack = stack;
                    }

                    public virtual void Import(Dictionary<string, object> arguments)
                    {
                    }

                    public virtual Dictionary<string, object> Export()
                    {
                        return null;
                    }
                }
            }
        }
    }
}
