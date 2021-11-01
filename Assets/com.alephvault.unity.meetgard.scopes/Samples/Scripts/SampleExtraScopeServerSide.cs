using AlephVault.Unity.Meetgard.Scopes.Authoring.Behaviours.Server;
using AlephVault.Unity.Support.Authoring.Behaviours;
using AlephVault.Unity.Support.Generic.Types.Sampling;
using AlephVault.Unity.Support.Types.Sampling;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace AlephVault.Unity.Meetgard.Scopes
{
    namespace Samples
    {
        [RequireComponent(typeof(ScopeServerSide))]
        public class SampleExtraScopeServerSide : MonoBehaviour
        {
            ScopeServerSide scope;

            private float refreshRate;
            private float currentRefresh = 0;
            private bool loaded = false;

            private void Awake()
            {
                scope = GetComponent<ScopeServerSide>();
                scope.OnLoad += Scope_OnLoad;
                scope.OnUnload += Scope_OnUnload;
                scope.OnLeaving += Scope_OnLeaving;
            }

            private void Update()
            {
                if (loaded)
                {
                    if (currentRefresh >= refreshRate)
                    {
                        currentRefresh -= refreshRate;
                        GetComponent<AsyncQueueManager>().QueueAction(DoRefresh);
                    }
                    currentRefresh += Time.deltaTime;
                }
            }

            private void DoRefresh()
            {
                foreach (ObjectServerSide obj in scope.Objects())
                {
                    if (obj is SampleObjectServerSide sobj)
                    {
                        sobj.Color = new Random<Color32>(new Color32[] { Color.white, Color.red, Color.green, Color.blue }).Get();
                        sobj.Position = new RandomBox3(new Vector3(-4, -4, -4), new Vector3(4, 4, 4)).Get();
                    }
                }
            }

            private async Task Scope_OnLeaving(ulong arg)
            {
                // Typically, the dev will make async calls here.
                Debug.Log("ServerSideScopeScope::OnLeaving");
                if (scope.connections.Count == 0)
                {
                    // Destroy this extra scene.
                    var _ = scope.Protocol.UnloadExtraScope(scope.Id);
                }
            }

            private async Task Scope_OnLoad()
            {
                // Typically, the dev will make async calls here.
                Debug.Log("ServerSideScope::OnLoad");
                // Take the protocol and make a default spawn.
                ScopesProtocolServerSide protocol = scope.Protocol;
                // Instantiate the object.
                refreshRate = 1f;
                SampleObjectServerSide obj = protocol.InstantiateHere(0) as SampleObjectServerSide;
                obj.Color = new Random<Color32>(new Color32[] { Color.white, Color.red, Color.green, Color.blue }).Get();
                obj.Position = new RandomBox3(new Vector3(-4, -4, -4), new Vector3(4, 4, 4)).Get();
                var _ = scope.AddObject(obj);
                loaded = true;
            }

            private async Task Scope_OnUnload()
            {
                loaded = false;
                // Typically, the dev will make async calls here.
                Debug.Log("ServerSideScopeScope::OnUnload");
                // Nothing else to do here.
            }

            private void OnDestroy()
            {
                scope.OnLoad -= Scope_OnLoad;
                scope.OnUnload -= Scope_OnUnload;
            }
        }
    }
}
