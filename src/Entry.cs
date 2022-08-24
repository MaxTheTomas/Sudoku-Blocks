using AI;
using Game;

// var network = new NeuralNetwork(9*9+3,100,9*9); 
// network.RandomizeAll(3, 3);

// GameManager.AddGame(network);
GameManager.AddRandomNetworkGames(100, 2);
new WindowRenderer().Start();


// var j = json.GetAsJSON();
// File.WriteAllText("./json.json", j);