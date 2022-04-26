record BlockchainBlock(string ParentHash, string Data, string Hash)
{
    public static string CalculateHash(IHashFunction hashFunction, string data, string parentHash)
    {
        return hashFunction.GetHash(parentHash + data);
    }
}