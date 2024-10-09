using System;
using System.Collections.Generic;
using System.Linq;

namespace OmiLAXR.Pseudonymization.Helper
{
    public class QueryBuilder
    {
        private readonly string _url;
        public readonly List<KeyValuePair<string, object>> Entries = new List<KeyValuePair<string, object>>(); 
        public QueryBuilder(string url)
        {
            _url = url;
        }

        public QueryBuilder Append(string key, object value)
        {
            if (value == null)
                return this;
            Entries.Add(new KeyValuePair<string, object>(key, value));
            return this;
        }

        public QueryBuilder Clear()
        {
            Entries.Clear();
            return this;
        }

        public string[] GetAssignments() => Entries.Select(kv => kv.Key + "=" + kv.Value).ToArray();

        public string GetQueryParams() => string.Join("&", GetAssignments());

        public Uri ToUri() => new Uri(new Uri(_url), "?" + GetQueryParams());

        public override string ToString() => ToUri().ToString();
    }
}