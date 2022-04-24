using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            public static async Task<ElementType[]> List<ElementType, CursorType, AuthType>(string endpoint,
                AuthType authorization, CursorType cursor) where AuthType : Authorization where CursorType : Cursor
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

            public static async Task<string> Create<ElementType, AuthType>(string endpoint,
                ElementType data, AuthType authorization) where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = "POST";
                request.uploadHandler = new UploadHandlerRaw(Serialize<ElementType>(data));
                // Send the request.
                await SendRequest(request);
                // Get the result.
                long status = request.responseCode;
                FailOnAccess(status);
                FailOnConflict(status, request.downloadHandler);
                FailOnBadRequest(status, request.downloadHandler);
                FailOnFormatError(status);
                FailOnServerError(status);
                FailOnOtherErrors(status);
                try
                {
                    return Deserialize<Created>(request.downloadHandler.data).Id;
                }
                catch (Exception)
                {
                    // The id will not be returned, but no error will be raised.
                    return "";
                }
            }
        }
    }
}