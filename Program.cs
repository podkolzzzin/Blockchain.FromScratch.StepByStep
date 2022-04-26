// See https://aka.ms/new-console-template for more information

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