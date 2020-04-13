using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace PartyScreenEnhancements.Comparers
{
    public class TrueTierComparer : PartySort
    {
        private bool _descending;

        /// <summary>
        /// Creates a <code>IComparer</code> instance which sorts Characters in the Party list based on their tier in game.
        /// </summary>
        /// <param name="descending">Whether to sort on descending order or ascending. (top to bottom)</param>
        public TrueTierComparer(bool descending)
        {
            _descending = descending;
        }

        protected override int localCompare(CharacterObject x, CharacterObject y)
        {
            if (_descending ? x.Tier < y.Tier : x.Tier > y.Tier)
            {
                    return 1;
            }
            else if (x.Tier == y.Tier)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}