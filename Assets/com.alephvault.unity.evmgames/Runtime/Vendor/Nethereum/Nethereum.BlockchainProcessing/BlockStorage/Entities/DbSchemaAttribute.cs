using System;

namespace AlephVault.Unity.EVMGames.Nethereum.BlockchainProcessing.BlockStorage.Entities
{
    public class DbSchemaAttribute: Attribute
    {
        public DbSchemaNames DbSchemaName { get; }

        public DbSchemaAttribute(DbSchemaNames dbSchemaName)
        {
            DbSchemaName = dbSchemaName;
        }
    }
}