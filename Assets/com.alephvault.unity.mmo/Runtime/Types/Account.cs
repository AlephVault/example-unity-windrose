namespace AlephVault.Unity.District
{
    namespace Types
    {
        public interface Account<AccountIDType, AccountDisplayType, AccountDataType>
        {
            AccountIDType GetID();
            AccountDisplayType GetDisplay();
            void SetDisplay(AccountDisplayType display);
            AccountDataType GetData();
            void SetData(AccountDataType data);
        }
    }
}
