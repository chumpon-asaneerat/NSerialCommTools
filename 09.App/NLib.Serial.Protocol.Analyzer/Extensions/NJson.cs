#region Using

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

#endregion

namespace NLib
{
    #region CorrectedIsoDateTimeConverter, CorrectedIsoNullableDateTimeConverter for System.Text.Json

    /// <summary>
    /// Custom DateTime converter for System.Text.Json that handles ISO date format with timezone support
    /// </summary>
    class CorrectedIsoDateTimeConverter : JsonConverter<DateTime>
    {
        #region Static Constant Fields

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";

        #endregion

        #region Read

        /// <summary>
        /// Read.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                string dateString = reader.GetString();

                // Try to parse with the default format first
                if (DateTime.TryParseExact(dateString, DefaultDateTimeFormat,
                    CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result))
                {
                    return result;
                }

                // Fallback to standard parsing
                if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out result))
                {
                    return result;
                }
            }

            throw new JsonException($"Unable to convert \"{reader.GetString()}\" to DateTime");
        }

        #endregion

        #region Write

        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            DateTime dateTime = value;
            // Handle unspecified DateTime kind - force to local as per original code
            if (dateTime.Kind == DateTimeKind.Unspecified)
            {
                dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
            }
            // Format the datetime using the default format
            string formattedDate = dateTime.ToString(DefaultDateTimeFormat,
                DateTimeFormatInfo.InvariantInfo);

            if (null != writer)
                writer.WriteStringValue(formattedDate);
        }

        #endregion
    }

    /// <summary>
    /// Custom nullable DateTime converter for System.Text.Json
    /// </summary>
    class CorrectedIsoNullableDateTimeConverter : JsonConverter<DateTime?>
    {
        #region Static Constant Fields

        private readonly CorrectedIsoDateTimeConverter _converter = new CorrectedIsoDateTimeConverter();

        #endregion

        #region Read

        /// <summary>
        /// Read.
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            return _converter.Read(ref reader, typeof(DateTime), options);
        }

        #endregion

        #region Write

        /// <summary>
        /// Write.
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                if (null != writer) writer.WriteNullValue();
            }
            else
            {
                _converter.Write(writer, value.Value, options);
            }
        }

        #endregion
    }

    #endregion

    #region NJson

    /// <summary>
    /// The Json Extension Methods.
    /// </summary>
    public static class NJson
    {
        #region Readonly Options

        /// <summary>
        /// Gets Default JsonSerializerOptions.
        /// </summary>
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // preserve peroperty names as they are
            PropertyNameCaseInsensitive = true,
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Use unsafe relaxed escaping for compactness
            Converters =
            {
                new CorrectedIsoDateTimeConverter(),
                new CorrectedIsoNullableDateTimeConverter()
            }
        };
        /// <summary>
        /// Gets Compact JsonSerializerOptions.
        /// </summary>
        public static readonly JsonSerializerOptions CompactOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = null, // preserve peroperty names as they are
            PropertyNameCaseInsensitive = true,
            WriteIndented = false,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, // Use unsafe relaxed escaping for compactness
            Converters =
            {
                new CorrectedIsoDateTimeConverter(),
                new CorrectedIsoNullableDateTimeConverter()
            }
        };

        #endregion

        #region ToJson

        /// <summary>
        /// Convert Object to Json String.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="minimized">True for minimize output.</param>
        /// <returns>Returns json string.</returns>
        public static string ToJson<T>(this T value, bool minimized = false)
        {
            string result = string.Empty;
            if (null == value)
                return result;

            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                var options = (minimized) ? CompactOptions : DefaultOptions;
                result = JsonSerializer.Serialize(value, options);
            }
            catch (Exception ex)
            {
                ex.Err(med);
            }
            return result;
        }

        #endregion

        #region FromJson

        /// <summary>
        /// Convert Object from Json String.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The json string.</param>
        /// <returns>Returns json string.</returns>
        public static T FromJson<T>(this string value)
        {
            T result = default;
            try
            {
                var options = DefaultOptions;
                result = JsonSerializer.Deserialize<T>(value, options);
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            return result;
        }

        #endregion

        #region SaveToFile

        /// <summary>
        /// Save object to json file.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="value">The object instance.</param>
        /// <param name="fileName">The target file name.</param>
        /// <param name="minimized">True for minimize output.</param>
        /// <returns>Returns true if save success.</returns>
        public static bool SaveToFile<T>(this T value, string fileName,
            bool minimized = false)
        {
            bool result = false;

            MethodBase med = MethodBase.GetCurrentMethod();

            try
            {
                // Check directory
                string pathname = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(pathname))
                {
                    Directory.CreateDirectory(pathname);
                }

                var options = (minimized) ? CompactOptions : DefaultOptions;
                // Gets json in bytes.
                var utf8Bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);
                // serialize JSON directly to a file
                File.WriteAllBytes(fileName, utf8Bytes);

                result = true;
            }
            catch (Exception ex)
            {
                ex.Err(med);
                result = false;
            }

            return result;
        }

        #endregion

        #region LoadFromFile

        /// <summary>
        /// Load Object from Json file.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="fileName">The target file name.</param>
        /// <returns>Returns object instance if load success.</returns>
        public static T LoadFromFile<T>(string fileName)
        {
            T result = default;

            MethodBase med = MethodBase.GetCurrentMethod();

            if (File.Exists(fileName))
            {
                try
                {
                    // Ensure the file exists and can be read
                    var fileBytes = File.ReadAllBytes(fileName);

                    var options = DefaultOptions;
                    result = JsonSerializer.Deserialize<T>(fileBytes, options);
                }
                catch (Exception ex)
                {
                    ex.Err(med);
                    result = default;
                }
            }

            return result;
        }

        #endregion
    }

    #endregion
}
