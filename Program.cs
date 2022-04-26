// See https://aka.ms/new-console-template for more information

var builder = new BlockchainBuilder(new CRC32Hash(), null);
foreach (var i in Enumerable.Range(0, 10))
{
    Console.WriteLine(builder.AddBlock(i.ToString()));
}

record BlockchainBlock(string ParentHash, string Data);

class BlockchainBuilder
{
    private readonly IHashFunction _hashFunction;
    private string? _tail = null;

    public BlockchainBuilder(IHashFunction hashFunction, string? tail)
    {
        _hashFunction = hashFunction;
        _tail = tail;
    }

    public BlockchainBlock AddBlock(string data)
    {
        var block = new BlockchainBlock(_tail, data);
        _tail = _hashFunction.GetHash(block.ParentHash + block.Data);
        return block;
    }
}