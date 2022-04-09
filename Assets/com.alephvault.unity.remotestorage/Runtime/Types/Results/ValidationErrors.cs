using System.Collections.Generic;


namespace AlephVault.Unity.RemoteStorage
{
    namespace Types
    {
        namespace Results
        {
            /// <summary>
            ///   A list of validation errors. By convention, the empty string
            ///   stands for cross-field validation errors.
            /// </summary>
            public class ValidationErrors : Dictionary<string, List<string>>
            {
                /// <summary>
                ///   This key stands for validation errors that span several
                ///   fields at once.
                /// </summary>
                public const string CrossFieldErrors = "";
                
                /// <summary>
                ///   Adds a single field error to the list.
                /// </summary>
                /// <param name="field">The field to add an error for</param>
                /// <param name="error">The error message to add</param>
                public void AddError(string field, string error)
                {
                    List<string> errors;
                    
                    try
                    {
                        errors = this[field];
                    }
                    catch (KeyNotFoundException)
                    {
                        errors = new List<string>();
                        this[field] = errors;
                    }
                    
                    errors.Add(error);
                }

                /// <summary>
                ///   Adds a single cross-field error to the list.
                /// </summary>
                /// <param name="error">The cross-field error to add</param>
                public void AddNonFieldError(string error)
                {
                    AddError(CrossFieldErrors, error);
                }
            }
        }
    }
}