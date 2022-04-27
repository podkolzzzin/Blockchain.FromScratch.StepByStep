public interface IHashFunction
{
    public string GetHash(string data);
}


public interface IEncryptor
{
    KeyPair GenerateKeys();
    string Sign(string data, string privateKey);
    bool VerifySign(string publicKey, string data, string sign);
}

interface IBlockchain : IEnumerable<BlockchainBlock>
{
    void AddBlock(BlockchainBlock data);
}

interface IProofOfWork
{
    int Nonce { get; }
}

interface IProofOfWorkRule<T> : IRule<T> where T : IProofOfWork
{
    bool Execute(int height, string hash);
}

interface IBlockchainBuilderService
{
    void AcceptBlock(BlockchainBlock block);
    BlockchainBlock BuildBlock(string data);
}

interface IProofOfWorkService<T> where T : IProofOfWork
{
    T Proof(int height, T block);
}