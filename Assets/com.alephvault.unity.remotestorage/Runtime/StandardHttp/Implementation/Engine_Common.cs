using System;
using System.IO;
using System.Threading.Tasks;
using AlephVault.Unity.RemoteStorage.Types.Results;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            /// <summary>
            ///   An exception to be raised on http queries. The
            ///   validation errors are given, when the case.
            /// </summary>
            public class Exception : System.Exception
            {
                /// <summary>
                ///   The result code.
                /// </summary>
                public readonly ResultCode Code;

                /// <summary>
                ///   The validation errors.
                /// </summary>
                public readonly JObject ValidationErrors;
                
                public Exception(ResultCode code, JObject errors = null) : base($"Storage access failure ({code})")
                {
                    Code = code;
                    ValidationErrors = errors;
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
                catch (System.Exception)
                {
                    throw new Exception(errorCode);
                }
            }

            // Deserializes content using Newtonsoft.Json into JObject.
            private static JObject DeserializeArbitrary(byte[] data, ResultCode errorCode = ResultCode.FormatError)
            {
                try
                {
                    MemoryStream stream = new MemoryStream(data);
                    StreamReader reader = new StreamReader(stream);
                    return JObject.Parse(reader.ReadToEnd());
                }
                catch (System.Exception e)
                {
                    throw new Exception(errorCode);
                }
            }
            
            // Serializes content using Newtonsoft.Json.
            private static byte[] Serialize<ElementType>(ElementType data, ResultCode errorCode = ResultCode.FormatError)
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    JsonSerializer.Create().Serialize(new JsonTextWriter(new StreamWriter(stream)), data);
                    return stream.GetBuffer();
                }
                catch (System.Exception)
                {
                    throw new Exception(errorCode);
                }
            }
            
            // Serializes a JObject content to byte array.
            private static byte[] SerializeArbitrary(JObject data, ResultCode errorCode = ResultCode.FormatError)
            {
                try
                {
                    MemoryStream stream = new MemoryStream();
                    new StreamWriter(stream).Write(data.ToString());
                    return stream.GetBuffer();
                }
                catch (System.Exception)
                {
                    throw new Exception(errorCode);
                }
            }
        }
    }
}