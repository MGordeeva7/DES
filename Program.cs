using System;
using System.Collections;
using System.Text;


class Program
{
    static string message;
    static DESKey key;
   

    static void Main(string[] args)
    {
        string messageFilepath = @"C:\Users\1\DES-master\DES-master\DES\message.txt";
        string keyFilepath = @"C:\Users\1\DES-master\DES-master\DES\key.txt";
        string cryptFilepath = @"C:\Users\1\DES-master\DES-master\DES\shifr.txt";

        message = System.IO.File.ReadAllText(messageFilepath);
        key = new DESKey(System.IO.File.ReadAllText(keyFilepath), "X");

        Console.WriteLine("Сообщение - " + message);
        Console.WriteLine("Ключ - " + key);
;       Console.WriteLine("Ключ слабый ? " + key.IsWeakKey());
        var start = DateTime.Now;
        BitArray crypt = DES.Encrypt_ECB(message, key);
        Console.WriteLine("Время шифрования - " + (DateTime.Now - start).TotalMilliseconds + " мс");
        Console.WriteLine("Зашифрованные данные  - " + crypt.ToString("X"));
        Console.WriteLine("Зашифрованные данные (base64) - " + Convert.ToBase64String(crypt.ToBytes()));
        System.IO.File.WriteAllText(cryptFilepath, crypt.ToString("X"));
        var decrypt = DES.Decrypt_ECB(crypt, key);

        Console.WriteLine("Расшифрованные данные - " + decrypt);

    }
}
