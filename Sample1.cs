namespace Sample1
{
    record Block(string Hash, string Data);

    class SampleBlockchainAntiCorruption
    {
        public void DoSample()
        {
            var hashFunction = new CRC32Hash();
            var data = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            string parentHash = null!;
            foreach (var item in data)
            {
                var block = new Block(parentHash, item.ToString());
                Console.WriteLine(block);
                parentHash = hashFunction.GetHash(block.Hash + block.Data);
            }

            Console.WriteLine("================================");
        
            data[2] = 50;
            parentHash = null!;
            foreach (var item in data)
            {
                var block = new Block(parentHash, item.ToString());
                Console.WriteLine(block);
                parentHash = hashFunction.GetHash(block.Hash + block.Data);
            }
        }
    }
}