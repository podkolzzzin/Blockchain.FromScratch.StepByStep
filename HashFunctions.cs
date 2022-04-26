using System.Security.Cryptography;
using System.Text;
using Force.Crc32;

public interface IHashFunction
{
    public string GetHash(string data);
}

public class SHA256Hash : IHashFunction
{
    public string GetHash(string data)
    {
        var sha = SHA256.HashData(Encoding.UTF8.GetBytes(data));
        return string.Concat(sha.Select(x => $"{x:x2}"));
    }
}

public class CRC32Hash : IHashFunction
{
    public string GetHash(string data)
    {
        var hash = Crc32Algorithm.Compute(Encoding.UTF8.GetBytes(data));
        return DecimalToArbitrarySystem(hash, 64);
    }
    
    private static string DecimalToArbitrarySystem(long decimalNumber, int radix)
    {
        const int BitsInLong = 64;
        const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_%";

        if (radix < 2 || radix > Digits.Length)
            throw new ArgumentException("The radix must be >= 2 and <= " + Digits.Length.ToString());

        if (decimalNumber == 0)
            return "0";

        int index = BitsInLong - 1;
        long currentNumber = Math.Abs(decimalNumber);
        char[] charArray = new char[BitsInLong];

        while (currentNumber != 0)
        {
            int remainder = (int)(currentNumber % radix);
            charArray[index--] = Digits[remainder];
            currentNumber = currentNumber / radix;
        }

        string result = new String(charArray, index + 1, BitsInLong - index - 1);
        if (decimalNumber < 0)
        {
            result = "-" + result;
        }

        return result;
    }
}