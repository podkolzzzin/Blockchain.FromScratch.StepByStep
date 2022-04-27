using System.Text.Json;

record Transaction(string From, string To, long Amount);

record TransactionBlock(Transaction Data, string Sign) : ISigned<Transaction>
{
    public string PublicKey => Data.From;
}

class AmountIsAvailableRule : IRule<TransactionBlock>
{
    public void Execute(IEnumerable<Block<TransactionBlock>> builtBlocks, Block<TransactionBlock> newData)
    {
        long balance = 100;
        var transaction = newData.Data;
        var from = transaction.Data.From;
        foreach (var block in builtBlocks)
        {
            var signedTransaction = block.Data;
            if (signedTransaction.Data.From == from)
                balance -= signedTransaction.Data.Amount;
            else
                balance += signedTransaction.Data.Amount;
        }

        if (balance < transaction.Data.Amount)
            throw new ApplicationException(
                $"User {transaction.PublicKey} does not have {transaction.Data.Amount} coins. It has only {balance} coins.");
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

    public KeyPair GenerateKeys() => _encryptor.GenerateKeys();

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