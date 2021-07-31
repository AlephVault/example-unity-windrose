using System;
using System.Threading.Tasks;
using UnityEngine;
using AlephVault.Unity.MMO.Types;
using MLAPI.Serialization;


namespace AlephVault.Unity.MMO
{
    namespace Authoring
    {
        namespace Behaviours
        {
            namespace Realms
            {
                using Authentication;

                /// <summary>
                ///   A realm is, basically, an account system.
                ///   It is a bit more, however: It may determine
                ///   the type of gameplay an account will have.
                /// </summary>
                [RequireComponent(typeof(Authenticator))]
                public abstract class Realm : MonoBehaviour
                {
                    /// <summary>
                    ///   The authenticator this object is related to.
                    /// </summary>
                    public Authenticator Authenticator { get; private set; }

                    private void Awake()
                    {
                        Authenticator = GetComponent<Authenticator>();
                    }

                    private void Start()
                    {
                        foreach(var entry in LoginMethods())
                        {
                            var callback = entry.Item2;
                            Authenticator.RegisterLoginMethod(entry.Item1, async (NetworkReader reader) =>
                            {
                                Tuple<Response, object> result = await callback(reader);
                                return new Tuple<Response, Authenticator.AccountId>(result.Item1, new Authenticator.AccountId(result.Item2, Name()));
                            });
                        }
                        Authenticator.OnAccountLoginOK += OnAccountLoginOK;
                        Authenticator.OnAccountLoginFailed += OnAccountLoginFailed;
                        Authenticator.OnAccountLoggedOut += OnAccountLoggedOut;
                    }

                    /// <summary>
                    ///   The realm name. It must be unique, if
                    ///   other realms coexist.
                    /// </summary>
                    protected abstract string Name();

                    /// <summary>
                    ///   A list of the login methods to install in this realm.
                    /// </summary>
                    protected abstract Tuple<string, Func<NetworkReader, Task<Tuple<Response, object>>>>[] LoginMethods();

                    /// <summary>
                    ///   This method is executed when a client
                    ///   has successfully logged in. This method
                    ///   is quite important, since game setup
                    ///   must be done in its implementation.
                    /// </summary>
                    /// <param name="clientId">The id of the current  connection succeeding the login attempt</param>
                    /// <param name="response">The success response. Typically, not needed</param>
                    /// <param name="accountId">The id of the account that succeeded the login attempt</param>
                    protected abstract Task OnAccountLoginOK(ulong clientId, Response response, Authenticator.AccountId accountId);

                    /// <summary>
                    ///   This method is executed when a client
                    ///   has failed an authentication attempt.
                    /// </summary>
                    /// <param name="clientId">The id of the current connection failing a login attempt</param>
                    /// <param name="response">The failure response</param>
                    /// <param name="accountId">The id of the account, if one is found, that was attempted to login</param>
                    protected abstract Task OnAccountLoginFailed(ulong clientId, Response response, Authenticator.AccountId accountId);

                    /// <summary>
                    ///   This method is executed when a client is
                    ///   kicked from the authentication system.
                    ///   This may involve disconnecting the client
                    ///   as well, as long as the client is not
                    ///   also the host. This method is quite
                    ///   important, since game cleanup must be
                    ///   done in its implementation.
                    /// </summary>
                    /// <param name="clientId">The id of the current connection being kicked</param>
                    /// <param name="reason">The kick reason</param>
                    /// <param name="accountId">The id of the account being kicked</param>
                    protected abstract Task OnAccountLoggedOut(ulong clientId, Reason reason, Authenticator.AccountId accountId);
                }
            }
        }
    }
}