using System.Text.Json;

record Transaction(string From, string To, long Amount);

record TransactionBlock(Transaction Data, string Sign, int Nonce) : ISigned<Transaction>, IProofOfWork
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
    private readonly IProofOfWorkRule<TransactionBlock> _proofOfWorkRule;

    public CoinBlockchainApp()
    {
        var hashFunction = new CRC32Hash();
        var lowLevelBlockchain = new Blockchain(hashFunction);
        _proofOfWorkRule = new ProofOfWorkRule<TransactionBlock>();
        _encryptor = new RSAEncryptor();
        _blockchain = new TypedBlockchain<TransactionBlock>(lowLevelBlockchain, hashFunction,
            new SignCheckRule<TransactionBlock, Transaction>(_encryptor),
            new AmountIsAvailableRule(),
            _proofOfWorkRule);
    }

    public KeyPair GenerateKeys() => _encryptor.GenerateKeys();

    public void AcceptTransaction(TransactionBlock transactionBlock)
    {
        var block = _blockchain.BuildBlock(transactionBlock);
        _blockchain.AcceptBlock(block);
    }

    public void PerformTransaction(KeyPair fromKeys, string to, long amount)
    {
        var transaction = new Transaction(fromKeys.PublicKey, to, amount);
        var transactionString = JsonSerializer.Serialize(transaction);
        var sign = _encryptor.Sign(transactionString, fromKeys.PrivateKey);
        int height = _blockchain.Count();
        var transactionBlock = new TransactionBlock(transaction, sign, 0);
        for (int i = 0; i < int.MaxValue; i++)
        {
            transactionBlock = transactionBlock with { Nonce = i };
            var block = _blockchain.BuildBlock(transactionBlock);
            if (_proofOfWorkRule.Execute(height, block.Hash))
                break;
        }

        AcceptTransaction(transactionBlock);
    }
}