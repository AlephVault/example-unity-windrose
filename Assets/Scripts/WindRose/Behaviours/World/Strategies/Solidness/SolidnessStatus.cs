namespace WindRose
{
    namespace Behaviours
    {
        namespace World
        {
            namespace Strategies
            {
                namespace Solidness
                {
                    public enum SolidnessStatus { Hole, Ghost, SolidForOthers, Solid };

                    static class SolidnessStatusMethods
                    {
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
