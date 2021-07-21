using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlephVault.Unity.District
{
    namespace Types
    {
        public interface CharacterData<CharacterDisplayType>
        {
            CharacterDisplayType GetDisplay();
        }
    }
}
