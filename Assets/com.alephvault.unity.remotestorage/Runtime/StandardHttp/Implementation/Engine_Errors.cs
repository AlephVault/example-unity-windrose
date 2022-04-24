using AlephVault.Unity.RemoteStorage.StandardHttp.Types;
using AlephVault.Unity.RemoteStorage.Types.Results;
using UnityEngine.Networking;


namespace AlephVault.Unity.RemoteStorage.StandardHttp
{
    namespace Implementation
    {
        public static partial class Engine
        {
            // Fails on errors 401 (auth required), 403 (permission error),
            // 404 (resource, or one of its parents, not found) or 405
            // (method not allowed over this resource). 410 is also taken
            // as a Not Found error.
            private static void FailOnAccess(long status)
            {
                switch (status)
                {
                    case 401:
                        throw new Exception(ResultCode.Unauthorized);
                    case 403:
                        throw new Exception(ResultCode.Forbidden);
                    case 404:
                    case 410:
                        throw new Exception(ResultCode.DoesNotExist);
                    case 405:
                        throw new Exception(ResultCode.Unsupported);
                }
            }
            
            // Fails on server error: 500 (application error, where the
            // error is logged in the server itself), 503 (maintenance
            // of the gateway), 502 (gateway not reachable) or 504
            // (gateway timeout). Also, an arbitrary error code is given
            // as an umbrella error.
            private static void FailOnServerError(long status)
            {
                switch (status)
                {
                    case 500:
                        throw new Exception(ResultCode.InternalError);
                    case 502:
                        throw new Exception(ResultCode.Unreachable);
                    case 503:
                        throw new Exception(ResultCode.ServiceUnavailable);
                    case 504:
                        throw new Exception(ResultCode.Timeout);
                    default:
                        throw new Exception(ResultCode.ServerError);
                }
            }

            // Fails on client format error (e.g. unexpected client settings
            // or unexpected input format).
            private static void FailOnFormatError(long status)
            {
                switch (status)
                {
                    case 406:
                    case 415:
                        throw new Exception(ResultCode.FormatError);
                }
            }
            
            // Fails on validation error (400). It also parses the errors
            // appropriately into the dictionary. This 400 error may
            // occur both in the application or the gateway (e.g. nginx),
            // so a format check for the answer will be performed to see
            // whether this is a validation error or another kind.
            private static void FailOnBadRequest(long status, DownloadHandler downloadHandler)
            {
                // TODO If the response is JSON and providing errors,
                // TODO then we raise an INVALID error. Otherwise,
                // TODO then we raise a FORMAT error.
            }
            
            // Fails on conflict. Conflicts can come in many flavors, but
            // two of them are of particular interest: The object already
            // found (failure on creation) or the object unable to be
            // deleted due to a referential integrity constraint (a manual
            // one, which causes failure on deletion). Other 409 errors
            // will not occur from this environment, but can occur from
            // the gateway, so special format checks will occur as well
            // to distinguish both cases.
            private static void FailOnConflict(long status, DownloadHandler downloadHandler)
            {
                if (status == 409)
                {
                    // Deserialize the conflict reason, falling back to
                    // plain Conflict code if something goes wrong or
                    // the conflict reason is not expected.
                    Conflict conflict = Deserialize<Conflict>(downloadHandler.data, ResultCode.Conflict);
                    ResultCode code;
                    switch (conflict.Code)
                    {
                        case "already-exists":
                            code = ResultCode.AlreadyExists;
                            break;
                        case "in-use":
                            code = ResultCode.InUse;
                            break;
                        case "duplicate-key":
                            code = ResultCode.DuplicateKey;
                            break;
                        default:
                            code = ResultCode.Conflict;
                            break;
                    }

                    // Throw the appropriate exception.
                    throw new Exception(code);
                }
            }

            // Fails on other 4xx or 5xx errors, in a generic way.
            private static void FailOnOtherErrors(long status)
            {
                if (status > 500) throw new Exception(ResultCode.ServerError);
                if (status > 400) throw new Exception(ResultCode.ClientError);
            }
        }
    }
}