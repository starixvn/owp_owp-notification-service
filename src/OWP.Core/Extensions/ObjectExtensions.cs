using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace OWP.Core.Extensions;

public static class ObjectExtensions
{
    public static string ToCamelCaseJson(this object @this)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        var contractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        };

        var settings = new JsonSerializerSettings
        {
            ContractResolver = contractResolver,
            Formatting = Formatting.Indented
        };

        var value = JsonConvert.SerializeObject(@this, settings);

        return value;
    }
}
