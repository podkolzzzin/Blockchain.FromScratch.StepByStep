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