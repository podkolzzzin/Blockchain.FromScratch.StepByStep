using System.Text.Json;

var hashFunction = new CRC32Hash();
var app = new PopulationCensusApp(
    new TypedBlockchain<Person>(
        new Blockchain(hashFunction),
        hashFunction,
        new DuplicationRule()
        )
    );

app.AddPerson(new Person("Iggy", "Pop"));
app.AddPerson(new Person("Mick", "Jagger"));
app.AddPerson(new Person("Iggy", "Pop"));

app.PrintAll();

interface ISigned<T>
{
    public string Sign { get; set; }
    public string PublicKey { get; set; }
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