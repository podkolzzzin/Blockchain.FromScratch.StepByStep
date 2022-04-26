// See https://aka.ms/new-console-template for more information

using System.Collections;

var hashFunction = new CRC32Hash();
var builder = new BlockchainBuilder(hashFunction, null);
var chain = new Blockchain(hashFunction);
foreach (var i in Enumerable.Range(0, 10))
{
    chain.AddBlock(builder.BuildBlock(i.ToString()));
}

var list = chain.ToList();
//list[3] = list[3] with { Data = "__" };
//list[3] = list[3] with { ParentHash = list[4].ParentHash };
//list[3] = list[3] with { Hash = list[5].Hash };
var newChain = new Blockchain(hashFunction);
foreach(var item in list)
    newChain.AddBlock(item);

class Blockchain : IEnumerable<BlockchainBlock>
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

record BlockchainBlock(string ParentHash, string Data, string Hash)
{
    public static string CalculateHash(IHashFunction hashFunction, string data, string parentHash)
    {
        return hashFunction.GetHash(parentHash + data);
    }
}

class BlockchainBuilder
{
    private readonly IHashFunction _hashFunction;
    private string? _tail = null;

    public BlockchainBuilder(IHashFunction hashFunction, string? tail)
    {
        _hashFunction = hashFunction;
        _tail = tail;
    }

    public BlockchainBlock BuildBlock(string data)
    {
        var blockHash = BlockchainBlock.CalculateHash(_hashFunction, data, _tail!);
        var block = new BlockchainBlock(_tail!, data, blockHash);
        _tail = blockHash;
        return block;
    }
}