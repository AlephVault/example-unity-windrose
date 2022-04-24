using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            /// <summary>
            ///   Lists the result from an endpoint. Typically, this is intended for
            ///   the "/foo" list endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="authorization">The authorization to use</param>
            /// <param name="cursor">The cursor to use for paging</param>
            /// <typeparam name="ElementType">The type of elements</typeparam>
            /// <typeparam name="CursorType">The cursor type</typeparam>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            /// <returns>The list of elements</returns>
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

            /// <summary>
            ///   Gets the result from an endpoint. Typically, this is intended for
            ///   both "/foo/{objectid}" list-element endpoints, and "/bar" simple
            ///   endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="authorization">The authorization to use</param>
            /// <typeparam name="ElementType">The type of elements</typeparam>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            /// <returns>The element</returns>
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

            /// <summary>
            ///   Creates an element using a create endpoint. Typically, this is
            ///   intended for both "/foo" list-element endpoints, and "/bar"
            ///   simple element endpoints. An "already-exists" conflict may
            ///   arise for the "/bar" simple element endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="data">The data to create the new element with</param>
            /// <param name="authorization">The authorization to use</param>
            /// <typeparam name="ElementType">The type of elements</typeparam>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            /// <returns>The id of the new element, or empty if the 200 response does not have expected format</returns>
            public static async Task<string> Create<ElementType, AuthType>(string endpoint,
                ElementType data, AuthType authorization) where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = "POST";
                request.uploadHandler = new UploadHandlerRaw(Serialize(data));
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

            /// <summary>
            ///   Updates an element using an update endpoint. Typically, this
            ///   is intended for both "/foo/{objectid}" list-element endpoints,
            ///   and "/bar" simple element endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="patch">The data to patch the new element with</param>
            /// <param name="authorization">The authorization to use</param>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            public static async Task Update<AuthType>(string endpoint, AuthType authorization, JObject patch)
                where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = "PATCH";
                request.uploadHandler = new UploadHandlerRaw(Serialize(patch));
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
                // Everything is OK by this point.                
            }

            /// <summary>
            ///   Replaces an element using a replace endpoint. Typically, this
            ///   intended intended for both "/foo/{objectid}" list-element
            ///   endpoints, and "/bar" simple element endpoints. An "already-exists"
            ///   conflict may arise for the "/bar" simple element endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="replacement">The data to replace the element with</param>
            /// <param name="authorization">The authorization to use</param>
            /// <typeparam name="ElementType">The type of elements</typeparam>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            public static async Task Replace<ElementType, AuthType>(string endpoint, ElementType replacement,
                AuthType authorization) where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = "PUT";
                request.uploadHandler = new UploadHandlerRaw(Serialize(replacement));
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
                // Everything is OK by this point.
            }

            /// <summary>
            ///   Deletes an element using a delete endpoint. Typically, this
            ///   intended intended for both "/foo/{objectid}" list-element
            ///   endpoints, and "/bar" simple element endpoints. An "already-exists"
            ///   conflict may arise for the "/bar" simple element endpoints.
            /// </summary>
            /// <param name="endpoint">The whole endpoint url</param>
            /// <param name="authorization">The authorization to use</param>
            /// <typeparam name="AuthType">The authentication type</typeparam>
            public static async Task Delete<AuthType>(string endpoint, AuthType authorization)
                where AuthType : Authorization
            {
                UnityWebRequest request = new UnityWebRequest(endpoint.Split('?')[0]);
                request.SetRequestHeader("Authorization", $"{authorization.Scheme} {authorization.Value}");
                request.SetRequestHeader("Content-Type", "application/json");
                request.method = "DELETE";
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
                // Everything is OK by this point.
            }
        }
    }
}