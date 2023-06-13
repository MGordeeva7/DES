using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;



class DES
{
    private static bool isLog = true; 
    static BitArray ApplyP(BitArray data, byte[] M) 
    {
        BitArray R = new BitArray(M.Length); 
        for (int i = 0; i < M.Length; i++)
        {
            R[i] = data[M[i] - 1];
        }
        return R;
    }
    static BitArray f(BitArray R, BitArray key)
    {
        BitArray Rexp = ApplyP(R, DESMatrix.MPexp);
        Rexp = Rexp.Xor(key);
        R = ApplyS(Rexp, DESMatrix.SMs);
        R = ApplyP(R, DESMatrix.MPforw);
        return R;
    }
    static BitArray ApplyS(BitArray data, byte[,,] SMs)
    {
        BitArray[] bs = data.Separate(6);//делим на массивы по 6 битов
        BitArray[] tbs = new BitArray[8];//по 8 строк
        for (int i = 0; i < 8; i++)
        {
            int m = (bs[i][0] ? 2 : 0) + (bs[i][5] ? 1 : 0); //первый бит переводим в 2-ую СЧ, бинарная арифм.Возможен обратный порядок
            int l = (bs[i][1] ? 8 : 0) + (bs[i][2] ? 4 : 0) + (bs[i][3] ? 2 : 0) + (bs[i][4] ? 1 : 0);
            tbs[i] = BitArrayExtension.FromByte(SMs[i, m, l]); //преобразуем в массив битов
        }
        BitArray R = BitArrayExtension.JoinBitArrays(tbs, 4);//обьединяем в общий массив битов
        return R;
    }

    public static BitArray Encrypt(BitArray data, BitArray key, int isReverse = 0)
    {
        //Делаем первоначальную перестановку
        data = ApplyP(data, DESMatrix.MIP);
        BitArray[] LR = data.Separate(32);//делим на левую и правую часть по 32 бита
        BitArray key56 = ApplyP(key, DESMatrix.MPkey);//делаем из 64-битного 56-битный ключ
        BitArray[] CD = key56.Separate(28);
        BitArray[] roundKeys = new BitArray[16];
        for (int i = 0; i < 16; i++)
        {
            CD[0] = CD[0].crotl(DESMatrix.shiftBits[i]);//CD[0], т е берем С и сдвигаем влево на количество битов из матрицы, согласно раунду
            CD[1] = CD[1].crotl(DESMatrix.shiftBits[i]);
            key56 = BitArrayExtension.JoinBitArrays(CD, 28);
            roundKeys[15 * isReverse + (-2 * isReverse + 1) * i] = ApplyP(key56, DESMatrix.MPcomp);//если число isrevers = 0(0..15), то результат i, иначе 1
        }
        for (int i = 0; i < 16; i++)
        {

            LR[0] = f(LR[1], roundKeys[i]).Xor(LR[0]);//берем правую часть, ключ

            (LR[0], LR[1]) = (LR[1], LR[0]);//меняем местами
            if (isLog) Console.WriteLine(LR[0].ToString("X") + "  " + LR[1].ToString("X") + "  " + roundKeys[i].ToString("X"));
        }
        (LR[0], LR[1]) = (LR[1], LR[0]);
        data = BitArrayExtension.JoinBitArrays(LR, 32);//обьединяем в общ часть
        data = ApplyP(data, DESMatrix.MIP_1);
        return data;
    }

    public static BitArray Decrypt(BitArray cipher, BitArray key)//вызываем энкрипт в обрат порядке
    {
        return Encrypt(cipher, key, 1);
    }

    public static BitArray Encrypt(BitArray data, DESKey key)//чтоб пользователю было проще создать обьект класса deskey
    {
        return Encrypt(data, key.key64);
    }

    public static BitArray Decrypt(BitArray data, DESKey key)//тоже самое, но обертка - ключ
    {
        return Encrypt(data, key.key64, 1);
    }

    public static string Encrypt(string data, DESKey key)//со строкой
    {
        return Encrypt(BitArrayExtension.GetFromString(data, "S"), key.key64).ToString("S");//преобразуем в строку 
    }

    public static BitArray Encrypt_ECB(string data, DESKey key)//метод шифрования блоков. каждый блок шифруется отдельно, а потом складываются воедино
    {
        if (data.Length % 8 != 0) for (; data.Length % 8 != 0;) data += " ";//если не кратно 8(1 символ - 1 байт), заполняем пустыми знаками
        BitArray bdata = BitArrayExtension.GetFromString(data, "S");//получаем из строки
        BitArray[] blocks = bdata.Separate(64);//создаем массив для зашифрованных блоков
        BitArray[] cryptBlocks = new BitArray[blocks.Length];
        for (int i = 0; i < blocks.Length; i++)
        {
            cryptBlocks[i] = Encrypt(blocks[i], key);//вызываем для каждого блока метод энкрипт
        }
        var crypt = BitArrayExtension.JoinBitArrays(cryptBlocks, 64);//обьединяет
        return crypt;
        //return Convert.ToBase64String(crypt.ToBytes());
    }

    public static string Decrypt_ECB(BitArray data, DESKey key)//дешифровка
    {
        BitArray[] blocks = data.Separate(64);//делим на 64 бита
        string crypt = "";
        for (int i = 0; i < blocks.Length; i++)
        {
            crypt += Decrypt(blocks[i], key).ToString("S");//используем decript
        }
        return crypt;
    }
}
