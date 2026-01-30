using System;
using Newtonsoft.Json;

namespace Other
{
    public class IntToInt32Converter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(object);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value is long l)
            {
                if (l <= int.MaxValue && l >= int.MinValue)
                    return (int)l;
            }
            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(value);
        }
    }
}