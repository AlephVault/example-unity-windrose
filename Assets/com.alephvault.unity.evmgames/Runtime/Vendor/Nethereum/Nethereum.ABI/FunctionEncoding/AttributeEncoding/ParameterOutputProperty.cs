using System.Collections.Generic;
using System.Reflection;

namespace AlephVault.Unity.EVMGames.Nethereum.ABI.FunctionEncoding.AttributeEncoding
{
    public class ParameterOutputProperty : ParameterOutput
    {
        public PropertyInfo PropertyInfo { get; set; }

        public List<ParameterOutputProperty> ChildrenProperties { get; set; }
    }
}