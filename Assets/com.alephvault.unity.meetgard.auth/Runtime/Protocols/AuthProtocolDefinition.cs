using AlephVault.Unity.Binary;
using AlephVault.Unity.Meetgard.Protocols;

namespace AlephVault.Unity.Meetgard.Auth
{
    namespace Protocols
    {
        /// <summary>
        ///   This protocol provides authentication features
        ///   and session management (concretely: login, logout,
        ///   and kick). The login message will not be implemented,
        ///   so multiple login messages (each one for a different 
        ///   realm) can be implemented in a single auth protocol.
        /// </summary>
        /// <typeparam name="LoginOK">The message content type for when the login is accepted</typeparam>
        /// <typeparam name="LoginFailed">The message content type for when the login is rejected</typeparam>
        /// <typeparam name="Kicked">The message content type for when the user is kicked (e.g. including reason)</typeparam>
        public class AuthProtocolDefinition<LoginRequest, LoginOK, LoginFailed, Kicked> : ProtocolDefinition
            where LoginRequest : ISerializable, new()
            where LoginOK : ISerializable, new()
            where LoginFailed : ISerializable, new()
            where Kicked : ISerializable, new()
        {
            protected override void DefineMessages()
            {
                // No "login" message will be provided explicitly, so
                // multiple realms can be implemented, each with a
                // different type.
                DefineServerMessage("Welcome");
                DefineServerMessage("Timeout");
                DefineServerMessage<LoginOK>("OK");
                DefineServerMessage<LoginFailed>("Failed");
                DefineServerMessage<Kicked>("Kicked");
                DefineClientMessage("Logout");
                DefineServerMessage("LoggedOut");
            }
        }
    }
}
