using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public static class BitArrayExtension
{
    

    public static BitArray GetFromString(string data, string format="B")
    {
        BitArray result;

        if (format == "B")
        {
            result = new BitArray(data.Length);
            for (int i = 0; i < data.Length; i++)
            {
                result[i] = data[i] == '1' ? true : false;
            }
        } else if (format=="X")
        {
            result = new BitArray(data.Length*4);
            for (int i = 0; i < data.Length; i++)
            {
                var val = data[i] - '0';
                if (data[i] > '9') val = data[i] - 'A' + 10;
                result[4 * i + 3] = val % 2 == 1; val /= 2;
                result[4 * i + 2] = val % 2 == 1; val /= 2;
                result[4 * i + 1] = val % 2 == 1; val /= 2;
                result[4 * i] = val % 2 == 1;
            }
        }
        else
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            result = new BitArray(bytes);
        }
        return result;
    }

    public static BitArray[] Separate(this BitArray data, byte subLength)
    {
        int k = data.Length / subLength;
        BitArray[] Rs = new BitArray[k];
        for (int i = 0; i < k; i++)
        {
            Rs[i] = new BitArray(subLength);
            for (int j = 0; j < subLength; j++) Rs[i][j] = data[i * subLength + j];
        }
        return Rs;
    }


    public static BitArray FromByte(byte b)
    {
        BitArray R = new BitArray(4);
        int i = 3;
        while (b != 0)
        {
            R[i] = b % 2 == 1;
            b /= 2;
            i--;
        }
        return R;
    }


    public static byte[] ToBytes(this BitArray data)
    {
        byte[] bytes = new byte[data.Length / 8];
        data.CopyTo(bytes, 0);
        return bytes;
    }

    public static string ToString(this BitArray data, string format="X")
    {
        if(format == "X")
        {
            if (data.Length % 4 != 0) throw new Exception("НЕВОЗМОЖНО ПРЕОБРАЗОВАТЬ В HEX");
            char[] hex = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            StringBuilder s = new StringBuilder();
            int i = 0;
            while (i < data.Length)
            {
                int ind = (data[i] ? 8 : 0) + (data[i + 1] ? 4 : 0) + (data[i + 2] ? 2 : 0) + (data[i + 3] ? 1 : 0);
                s.Append(hex[ind]);
                i += 4;
            }
            return s.ToString();
        }
        if (format[0] == 'B')
        {
            bool isShowLength = format[^1] == '+';
            string result = "";
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i] ? '1' : '0';
            }
            return result + (isShowLength ? $" - {data.Length} bites" : "");
        } else
        {
            byte[] bytes = new byte[data.Length / 8];
            data.CopyTo(bytes, 0);
            return Encoding.UTF8.GetString(bytes);
        }
       
    }

    public static BitArray JoinBitArrays(BitArray[] datas, int subLength)
    {
        BitArray R = new BitArray(datas.Length * subLength);
        for (int i = 0; i < datas.Length; i++)
        {
            for (int j = 0; j < subLength; j++)
            {
                R[subLength * i + j] = datas[i][j];
            }
        }
        return R;
    }

    public static BitArray crotl(this BitArray data, int i)
    {
        int N = data.Length;
        BitArray R = new BitArray(N);
        for (int j = 0; j < N; j++)
        {
            R[(N+ j - i) % N] = data[j];
        }
        return R;
    }
    
}

