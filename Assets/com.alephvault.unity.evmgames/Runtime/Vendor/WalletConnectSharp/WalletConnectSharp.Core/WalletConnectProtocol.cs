using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Events;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Network;

namespace AlephVault.Unity.EVMGames.WalletConnectSharp.Core
{
    public class WalletConnectProtocol : IDisposable
    {
        public static readonly string[] SigningMethods = new[]
        {
            "eth_sendTransaction",
            "eth_signTransaction",
            "eth_sign",
            "eth_signTypedData",
            "eth_signTypedData_v1",
            "eth_signTypedData_v2",
            "eth_signTypedData_v3",
            "eth_signTypedData_v4",
            "personal_sign",
        };

        public readonly EventDelegator Events;
        
        protected string Version = "1";
        protected string _bridgeUrl;
        protected string _key;
        protected byte[] _keyRaw;
        protected List<string> _activeTopics = new List<string>();

        public event EventHandler<WalletConnectProtocol> OnTransportConnect;
        public event EventHandler<WalletConnectProtocol> OnTransportDisconnect;

        public bool SessionConnected { get; protected set; }
        
        public bool Disconnected { get; protected set; }

        public bool Connected => SessionConnected && TransportConnected;

        public bool Connecting { get; protected set; }

        public bool TransportConnected => Transport != null && Transport.Connected && Transport.URL == _bridgeUrl;

        public ITransport Transport { get; private set; }

        public ICipher Cipher { get; private set; }
        
        public ClientMeta DappMetadata { get; set; }
        
        public ClientMeta WalletMetadata { get; set; }

        public ReadOnlyCollection<string> ActiveTopics => _activeTopics.AsReadOnly();

        public string PeerId
        {
            get;
            protected set;
        }


        /// <summary>
        /// Create a new WalletConnectProtocol object using a SavedSession as the session data. This will effectively resume
        /// the session, as long as the session data is valid
        /// </summary>
        /// <param name="savedSession">The SavedSession data to use. Cannot be null</param>
        /// <param name="transport">The transport interface to use for sending/receiving messages, null will result in the default transport being used</param>
        /// <param name="cipher">The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being used</param>
        /// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
        /// <exception cref="ArgumentException">If a null SavedSession object was given</exception>
        public WalletConnectProtocol(SavedSession savedSession, ITransport transport = null, 
                                     ICipher cipher = null, EventDelegator eventDelegator = null)
        {
            if (savedSession == null)
                throw new ArgumentException("savedSession cannot be null");
            _bridgeUrl = savedSession.BridgeURL;
            _keyRaw = savedSession.KeyRaw;
            _key = savedSession.Key;
            PeerId = savedSession.PeerID;
            
            Events = eventDelegator ?? new EventDelegator();
            Transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(Events);
            Cipher = cipher ?? new AESCipher();
        }

        /// <summary>
        /// Create a new WalletConnectProtocol object and create a new dApp session.
        /// </summary>
        /// <param name="transport">The transport interface to use for sending/receiving messages, null will result in the default transport being used</param>
        /// <param name="cipher">The cipher to use for encrypting and decrypting payload data, null will result in AESCipher being used</param>
        /// <param name="chainId">The chainId this dApp is using</param>
        /// <param name="bridgeUrl">The bridgeURL to use to communicate with the wallet</param>
        /// <param name="eventDelegator">The EventDelegator class to use, null will result in the default being used</param>
        /// <exception cref="ArgumentException">If an invalid ClientMeta object was given</exception>
        public WalletConnectProtocol(ITransport transport = null, ICipher cipher = null,
            EventDelegator eventDelegator = null)
        {
            Events = eventDelegator ?? new EventDelegator();
            Transport = transport ?? TransportFactory.Instance.BuildDefaultTransport(Events);
            Cipher = cipher ?? new AESCipher();
        }

        protected async Task SetupTransport()
        {
            Transport.MessageReceived += TransportOnMessageReceived;
            await Transport.Open(_bridgeUrl);
            TriggerOnTransportConnect();
        }

        protected async Task DisconnectTransport()
        {
            await Transport.Close();
            Transport.MessageReceived -= TransportOnMessageReceived;
            OnTransportDisconnect?.Invoke(this, this);
        }

        protected virtual void TriggerOnTransportConnect()
        {
            OnTransportConnect?.Invoke(this, this);
        }
        
        public virtual async Task<WCSessionData> Connect()
        {
            await SetupTransport();
            return null;
        }
        
        public async Task SubscribeAndListenToTopic(string topic)
        {
            await Transport.Subscribe(topic);
            
            ListenToTopic(topic);
        }

        public void ListenToTopic(string topic)
        {
            if (!_activeTopics.Contains(topic))
            {
                _activeTopics.Add(topic);
            }
        }

        private async void TransportOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            var networkMessage = e.Message;

            if (!_activeTopics.Contains(networkMessage.Topic))
                return;

            var encryptedPayload = JsonConvert.DeserializeObject<EncryptedPayload>(networkMessage.Payload);

            var json = await Cipher.DecryptWithKey(_keyRaw, encryptedPayload);

            var response = JsonConvert.DeserializeObject<JsonRpcResponse>(json);

            bool wasResponse = false;
            if (response != null && response.Event != null)
                wasResponse = Events.Trigger(response.Event, json);

            if (!wasResponse)
            {
                var request = JsonConvert.DeserializeObject<JsonRpcRequest>(json);

                if (request != null && request.Method != null)
                    Events.Trigger(request.Method, json);
            }
        }

        public async Task SendRequest<T>(T requestObject, string sendingTopic = null, bool? forcePushNotification = null)
        {
            bool silent;
            if (forcePushNotification != null)
            {
                silent = (bool) !forcePushNotification;
            }
            else if (requestObject is JsonRpcRequest request)
            {
                silent = request.Method.StartsWith("wc_") || !SigningMethods.Contains(request.Method);
            }
            else
            {
                silent = false;
            }

            string json = JsonConvert.SerializeObject(requestObject);

            var encrypted = await Cipher.EncryptWithKey(_keyRaw, json);

            sendingTopic ??= PeerId;

            var message = new NetworkMessage()
            {
                Payload = JsonConvert.SerializeObject(encrypted),
                Silent = silent,
                Topic = sendingTopic,
                Type = "pub"
            };

            await Transport.SendMessage(message);
        }
        
        public void Dispose()
        {
            if (Transport != null)
            {
                Transport.Dispose();
                Transport = null;
            }
        }

        public virtual async Task Disconnect()
        {
            await DisconnectTransport();
        }
    }
}