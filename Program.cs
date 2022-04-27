var coinApp = new CoinBlockchainApp();
var user1 = coinApp.GenerateKeys();
var user2 = coinApp.GenerateKeys();
coinApp.PerformTransaction(user1, user2.PublicKey, 50);
coinApp.PerformTransaction(user2, user1.PublicKey, 105);
coinApp.PerformTransaction(user2, user1.PublicKey, 50);

interface IProofOfWork
{
    int Nonce { get; }
}

interface IProofOfWorkRule<T> : IRule<T> where T : IProofOfWork
{
    bool Execute(int height, string hash);
}

class ProofOfWorkRule<T> : IProofOfWorkRule<T> where T : IProofOfWork
{
    public void Execute(IEnumerable<Block<T>> builtBlocks, Block<T> newData)
    {
        var height = builtBlocks.Count();
        if (Execute(height, newData.Hash))
            throw new ApplicationException("Proof of work is incorrect for this block.");
    }

    public bool Execute(int height, string hash)
    {
        var complexity = (int)(Math.Log2(height + 1) + 1);
        for (int i = 0; i < complexity; i++)
        {
            if (hash[i] != '0')
                return false;
        }

        return true;
    }
}