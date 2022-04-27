using System.Collections;
using System.Text.Json;
record struct Block<T>(string Hash, string ParentHash, string Raw, T Data);

interface ITypedBlockchain<T> : IEnumerable<Block<T>>
{
    void AddBlock(T data);
}

class TypedBlockchain<T> : ITypedBlockchain<T>
{
    private readonly IBlockchain _blockchain;
    private readonly BlockchainBuilder _builder;
    
    public TypedBlockchain(IBlockchain blockchain, IHashFunction hashFunction)
    {
        _blockchain = blockchain;
        _builder = new BlockchainBuilder(hashFunction, _blockchain.LastOrDefault()?.Hash);
    }

    public void AddBlock(T data)
    {
        var dataStr = JsonSerializer.Serialize(data);
        var block = _builder.BuildBlock(dataStr);
        _blockchain.AddBlock(block);
    }

    public IEnumerator<Block<T>> GetEnumerator()
    {
        return _blockchain.Select(x => new Block<T>(x.Hash, x.ParentHash, x.Data, JsonSerializer.Deserialize<T>(x.Data)!))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}