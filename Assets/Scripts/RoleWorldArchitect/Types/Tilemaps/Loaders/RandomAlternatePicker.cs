using System;
using UnityEngine;

namespace RoleWorldArchitect.Types.Tilemaps.Loaders
{
    class RandomAlternatePicker
    {
        public struct RandomOption
        {
            public readonly Rect Region;
            public readonly uint Odds;

            public RandomOption(Rect region, uint odds)
            {
                Region = region;
                Odds = odds;
            }
        }

        public readonly Texture2D Source;
        private readonly RandomOption[] Options;

        public RandomAlternatePicker(Texture2D source, RandomOption[] options)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (options == null)
            {
                throw new ArgumentNullException("options");
            }
            if (options.Length == 0)
            {
                throw new ArgumentException("Options cannot be empty", "options");
            }
            uint oddsSum = Utils.Functional.Inject<uint>(0, Utils.Functional.Map<RandomOption, uint>(options, (RandomOption option) => option.Odds), (uint a, uint b) => a + b);
            if (oddsSum > 50)
            {
                throw new ArgumentException("Odds sum of all alternate options cannot add up to more than 50%");
            }
            Source = source;
            Options = options;
        }

        /**
         * Rolls a dice of 1d100 (1..100) and calculates whether the odds match for any odd in
         *   any of the alternatives. The odds are calculated by cummulative sum like this:
         *   
         *   Region 1 - 10% -> Chosen if dice roll is lower than or equal to 10
         *   Region 2 - 13% -> Chosen if dice roll is lower than or equal to 23
         *   Region 3 - 5% -> Chosen if dice roll is lower than or equal to 28
         *   None - N/A -> Chosen otherwise.
         */
        public Rect? Pick()
        {
            uint value = (uint)UnityEngine.Random.Range(0, 100) + 1;
            uint cumOddsSum = 0;
            for(int idx = 0; idx < Options.Length; idx++)
            {
                cumOddsSum += Options[idx].Odds;
                if (value <= cumOddsSum)
                {
                    return Options[idx].Region;
                }
            }
            return null;
        }
    }
}
