namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Results
        {
            /// <summary>
            ///   The result code for a resource operation.
            /// </summary>
            public enum ResultCode
            {
                // Regarding permission and availability.
                
                /// <summary>
                ///   The user is not authorized (logged in).
                /// </summary>
                Unauthorized,
                
                /// <summary>
                ///   The user is authorized, but the method is forbidden for them.
                /// </summary>
                Forbidden,
                
                /// <summary>
                ///   The user is authorized and not forbidden, but this resource
                ///   does not accept this method.
                /// </summary>
                Unsupported,
                
                /// <summary>
                ///   The user is authorized and not forbidden. The method is supported.
                ///   However, the resource is not found (perhaps the resource itself
                ///   or one of its items).
                /// </summary>
                DoesNotExist,
                
                // Regarding edition.
                
                /// <summary>
                ///   The user is authenticated and not forbidden. The method is supported.
                ///   However, when trying to create (to a single resource), the only allowed
                ///   resource already exists and cannot be created another time. This does
                ///   not make sense for list resources, but only single ones.
                /// </summary>
                AlreadyExists,
                
                /// <summary>
                ///   The user is authenticated and not forbidden. The method is supported.
                ///   However, when trying to create, update or replace, there was a validation
                ///   error. Additional data is provided with the validation messages.
                /// </summary>
                ValidationError,
                
                /// <summary>
                ///   The user is authenticated and not forbidden. The method is supported.
                ///   However, when trying to create, update or replace, there was a conflict
                ///   (which is not a validation error but a posterior check). This applies
                ///   for the delete method as well (e.g. a referential integrity error).
                /// </summary>
                Conflict,
                
                /// <summary>
                ///   The query or request went well, but the result is a list when it should
                ///   not, or vice versa. Alternatively, the result was not appropriately
                ///   deserialized (this applies to any engine) or it is not of appropriate
                ///   format (this applies to engines like HTTP).
                /// </summary>
                FormatError,
                
                // Regarding other client-side (i.e. unity server) errors.
                
                /// <summary>
                ///   Another type of client error has occurred (e.g. wrong format). This involves
                ///   an error that should never be released to the end-user, but instead logged.
                ///   A descriptive string is also included with the error.
                /// </summary>
                ClientError,
                
                // Regarding server errors.

                /// <summary>
                ///   The server could not be contacted. This occurs either as a connectivity error
                ///   (i.e. Connection Refused) or as a gateway error (in http: 502).
                /// </summary>
                Unreachable,

                /// <summary>
                ///   The server cannot be contacted. This time, this is internal (i.e. an intended
                ///   maintenance activity on the storage server).
                /// </summary>
                ServiceUnavailable,
                
                /// <summary>
                ///   The server can be contacted: the connection could be established. However,
                ///   it is taking a lot. This occurs either as a connectivity error (i.e. Timeout)
                ///   or as a gateway error (in http: 504).
                /// </summary>
                Timeout,
                
                /// <summary>
                ///   Another type of server error has occurred (e.g. HTTP version not supported,
                ///   Request not extended, ...). This involves an error that should never be
                ///   released to the end user, but instead logged.
                /// </summary>
                ServerError,

                /// <summary>
                ///   An internal error on the storage server. Logs will NOT be kept here, and
                ///   should instead be checked in the storage server itself, since this belongs
                ///   to a programming error or somewhat of (hosted) software misconfiguration
                ///   (framework error, application error).
                /// </summary>
                InternalError,
                
                // Success messages.
                
                /// <summary>
                ///   The element was successfully created. In non-weak elements (single, list)
                ///   an ID of the newly created element is added.
                /// </summary>
                Created,
                
                /// <summary>
                ///   The element was successfully updated.
                /// </summary>
                Updated,
                
                /// <summary>
                ///   The element was successfully replaced.
                /// </summary>
                Replaced,
                
                /// <summary>
                ///   The element was successfully deleted.
                /// </summary>
                Deleted,
                
                /// <summary>
                ///   The element was found (or elements, if a list was requested).
                /// </summary>
                Ok,
            }
        }
    }
}