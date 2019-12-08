using System.ComponentModel;

namespace APIS.Enums
{
    public enum EContentType
    {
        [Description("application/x-www-form-urlencoded")]
        FormUrlEncoded,

        [Description("multipart/form-data")]
        MultipartFormData,
    }
}