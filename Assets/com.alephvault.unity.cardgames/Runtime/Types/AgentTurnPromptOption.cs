namespace AlephVault.Unity.CardGames
{
    namespace Types
    {
        /// <summary>
        ///   Prompt actions for the agents. They'll be sent into
        ///   an array, and they will have an in-array unique code.
        /// </summary>
        public class AgentTurnPromptOption
        {
            /// <summary>
            ///   The prompt code.
            /// </summary>
            public readonly string Code;

            /// <summary>
            ///   The first argument. Some actions do not
            ///   require any argument at all.
            /// </summary>
            public readonly int Arg1;

            /// <summary>
            ///   The second argument. Some actions do not
            ///   require a second argument at all.
            /// </summary>
            public readonly int Arg2;

            public AgentTurnPromptOption(string code, int arg1, int arg2)
            {
                Code = code;
                Arg1 = arg1;
                Arg2 = arg2;
            }

            /// <summary>
            ///   Tells whether this prompt considers valid an answer
            ///   or not (i.e. expects it or not - also checking the
            ///   parameters, if needed).
            /// </summary>
            /// <param name="answer">The answer to validate</param>
            /// <returns></returns>
            public virtual bool Accepts(AgentTurnAnswer answer)
            {
                return Code == answer.Code;
            }
        }
    }        
}
