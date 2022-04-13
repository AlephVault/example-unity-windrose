using System.IO;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Newtonsoft.Json;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            public static async Task<ElementType[]> List<ElementType, CursorType, AuthType>(string endpoint, AuthType authorization, CursorType cursor)
                where AuthType : Authorization where CursorType : Cursor
            {
                UnityWebRequest request = new UnityWebRequest($"{endpoint.Split('?')[0]}?{cursor.QueryString()}");
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.method = "GET";
                // Send the request.
                await SendRequest(request);
                // Get the result.
                long status = request.responseCode;
                // Check it against standard codes.
                FailOnAccess(status);
                FailOnFormatError(status);
                FailOnServerError(status);
                FailOnOtherErrors(status);
                // Deserialize everything.
                return Deserialize<ElementType[]>(request.downloadHandler.data);
            }

            public static async Task<ElementType> One<ElementType, AuthType>(string endpoint, AuthType authorization)
                where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.method = "GET";
                // Send the request.
                await SendRequest(request);
                // Get the result.
                long status = request.responseCode;
                // Check it against standard codes.
                FailOnAccess(status);
                FailOnFormatError(status);
                FailOnServerError(status);
                FailOnOtherErrors(status);
                // Deserialize everything.
                return Deserialize<ElementType>(request.downloadHandler.data);
            }
        }
    }
}