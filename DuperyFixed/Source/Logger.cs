using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dupery
{
    class Logger
    {
        public static void Log(string message)
        {
            Debug.Log($"[Dupery] {message}");
        }

        public static void LogError(string message)
        {
            Debug.LogError($"[Dupery] {message}");
        }
    }
}
