class BlockchainBuilder
{
    private readonly IHashFunction _hashFunction;
    private string? _tail = null;

    public BlockchainBuilder(IHashFunction hashFunction, string? tail)
    {
        _hashFunction = hashFunction;
        _tail = tail;
    }

    public void AcceptBlock(BlockchainBlock block)
    {
        _tail = block.Hash;
    }

    public BlockchainBlock BuildBlock(string data)
    {
        var blockHash = BlockchainBlock.CalculateHash(_hashFunction, data, _tail!);
        var block = new BlockchainBlock(_tail!, data, blockHash);
        return block;
    }
}