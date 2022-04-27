using System.Collections;
using System.Text.Json;

record struct Block<T>(string Hash, string ParentHash, string Raw, T Data);

interface ITypedBlockchain<T> : IEnumerable<Block<T>>
{
    Block<T> BuildBlock(T data);
    void AcceptBlock(Block<T> typedBlock);
}

interface IRule<T>
{
    void Execute(IEnumerable<Block<T>> builtBlocks, Block<T> newData);
}

class TypedBlockchain<T> : ITypedBlockchain<T>
{
    private readonly IBlockchain _blockchain;
    private readonly BlockchainBuilder _builder;
    private readonly IRule<T>[] _businessRules;

    public TypedBlockchain(IBlockchain blockchain, IHashFunction hashFunction, params IRule<T>[] businessRules)
    {
        _blockchain = blockchain;
        _builder = new BlockchainBuilder(hashFunction, _blockchain.LastOrDefault()?.Hash);
        _businessRules = businessRules;
    }

    public Block<T> BuildBlock(T data)
    {
        var dataStr = JsonSerializer.Serialize(data);
        var block = _builder.BuildBlock(dataStr);
        var typedBlock = new Block<T>(block.Hash, block.ParentHash, dataStr, data);
        return typedBlock;
    }

    public void AcceptBlock(Block<T> typedBlock)
    {
        foreach (var rule in _businessRules)
        {
            rule.Execute(this, typedBlock);
        }

        var block = _builder.BuildBlock(typedBlock.Raw);
        _blockchain.AddBlock(block);
        _builder.AcceptBlock(block);
    }

    public IEnumerator<Block<T>> GetEnumerator()
    {
        return _blockchain.Select(x => new Block<T>(x.Hash, x.ParentHash, x.Data, JsonSerializer.Deserialize<T>(x.Data)!))
            .GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}