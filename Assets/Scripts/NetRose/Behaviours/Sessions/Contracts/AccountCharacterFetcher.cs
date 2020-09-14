using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetRose
{
    namespace Behaviours
    {
        namespace Sessions
        {
            namespace Contracts
            {
                /// <summary>
                ///   Account character fetchers provide a way to load the
                ///     characters' data from a given account. The data is
                ///     loaded in an asynchronous way, returning an empty
                ///     value (<c>default(CharacterFullData)</c>) if the
                ///     character for a given ID does not exist. A method
                ///     to list all the characters is also part of the
                ///     contract. A third method is provided to tell whether
                ///     the accounts support one or multiple characters (the
                ///     first case automatically loads a character and the
                ///     second case will let the player choose which one to
                ///     play with).
                /// </summary>
                /// <typeparam name="AccountID">The type of the id of an account</typeparam>
                /// <typeparam name="CharacterID">The type of the id of a character</typeparam>
                /// <typeparam name="CharacterPreviewData">The type of the partial preview data of a character</typeparam>
                /// <typeparam name="CharacterFullData">The type of the full data of a character</typeparam>
                public interface AccountCharacterFetcher<AccountID, CharacterID, CharacterPreviewData, CharacterFullData>
                {
                    /// <summary>
                    ///   Tells whether this fetcher assumes accounts can heve
                    ///     multiple characters or just one.
                    /// </summary>
                    /// <returns>Whether this fetcher supports multiple characters per account or only one</returns>
                    bool AccountsHaveMultipleCharacters();

                    /// <summary>
                    ///   Asynchronously loads the preview data from all the
                    ///     available / selectable characters.
                    /// </summary>
                    /// <param name="accountId">The id of the account to list the characters from</param>
                    /// <returns>The list of available characters as (id, data) pairs</returns>
                    Task<List<Tuple<CharacterID, CharacterPreviewData>>> ListCharacters(AccountID accountId);

                    /// <summary>
                    ///   Asynchronously loads the full data of a single character.
                    /// </summary>
                    /// <param name="accountId">The id of the account to get the character from</param>
                    /// <param name="characterId">The id of the character to get the full data from</param>
                    /// <returns>The full data of the character. If <c>default(CharacterFullData)</c>, either the account or the character do not exist</returns>
                    Task<CharacterFullData> GetCharacterData(AccountID accountId, CharacterID characterId);
                }
            }
        }
    }
}
