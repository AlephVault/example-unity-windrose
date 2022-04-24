namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Results
        {
            /// <summary>
            ///   A resource operation result, aware of the element type.
            ///   Stands for "single" or "list" resources.
            /// </summary>
            public class Result<ElementType, ElementIDType>
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
                ///   The ID of the created object, <see cref="ResultCode.Created"/>.
                /// </summary>
                public ElementIDType CreatedID;
                
                /// <summary>
                ///   A retrieved element, on <see cref="ResultCode.Ok" />
                ///   for a "single" resource result.
                /// </summary>
                public ElementType Element;

                /// <summary>
                ///   Many retrieved elements, on <see cref="ResultCode.Ok" />
                ///   for a "list" resource result.
                /// </summary>
                public ElementType[] Elements;
            }
        }
    }
}