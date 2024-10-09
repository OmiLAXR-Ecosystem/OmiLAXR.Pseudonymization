using System;

namespace OmiLAXR.Pseudonymization.Helper
{
    public class UriHelper
    {
        public static Uri Combine(params string[] uris)
        {
            var uri = Combine(uris[0], uris[1]);
            for (var i = 2; i < uris.Length; i++)
            {
                uri = Combine(uri.ToString(), uris[i]);
            }
            return uri;           
        }
        public static Uri Combine(string baseUri, string relativeOrAbsoluteUri)
            => new Uri(new Uri(baseUri), relativeOrAbsoluteUri);

        public static string CombineToString(string baseUri, string relativeOrAbsoluteUri)
            => new Uri(new Uri(baseUri), relativeOrAbsoluteUri).ToString();

        public static string CombineToString(params string[] uris)
            => Combine(uris).ToString();
    }

}