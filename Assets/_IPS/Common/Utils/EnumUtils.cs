using System;

namespace IPS {
    public class EnumUtils {
        public static T GetRandomEnumValue<T>() where T: Enum {
            try {
                System.Array values = System.Enum.GetValues(typeof(T));
                int randomIndex = UnityEngine.Random.Range(0, values.Length);
                return (T)values.GetValue(randomIndex);
            }
            catch { 
                return default(T);
            }
        }
    }
}