namespace Game { 
  using AI;
  using System.Text.Json.Nodes;
  public static class GameManager { 
    public static int[] LayerConfiguration = new int[] { 9*9+3, 
      1000, 100, 10, 9*9 }; 
    public static List<Game> Games = new List<Game>();

    public static decimal LearningRate = 0.00100m;
    public static bool Learning = false;
    public static bool AutoAdjustRate = true;
  
    public static void AddGame(AI.NeuralNetwork network) { 
      Games.Add(new Game(network));
    }

    public static void AddGames(AI.NeuralNetwork network, int times) { 
      for (int i = 0; i < times; i++) {
        Games.Add(new Game(network.Clone()));
      }
    }

    public static void LoadFromJSON() { 
      var j = JsonObject.Parse(File.ReadAllText("ai-config.json"));
      var ai = NeuralNetwork.LoadFromJSON(j["network"].ToJsonString());
      Console.WriteLine("Layers: " + String.Join(", ", ai.LayerConfiguration));
      var am = Games.Count;
      Games.Clear();
      AddGames(ai, am);
    }

    public static void AddRandomNetworkGames(int amount = 1, float randomness = 1f) { 
      for (int i = 0; i < amount; i++) {
        // HERE CONFIG:
        var n = new NeuralNetwork(layerSizes: LayerConfiguration);
        
        n.RandomizeAll(randomness, randomness);
        AddGame(n);
      }
    }

    public static void TickAll() { 
      foreach (var g in Games) 
        g._tick();
    }

    public static void ResetAll() { 
      foreach (var g in Games) 
        g.Reset();
    }

    public static void MutateAll(float weightRate = .05f, float biasRate = .05f) { 
      foreach (var g in Games) 
        g.network.RandomizeAll(weightRate, biasRate);
    }

    public static int GetRunningGames() { 
      int i = 0;
      foreach (var g in Games) if (!g.Finished) i++;
      return i;
    }

    public static void StartAll() { 
      new Thread(_start).Start();
    }

    public static bool has_running_games() { 
      foreach (var g in Games) if (!g.Finished) return true;
      return false;
    }

    public static void SortGames() { 
      Games.Sort((a, b) => b.SuccessfulMoves - a.SuccessfulMoves);
    }

    public static int BestMoves { get; private set; } = 0;
    public static int BestScore { get; private set; } = 0;

    public static int BestMoves_G { get; private set; } = 0;
    public static int BestScore_G { get; private set; } = 0;
    public static int WorstMoves_G { get; private set; } = 0;
    public static int WorstScore_G { get; private set; } = 0;

    public static int Iterations { get; private set; } = 0;
    // static Game BestGame;

    static void _start() { 
      while (has_running_games() || Learning) { 
        var bs = 0;
        var bm = 0;
        var ws = Int32.MaxValue;
        var wm = Int32.MaxValue;

        foreach (var g in Games) { 
          g._tick();
          if (g.SuccessfulMoves > BestMoves) BestMoves = g.SuccessfulMoves;
          if (g.Score > BestScore) BestScore = g.Score; 
          
          if (g.SuccessfulMoves > bm) bm = g.SuccessfulMoves;
          if (g.Score > bs) bs = g.Score; 

          if (g.Finished) { 
            if (g.Score < ws) ws = g.Score;
            if (g.SuccessfulMoves < wm) wm = g.SuccessfulMoves;
          }
        }

        if (Learning && !has_running_games()) { 
          Iterations++;
          
          // get first 
          SortGames();

          List<Game> best = new List<Game>();
          best.AddRange(Games.Take(Games.Count / 5));

          Games.Clear();

          foreach (var i in best) { AddGames(i.network.Clone(), 2); }
          
          var b2 = best.Take(best.Count / 2).ToList();
          b2.Sort((a, b) => a.UnsuccessfulMoveTries - b.UnsuccessfulMoveTries);
          
          foreach (var i in b2) { AddGames(i.network.Clone(), 4); }

          // mutate
          // todo if rate is too small, mutate all
          if (AutoAdjustRate) { 
            if ((wm == bm && ws == bs) && (wm == WorstMoves_G && ws == WorstScore_G)) LearningRate += .005m;
            if (wm == 0 || ((float) bm / (float) wm > 3)) LearningRate -= .0001m;
            if (ws != 0 && ((float) bs / (float) ws > 3)) LearningRate -= .0001m;
          }

          MutateAll(((float)LearningRate), (float)LearningRate / 3);

          foreach (var i in best) { 
            AddGames(i.network.Clone(), 1);
          }

          // restart
          BestMoves_G = bm;
          BestScore_G = bs;

          WorstMoves_G = wm;
          WorstScore_G = ws;
        }
      }
    }
  }

  public class Game { 
    public AI.NeuralNetwork network { get; private set; }
    public GameState state { get; private set; }
    public int SuccessfulMoves { get; private set; } = 0;
    public int Score => state.score;
    public bool Finished { get; private set; } = false;

    bool should_stop_thread = false;
    public Game(AI.NeuralNetwork network) { 
      this.network = network;
      state = new GameState();
    }

    public void Reset() { 
      SuccessfulMoves = 0;
      Finished = false;
      state.Reset();

      Start();
    }

    // Thread t;

    public void Start() {
      // Abort();
      // t = new Thread(_start);     
      // t.Start();
    }

    public void Abort() { 
      // if (t==null) return;
      // should_stop_thread = true;
      // while (t.ThreadState != ThreadState.Stopped) { }
    }


    void _start() {
      should_stop_thread = false;
      while (!should_stop_thread) {  
        _tick();
      }
    }

    public int UnsuccessfulMoveTries { get; private set; } = 0;

    public void _tick() { 
      if (state.GetPossibleMoves(state.state, state.currentShape).Length == 0) {
        Finished = true;
        return;
      }

      List<float> inputs = new List<float>();

      foreach (var i in state.state) { 
        inputs.Add(i);
      }

      inputs.Add(Array.IndexOf(ShapeTypes.SHAPES, state.currentShape));
      inputs.Add(Array.IndexOf(ShapeTypes.SHAPES, state.upcomingShape));
      inputs.Add(Array.IndexOf(ShapeTypes.SHAPES, state.nextUpcomingShape));
      
      var outputs = network.CalculateOutputs(inputs.ToArray());
      
      Dictionary<int, float> dic = new Dictionary<int, float>();
      for (int i = 0; i < outputs.Length; i++) {
        dic.Add(i, outputs[i]);
      }

      var sorted = from entry in dic orderby entry.Value descending select entry;

      var placed = false;
      var place_tries = 0;
      foreach (var a in sorted) { 
        if (state.PlaceShape(a.Key % 9, a.Key / 9, state.currentShape)) { 
          placed = true;
          break;
        }
        place_tries++;
      } 

      UnsuccessfulMoveTries += place_tries;

      if (!placed) {
        Console.Error.WriteLine("[WARN] AI cannot place shape!");
        return;
      }

      // Console.WriteLine($"Tried to place { place_tries } times");
      
      state.Proceed();
      SuccessfulMoves++;

      // Thread.Sleep(100);
    }

    public string GetAsJSON() { 
      return new JsonObject { 
        ["network"] = JsonObject.Parse(network.GetAsJSON()),
        ["moves"] = SuccessfulMoves,
        ["score"] = Score
      }.ToJsonString();
    }

    public Game Clone() {
      return new Game(network.Clone()) { 
        state = state,
        SuccessfulMoves = SuccessfulMoves,
        Finished = Finished
      };
    }
  }
}