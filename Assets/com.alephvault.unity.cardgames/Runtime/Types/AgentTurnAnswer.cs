namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   Answers from the agents. As a simple implementation,
        ///   answers allow up to 2 arguments. Only one answer
        ///   will be returned, and the code will be among the
        ///   prompt's options.
        /// </summary>
        public class AgentTurnAnswer
        {
            /// <summary>
            ///   The answer code. It will match a prompt code.
            /// </summary>
            public readonly string Code;

            /// <summary>
            ///   The first argument. Some answers do not
            ///   require any argument at all.
            /// </summary>
            public readonly int Arg1;

            /// <summary>
            ///   The second argument. Some answers do not
            ///   require a second argument at all.
            /// </summary>
            public readonly int Arg2;

            public AgentTurnAnswer(string code, int arg1, int arg2)
            {
                Code = code;
                Arg1 = arg1;
                Arg2 = arg2;
            }
        }
    }        
}
