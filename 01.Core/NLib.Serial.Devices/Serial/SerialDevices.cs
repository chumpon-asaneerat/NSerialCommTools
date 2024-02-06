#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Windows.Threading;
using System.Linq.Expressions;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using NLib.IO;
using NLib.Serial.Json;
using System.IO;
using System.Windows.Media.Animation;

#endregion

namespace NLib.Serial.Json
{
    #region IsoDateTimeConverter (Original https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Converters/IsoDateTimeConverter.cs)

    /*
    /// <summary>
    /// Converts a <see cref="DateTime"/> to and from the ISO 8601 date format (e.g. <c>"2008-04-12T12:53Z"</c>).
    /// </summary>
    public class IsoDateTimeConverter : DateTimeConverterBase
    {
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        /// <summary>
        /// Gets or sets the date time styles used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time styles used when converting a date to and from JSON.</value>
        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        /// <summary>
        /// Gets or sets the date time format used when converting a date to and from JSON.
        /// </summary>
        /// <value>The date time format used when converting a date to and from JSON.</value>
        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (StringUtils.IsNullOrEmpty(value)) ? null : value;
        }

        /// <summary>
        /// Gets or sets the culture used when converting a date to and from JSON.
        /// </summary>
        /// <value>The culture used when converting a date to and from JSON.</value>
        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            string text;

            if (value is DateTime dateTime)
            {
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                text = dateTime.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
#if HAVE_DATE_TIME_OFFSET
            else if (value is DateTimeOffset dateTimeOffset)
            {
                if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
                {
                    dateTimeOffset = dateTimeOffset.ToUniversalTime();
                }

                text = dateTimeOffset.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);
            }
#endif
            else
            {
                throw new JsonSerializationException("Unexpected value when converting date. Expected DateTime or DateTimeOffset, got {0}.".FormatWith(CultureInfo.InvariantCulture, ReflectionUtils.GetObjectType(value)!));
            }

            writer.WriteValue(text);
        }

        /// <summary>
        /// Reads the JSON representation of the object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> to read from.</param>
        /// <param name="objectType">Type of the object.</param>
        /// <param name="existingValue">The existing value of object being read.</param>
        /// <param name="serializer">The calling serializer.</param>
        /// <returns>The object value.</returns>
        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            bool nullable = ReflectionUtils.IsNullableType(objectType);
            if (reader.TokenType == JsonToken.Null)
            {
                if (!nullable)
                {
                    throw JsonSerializationException.Create(reader, "Cannot convert null value to {0}.".FormatWith(CultureInfo.InvariantCulture, objectType));
                }

                return null;
            }

#if HAVE_DATE_TIME_OFFSET
            Type t = (nullable)
                ? Nullable.GetUnderlyingType(objectType)
                : objectType;
#endif

            if (reader.TokenType == JsonToken.Date)
            {
#if HAVE_DATE_TIME_OFFSET
                if (t == typeof(DateTimeOffset))
                {
                    return (reader.Value is DateTimeOffset) ? reader.Value : new DateTimeOffset((DateTime)reader.Value!);
                }

                // converter is expected to return a DateTime
                if (reader.Value is DateTimeOffset offset)
                {
                    return offset.DateTime;
                }
#endif

                return reader.Value;
            }

            if (reader.TokenType != JsonToken.String)
            {
                throw JsonSerializationException.Create(reader, "Unexpected token parsing date. Expected String, got {0}.".FormatWith(CultureInfo.InvariantCulture, reader.TokenType));
            }

            string? dateText = reader.Value?.ToString();

            if (StringUtils.IsNullOrEmpty(dateText) && nullable)
            {
                return null;
            }

#if HAVE_DATE_TIME_OFFSET
            if (t == typeof(DateTimeOffset))
            {
                if (!StringUtils.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
                }
            }
#endif

            if (!StringUtils.IsNullOrEmpty(_dateTimeFormat))
            {
                return DateTime.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
            }
            else
            {
                return DateTime.Parse(dateText, Culture, _dateTimeStyles);
            }
        }
    }
    */

    #endregion

    #region CorrectedIsoDateTimeConverter

    /// <summary>
    /// The CorrectedIsoDateTimeConverter class.
    /// </summary>
    public class CorrectedIsoDateTimeConverter : IsoDateTimeConverter
    {
        #region Consts

        //private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
        //private const string DefaultDateTimeFormat = "yyyy-MM-ddTHH:mm:ss.fffK";
        //private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK";
        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";

        #endregion

        #region Override methods

        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;

                if (dateTime.Kind == DateTimeKind.Unspecified)
                {
                    if (DateTimeStyles.HasFlag(DateTimeStyles.AssumeUniversal))
                    {
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
                    }
                    else if (DateTimeStyles.HasFlag(DateTimeStyles.AssumeLocal))
                    {
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                    }
                    else
                    {
                        // Force local
                        dateTime = DateTime.SpecifyKind(dateTime, DateTimeKind.Local);
                    }
                }

                if (DateTimeStyles.HasFlag(DateTimeStyles.AdjustToUniversal))
                {
                    dateTime = dateTime.ToUniversalTime();
                }

                string format = string.IsNullOrEmpty(DateTimeFormat) ? DefaultDateTimeFormat : DateTimeFormat;
                string str = dateTime.ToString(format, DateTimeFormatInfo.InvariantInfo);
                writer.WriteValue(str);
                //writer.WriteValue(dateTime.ToString(format, Culture));
            }
            else
            {
                base.WriteJson(writer, value, serializer);
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
        private static JsonSerializerSettings _defaultSettings = null;
        private static CorrectedIsoDateTimeConverter _dateConverter = new CorrectedIsoDateTimeConverter();

        /// <summary>
        /// Gets Default JsonSerializerSettings.
        /// </summary>
        public static JsonSerializerSettings DefaultSettings
        {
            get
            {
                if (null == _defaultSettings)
                {
                    lock (typeof(NJson))
                    {
                        _defaultSettings = new JsonSerializerSettings()
                        {
                            DateFormatHandling = DateFormatHandling.IsoDateFormat,
                            DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
                            DateParseHandling = DateParseHandling.DateTimeOffset,
                            //DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK"
                            //DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK"
                            //DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK"
                            DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK"
                        };
                        if (null == _defaultSettings.Converters)
                        {
                            // Create new List of not exists.
                            _defaultSettings.Converters = new List<JsonConverter>();
                        }
                        if (null != _defaultSettings.Converters &&
                            !_defaultSettings.Converters.Contains(_dateConverter))
                        {
                            _defaultSettings.Converters.Add(_dateConverter);
                        }
                    }
                }
                return _defaultSettings;
            }
        }

        /// <summary>
        /// Convert Object to Json String.
        /// </summary>
        /// <param name="value">The object instance.</param>
        /// <param name="minimized">True for minimize output.</param>
        /// <returns>Returns json string.</returns>
        public static string ToJson(this object value, bool minimized = false)
        {
            string result = string.Empty;
            if (null == value) return result;
            try
            {
                var settings = NJson.DefaultSettings;
                result = JsonConvert.SerializeObject(value,
                    (minimized) ? Formatting.None : Formatting.Indented, settings);
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            return result;
        }
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
                var settings = NJson.DefaultSettings;
                result = JsonConvert.DeserializeObject<T>(value, settings);
            }
            catch (Exception ex)
            {
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            return result;
        }
        /// <summary>
        /// Save object to json file.
        /// </summary>
        /// <param name="value">The object instance.</param>
        /// <param name="fileName">The target file name.</param>
        /// <param name="minimized">True for minimize output.</param>
        /// <returns>Returns true if save success.</returns>
        public static bool SaveToFile(this object value, string fileName,
            bool minimized = false)
        {
            bool result = true;
            try
            {
                // serialize JSON directly to a file
                using (StreamWriter file = File.CreateText(fileName))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    //serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    serializer.Formatting = (minimized) ? Formatting.None : Formatting.Indented;
                    serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                    serializer.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                    //serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
                    //serializer.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK";
                    //serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK";
                    serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
                    serializer.Serialize(file, value);

                    try
                    {
                        file.Flush();
                        file.Close();
                        file.Dispose();
                    }
                    catch (Exception ex2)
                    {
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex2.Err(med);
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }
            return result;
        }
        /// <summary>
        /// Load Object from Json file.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="fileName">The target file name.</param>
        /// <returns>Returns object instance if load success.</returns>
        public static T LoadFromFile<T>(string fileName)
        {
            T result = default(T);

            Stream stm = null;
            int iRetry = 0;
            int maxRetry = 5;

            try
            {
                while (null == stm && iRetry < maxRetry)
                {
                    try
                    {
                        stm = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                            FileShare.Read);
                    }
                    catch (Exception ex2)
                    {
                        MethodBase med = MethodBase.GetCurrentMethod();
                        ex2.Err(med);

                        if (null != stm)
                        {
                            stm.Close();
                            stm.Dispose();
                        }
                        stm = null;
                        ++iRetry;

                        ApplicationManager.Instance.Sleep(50);
                    }
                }

                if (null != stm)
                {
                    // deserialize JSON directly from a file
                    using (StreamReader file = new StreamReader(stm))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        //serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                        serializer.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                        serializer.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                        //serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";
                        //serializer.DateFormatString = "yyyy-MM-ddTHH:mm:ss.fffK";
                        //serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFK";
                        serializer.DateFormatString = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.fffK";
                        result = (T)serializer.Deserialize(file, typeof(T));

                        file.Close();
                        file.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                result = default(T);
                MethodBase med = MethodBase.GetCurrentMethod();
                ex.Err(med);
            }

            if (null != stm)
            {
                try
                {
                    stm.Close();
                    stm.Dispose();
                }
                catch { }
            }
            stm = null;

            return result;
        }
        /// <summary>
        /// Gets application path name.
        /// </summary>
        public static string AppPath
        {
            get { return Folders.Assemblies.CurrentExecutingAssembly; }
        }
        /// <summary>
        /// Gets application config path name.
        /// </summary>
        public static string AppConfigPath
        {
            get { return Path.Combine(Folders.Assemblies.CurrentExecutingAssembly, "Configs"); }
        }
        /// <summary>
        /// Gets product path.
        /// </summary>
        public static string ProductPath
        {
            get { return NLib.ApplicationManager.Instance.Environments.Product.Configs.FullName; }
        }
    }

    #endregion
}

namespace NLib.Serial
{
    #region SerialPortConfig

    /// <summary>
    /// The SerialPortConfig class.
    /// </summary>
    public class SerialPortConfig
    {
        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialPortConfig()
        {
            this.PortName = "COM1";
            this.BaudRate = 9600;
            this.Parity = Parity.None;
            this.StopBits = StopBits.One;
            this.Handshake = Handshake.None;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets Defice Name.
        /// </summary>
        public string DeviceName { get; set; }    
        /// <summary>
        /// Gets or sets Port Name (i.e. COM1, COM2,... ).
        /// </summary>
        public string PortName { get; set; }
        /// <summary>
        /// Gets or sets Boad Rate (default 9600).
        /// </summary>
        public int BaudRate { get; set; }
        /// <summary>
        /// Gets or sets Parity (default None).
        /// </summary>
        public Parity Parity { get; set; }
        /// <summary>
        /// Gets or sets StopBits (default One).
        /// </summary>
        public StopBits StopBits { get; set; }
        /// <summary>
        /// Gets or sets DataBits (default 8).
        /// </summary>
        public int DataBits { get; set; }
        /// <summary>
        /// Gets or sets Handshake (default None).
        /// </summary>
        public Handshake Handshake { get; set; }

        #endregion

        #region Static Methods

        /// <summary>
        /// Gets avaliable port names.
        /// </summary>
        /// <returns>Returns avaliable port names</returns>
        public static List<string> GetPortNames()
        {
            var rets = new List<string>();
            foreach (string s in SerialPort.GetPortNames())
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets avaliable supports parities.
        /// </summary>
        /// <returns>Returns supports parities</returns>
        public static List<string> GetParities()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// Gets Parity from string.
        /// </summary>
        /// <param name="value">The Parity value in string.</param>
        /// <returns>Returns Parity value.</returns>
        public static Parity GetParity(string value)
        {
            return (Parity)Enum.Parse(typeof(Parity), value, true);
        }
        /// <summary>
        /// Gets avaliable supports stop bits.
        /// </summary>
        /// <returns>Returns supports stop bits</returns>
        public static List<string> GetStopBits()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets StopBits from string.
        /// </summary>
        /// <param name="value">The StopBits value in string.</param>
        /// <returns>Returns StopBits value.</returns>
        public static StopBits GetStopBits(string value)
        {
            return (StopBits)Enum.Parse(typeof(StopBits), value, true);
        }
        /// <summary>
        /// Gets avaliable supports handshakes.
        /// </summary>
        /// <returns>Returns supports handshakes</returns>
        public static List<string> GetHandshakes()
        {
            var rets = new List<string>();
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                rets.Add(s);
            }
            return rets;
        }
        /// <summary>
        /// Gets Handshake from string.
        /// </summary>
        /// <param name="value">The handshake value in string.</param>
        /// <returns>Returns Handshake value.</returns>
        public static Handshake GetHandshake(string value)
        {
            return (Handshake)Enum.Parse(typeof(Handshake), value, true);
        }

        #endregion
    }

    #endregion

    #region ByteArray Helper

    /// <summary>
    /// The Byte Array Helper class.
    /// </summary>
    public class ByteArrayHelper
    {
        #region ToHexString

        public static string ToHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace('-', ' ');
        }

        #endregion
    }

    #endregion

    #region SerialDeviceData

    /// <summary>
    /// The SerialDeviceData class.
    /// </summary>
    public abstract class SerialDeviceData : INotifyPropertyChanged
    {
        #region Consts

        public class ascii
        {
            public static string x0D = "\x0D";
            public static string x0A = "\x0A";
        }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceData() : base() { }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceData()
        {

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Internal Raise Property Changed event (Lamda function).
        /// </summary>
        /// <param name="selectorExpression">The Expression function.</param>

        private void InternalRaise<T>(Expression<Func<T>> selectorExpression)
        {
            if (null == selectorExpression)
            {
                throw new ArgumentNullException("selectorExpression");
                // return;
            }
            var me = selectorExpression.Body as MemberExpression;

            // Nullable properties can be nested inside of a convert function
            if (null == me)
            {
                var ue = selectorExpression.Body as UnaryExpression;
                if (null != ue)
                {
                    me = ue.Operand as MemberExpression;
                }
            }

            if (null == me)
            {
                throw new ArgumentException("The body must be a member expression");
                // return;
            }
            Raise(me.Member.Name);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raise Property Changed event.
        /// </summary>
        /// <param name="propertyName">The property name.</param>
        protected void Raise(string propertyName)
        {
            // raise event.
            if (null != PropertyChanged)
            {
                PropertyChanged.Call(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        /// <summary>
        /// Raise Property Changed event (Lamda function).
        /// </summary>
        /// <param name="actions">The array of lamda expression's functions.</param>
        protected void Raise(params Expression<Func<object>>[] actions)
        {
            if (null != actions && actions.Length > 0)
            {
                foreach (var item in actions)
                {
                    if (null != item) InternalRaise(item);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Convert content to byte array.
        /// </summary>
        /// <returns>Returns content in byte array.</returns>
        public abstract byte[] ToByteArray();
        /// <summary>
        /// Parse byte array and update content.
        /// </summary>
        /// <param name="buffers">The buffer data.</param>
        public abstract void Parse(byte[] buffers);

        #endregion

        #region Public Events

        /// <summary>
        /// The PropertyChanged event.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    #endregion

    #region SerialDevice

    /// <summary>
    /// ISerialDevice interface.
    /// </summary>
    public interface ISerialDevice
    {
        /// <summary>
        /// Send.
        /// </summary>
        /// <param name="data">The data buffer to send.</param>
        void Send(byte[] data);
        /// <summary>
        /// LoadConfig.
        /// </summary>
        void LoadConfig();
        /// <summary>
        /// SaveConfig.
        /// </summary>
        void SaveConfig();
        /// <summary>
        /// Gets or sets SerialPortConfig.
        /// </summary>
        SerialPortConfig Config { get; set; }
        /// <summary>
        /// Checks is port opened.
        /// </summary>
        bool IsOpen { get; }
    }

    /// <summary>
    /// The SerialDevice class (abstract).
    /// </summary>
    public abstract class SerialDevice
    {
        #region Internal Variables

        private SerialPortConfig _config = null;

        private SerialPort _comm = null;

        private bool _isProcessing = false;
        private Thread _th = null;

        private int _maxRxQueueSize = 1000;

        protected object _lock = new object();
        private List<byte> queues = new List<byte>();

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDevice() : base() 
        {
            _config = new SerialPortConfig();
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDevice()
        {
            ClosePort();
        }

        #endregion

        #region Thread Related Methods

        private void CreateRXThread()
        {
            if (null != _th) return; // thread instance exist.

            MethodBase med = MethodBase.GetCurrentMethod();
            med.Info("Creeate RX Thread");

            _th = new Thread(this.RxProcessing);
            _th.Name = "Serial RX thread";
            _th.IsBackground = true;
            _th.Priority = ThreadPriority.BelowNormal;

            _isProcessing = true; // mark flag.
            
            _th.Start();
        }

        private void FreeRXThread()
        {
            MethodBase med = MethodBase.GetCurrentMethod();
            med.Info("Free RX Thread");

            _isProcessing = false; // mark flag.

            if (null == _th) return; // no thread instance.
            try
            {
                _th.Abort();
            }
            catch (ThreadAbortException) 
            { 
                Thread.ResetAbort(); 
            }
            _th = null;
        }

        private void RxProcessing()
        {
            while (null != _th && _isProcessing && !ApplicationManager.Instance.IsExit)
            {
                try
                {
                    if (null != _comm)
                    {
                        // Read data and add to queue
                        int byteToRead = _comm.BytesToRead;
                        if (byteToRead > 0)
                        {
                            lock (_lock)
                            {
                                byte[] buffers = new byte[byteToRead];
                                _comm.Read(buffers, 0, byteToRead);

                                if (queues.Count < _maxRxQueueSize)
                                {
                                    queues.AddRange(buffers);
                                }
                            }
                        }

                        // check queue has avaliable data.
                        int cnt = 0;
                        lock (_lock)
                        {
                            cnt = queues.Count;
                        }
                        if (cnt > 0)
                        {
                            // process rx queue in main ui thread.
                            ProcessRXQueue();
                            // Raise event.
                            OnRx.Call(this, EventArgs.Empty);
                        }
                    }
                }
                catch (TimeoutException) { }
                catch (Exception) { }

                Thread.Sleep(10);
                Application.DoEvents();
            }
            FreeRXThread();
        }

        #endregion

        #region Private Methods

        private bool ConfigExists(string filename)
        {
            string configPath = ConfigFolder;
            if (!Directory.Exists(configPath))
            {
                Directory.CreateDirectory(configPath);
            }

            string fullFileName = Path.Combine(configPath, filename);
            return File.Exists(fullFileName);
        }

        private string ConfigFolder
        {
            get { return Path.Combine(NJson.AppConfigPath, "Devices"); }
        }

        private string ConfigFileName
        {
            get { return DeviceName + ".config.json"; }
        }

        private SerialPortConfig GetConfig()
        {
            SerialPortConfig cfg;

            var folder = ConfigFolder;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string fileName = Path.Combine(ConfigFolder, ConfigFileName);
            if (!ConfigExists(fileName))
            {
                // create new one and save.
                cfg = new SerialPortConfig();
                NJson.SaveToFile(cfg, fileName, false);
            }

            cfg = NJson.LoadFromFile<SerialPortConfig>(fileName);

            if (null == cfg)
            {
                // create new one and save.
                cfg = new SerialPortConfig();
                //NJson.SaveToFile(cfg, fileName, false);
            }
            return cfg;
        }

        #endregion

        #region Protected Methods

        #region Open/Close port

        /// <summary>
        /// OpenPort.
        /// </summary>
        protected void OpenPort()
        {
            if (null == _config)
                _config = new SerialPortConfig();
            OpenPort(_config);
        }
        /// <summary>
        /// OpenPort.
        /// </summary>
        /// <param name="value">The Serial Port Config instance.</param>
        protected void OpenPort(SerialPortConfig value)
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null == value)
            {
                med.Err("The SerialPortConfig is null.");
                return;
            }
            // Open Port.
            OpenPort(value.PortName, value.BaudRate, 
                value.Parity, value.DataBits, value.StopBits, 
                value.Handshake);
        }
        /// <summary>
        /// Open Serial Port connection.
        /// </summary>
        /// <param name="portName">The port name. i.e. COM1, COM2,...</param>
        /// <param name="boadRate">The boad rate. default 9600.</param>
        /// <param name="parity">The parity. default None.</param>
        /// <param name="dataBits">The data bits. default 8.</param>
        /// <param name="stopBits">The stop bits. default One.</param>
        /// <param name="handshake">The handshake. default None.</param>
        protected void OpenPort(string portName, int boadRate = 9600, 
            Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One,
            Handshake handshake = Handshake.None) 
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            if (null != _comm)
                return; // already connected.

            // clear queue
            ClearQueue();

            #region Set port

            try
            {
                _comm = new SerialPort();
                _comm.PortName = portName;
                _comm.BaudRate = boadRate;
                _comm.Parity = parity;
                _comm.DataBits = dataBits;
                _comm.StopBits = stopBits;
                _comm.Handshake = handshake;
            }
            catch (Exception ex)
            {
                med.Err(med, ex);
            }

            #endregion

            #region Open port

            // Open Port
            string msg = string.Format("Open port {0} - {1}, {2}, {3}, {4}",
                _comm.PortName, _comm.BaudRate, _comm.Parity, _comm.DataBits, _comm.StopBits);
            med.Info(msg);

            try
            {
                _comm.Open();
            }
            catch (Exception ex2)
            {
                med.Err(ex2.ToString());
            }

            #endregion

            #region Check port opened

            if (null != _comm && !_comm.IsOpen)
            {
                med.Info("Cannot open port. Free resource.");
                try
                {
                    _comm.Close();
                    _comm.Dispose();
                }
                catch { }
                _comm = null;

                return; // cannot open port
            }
            // Port opened so Create Read Thread
            CreateRXThread();

            #endregion
        }
        /// <summary>
        /// Close Serial Port connection.
        /// </summary>
        protected void ClosePort()
        {
            MethodBase med = MethodBase.GetCurrentMethod();

            FreeRXThread();
            // Free Serial ports
            if (null != _comm)
            {
                string msg = string.Format("Close port {0}", _comm.PortName);
                med.Info(msg);

                try
                {
                    _comm.Close();
                    _comm.Dispose();
                }
                catch { }
            }
            _comm = null;

            // clear queue
            ClearQueue();
        }

        #endregion

        #region RX Data processing

        /// <summary>
        /// Gets RX Queues.
        /// </summary>
        public List<byte> Queues { get { return queues; } }
        /// <summary>
        /// Process RX Queue.
        /// </summary>
        protected abstract void ProcessRXQueue();

        #endregion

        #region Byte Array Methods

        /// <summary>
        /// IndexOf.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="pattern">The search pattern array.</param>
        /// <returns>Returns first match occurance index.</returns>
        protected int IndexOf(byte[] source, byte[] pattern)
        {
            var len = pattern.Length;
            var limit = source.Length - len;
            for (var i = 0; i <= limit; i++)
            {
                var k = 0;
                for (; k < len; k++)
                {
                    if (pattern[k] != source[i + k]) break;
                }
                if (k == len) return i;
            }
            return -1;
        }

        private bool Equals(byte[] source, byte[] separator, int index)
        {
            for (int i = 0; i < separator.Length; ++i)
                if (index + i >= source.Length || source[index + i] != separator[i])
                    return false;
            return true;
        }
        /*
        private byte[] SeparateAndGetLast(byte[] source, byte[] separator)
        {
            for (var i = 0; i < source.Length; ++i)
            {
                if (Equals(source, separator, i))
                {
                    var index = i + separator.Length;
                    var part = new byte[source.Length - index];
                    Array.Copy(source, index, part, 0, part.Length);
                    return part;
                }
            }
            throw new Exception("not found");
        }
        */

        /// <summary>
        /// Split.
        /// </summary>
        /// <param name="source">The source array.</param>
        /// <param name="separator">The separator patterns.</param>
        /// <returns>Returns array of byte array that split by pattern.</returns>
        protected byte[][] Split(byte[] source, byte[] separator)
        {
            var Parts = new List<byte[]>();
            var Index = 0;
            var slen = separator.Length;
            byte[] Part;
            for (var I = 0; I < source.Length; ++I)
            {
                if (Equals(source, separator, I))
                {
                    Part = new byte[I - Index + slen];
                    Array.Copy(source, Index, Part, 0, Part.Length);
                    Parts.Add(Part);
                    Index = I + slen;
                    I += slen - 1;
                }
            }
            Part = new byte[source.Length - Index];
            Array.Copy(source, Index, Part, 0, Part.Length);
            Parts.Add(Part);
            return Parts.ToArray();
        }

        #endregion

        #endregion

        #region Public Methods

        #region Send

        /// <summary>
        /// Send.
        /// </summary>
        /// <param name="data">The data buffer to send.</param>
        public void Send(byte[] data)
        {
            if (null != _comm && !_comm.IsOpen)
                return;
            if (null == data || data.Length <= 0)
                return;
            MethodBase med = MethodBase.GetCurrentMethod();
            try
            {
                _comm.Write(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                med.Err(med, ex);
            }
        }
        /// <summary>
        /// Clear queue.
        /// </summary>
        public void ClearQueue()
        {
            // clear queue
            lock (_lock)
            {
                if (null == queues) queues = new List<byte>();
                queues.Clear();
            }
        }

        #endregion

        #region Config Load/Save

        /// <summary>
        /// Load Config.
        /// </summary>
        public void LoadConfig()
        {
            var cfg = GetConfig();
            Config = cfg; // update current config.
        }
        /// <summary>
        /// Save Config.
        /// </summary>
        public void SaveConfig()
        {
            var cfg = Config;
            if (null != cfg)
            {
                var folder = ConfigFolder;
                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string cfgFileName = ConfigFileName;
                string fileName = Path.Combine(ConfigFolder, cfgFileName);
                NJson.SaveToFile(cfg, fileName, false);
            }
        }

        #endregion

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets device name.
        /// </summary>
        public abstract string DeviceName { get; }
        /// <summary>
        /// Gets or sets Serial Port Config.
        /// </summary>
        public SerialPortConfig Config
        {
            get
            {
                if (null == _config) _config = new SerialPortConfig();
                return _config;
            }
            set { _config = value; }
        }
        /// <summary>
        /// Checks is thread still processing.
        /// </summary>
        public bool IsProcessing { get { return (null != _th && _isProcessing); } }
        /// <summary>
        /// Checks is port opened.
        /// </summary>
        public bool IsOpen { get { return (null != _comm && _comm.IsOpen); } }
        /// <summary>
        /// Gets or sets Max Rx Queue Size. Default is 1000 bytes.
        /// </summary>
        public int MaxRxQueueSize 
        { 
            get { return _maxRxQueueSize; } 
            set { _maxRxQueueSize = value; } 
        }

        #endregion

        #region Public Events

        /// <summary>
        /// The OnRx event handler.
        /// </summary>
        public event EventHandler OnRx;

        #endregion
    }

    #endregion

    #region SerialDeviceEmulator

    /// <summary>
    /// ISerialDeviceEmulator interface.
    /// </summary>
    public interface ISerialDeviceEmulator : ISerialDevice
    {
        /// <summary>
        /// Start.
        /// </summary>
        void Start();
        /// <summary>
        /// Shutdown.
        /// </summary>
        void Shutdown();
    }

    /// <summary>
    /// The SerialDeviceEmulator class (abstract).
    /// </summary>
    public abstract class SerialDeviceEmulator<T> : SerialDevice, ISerialDeviceEmulator
        where T : SerialDeviceData, new()
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceEmulator() : base() 
        {
            Value = new T();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceEmulator()
        {
            Shutdown();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Start.
        /// </summary>
        public void Start() 
        {
            OpenPort();
        }
        /// <summary>
        /// Shutdown.
        /// </summary>
        public void Shutdown() 
        {
            ClosePort();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets content value.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }

    #endregion

    #region SerialDeviceTerminal

    /// <summary>
    /// ISerialDeviceTerminal interface.
    /// </summary>
    public interface ISerialDeviceTerminal : ISerialDevice
    {
        /// <summary>
        /// Connect.
        /// </summary>
        void Connect();
        /// <summary>
        /// Disconnect.
        /// </summary>
        void Disconnect();
    }

    /// <summary>
    /// The SerialDeviceTerminal class (abstract).
    /// </summary>
    public abstract class SerialDeviceTerminal<T> : SerialDevice, ISerialDeviceTerminal
        where T : SerialDeviceData, new()
    {
        #region Constructor and Destructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SerialDeviceTerminal() : base() 
        {
            Value = new T();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~SerialDeviceTerminal()
        {
            Disconnect();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connect.
        /// </summary>
        public void Connect() 
        {
            OpenPort();
        }
        /// <summary>
        /// Disconnect.
        /// </summary>
        public void Disconnect() 
        {
            ClosePort();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets content value.
        /// </summary>
        public T Value { get; set; }

        #endregion
    }

    #endregion
}
