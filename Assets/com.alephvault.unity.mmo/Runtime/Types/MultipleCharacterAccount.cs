using System;
using System.Collections.Generic;

namespace AlephVault.Unity.District
{
    namespace Types
    {
        public interface MultipleCharacterAccount<AccountIDType, AccountDisplayType, AccountDataType,
                                                  CharacterIDType, CharacterDisplayType, CharacterDataType>
            : Account<AccountIDType, AccountDisplayType, AccountDataType>
            where CharacterDataType : CharacterData<CharacterDisplayType>
        {
            List<Tuple<CharacterIDType, CharacterDisplayType>> GetCharacters();
            CharacterDataType GetCharacterData(CharacterIDType characterID);
            void SetCharacterData(CharacterIDType characterID, CharacterDataType characterData);

            bool SupportsCharacterAdd();
            CharacterIDType AddCharacter(CharacterDataType characterData);

            bool SupportsCharacterRemove();
            void RemoveCharacter(CharacterIDType characterID);
        }
    }
}
