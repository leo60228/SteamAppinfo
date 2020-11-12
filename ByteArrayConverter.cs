using System;
using System.Globalization;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SteamAppinfo {
    public class ByteArrayConverter : JsonConverter<byte[]> {
        public override byte[] Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) {
            String hex = reader.GetString();
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2) {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        public override void Write(
            Utf8JsonWriter writer,
            byte[] val,
            JsonSerializerOptions options
        ) {
            StringBuilder hex = new StringBuilder(val.Length * 2);
            foreach (byte b in val) {
                hex.AppendFormat("{0:x2}", b);
            }
            writer.WriteStringValue(hex.ToString());
        }
    }
}
