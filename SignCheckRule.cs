using System.Text.Json;

interface ISigned<T>
{
    public string Sign { get; }
    public string PublicKey { get; }
    public T Data { get; }
}

class SignCheckRule<TBlockData, TSignedData> : IRule<TBlockData> where TBlockData : ISigned<TSignedData>
{
    private readonly IEncryptor _encryptor;
    
    public SignCheckRule(IEncryptor encryptor)
    {
        _encryptor = encryptor;
    }
    
    public void Execute(IEnumerable<Block<TBlockData>> builtBlocks, TBlockData newData)
    {
        var dataThatShouldBeSigned = JsonSerializer.Serialize(newData.Data);
        if (!_encryptor.VerifySign(newData.PublicKey, dataThatShouldBeSigned, newData.Sign))
            throw new ApplicationException("Block sign is incorrect.");
    }
}