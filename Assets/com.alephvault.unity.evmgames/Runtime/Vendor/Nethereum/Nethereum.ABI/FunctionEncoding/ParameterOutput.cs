using System;
using AlephVault.Unity.EVMGames.Nethereum.ABI.Model;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding
{
    public class ParameterOutput
    {
        public Parameter Parameter { get; set; }
        public int DataIndexStart { get; set; }
        public object Result { get; set; }
        
    }
}