using UnityEngine;
using Mirror;
using NetRose.Behaviours.Auth;
using System.Threading.Tasks;

namespace NetRose
{
    namespace Behaviours
    {
        namespace UI
        {
            /// <summary>
            ///   <para>
            ///     A standard player is tightly linked to a networked
            ///       world manager, which is tightly linked to its
            ///       authenticator (which has the same type arguments).
            ///   </para>
            ///   <para>
            ///     Standard players can account for games having multiple
            ///       or single characters, and will handle the workflow
            ///       in a different way. This means: Single-character games
            ///       load the character at once and have no character workflow
            ///       while the multi-character games do not load anything at
            ///       once and provide a workflow to load/unload characters.
            ///   </para>
            /// </summary>
            /// <typeparam name="AuthMessage">The type of the auth message to send to the server to perform login</typeparam>
            /// <typeparam name="AccountID">The type of the id for the player's account</typeparam>
            /// <typeparam name="CharacterID">The type of the id for the player's characters</typeparam>
            /// <typeparam name="PreviewCharacterData">The type of the preview data for the player's characters</typeparam>
            /// <typeparam name="FullCharacterData">The type of the full data for the player's characters</typeparam>
            public abstract class StandardPlayer<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData> : NetworkBehaviour where AuthMessage : IMessageBase, new()
            {
                /// <summary>
                ///   This callback is triggered when a player is initialized
                ///     in a multi-character environment, or attempted to
                ///     load the only character in a single-character ones.
                ///     It is also triggered on multi-character environments
                ///     when the current character is released (single-character
                ///     games don't have that feature).
                /// </summary>
                protected abstract void OnNoCharacter();

                /// <summary>
                ///   This callback is triggered when a character is selected
                ///     for the current player.
                /// </summary>
                /// <param name="characterID">The id of the character being selected</param>
                /// <param name="fullCharacterData">The full data of the character being selected</param>
                protected abstract void OnCharacter(CharacterID characterID, FullCharacterData fullCharacterData);

                /// <summary>
                ///   This callback is triggered when a character is attempted
                ///     to be selected but the character is not found. This is
                ///     only meaningful in multi-character games.
                /// </summary>
                /// <param name="characterID">The attempted id for character selection</param>
                protected abstract void OnNotFound(CharacterID characterID);

                /// <summary>
                ///   This callback is triggered when a character is attempted
                ///     to be selected but an error occurs in the process.
                /// </summary>
                /// <param name="characterID">The attempted id for character selection</param>
                /// <param name="error">The raised error</param>
                protected abstract void OnCharacterError(CharacterID characterID, StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException error);

                /// <summary>
                ///   Using a character involves picking one by its character id
                ///     and it has different meaning depending on whether the
                ///     game uses  single-character authenticator or a multiple
                ///     characters one. The first will only allow the zero key
                ///     as character (and will invoke <see cref="OnNoCharacter"/>
                ///     callback if it does not exist, or will invoke the
                ///     <see cref="OnCharacter(CharacterID, FullCharacterData)"/>
                ///     if it does, while <see cref="OnCharacterError(CharacterID, StandardAuthenticator{AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData}.AccountException)"/>
                ///     will be invoked on error), while the second one will allow
                ///     non-zero keys, and the zero key will be used to invoke the
                ///     <see cref="OnNoCharacter"/> callback. Under this mode, when
                ///     a character is attempted but not found, another callback
                ///     is triggered: <see cref="OnNotFound(CharacterID)"/>, while
                ///     <see cref="OnCharacterError(CharacterID, StandardAuthenticator{AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData}.AccountException)"/>
                ///     will be invoked on error. This task is asynchronous and
                ///     should be waited for.
                /// </summary>
                /// <param name="characterID">The ID of the character to lookup</param>
                public async Task UseCharacter(CharacterID characterID)
                {
                    var manager = (StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>)NetworkManager.singleton.authenticator;
                    if (manager.AllowsMultipleCharactersPerAccount())
                    {
                        if (characterID.Equals(default(CharacterID)))
                        {
                            OnNoCharacter();
                        }
                        else
                        {
                            try
                            {
                                FullCharacterData data = await manager.Load(connectionToClient, characterID);
                                OnCharacter(characterID, data);
                            }
                            catch(StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException error)
                            {
                                if (error.Code == StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException.NotFound)
                                {
                                    OnNotFound(characterID);
                                }
                                else
                                {
                                    OnCharacterError(characterID, error);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (characterID.Equals(default(CharacterID)))
                        {
                            try
                            {
                                FullCharacterData data = await manager.Load(connectionToClient, characterID);
                                OnCharacter(characterID, data);
                            }
                            catch (StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException error)
                            {
                                if (error.Code == StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException.NotFound)
                                {
                                    OnNoCharacter();
                                }
                                else
                                {
                                    OnCharacterError(characterID, error);
                                }
                            }
                        }
                        else
                        {
                            OnCharacterError(characterID, new StandardAuthenticator<AuthMessage, AccountID, CharacterID, PreviewCharacterData, FullCharacterData>.AccountException("invalid-key", null));
                        }
                    }
                }

                /// <summary>
                ///   Releasing a character will mean to use the zero key
                ///     character. In multi-character games it means that the
                ///     existing character will be popped out and everything
                ///     will go back to the character selection menu. In
                ///     single-character games, it will do nothing. This task
                ///     is asynchronous and should be waited for.
                /// </summary>
                public async Task ReleaseCharacter()
                {
                    await UseCharacter(default(CharacterID));
                }

                private async void DoUseCharater(CharacterID characterID)
                {
                    await UseCharacter(characterID);
                }

                /// <summary>
                ///   When this player starts on server, it attempts to use the zero key
                ///     to select a character. In multi-character games, this zero key
                ///     will call the <see cref="OnNoCharacter"/> callback to force the
                ///     player to choose (or perhaps create) a character. On
                ///     single-character games, this zero key will try to pick the only
                ///     available character and, on failure due to non-existence, it will
                ///     call the <see cref="OnNoCharacter"/> callback as well.
                /// </summary>
                public override void OnStartServer()
                {
                    DoUseCharater(default(CharacterID));
                }
            }
        }
    }
}
