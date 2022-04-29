using MongoDB.Bson;
using System.Collections.Generic;

namespace ElectronicQueueServer.Models
{
    public static class DictionaryExtension
    {
        public static T ToObject<T>(this Dictionary<string, object> source) where T : class, new()
        {
            var obj = new T();
            var objType = obj.GetType();

            foreach(var pair in source)
            {
                var objKey = pair.Key.Capitalize();
                var property = objType.GetProperty(objKey);
                if (property.PropertyType == typeof(ObjectId))
                {
                    property.SetValue(obj, ObjectId.Parse(pair.Value as string));
                }
                else
                {
                    property.SetValue(obj, pair.Value);
                }
            }

            return obj;
        }
    }
}
