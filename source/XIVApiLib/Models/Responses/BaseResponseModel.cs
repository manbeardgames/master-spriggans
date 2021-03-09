using System;

namespace XIVApiLib.Models.Responses
{
    /// <summary>
    ///     Base model class that all other response models are extended from.  The properties
    ///     of this base model allow for catching error responses from the XIVApi.
    /// </summary>
    public abstract class BaseResponseModel
    {
        /// <summary>
        ///     Gets or Sets a boolean value indicating if the response is an error
        ///     response. If this property is null, then there is no error.
        /// </summary>
        public bool? Error { get; set; }

        /// <summary>
        ///     Gets or Sets the error message if the response is an error
        ///     response. This will only contain a value if <see cref+"Error" />
        ///     is not null.
        /// </summary> 
        public string Message { get; set; }
    }
}