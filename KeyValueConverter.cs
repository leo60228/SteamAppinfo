using System;
using System.Linq;
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
                if (val.Children.Select((x, i) => x.Name == i.ToString()).All(x => x)) {
                    writer.WriteStartArray();
                    foreach (var child in val.Children) {
                        Write(writer, child, options);
                    }
                    writer.WriteEndArray();
                } else {
                    writer.WriteStartObject();
                    foreach (var child in val.Children) {
                        writer.WritePropertyName(child.Name);
                        Write(writer, child, options);
                    }
                    writer.WriteEndObject();
                }
            } else {
                decimal dec;
                bool boolean;
                if (Decimal.TryParse(val.Value, out dec)) {
                    writer.WriteNumberValue(dec);
                } else if (Boolean.TryParse(val.Value, out boolean)) {
                    writer.WriteBooleanValue(boolean);
                } else {
                    writer.WriteStringValue(val.Value);
                }
            }
        }
    }
}
