


using UnityEngine;

public static class StringUtils {
    // Note that Color32 and Color implictly convert to each other. You may pass a Color object to this method without first casting it.
    public static string ToHexColor(this Color32 color) {
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }

    public static bool ToColor(this string hex, out Color color) {
        color = Color.white;
        try {
            hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
            hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
            byte a = 255;//assume fully visible unless specified in hex
            byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            //Only use alpha if the string has enough characters
            if (hex.Length == 8) {
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            color = new Color32(r, g, b, a);
            return true;
        }
        catch {
            Debug.LogError($"Invalid color with hex={hex}");
            return false;
        }
    }

    public static string ToDotCurrency(this string value) {
        string result = value;

        int idx = value.Length;
        if (value.IndexOf(',') > 0) idx = value.IndexOf(',');
        else if (value.IndexOf('.') > 0) idx = value.IndexOf('.');

        idx -= 3;

        while (idx > 0) {
            result = result.Insert(idx, ",");
            idx -= 3;
        }

        return result;
    }

    public static string ToBigNumber(this string value) {
        string result = value;

        int idx = value.Length;
        if (value.IndexOf(',') > 0) idx = value.IndexOf(',');
        else if (value.IndexOf('.') > 0) idx = value.IndexOf('.');

        idx -= 3;

        int step = 0;
        while (idx > 0) {
            result = result.Substring(0, idx);
            idx -= 3;
            step++;
        }

        switch (step) {
            case (int)BigNumber.K: result += BigNumber.K.ToString(); break;
            case (int)BigNumber.M: result += BigNumber.M.ToString(); break;
            case (int)BigNumber.B: result += BigNumber.B.ToString(); break;
            case (int)BigNumber.T: result += BigNumber.T.ToString(); break;
            case (int)BigNumber.Q: result += BigNumber.Q.ToString(); break;
            default: break;
        }

        return result;
    }

    public enum BigNumber : int {
        p, // p is a placeholder if the value is under 1 thousand
        K = 1, // Thousand
        M, // Million
        B, // Billion
        T, // Trillion
        Q, //Quadrillion
    }
}
