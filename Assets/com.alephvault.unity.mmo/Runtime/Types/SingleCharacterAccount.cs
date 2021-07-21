namespace AlephVault.Unity.District
{
    namespace Types
    {
        public interface SingleCharacterAccount<AccountIDType, AccountDisplayType, AccountDataType, CharacterDataType>
            : Account<AccountIDType, AccountDisplayType, AccountDataType>
        {
            CharacterDataType GetCharacterData();
            void SetCharacterData(CharacterDataType characterData);
        }
    }
}
