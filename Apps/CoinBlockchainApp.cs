using System.Text.Json;



class CoinBlockchainApp
{
    private readonly ITypedBlockchain<TransactionBlock> _blockchain;
    private readonly IEncryptor _encryptor;
    private readonly IProofOfWorkService<TransactionBlock> _proofOfWorkService;

    public CoinBlockchainApp()
    {
        var hashFunction = new CRC32Hash();
        var lowLevelBlockchain = new Blockchain(hashFunction);
        IProofOfWorkRule<TransactionBlock> proofOfWorkRule = new ProofOfWorkRule<TransactionBlock>();
        _encryptor = new RSAEncryptor();
        _blockchain = new TypedBlockchain<TransactionBlock>(lowLevelBlockchain, hashFunction,
            new SignCheckRule<TransactionBlock, Transaction>(_encryptor),
            new AmountIsAvailableRule(),
            proofOfWorkRule);
        _proofOfWorkService =
            new ProofOfWorkService<TransactionBlock>(_blockchain, x => x with { Nonce = x.Nonce + 1 }, proofOfWorkRule);
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
        var transactionBlock = _proofOfWorkService.Proof(height, new TransactionBlock(transaction, sign, 0));

        AcceptTransaction(transactionBlock);
    }
}