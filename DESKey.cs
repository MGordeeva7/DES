using System;
using System.Collections;
using System.Linq;

class DESKey
{
    public BitArray key64;
    public BitArray key56;
    public static bool INVERSE_PARITY = false;
    public static string[] WEAK_KEYS = new string[16]{ "0101010101010101",
        "1F1F1F1F1F1F1F1F", "E0E0E0E0E0E0E0E0", "FEFEFEFEFEFEFEFE",
        "01FE01FE01FE01FE", "FE01FE01FE01FE01", "1FE01FE01FE01FE0",
        "E0F1E0F1E0F1E0F1", "01E001E001F101F1", "E001E001F101F101",
        "1FFE1FFE0EFE0EFE", "FE1FFE1FFE0EFE0E", "O11F011F010E010E",
        "1F011F010E010E01 ","E0FEE0FEF1FEF1FE", "FEE0FEE0FEF1FEF1"};

    override public string ToString()
    {
        return key64.ToString("X");
    }

    public static DESKey GetFromFile(string filepath, string format="B")
    {
        return new DESKey(System.IO.File.ReadAllText(filepath), format);
    }

    public DESKey()
    {
        Random rd = new Random();
        byte[] rawKey = new byte[7];
        rd.NextBytes(rawKey);
        key56 = new BitArray(rawKey);
        key64 = ConvertKeyTo64(key56, INVERSE_PARITY ? 1 : 0);
    }

    public DESKey(string s, string format = "B")
    {
        BitArray rawKey;
        rawKey = BitArrayExtension.GetFromString(s, format);
        if (rawKey.Length == 56)
        {
            key56 = rawKey;
            key64 = ConvertKeyTo64(key56, INVERSE_PARITY ? 1 : 0);
        } else if (rawKey.Length == 64)
        {
            if (CheckSum(rawKey) || CheckSum(rawKey, 1)) key64 = rawKey;
            else throw new Exception("Ключ поврежден");
            key56 = ConvertKeyTo56(key64);
        }
        else throw new Exception("Недопустимая длина ключа != 56, 64");
    }

    static BitArray ConvertKeyTo64(BitArray key56, int parityf = 0)
    {
        BitArray key64 = new BitArray(64);
        byte parity = 0;
        int j = 0;
        for (int i = 1; i <= 56; i++)
        {
            key64[j] = key56[i - 1];
            if (key56[i - 1]) parity++;
            if (i % 7 == 0)
            {
                key64[++j] = parity % 2 == parityf;
                parity = 0;
            }
            j++;
        }
        return key64;
    }

    static BitArray ConvertKeyTo56(BitArray key64)
    {
        BitArray key56 = new BitArray(56);
        int j = -1;
        for (int i = 0; i < 64; i++)
        {

            if ((i + 1) % 8 != 0)
            {
                j++;
                key56[j] = key64[i];
            }
            

        }
        return key56;
    }

    static bool CheckSum(BitArray key64, int parityf = 0)
    {
        byte parity = 0;
        for (int i = 0; i < 64; i++)
        {
            if ((i + 1) % 8 == 0)
            {
                if (key64[i] != (parity % 2 == parityf)) return false;
                parity = 0;
            }
            else if (key64[i]) parity++;
        }
        return true;
    }

    public bool IsWeakKey()
    {
        string s = ConvertKeyTo64(key56, 0).ToString("X");
        return WEAK_KEYS.Contains(s);
    } 
}

