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
                /// <summary>
                ///    <para>
                ///      A stack strategy will be barely more than a simple
                ///        data bundle: in many cases it will not hold logic
                ///        on its own except for non-side-effected logic that
                ///        would compute data without altering anything.
                ///    </para>
                ///    <para>
                ///      Stack strategies are created by corresponding item
                ///        strategies, and so they will return appropriate
                ///        reference to their creators.It will also hold a
                ///        reference to the stack it is bound to.
                ///    </para>
                ///    <para>
                ///      It has a method to export its settings, as a counterpart
                ///        of the fact that it receives certain arguments in
                //         its constructor.By default it returns null.
                ///    </para>
                /// </summary>
                /// <typeparam name="T">The base item strategy type</typeparam>
                public abstract class StackStrategy<T> where T : class
                {
                    /// <summary>
                    ///   The related item strategy.
                    /// </summary>
                    public T ItemStrategy
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   The related stack.
                    /// </summary>
                    public Stack Stack
                    {
                        get; private set;
                    }

                    /// <summary>
                    ///   Constructs this strategy by taking the item's strategy
                    ///     as base.
                    /// </summary>
                    /// <param name="itemStrategy">The base item strategy</param>
                    public StackStrategy(T itemStrategy)
                    {
                        ItemStrategy = itemStrategy;
                    }

                    /// <summary>
                    ///   Sets once the stack.
                    /// </summary>
                    /// <param name="stack">The stack to bind when initialized</param>
                    public void Initialize(Stack stack)
                    {
                        if (Stack == null) Stack = stack;
                    }
                }
            }
        }
    }
}
