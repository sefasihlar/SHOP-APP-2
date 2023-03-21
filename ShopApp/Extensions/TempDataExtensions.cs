using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace ShopApp.WebUI.Extensions
{
    public static class TempDataExtensions
    {
        public static void Put<T>(this ITempDataDictionary tempData, string key, T value) where T : class
        {
            tempData[key] = JsonConvert.SerializeObject(value);
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key) where T : class
        {
            if (tempData.TryGetValue(key, out object o) && o is string s && !string.IsNullOrEmpty(s))
            {
                return JsonConvert.DeserializeObject<T>(s);
            }

            return null;
        }
    }
}
