﻿using System.Text.Json;

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

record Transaction(string From, string To, long Amount);

record TransactionBlock(Transaction Data, string Sign) : ISigned<Transaction>
{
    public string PublicKey => Data.From;
}

class AmountIsAvailableRule : IRule<TransactionBlock>
{
    public void Execute(IEnumerable<Block<TransactionBlock>> builtBlocks, TransactionBlock newData)
    {
        long balance = 0;
        var from = newData.Data.From;
        foreach (var block in builtBlocks)
        {
            var signedTransaction = block.Data;
            if (signedTransaction.Data.From == from)
                balance -= signedTransaction.Data.Amount;
            else
                balance += signedTransaction.Data.Amount;
        }

        if (balance < newData.Data.Amount)
            throw new ApplicationException(
                $"User {newData.PublicKey} does not have {newData.Data.Amount} coins. It has only {balance} coins.");
    }
}

class CoinBlockchainApp
{
    private readonly ITypedBlockchain<TransactionBlock> _blockchain;
    private readonly IEncryptor _encryptor;

    public CoinBlockchainApp()
    {
        var hashFunction = new CRC32Hash();
        var lowLevelBlockchain = new Blockchain(hashFunction);
        _encryptor = new RSAEncryptor();
        _blockchain = new TypedBlockchain<TransactionBlock>(lowLevelBlockchain, hashFunction,
            new SignCheckRule<TransactionBlock, Transaction>(_encryptor),
            new AmountIsAvailableRule());
    }

    public void AcceptTransaction(TransactionBlock transactionBlock)
    {
        _blockchain.AddBlock(transactionBlock);
    }

    public void PerformTransaction(KeyPair fromKeys, string to, long amount)
    {
        var transaction = new Transaction(fromKeys.PublicKey, to, amount);
        var transactionString = JsonSerializer.Serialize(transaction);
        var sign = _encryptor.Sign(transactionString, fromKeys.PrivateKey);
        var block = new TransactionBlock(transaction, sign);
        AcceptTransaction(block);
    }
}

interface ISigned<T>
{
    public string Sign { get; }
    public string PublicKey { get; }
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