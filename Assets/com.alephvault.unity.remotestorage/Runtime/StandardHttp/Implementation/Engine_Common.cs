using System.IO;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Newtonsoft.Json;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            /// <summary>
            ///   An exception to be raised on http queries.
            /// </summary>
            public class Exception : System.Exception
            {
                /// <summary>
                ///   The result code.
                /// </summary>
                public readonly ResultCode Code;

                public Exception(ResultCode code) : base($"Storage access failure ({code})")
                {
                    Code = code;
                }
            }

            // Sends a request, waits for it, and captures some errors.
            private static async Task SendRequest(UnityWebRequest request)
            {
                await request.SendWebRequest();
                // Check whether the request was done successfully.
                switch (request.result)
                {
                    case UnityWebRequest.Result.ConnectionError:
                        throw new Exception(ResultCode.Unreachable);
                    case UnityWebRequest.Result.ProtocolError:
                    case UnityWebRequest.Result.DataProcessingError:
                        throw new Exception(ResultCode.ClientError);
                    // default: continue.
                }
            }

            // Deserializes content using Newtonsoft.Json.
            private static ElementType Deserialize<ElementType>(byte[] data, ResultCode errorCode = ResultCode.FormatError)
            {
                try
                {
                    return JsonSerializer.Create().Deserialize<ElementType>(
                        new JsonTextReader(new StreamReader(new MemoryStream(data)))
                    );
                }
                catch (Exception)
                {
                    throw new Exception(errorCode);
                }
            }
        }
    }
}