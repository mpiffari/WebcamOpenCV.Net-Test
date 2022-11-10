using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static AnchorTest.Enums;

namespace AnchorTest
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Extension method for all Enums
        /// </summary>
        /// <param name="value">Enum extended</param>
        /// <param name="format">Format used to return key as string</param>
        /// <returns>Formatted string representation of enum key</returns>
        public static string Key(this Enum value, string format = "g")
        {
            return value.ToString(format);
        }

        /// <summary>
        /// Extension method for all Enums
        /// </summary>
        /// <param name="value">Enum extended</param>
        /// <returns>String description of enum key provided through Description attribute</returns>
        public static string ToDescription(this Enum value)
        {
            var type = value.GetType();
            string name = Enum.GetName(type, value);

            if (string.IsNullOrEmpty(name))
            {
                return string.Empty;
            }

            var field = type.GetField(name);
            var attr = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;

            return attr?.Description ?? string.Empty;
        }

        /// <summary>
        /// Convert a string into enum typeof(T) which Description or Key or Localized string match the value passed as param
        /// </summary>
        /// <param name="value">String to parse as enum</param>
        /// <returns>Enum typeof(T) parsed from string. If parsing goes wrong, null value is return</returns>
        public static T ParseEnum<T>(string value) where T : Enum
        {
            if (string.IsNullOrEmpty(value))
            {
                return default;
            }

            foreach (var e in AllCases<T>())
            {
                if (e.ToDescription() == value || e.Key() == value)
                {
                    return e;
                }
            }

            return default;
        }

        /// <summary>
        /// Extension method for all Enums
        /// </summary>
        /// <returns>List of element typeOf(T) that represent all enum cases</returns>
        public static List<T> AllCases<T>() where T : Enum
        {
            var enumValues = (T[])Enum.GetValues(typeof(T));

            return enumValues.ToList();
        }

        /// <summary>
        /// Extension method for all Enums
        /// </summary>
        /// <returns>List of element typeOf(T) that represent all enum cases</returns>
        public static List<Enum> AllCasesAsEnum<T>() where T : Enum
        {
            var enumValues = (T[])Enum.GetValues(typeof(T));

            return enumValues.Select(e => (Enum)e).ToList();
        }
    }

    public static class LoggerExtensions
    {
        public static bool IsEnabled(this LogLevel logLevel)
        {
            // Select which levels keep active
            switch (logLevel)
            {
                case LogLevel.Trace: return false;
                case LogLevel.Critical: return true;
                case LogLevel.Debug: return true;
                case LogLevel.Error: return true;
                case LogLevel.Information: return true;
                case LogLevel.None: return true;
                case LogLevel.Warning: return true;
                default: return false;
            }
        }

        public static bool IsEnabled(this LogScope scope)
        {
            // Select which category keep active
            switch (scope)
            {
                case LogScope.SERIAL: return true;
                case LogScope.WEBCAM: return true;
                case LogScope.WORK: return true;
                case LogScope.NORMATIVE: return true;
                default: return false;
            }
        }

        public static EventId EventId(this LogScope scope)
        {
            return new EventId((int)scope, scope.ToDescription());
        }

        public static LogScope? Scope(this EventId eventId)
        {

            if (eventId.Name == LogScope.WORK.ToDescription())
            {
                return LogScope.WORK;
            }
            else if (eventId.Name == LogScope.SERIAL.ToDescription())
            {
                return LogScope.SERIAL;
            }
            else if (eventId.Name == LogScope.WEBCAM.ToDescription())
            {
                return LogScope.WEBCAM;
            }
            else if (eventId.Name == LogScope.NORMATIVE.ToDescription())
            {
                return LogScope.NORMATIVE;
            }

            return null;
        }
    }

    public class Logger : ILogger
    {
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!logLevel.IsEnabled())
            {
                return;
            }

            if (eventId.Scope() != null)
            {
                var logScope = (LogScope)eventId.Scope();
                if (!logScope.IsEnabled())
                {
                    return;
                }
            }

            var logMessage = new StringBuilder($"{GetHeaderInfoLog(logLevel, eventId)} {formatter(state, null)}");

            if (exception != null || logLevel == LogLevel.Error)
            {
                if (formatter(state, null) != null)
                {
                    logMessage.Append($"\n  Custom message: {formatter(state, null)}");
                }

                if (!string.IsNullOrEmpty(exception?.Message))
                {
                    logMessage.Append($"\n  Exception message: {exception.Message}");
                }

                if (!string.IsNullOrEmpty(exception?.StackTrace))
                {
                    logMessage.Append($"\n  Exception stack: {exception.StackTrace}");
                }
            }

            System.Diagnostics.Debug.WriteLine(logMessage.ToString());
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel.IsEnabled();
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        private static string GetHeaderInfoLog(LogLevel logLevel, EventId eventId)
        {
            var fileName = new StackTrace(true).GetFrame(5)?.GetFileName()?.Split(Path.DirectorySeparatorChar).Last() ?? string.Empty;
            var lineNumber = new StackTrace(true).GetFrame(5)?.GetFileLineNumber();

            if (logLevel == LogLevel.Error)
            {
                fileName = new StackTrace(true).GetFrame(4)?.GetFileName()?.Split(Path.DirectorySeparatorChar).Last() ?? string.Empty;
                lineNumber = new StackTrace(true).GetFrame(4)?.GetFileLineNumber();
            }

            // Log called from classes
            return $"({($"{eventId.Name}")}) {DateTime.Now:dd-MM-yyyy HH:mm:ss.fff} [{logLevel.ToString()}] [{fileName}:{lineNumber}] > ";
        }
    }
}
