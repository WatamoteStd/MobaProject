
using NetworkLayer;

NetworkManager networkManager = new NetworkManager(1234);
networkManager.Start();



Console.WriteLine($"============================= TEST SERVER STARTED =======================");

Console.ReadKey();