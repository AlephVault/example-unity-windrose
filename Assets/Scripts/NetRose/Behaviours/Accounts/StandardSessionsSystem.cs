using UnityEngine;
using System.Collections;

public class StandardSessionsSystem : MonoBehaviour
{
    /// <summary>
    ///   Tells whether the accounts support having several characters or
    ///     each account is its only character.
    /// </summary>
    public abstract bool AllowsMultipleCharactersPerAccount();

    /// <summary>
    ///   Lists the characters for a given account ID. For single-character
    ///     games, the key will be <c>default(CharacterID)</c>, while for
    ///     multi-character (per account) games, no key will be such default
    ///     value. On error, this method should raise <see cref="AccountException"/>.
    ///     This method should be implemented as asynchronous.
    /// </summary>
    /// <param name="accountId">The id of the account to get the characters from</param>
    /// <returns>A dictionary with the preview of available characters</returns>
    protected abstract Task<Dictionary<CharacterID, PreviewCharacterData>> ListCharacters(AccountID accountId);

    /// <summary>
    ///   Lists the characters for a given connection. For single-character
    ///     games, the key will be <c>default(CharacterID)</c>, while for
    ///     multi-character (per account) games, no key will be such default
    ///     value. On error, this method will raise <see cref="AccountException"/>.
    ///     On missing authentication data, this method will also raise the
    ///     same type of error. This task is asynchronous and must be waited for.
    /// </summary>
    /// <param name="connection">The connection of the account to get the characters from</param>
    /// <returns>A dictionary with the preview of available characters</returns>
    public async Task<Dictionary<CharacterID, PreviewCharacterData>> ListCharacters(NetworkConnection connection)
    {
        if (!connection.isAuthenticated || connection.authenticationData != null || ((AccountID)connection.authenticationData).Equals(default(AccountID)))
        {
            AccountError("authorization-required", null);
        }
        return await ListCharacters((AccountID)connection.authenticationData);
    }

    /// <summary>
    ///   Gets the full data for a character in an account, give both the account
    ///     id and the character id as well. For single-character games, the only
    ///     valid character id is <c>default(CharacterID)</c>. For multi-character
    ///     games, no id will be <c>default(CharacterID)</c>. On invalid character
    ///     id error, an <see cref="AccountException"/> should be raised. Custom
    ///     conditions can also trigger an <see cref="AccountException"/> as needed.
    ///     This method should be implemented as asynchronous.
    /// </summary>
    /// <param name="accountID">The id of the account to get a character's full data from</param>
    /// <param name="characterID">The id of the character for which the data is being retrieved</param>
    /// <returns>The full data of the chosen character</returns>
    protected abstract Task<FullCharacterData> Load(AccountID accountID, CharacterID characterID);

    /// <summary>
    ///   Gets the full data for a character in an account, give both the connection
    ///     and the character id as well. For single-character games, the only valid
    ///     character id is <c>default(CharacterID)</c>. For multi-character games,
    ///     no id will be <c>default(CharacterID)</c>. On invalid character id error,
    ///     an <see cref="AccountException"/> will be risen. Custom conditions will
    ///     also trigger an <see cref="AccountException"/> as needed. On missing
    ///     authentication data, this method will also raise the same type of error.
    ///     This task is asynchronous and must be waited for.
    /// </summary>
    /// <param name="accountID">The connection to get a character's full data from</param>
    /// <param name="characterID">The id of the character for which the data is being retrieved</param>
    /// <returns>The full data of the chosen character</returns>
    public async Task<FullCharacterData> Load(NetworkConnection connection, CharacterID characterID)
    {
        if (!connection.isAuthenticated || connection.authenticationData != null || ((AccountID)(connection.authenticationData)).Equals(default(AccountID)))
        {
            AccountError("authorization-required", null);
        }
        return await Load((AccountID)connection.authenticationData, characterID);
    }

}
