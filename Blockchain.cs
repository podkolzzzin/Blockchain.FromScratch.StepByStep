using System.Collections;



class Blockchain : IBlockchain
{
    private readonly List<BlockchainBlock> _blocks = new List<BlockchainBlock>();
    private readonly IHashFunction _hashFunction;

    public Blockchain(IHashFunction hashFunction)
    {
        _hashFunction = hashFunction;
    }
    
    public void AddBlock(BlockchainBlock block)
    {
        var tail = _blocks.LastOrDefault();
        if (block.ParentHash == tail?.Hash)
        {
            var expectedHash = BlockchainBlock.CalculateHash(_hashFunction, block.Data, block.ParentHash);
            if (expectedHash == block.Hash)
                _blocks.Add(block);
            else
                throw new ApplicationException($"Block {block} has invalid hash. It should be {expectedHash}.");
        }
        else
            throw new ApplicationException($"{block.ParentHash} is not following the current block {tail}");
    }

    public IEnumerator<BlockchainBlock> GetEnumerator() => _blocks.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}