class ProofOfWorkService<T> : IProofOfWorkService<T> where T : IProofOfWork
{
    private readonly ITypedBlockchain<T> _blockchain;
    private readonly Func<T, T> _nextVariant;
    private readonly IProofOfWorkRule<TransactionBlock> _proofOfWorkRule;
    
    public ProofOfWorkService(ITypedBlockchain<T> blockchain, Func<T, T> nextVariant, IProofOfWorkRule<TransactionBlock> proofOfWorkRule)
    {
        _blockchain = blockchain;
        _nextVariant = nextVariant;
        _proofOfWorkRule = proofOfWorkRule;
    }

    public T Proof(int height, T block)
    {
        for (int i = 0; i < int.MaxValue; i++)
        {
            var lowLevelBlock = _blockchain.BuildBlock(block);
            if (_proofOfWorkRule.Execute(height, lowLevelBlock.Hash))
                return block;
            block = _nextVariant(block);
        }

        throw new ApplicationException("Block is not possible to build. Try again after some blocks will be put.");
    }
}