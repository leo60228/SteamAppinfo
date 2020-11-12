using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using SteamKit2;

namespace SteamAppinfo {
    public class KeyValueConverter : JsonConverter<KeyValue> {
        public override KeyValue Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            KeyValue val,
            JsonSerializerOptions options
        ) {
            if (val.Children.Count > 0) {
                writer.WriteStartObject();
                foreach (var child in val.Children) {
                    writer.WritePropertyName(child.Name);
                    Write(writer, child, options);
                }
                writer.WriteEndObject();
            } else {
                writer.WriteStringValue(val.Value);
            }
        }
    }
}
