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

class DuplicationRule : IRule<Person>
{
    public void Execute(IEnumerable<Block<Person>> builtBlocks, Block<Person> newData)
    {
        var person = newData.Data;
        if (builtBlocks.Any(x => x.Data.Equals(person)))
            throw new ApplicationException($"Person {newData} is already presented in blockchain.");
    }
}