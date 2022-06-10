using System;
using AlephVault.Unity.EVMGames.Nethereum.Signer;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Core.Models;
using AlephVault.Unity.EVMGames.WalletConnectSharp.Unity;
using UnityEngine;


namespace AlephVault.Unity.EVMGames
{
    namespace Samples
    {
        [RequireComponent(typeof(WalletConnect))]
        public class SampleSignerAndVerifier : MonoBehaviour
        {
            private WalletConnect walletConnect;
            
            private void Awake()
            {
                walletConnect = GetComponent<WalletConnect>();
            }

            private void Start()
            {
                walletConnect.ConnectedEventSession.AddListener(OnConnected);
            }

            private async void OnConnected(WCSessionData session)
            {
                try
                {
                    Debug.Log($"Current session is: {walletConnect.Session}");
                
                    // 1. Generating the ISO Date for Now.
                    string isoNow = DateTime.Now.ToString("o");
                    Debug.Log($"The current date/time is: {isoNow}");
                    // 2. Signing with WalletConnect.
                    string signed = await walletConnect.Session.EthSign(
                        walletConnect.Session.Accounts[0], isoNow
                    );
                    // 3. Once signed, we need to create a signature verifier.
                    //    This ETH Message Signer will be used on server-side,
                    //    actually, while the previous steps are client-side
                    //    only (WalletConnect is useful in client-side, while
                    //    verifying the signatures is mostly used only on the
                    //    server-side, while in client should only serve as
                    //    some sort of hinting instead).
                    var signer = new EthereumMessageSigner();
                    string result = signer.EncodeUTF8AndEcRecover(isoNow, signed);
                    // 4. We process the result. We obtain what's going on.
                    //    Typically, this means that we get the underlying
                    //    address used for signature.
                    Debug.Log($"The signature result is: {result}");
                    // 5. Close the session.
                    walletConnect.CloseSession(true);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    throw;
                }
            }
        }
    }
}