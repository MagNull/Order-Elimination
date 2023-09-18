using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OrderElimination
{
    public static class Logging
    {
        public static void Log(object message, Colorize color = default, Object context = null, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            Debug.Log(message.ToString() % (color ?? Colorize.ByColor(Color.white)), context);
        }
        
        public static void LogWarning(object message, Colorize color = default, Object context = null, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            Debug.LogWarning(message.ToString() % (color ?? Colorize.ByColor(Color.yellow)), context);
        }
        
        public static void LogError(object message, Colorize color = default, Object context = null, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            ConfigureScope(context, caller, lineNumber);
            Debug.LogError(message.ToString() % Colorize.Red, context);
        }
        
        public static void LogException(Exception exception, Object context = null, [CallerLineNumber] int lineNumber = 0,
            [CallerMemberName] string caller = null)
        {
            ConfigureScope(context, caller, lineNumber);
            Debug.LogException(exception, context);
        }

        private static void ConfigureScope(Object context, string className, int lineNumber)
        {/*
            SentrySdk.ConfigureScope(scope =>
            {
                scope.Contexts["description"] = new Error
                {
                    ObjectName = context ? "None" : context.ToString(),
                    ClassName = className,
                    LineNumber = lineNumber
                };
            });
        */}
    }

    public class Error
    {
        public string ObjectName;
        public string ClassName;
        public int LineNumber;
    }
}