using System;
using System.Collections.Generic;
using System.Linq;

namespace NLib.Serial.Protocol.Analyzer.Models
{
    /// <summary>
    /// Represents a loaded log file with all entries
    /// </summary>
    public class LogFile
    {
        /// <summary>
        /// Full file path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// File name only (without path)
        /// </summary>
        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                    return string.Empty;
                return System.IO.Path.GetFileName(FilePath);
            }
        }

        /// <summary>
        /// List of log entries
        /// </summary>
        public List<LogEntry> Entries { get; set; }

        /// <summary>
        /// Total number of entries
        /// </summary>
        public int EntryCount
        {
            get { return Entries != null ? Entries.Count : 0; }
        }

        /// <summary>
        /// Total bytes across all entries
        /// </summary>
        public int TotalBytes
        {
            get { return Entries != null ? Entries.Sum(e => e.Length) : 0; }
        }

        /// <summary>
        /// When the file was loaded
        /// </summary>
        public DateTime LoadedAt { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public LogFile()
        {
            FilePath = string.Empty;
            Entries = new List<LogEntry>();
            LoadedAt = DateTime.Now;
        }

        /// <summary>
        /// Constructor with file path
        /// </summary>
        /// <param name="filePath">Full file path</param>
        public LogFile(string filePath)
        {
            FilePath = filePath;
            Entries = new List<LogEntry>();
            LoadedAt = DateTime.Now;
        }

        /// <summary>
        /// Adds an entry to the log
        /// </summary>
        /// <param name="entry">Log entry to add</param>
        public void AddEntry(LogEntry entry)
        {
            if (Entries == null)
                Entries = new List<LogEntry>();

            Entries.Add(entry);
        }

        /// <summary>
        /// Clears all entries
        /// </summary>
        public void Clear()
        {
            if (Entries != null)
                Entries.Clear();
        }
    }
}
