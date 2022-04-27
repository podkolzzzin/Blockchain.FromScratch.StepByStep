var coinApp = new CoinBlockchainApp();
var user1 = coinApp.GenerateKeys();
var user2 = coinApp.GenerateKeys();
coinApp.PerformTransaction(user1, user2.PublicKey, 50);
coinApp.PerformTransaction(user2, user1.PublicKey, 105);
coinApp.PerformTransaction(user2, user1.PublicKey, 50);



