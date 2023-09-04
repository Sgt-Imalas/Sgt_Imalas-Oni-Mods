using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class IO_Util
    {
        public static bool ReadFromFile<T>(string FileOrigin, out T output, string forceExtensionTo = "")
        {
            var filePath = new FileInfo(FileOrigin);
            if (!filePath.Exists || (forceExtensionTo!= string.Empty && filePath.Extension != forceExtensionTo))
            {
                SgtLogger.logwarning(FileOrigin,"File does not exist!");
                output = default(T);
                return false;
            }
            else
            {
                FileStream filestream = filePath.OpenRead();
                using (var sr = new StreamReader(filestream))
                {
                    string jsonString = sr.ReadToEnd();
                    output = JsonConvert.DeserializeObject<T>(jsonString);
                    return true;
                }
            }
        }

        public static bool WriteToFile<T>(T DataObject, string filePath)
        {
            try
            {

                var fileInfo = new FileInfo(filePath);
                FileStream fcreate = fileInfo.Open(FileMode.Create);

                var JsonString = JsonConvert.SerializeObject(DataObject, Formatting.Indented);
                using (var streamWriter = new StreamWriter(fcreate))
                {
                    streamWriter.Write(JsonString);
                }
                return true;
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not write file, Exception: " + e);
                return false;
            }
        }
        public static bool DeleteFile(string filePath)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                fileInfo.Delete();
                return true;
            }
            catch (Exception e)
            {
                SgtLogger.logError("Could not delete file, Exception: " + e);
                return false;
            }
        }
    }
}
