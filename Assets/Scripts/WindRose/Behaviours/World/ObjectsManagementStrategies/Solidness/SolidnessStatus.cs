namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace ObjectsManagementStrategies
            {
                namespace Solidness
                {
                    /// <summary>
                    ///   Solidness status involves the ability of objects to be completely solid (i.e. not traverse anything,
                    ///     but also not allow other solid objects to traverse them), solid-for-others (i.e. objects that
                    ///     do not allow other strictly solid objects to traverse them, but they can traverse anything),
                    ///     ghost objects (they can traverse and be traversed), and "hole" objects: they occupy negative
                    ///     space, so when overlapping with solid objects, those solid objects become traversable in the
                    ///     overlapped area.
                    /// </summary>
                    public enum SolidnessStatus { Hole, Ghost, SolidForOthers, Solid };

                    static class SolidnessStatusMethods
                    {
                        /// <summary>
                        ///   Solid objects cannot traverse anything. Others will.
                        /// </summary>
                        /// <param name="status"></param>
                        /// <returns></returns>
                        public static bool Traverses(this SolidnessStatus status)
                        {
                            switch (status)
                            {
                                case SolidnessStatus.Solid:
                                    return false;
                                default:
                                    return true;
                            }
                        }

                        /// <summary>
                        ///   Solid, and solid-for-others objects occupy space. Others don't.
                        /// </summary>
                        /// <param name="status"></param>
                        /// <returns></returns>
                        public static bool Occupies(this SolidnessStatus status)
                        {
                            switch (status)
                            {
                                case SolidnessStatus.Solid:
                                case SolidnessStatus.SolidForOthers:
                                    return true;
                                default:
                                    return false;
                            }
                        }

                        /// <summary>
                        ///   Hole objects produce negative occupancy. Others don't.
                        /// </summary>
                        /// <param name="status"></param>
                        /// <returns></returns>
                        public static bool Carves(this SolidnessStatus status)
                        {
                            switch (status)
                            {
                                case SolidnessStatus.Hole:
                                    return true;
                                default:
                                    return false;
                            }
                        }

                        /// <summary>
                        ///   Tells whether the occupancy/carving quality of the compared values
                        ///     is different or not.
                        /// </summary>
                        public static bool OccupancyChanges(this SolidnessStatus oldStatus, SolidnessStatus newStatus)
                        {
                            return (oldStatus.Occupies()) != newStatus.Occupies() || (oldStatus.Carves() != newStatus.Carves());
                        }
                    }
                }
            }
        }
    }
}
