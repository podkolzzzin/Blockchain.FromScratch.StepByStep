public record KeyPair(string PublicKey, string PrivateKey);

record BlockchainBlock(string ParentHash, string Data, string Hash)
{
    public static string CalculateHash(IHashFunction hashFunction, string data, string parentHash)
    {
        return hashFunction.GetHash(parentHash + data);
    }
}

record Transaction(string From, string To, long Amount);

record TransactionBlock(Transaction Data, string Sign, int Nonce) : ISigned<Transaction>, IProofOfWork
{
    public string PublicKey => Data.From;
}