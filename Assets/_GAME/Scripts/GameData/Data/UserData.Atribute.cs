using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IPS {
    public partial class UserData {
        private const string atribute = "atb_";
        public static int GetLevelAtribute(byte type) {
            return GetInt($"{atribute}{type}",1);
        }

        public static void SetLevelAtribute(byte type, int value) {
            SetInt($"{atribute}{type}", value > 0 ? value : 0);
        }
        private static void IncreaseAtribute(byte type, int value) {
            SetInt($"{atribute}{type}", GetLevelAtribute(type) + value);
        }
        private static void DeCreaseAtribute(byte type, int value) {
            SetInt($"{atribute}{type}", GetLevelAtribute(type) - value);
        }
        public static void IncreaseAtribute(EAtribute atributeType, int increaseCount) {
            IncreaseAtribute((byte)atributeType, increaseCount);
            EventDispatcher.Instance.Dispatch(new OnUpdateAtribute() { type = atributeType });

            if (atributeType == EAtribute.None) {
                Logs.LogError($"Missing Atribute Type: {atributeType}__value: {increaseCount}");
            }
        }
    }
}
