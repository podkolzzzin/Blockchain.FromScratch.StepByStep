using System.Collections;
using System.Text.Json;

var hashFunction = new CRC32Hash();
var app = new PopulationCensusApp(new TypedBlockchain<Person>(new Blockchain(hashFunction), hashFunction));
app.AddPerson(new Person("Iggy", "Pop"));
app.AddPerson(new Person("Mick", "Jagger"));

app.PrintAll();

record Person(string Name, string Surname);
class PopulationCensusApp
{
    private readonly ITypedBlockchain<Person> _registry;

    public PopulationCensusApp(ITypedBlockchain<Person> registry)
    {
        _registry = registry;
    }

    public void AddPerson(Person person)
    {
        _registry.AddBlock(person);
    }

    public void PrintAll()
    {
        foreach (var personBlock in _registry)
        {
            Console.WriteLine(personBlock);
        }
    }
}

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