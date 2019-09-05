using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BackPack
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

                    /// <summary>
                    ///   <para>
                    ///     Usage strategies try consuming or using certain items. They should have a chained
                    ///       behaviour (i.e. depend among them), and ultimately a base usage strategy would
                    ///       try consuming quantities of the stack being used.
                    ///   </para>
                    ///   <para>
                    ///     They will interact with stacks having compatible usage (state) strategies, and
                    ///       will run the usage behaviour in coroutines, so interactions with UI and even
                    ///       asynchronous server connection are feasible.
                    ///   </para>
                    /// </summary>
                    public abstract class InventoryUsageManagementStrategy : InventoryManagementStrategy
                    {
                        /// <summary>
                        ///   Tells when something went wrong trying to use a stack: another
                        ///     stack is already being used, or the new stack to use does not
                        ///     belong to the current inventory manager.
                        /// </summary>
                        public class UsageException : Exception
                        {
                            public UsageException(string message) : base(message) {}
                        }

                        /// <summary>
                        ///   Tells when a stack as an invalid (incompatible) usage strategy.
                        /// </summary>
                        public class InvalidStackUsageStrategyCounterparyType : Support.Types.Exception
                        {
                            public InvalidStackUsageStrategyCounterparyType(string message) : base(message) { }
                        }

                        /**
                         * Compatibility-related stuff.
                         */

                        /**
                         * 
                         */
                        /// <summary>
                        ///   Tells whether a stack usage strategy is accepted by this class, or not.
                        ///   Usually, the check would by type-to-type, but there are cases where dummy
                        ///     inventory usage strategies would accept any strategy, but make no use
                        ///     of them(these would be like "agnostic" usage strategies).
                        /// </summary>
                        public abstract bool Accepts(StackUsageStrategy strategy);

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


                        /// <summary>
                        ///   Uses a certain stack, which would be present in the inventory. The stack will have a compatible
                        ///     strategy to be used. This method may dispatch calls to <see cref="DoUse(Stack, object)"/>
                        ///     defined in dependencies (and related strategies in the stack will be attended there).
                        /// </summary>
                        /// <param name="stack">The stack being used</param>
                        /// <param name="argument">A custom argument to setup the usage</param>
                        /// <returns>The enumerator for the coroutine</returns>
                        protected abstract IEnumerator DoUse(Stack stack, object argument);

                        /// <summary>
                        ///   This wrapper method performs the usage and, when done (or error) it clears the flag of an item
                        ///     being used.
                        /// </summary>
                        /// <param name="stack">The stack being used</param>
                        /// <param name="argument">A custom argument to setup the usage</param>
                        /// <returns>The enumerator for the coroutine</returns>
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

                        /// <summary>
                        ///   Uses a stack. It must belong to this inventory manager, and no stack must be already
                        ///     being used. A coroutine will be spawned under the hood.
                        /// </summary>
                        /// <param name="stack">The stack to use</param>
                        /// <param name="argument">An optional argument to setup the usage</param>
                        public void Use(Stack stack, object argument = null)
                        {
                            if (currentlyUsingAnItem)
                            {
                                throw new UsageException("Currently, a stack is being used - please finish the interaction in order to use another stack");
                            }

                            try
                            {
                                if (stack.QualifiedPosition.Third.SpatialStrategy.GetComponent<InventoryManagementStrategyHolder>() != StrategyHolder)
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
