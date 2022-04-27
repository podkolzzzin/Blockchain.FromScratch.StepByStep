namespace SampleRSA;

public class SampleRSAClass
{
    public void DoSample()
    {
        var rsaEncryptor = new RSAEncryptor();
        var keyPair = rsaEncryptor.GenerateKeys();
        Console.WriteLine(keyPair);

        var someData = "Hello World";
        var signedBlock = rsaEncryptor.Sign(keyPair.PrivateKey, someData);
        if (rsaEncryptor.VerifySign(keyPair.PublicKey, someData, signedBlock))
            Console.WriteLine("Signed correctly");
        else
            Console.Error.WriteLine("Signed incorrectly");

        if (rsaEncryptor.VerifySign(keyPair.PublicKey, someData, signedBlock.Replace('A', 'B')))
            Console.WriteLine("Signed correctly");
        else
            Console.Error.WriteLine("Signed incorrectly");


        var anotherPair = rsaEncryptor.GenerateKeys();
        if (rsaEncryptor.VerifySign(anotherPair.PublicKey, someData, signedBlock))
            Console.WriteLine("Signed correctly");
        else
            Console.Error.WriteLine("Signed incorrectly");
    }
}