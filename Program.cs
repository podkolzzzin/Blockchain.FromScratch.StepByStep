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