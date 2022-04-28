using System.Text.Json;

record NFTTransfer(string WorkOfArt, string From, string To);

record NFTBlock(NFTTransfer Data, string Sign, int Nonce) : ISigned<NFTTransfer>, IProofOfWork
{
    public string PublicKey => Data.From;
}

public class NFTApp
{
    private readonly ITypedBlockchain<NFTBlock> _blockchain;
    private readonly IEncryptor _encryptor;
    private readonly IProofOfWorkService<NFTBlock> _proofOfWorkService;

    public NFTApp()
    {
        var hashFunction = new CRC32Hash();
        var lowLevelBlockchain = new Blockchain(hashFunction);
        var proofOfWorkRule = new ProofOfWorkRule<NFTBlock>();
        _encryptor = new RSAEncryptor();
        _blockchain = new TypedBlockchain<NFTBlock>(lowLevelBlockchain, hashFunction,
            new SignCheckRule<NFTBlock, NFTTransfer>(_encryptor),
            new OwningRule(),
            proofOfWorkRule);
        _proofOfWorkService =
            new ProofOfWorkService<NFTBlock>(_blockchain, x => x with { Nonce = x.Nonce + 1 }, proofOfWorkRule);
    }

    public void RegisterWorkOfArt(KeyPair author, string workOfArt)
        => TransferWorkOfArt(author, author.PublicKey, workOfArt);

    public void TransferWorkOfArt(KeyPair from, string to, string workOfArt)
    {
        var block = new NFTTransfer(workOfArt, from.PublicKey, to);
        var transactionString = JsonSerializer.Serialize(block);
        var sign = _encryptor.Sign(transactionString, from.PrivateKey);
        var nftBlock = _proofOfWorkService.Proof(_blockchain.Count(), new NFTBlock(block, sign, 0));
        var result = _blockchain.BuildBlock(nftBlock);
        _blockchain.AcceptBlock(result);
    }
}

class OwningRule : IRule<NFTBlock>
{
    public void Execute(IEnumerable<Block<NFTBlock>> builtBlocks, Block<NFTBlock> newData)
    {
        var block = newData.Data;
        if (block.Data.From == block.Data.To)
        {
            // Verify that nobody else has registered this WorkOfArt
            if (builtBlocks.Any(x => x.Data.Data.WorkOfArt == block.Data.WorkOfArt))
                throw new ApplicationException(
                    "You are trying to register the work of art that is already registered.");
        }
        else
        {
            // Verify that the person who owns this WorkOfArt at the current state is transferring it.
            foreach (var b in builtBlocks.Reverse())
            {
                if (b.Data.Data.WorkOfArt == newData.Data.Data.WorkOfArt)
                {
                    if (b.Data.Data.To == block.Data.To)
                        return;
                    throw new ApplicationException(
                        "You are trying to transfer the work of art that you are not owning.");
                }
            }

            throw new ApplicationException("You are trying- to transfer the work of art that has not been yet registered.");
        }
    }
}