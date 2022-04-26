using AlephVault.Unity.RemoteStorage.Input.Samples;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Samples
    {
        public class SampleHTTPInteractor : MonoBehaviour
        {
            void Start()
            {
                HttpInteract();
            }

            private async void HttpInteract()
            {
                Root root = new Root("http://localhost:6666", new Authorization("Bearer", "abcdef"));
                ListResource<Account, Account> accounts = (ListResource<Account, Account>)root.GetList<Account, Account>("accounts");
                SimpleResource<Universe> universe = (SimpleResource<Universe>) root.GetSimple<Universe>("universe");
                
                var resultUD1 = await universe.Delete();
                Debug.Log($"Universe.Delete: {resultUD1.Code} {resultUD1.CreatedID}");

                var resultUC1 = await universe.Create(new Universe()
                    {
                        Caption = "Sample Universe", MOTD = "Welcome!",
                        Version = new Version() {Major = 0, Minor = 0, Revision = 0}
                    }
                );
                Debug.Log($"Universe.Create: {resultUC1.Code} {resultUC1.CreatedID}");

                var resultURd1 = await universe.Read();
                // Warning: due to projection settings, the version will not come
                // as a member of the universe. Only caption and motd.
                Debug.Log($"Universe.Read: {resultURd1.Code} {resultURd1.Element}");

                var resultURp1 = await universe.Replace(new Universe()
                {
                    Caption = "Sample Universe (2.1)", MOTD = "Welcome! (2)",
                    Version = new Version() {Major = 2, Minor = 1, Revision = 4}
                });
                Debug.Log($"Universe.Replace: {resultURp1.Code} {resultURp1.Element}");
                
                var resultURd2 = await universe.Read();
                // Warning: due to projection settings, the version will not come
                // as a member of the universe. Only caption and motd.
                Debug.Log($"Universe.Read: {resultURd2.Code} {resultURd2.Element}");

                // This one should be an error.
                var resultURp2 = await universe.Replace(new Universe()
                {
                    Caption = "Sample Universe (2.1)", MOTD = null,
                    Version = new Version {Major = 2, Minor = 1, Revision = 0}
                });
                Debug.Log($"Universe.Replace: {resultURp2.Code} {resultURp2.ValidationErrors}");

                JObject updates = new JObject();
                updates["$set"] = new JObject();
                updates["$set"]["caption"] = "Sample Universe (3)";
                updates["$set"]["version.revision"] = 9;
                var resultUUp1 = await universe.Update(updates);
                Debug.Log($"Universe.Update: {resultUUp1.Code} {resultUUp1.Element}");

                JObject updates2 = new JObject();
                updates2["$set"] = new JObject();
                updates2["$set"]["caption"] = "";
                updates2["$set"]["version.revision"] = -1;
                var resultUUp2 = await universe.Update(updates2);
                Debug.Log($"Universe.Update: {resultUUp2.Code} {resultUUp2.ValidationErrors}");

                var resultUM1 = await universe.View("version", new Dictionary<string, string>() {{"foo", "bar"}});
                Debug.Log($"Universe.[Version]: {resultUM1.Code} {resultUM1.Element}");

                var resultUM2 = await universe.Operation("set-motd", new Dictionary<string, string>() {{"foo", "bar"}}, new MOTDInput { MOTD = "New MOTD!!!!!!!"});
                Debug.Log($"Universe.[SetMotd]: {resultUM2.Code} {resultUM2.Element}");
                // var resultUD2 = await universe.Delete();
                // Debug.Log($"Universe.Delete: {resultUD2.Code} {resultUD2.CreatedID}");
            }
        }
    }
}
