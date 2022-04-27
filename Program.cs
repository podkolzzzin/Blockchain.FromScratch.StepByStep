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

interface IRule<T>
{
    void Execute(IEnumerable<Block<T>> builtBlocks, T newData);
}

class DuplicationRule : IRule<Person>
{
    public void Execute(IEnumerable<Block<Person>> builtBlocks, Person newData)
    {
        if (builtBlocks.Any(x => x.Data.Equals(newData)))
            throw new ApplicationException($"Person {newData} is already presented in blockchain.");
    }
}