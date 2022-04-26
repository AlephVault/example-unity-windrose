using System.Linq;
using AlephVault.Unity.RemoteStorage.Input.Samples;
using AlephVault.Unity.Support.Generic.Authoring.Types;
using Newtonsoft.Json.Linq;
using UnityEngine;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Samples
    {
        using AlephVault.Unity.RemoteStorage.StandardHttp.Types;

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

                var resultAC1 = await accounts.Create(new Account()
                {
                    Address = "address",
                    Inventory = new System.Collections.Generic.Dictionary<string, string>() {{"112357", "10000"}},
                    Name = "My-Account"
                });
                Debug.Log($"Accounts.Create: {resultAC1.Code} {resultAC1.CreatedID} {resultAC1.ValidationErrors}");
                
                var resultAC2 = await accounts.Create(new Account()
                {
                    Address = "address2",
                    Inventory = new System.Collections.Generic.Dictionary<string, string>() {{"112358", "20000"}},
                    Name = "My-Account2"
                });
                Debug.Log($"Accounts.Create: {resultAC2.Code} {resultAC2.CreatedID} {resultAC1.ValidationErrors}");
                
                var resultAL1 = await accounts.List(new Cursor(0, 20));
                // Warning: The inventory will not be retrieved due to projection.
                string reps = string.Join(",", from account in resultAL1.Elements select account.ToString());
                Debug.Log($"Accounts.List: {resultAL1.Code} {reps}");

                Account acc1 = resultAL1.Elements[0];
                Account acc2 = resultAL1.Elements[1];

                JObject updates3 = new JObject();
                updates3["$set"] = new JObject();
                updates3["$set"]["name"] = "My New Account Name 1";
                updates3["$set"]["address"] = "address1++";
                var resultAU1 = await accounts.Update(acc1.Id, updates3);
                Debug.Log($"Accounts.Update: {resultAU1.Code} {resultAU1.Element}");
                
                JObject updates4 = new JObject();
                updates4["$set"] = new JObject();
                updates4["$set"]["name"] = null;
                updates4["$set"]["address"] = null;
                var resultAU2 = await accounts.Update(acc2.Id, updates4);
                Debug.Log($"Accounts.Update: {resultAU2.Code} {resultAU2.Element}");
                
                var resultAL2 = await accounts.List(new Cursor(0, 20));
                // Warning: The inventory will not be retrieved due to projection.
                string reps2 = string.Join(",", from account in resultAL2.Elements select account.ToString());
                Debug.Log($"Accounts.List: {resultAL2.Code} {reps}");

                var resultARp1 = await accounts.Replace(acc1.Id, new Account()
                {
                    Address = "address-replacement",
                    Inventory = new System.Collections.Generic.Dictionary<string, string>() {{"112357", "10000"}},
                    Name = "My-Account-replacement"
                });
                Debug.Log($"Accounts.Replace: {resultARp1.Code} {resultARp1.Element}");

                var resultAD1 = await accounts.Delete(acc1.Id);
                Debug.Log($"Accounts.Delete: {resultAD1.Code} {resultAD1.Element}");
                
                var resultAL3 = await accounts.List(new Cursor(0, 20));
                // Warning: The inventory will not be retrieved due to projection.
                string reps3 = string.Join(",", from account in resultAL3.Elements select account.ToString());
                Debug.Log($"Accounts.List: {resultAL3.Code} {reps}");
                
                // Now, on the methods:
                //   Collection: total-items.
                //   Item (acc2.Id): total-items, total-items-for-type, add-items-for-type, subtract-items-for-type.

                var resultACM1 = await accounts.View("total-items", null);
                Debug.Log($"Accounts.[View:'total-items']: {resultACM1.Code} {resultACM1.Element}");
                
                var resultAIM1 = await accounts.ItemView(acc2.Id, "total-items", null);
                Debug.Log($"Accounts.[ItemView:'total-items']: {resultAIM1.Code} {resultAIM1.Element}"); 
                
                var resultAIM2 = await accounts.ItemView(acc2.Id, "total-items-for-type", new Dictionary<string, string>(){{"type", "112358"}});
                Debug.Log($"Accounts.[ItemView:'total-items-for-type']: {resultAIM2.Code} {resultAIM2.Element}");

                var resultAIM3 = await accounts.ItemOperation(acc2.Id, "add-items-for-type", null, new ItemDelta() { Item = "112358", By = "10"});
                Debug.Log($"Accounts.[ItemView:'add-items-for-type']: {resultAIM3.Code} {resultAIM3.Element}");
                
                var resultAIM4 = await accounts.ItemOperation(acc2.Id, "subtract-items-for-type", null, new ItemDelta() { Item = "112358", By = "7"});
                Debug.Log($"Accounts.[ItemView:'subtract-items-for-type']: {resultAIM4.Code} {resultAIM4.Element}");
                
                var resultAIM5 = await accounts.ItemView(acc2.Id, "total-items", null);
                Debug.Log($"Accounts.[ItemView:'total-items']: {resultAIM5.Code} {resultAIM5.Element}"); 
                
                var resultAIM6 = await accounts.ItemView(acc2.Id, "total-items-for-type", new Dictionary<string, string>(){{"type", "112358"}});
                Debug.Log($"Accounts.[ItemView:'total-items-for-type']: {resultAIM6.Code} {resultAIM6.Element}");
            }
        }
    }
}
