using System;
using UnityEngine;

namespace WindRose
{
    namespace Behaviours
    {
        namespace Objects
        {
            namespace Strategies
            {
                /**
                 * This is a combined object strategy. It is intended to initialize
                 *   somehow the inner strategies to be part of this super-strategy.
                 *   
                 * Then it may define fields that actually delegate to inner strategies.
                 * An example:
                 * - We have a SolidSpaceObjectStrategy which provides the Solidness field.
                 * - We have a WaterAwareStrategy which provides a Navigating field.
                 * - We create a custom subclass that initializes one instance of each
                 *   of those two classes, and adds custom field telling type of ship.
                 * - Now this instance will invoke PropertyWasUpdated for the new field,
                 *   while sub-strategies will still invoke PropertyWasUpdated for their
                 *   respective fields (however this should be implemented per-field as
                 *   well).
                 *   
                 * Although the behaviour of this class is fully implemented, this class is
                 *   marked as abstract so the user is forced to implement a subclass and
                 *   that would serve to validate the subclass by the appropriate subclass
                 *   of map strategy.
                 */
                public abstract class CombinedObjectStrategy : ObjectStrategy
                {
                    /**
                     * This strategy also knows which sub-strategies does it handle.
                     *   ORDER WILL BE IMPORTANT HERE.
                     */
                    private ObjectStrategy[] childrenStrategies = null;

                    /**
                     * This constructor will take the holder for the base constructor
                     *   and also a function to execute to initialize the children
                     *   strategies. This function should be created as a closure from
                     *   the child class (this closure will be made on-demand and based
                     *   on the input data that the strategy could gather beyond the 
                     *   strategy holder, while getting the strategy holder as the same
                     *   parameter that is present in the constructor.
                     */
                    public CombinedObjectStrategy(ObjectStrategyHolder StrategyHolder, Func<ObjectStrategyHolder, ObjectStrategy[]> initializer) : base(StrategyHolder)
                    {
                        childrenStrategies = initializer(StrategyHolder);
                    }

                    /**
                     * Now we allow getting the count and each sub-strategy.
                     */
                    
                    /**
                     * Count of sub-strategies.
                     */
                    public int Length
                    {
                        get { return childrenStrategies.Length; }
                    }

                    /**
                     * Getting a sub-strategy.
                     */
                    public ObjectStrategy this[int index]
                    {
                        get { return childrenStrategies[index]; }
                    }
                }
            }
        }
    }
}
