using System;
using System.IO;

namespace IPS {
    public static class FileUtils {
        public static void SaveFile(string path, byte[] bytes) {
            if (string.IsNullOrEmpty(path) || bytes.Length <= 0) {
                Logs.LogError($"<color=red>Save File Fail</color> {path} - {bytes.Length}");
                return;
            }

            try {
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(SaveFile), $"path={path}, err={(e != null ? e.Message : "Unknow")}");
            }
        }

        public static void SaveFile(string path, string data) {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(data) ||data.Length <= 0) {
                Logs.LogError($"<color=red>Check File Fail</color> {path}");
                return;
            }
            
            try {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
                
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(SaveFile), $"path={path}, err={(e != null ? e.Message : "Unknow")}");
            }
        }

        public static bool IsExitFile(string path) {
            if (string.IsNullOrEmpty(path)) return false;
            
            if (!File.Exists(path)) {
                Logs.LogError($"File not found: {path}");
                return false;
            }

            return true;
        }

        public static void CheckDirectory(string path) {
            if (string.IsNullOrEmpty(path)) {
                Logs.LogError($"CheckDirectory wrong with path null'");
                return;
            }
            
            if (!Directory.Exists(path)) {
                try {
                    Directory.CreateDirectory(path);
                }
                catch (Exception e) {
                    Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(CheckDirectory), $"path={path}, err={(e != null ? e.Message : "Unknow")}");
                }
            }
        }

        public static bool ReadAllBytes(out byte[] result, string path) {
            result = new byte[0];
            if (!IsExitFile(path)) return false;

            try {
                using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read)) {
                    result = new byte[fileStream.Length];
                    int bytesRead = fileStream.Read(result, 0, (int)fileStream.Length);
                    if (bytesRead != fileStream.Length) {
                        Logs.Log("Not all bytes were read from the file.");
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(ReadAllBytes), $"path={path}, err={(e != null ? e.Message : "Unknow")}");            
            }

            return false;
        }

        public static bool WriteAllBytes(string path, byte[] bytes) {
            if (string.IsNullOrEmpty(path) || bytes.Length <= 0) {
                Logs.LogError($"<color=red>Write All Bytes Fail</color> {path} - {bytes.Length}");
                return false;
            }
            
            try {
                File.WriteAllBytes(path, bytes);
                return true;
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(WriteAllBytes), $"path={path}, err={(e != null ? e.Message : "Unknow")}");            
                return false;
            }
        }

        public static bool ReadAllText(out string result, string path) {
            result = String.Empty;
            if (string.IsNullOrEmpty(path) || !IsExitFile(path)) {
                Logs.LogError($"ReadAllText wrong with path: '{path}'");
                return false;
            }

            try {
                result = File.ReadAllText(path);
                return true;
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(ReadAllText), $"path={path}, err={(e != null ? e.Message : "Unknow")}");            
                return false;
            }
        }

        public static void WriteAllText(string path, string data) {
            if (string.IsNullOrEmpty(path)) {
                Logs.LogError($"WriteAllText wrong with path: '{path}'");
                return;
            }

            try {
                //if( || !IsExitFile(path))
                using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate)) {
                    var bytes  = System.Text.Encoding.UTF8.GetBytes(data);
                    fs.Write(bytes, 0, bytes.Length);
                    fs.Flush();
                }
            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(WriteAllText), $"path={path}, err={(e != null ? e.Message : "Unknow")}");
            }
        }

        public static void Delete(string path) {
            if (string.IsNullOrEmpty(path) || !IsExitFile(path)) {
                Logs.LogError($"Delete wrong with path: '{path}'");
                return;
            }

            try {
                File.Delete(path);

            }
            catch (Exception e) {
                Tracking.Instance.LogException(typeof(FileUtils).Name, nameof(Delete), $"path={path}, err={(e != null ? e.Message : "Unknow")}");
            }
        }
    }
}