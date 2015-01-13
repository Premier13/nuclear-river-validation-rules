using System;
using System.Net.Http;
using System.Web.OData;
using System.Web.OData.Query;

using Microsoft.OData.Edm;

namespace NuClear.AdvancedSearch.OData.Query.Tests
{
    public static class TestHelper
    {
        public static ODataQueryOptions CreateQueryOptions(IEdmModel model, Type elementClrType, HttpRequestMessage request)
        {
            var context = new ODataQueryContext(model, elementClrType, null);
            var queryOptions = new ODataQueryOptions(context, request);
            return queryOptions;
        }

        public static HttpRequestMessage CreateRequest(string query = null)
        {
            var uriBuilder = new UriBuilder();
            if (query != null)
            {
                uriBuilder.Query = query;
            }

            return new HttpRequestMessage { RequestUri = uriBuilder.Uri };
        }
    }
}