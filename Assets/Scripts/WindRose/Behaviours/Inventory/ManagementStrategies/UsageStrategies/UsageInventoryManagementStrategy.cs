using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Inventory
        {
            namespace ManagementStrategies
            {
                namespace UsageStrategies
                {
                    using Types.Inventory.Stacks;
                    using Types.Inventory.Stacks.UsageStrategies;

                    public abstract class UsageInventoryManagementStrategy : InventoryManagementStrategy
                    {
                        /**
                         * Usage strategies try consuming or using certain items. Usage strategies should have a chained
                         *   behaviour (i.e. depend among them). Ultimately, a base usage strategy will try consuming
                         *   quantities of the stack.
                         * 
                         * Usage is being run as a coroutine since it may involve UI or even server-side interaction.
                         * 
                         * Usages will also consider their counterpart types: they will know how to interact with the stack
                         *   based on its underlying item.
                         */

                        public class UsageException : Exception
                        {
                            public UsageException(string message) : base(message) {}
                        }

                        public class InvalidStackUsageStrategyCounterparyType : Types.Exception
                        {
                            public InvalidStackUsageStrategyCounterparyType(string message) : base(message) { }
                        }

                        /**
                         * Compatibility-related stuff.
                         */

                        public Type StackUsageStrategyCounterpartType {
                            get; private set;
                        }

                        protected abstract Type GetStackUsageStrategyCounterpartType();

                        protected override void Awake()
                        {
                            base.Awake();
                            StackUsageStrategyCounterpartType = GetStackUsageStrategyCounterpartType();
                            if (!StackUsageStrategyCounterpartType.IsSubclassOf(typeof(StackUsageStrategy)))
                            {
                                throw new InvalidStackUsageStrategyCounterparyType(string.Format("The type returned by GetStackUsageStrategyCounterpartType must be a subclass of {0}", typeof(StackUsageStrategy).FullName));
                            }
                        }

                        /**
                         * Usage-related methods.
                         */

                        /**
                         * Flag  to avoid race conditions - i.e. to ensure only one corroutine is being run at
                         *   once.
                         */
                        private bool currentlyUsingAnItem = false;

                        /**
                         * This method is the key. It should:
                         * - Use a stack only in terms of the counterpart-expected behaviour. If such counterpart behaviour
                         *     is missing, then it should cry.
                         * - Delegation is done through `yield return anotherComponent.DoUse(stack, argument)`.
                         * - If you are at `DoUse` is because you are aware that the stack has certain main usage strategy
                         *     and it will have their dependencies as well. Delegating will imply that this inventory usage
                         *     strategy will find the compatible dependencies for such stack's dependencies. Ensure the
                         *     dependencies are properly set in both stack usage strategies and inventory usage strategy.
                         * 
                         * There is an optional argument to customize the usage type. It may be null, so when that happens
                         *   you must be prepared to implement a default usage.
                         */
                        protected abstract IEnumerator DoUse(Stack stack, object argument);

                        /**
                         * This wrapper just clears the usage flag after running the coroutines, even if they generate
                         *   an error.
                         */
                        protected IEnumerator DoUseWrapper(Stack stack, object argument)
                        {
                            try
                            {
                                yield return StartCoroutine(DoUse(stack, argument));
                            }
                            finally
                            {
                                currentlyUsingAnItem = false;
                            }
                        }

                        /**
                         * Uses a stack. The stack must belong to an inventory managed by this strategy, and no other element must be being used
                         *   right now (this will also imply: there will not be chained usages).
                         */
                        public void Use(Stack stack)
                        {
                            Use(stack, null);
                        }

                        /**
                         * The same, but also accepts an argument. `null` value is allowed and will trigger the default behaviour. Strategies may
                         *   ignore any passed argument silently, but a `null` case will always be allowed. However, if you want to pass `null`
                         *   as a constant then you can use the single argument version.
                         */
                        public void Use(Stack stack, object argument)
                        {
                            if (currentlyUsingAnItem)
                            {
                                throw new UsageException("Currently, a stack is being used - please finish the interaction in order to use another stack");
                            }

                            try
                            {
                                if (stack.SpatialStrategy.QualifiedPosition.Second.SpatialStrategy.GetComponent<InventoryManagementStrategyHolder>() != StrategyHolder)
                                {
                                    throw new UsageException("The stack being used is not managed by this inventory");
                                }
                            }
                            catch(NullReferenceException)
                            {
                                throw new UsageException("The stack being used is not managed by this inventory");
                            }

                            currentlyUsingAnItem = true;
                            StartCoroutine(DoUseWrapper(stack, argument));
                        }
                    }
                }
            }
        }
    }
}
