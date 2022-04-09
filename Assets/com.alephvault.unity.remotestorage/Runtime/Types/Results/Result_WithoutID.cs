using System.Collections.Generic;

namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Results
        {
            /// <summary>
            ///   A resource operation result, aware of the element type.
            ///   Stands for "weak" and "element" resources.
            /// </summary>
            public class Result<ElementType>
            {
                /// <summary>
                ///   The operation result.
                /// </summary>
                public ResultCode Code;

                /// <summary>
                ///   The validation errors, suitable for when a Create,
                ///   Update or Replace has validation errors.
                /// </summary>
                public ValidationErrors ValidationErrors;
                
                /// <summary>
                ///   A retrieved element, on <see cref="ResultCode.Ok" />
                ///   for a "weak" or "element" resource result.
                /// </summary>
                public ElementType Element;
            }
        }
    }
}