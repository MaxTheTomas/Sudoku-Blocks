namespace Game {
  using AI;
  public static class GameState { 
    public const int DESTROY_COST = 100;
    public const int DESTROY_COST_MULTIPLIER = 2;
    public const int RANDOM_SEED = 35484629;
    public static Random seededRandom = new Random(RANDOM_SEED);
    public const int SIZE = 9;
    public static bool AlgorithmPaused = true;
    public static bool AlgorithmSuccessful { get; private set; } = true;
    public static int SuccessfulMoves { get; private set; } = 0;
    public static int score = 0;
    public static int[] state { private set; get; } = new int[SIZE*SIZE];
    public static int[][] currentShape = ShapeTypes.GetRandomShape();
    public static int[][] upcomingShape = ShapeTypes.GetRandomShape();
    public static int[][] nextUpcomingShape = ShapeTypes.GetRandomShape();

    // happens in other thread!!!
    public static void OnFrame() {
       if (!AlgorithmPaused) DoAIStep();

       Destroy();
    }

    public static void DoAIStep() { 
      var move = Algorithm.GetNextMove();
      AlgorithmSuccessful = move != -1;
      if (move == -1) { /* Console.Error.WriteLine("[WARN] AI does not know what to do!"); */ return; }
      SuccessfulMoves++;
      PlaceShape(move % SIZE, move / SIZE, currentShape);
      currentShape = upcomingShape;
      upcomingShape = nextUpcomingShape;
      nextUpcomingShape = ShapeTypes.GetRandomShape();
    }

    public static void Reset() { 
      seededRandom = new Random(RANDOM_SEED);
      state = new int[SIZE*SIZE];
      score = 0;
      currentShape = ShapeTypes.GetRandomShape();
      upcomingShape = ShapeTypes.GetRandomShape();
      nextUpcomingShape = ShapeTypes.GetRandomShape();
      SuccessfulMoves = 0;
    }

    public static bool HasBlock(int x, int y) { 
      var i = y * SIZE + x;

      if (x >= SIZE) return true;
      if (y >= SIZE) return true;
      
      if (i < 0 || i >= SIZE*SIZE) 
        return true;
      
      return state[i] == 1;
    }

    public static void Destroy() { 
      score += GetDestroy(out var toRemove);
      foreach (var i in toRemove) { 
        SetBlock(i % SIZE, i / SIZE, false);
      }
    }

    public static void PlaceBlock(int x, int y) { SetBlock(x, y, true); }
    public static void RemoveBlock(int x, int y) { SetBlock(x, y, false); }
    public static void SetBlock(int x, int y, bool s) { state[y * SIZE + x] = s ? 1 : 0; }

    public static void PlaceShape(int x, int y, int[][] shape) { 
      if (CanPlaceShape(x, y, shape)) PlaceShapeForcefully(x, y, shape);
    }

    public static void PlaceShapeForcefully(int x, int y, int[][] shape) {  
      for (int sY = 0; sY < shape.Length; sY++) {
        for (int sX = 0; sX < shape[sY].Length; sX++) {
          if (shape[sY][sX] == 1) 
            PlaceBlock(x + sX, y + sY);
        }
      }
    }

    public static bool CanPlaceShape(int x, int y, int[][] shape) {
      for (int sY = 0; sY < shape.Length; sY++) {
        for (int sX = 0; sX < shape[sY].Length; sX++) {
          if (shape[sY][sX] == 1 && HasBlock(x + sX, y + sY)) 
            return false;
        }
      }

      return true;
    }

    public static int GetDestroy(out int[] toRemove) { 
      List<int> columnsToDestroy = new List<int>();
      List<int> rowsToDestroy = new List<int>();
      List<int> blobsToDestroy = new List<int>();


      // Check occupied columns
      for (int y = 0; y < SIZE; y++) {
        int occupiedBlocks = 0;
        for (int x = 0; x < SIZE; x++) {
          if (HasBlock(x, y)) occupiedBlocks++;
        }

        if (occupiedBlocks == SIZE) { columnsToDestroy.Add(y); }
      }
      
      // Check occupied rows
      for (int x = 0; x < SIZE; x++) {
        int occupiedBlocks = 0;
        for (int y = 0; y < SIZE; y++) {
          if (HasBlock(x, y)) occupiedBlocks++;
        }

        if (occupiedBlocks == SIZE) { rowsToDestroy.Add(x); }
      }

      // Check occupied blobs
      for (int blobX = 0; blobX < 3; blobX++) {
        for (int blobY = 0; blobY < 3; blobY++) {
          int occupied = 0;
          for (int i = 0; i < 9; i++) {
            if (HasBlock((blobX * 3) + i % 3, (blobY * 3) + i / 3)) occupied++;
          }
          if (occupied == SIZE) { 
            blobsToDestroy.Add(blobY * 3 + blobX);
          }
        }
      }

      
      List<int> blocksToRemove = new List<int>();

      foreach (var y in columnsToDestroy) { 
        for (int x = 0; x < SIZE; x++) 
          blocksToRemove.Add(y * SIZE + x);
      }

      foreach (var x in rowsToDestroy) { 
        for (int y = 0; y < SIZE; y++) 
          blocksToRemove.Add(y * SIZE + x);
      }

      foreach (var blob in blobsToDestroy) {
        var bX = blob % 3;
        var bY = blob / 3;
        
        for (int i = 0; i < SIZE; i++) {
          var x = (bX * 3) + (i % 3);
          var y = (bY * 3) + (i / 3);

          blocksToRemove.Add(y * SIZE + x);
        }
      }

      toRemove = blocksToRemove.ToArray();

      var score = columnsToDestroy.Count + rowsToDestroy.Count + blobsToDestroy.Count;
      return DESTROY_COST * score;
    }

    
    public static int GetDestroyFor(int[] field, out int[] toRemove) { 
      int[] start = new int[SIZE * SIZE];
      Array.Copy(state, start, state.Length);
      
      state = field;
      var score = GetDestroy(out toRemove);

      state = start;
      return score;
    }

    public static int[] GetFieldWithShape(int x, int y, int[][] shape) { 
      return GetFieldWithShape(state, x, y, shape);
    }

    public static int[] GetFieldWithShape(int[] field, int x, int y, int[][] shape) { 
      int[] start = new int[SIZE * SIZE];
      Array.Copy(state, start, state.Length);
      
      for (int sY = 0; sY < shape.Length; sY++) {
        for (int sX = 0; sX < shape[sY].Length; sX++) {
          if (shape[sY][sX] == 1) {
            var i = ((y + sY) * SIZE) + (sX + x);
            if (i < 0 || i >= start.Length) continue;
            start[i] = 1;
          }
        }
      }

      return start;
    }


    // public static int[] GetPossibleMoves() { 
    //   List<int> moves = new List<int>();
    //   for (int i = 0; i < SIZE*SIZE; i++) {
    //     var x = i % SIZE; var y = i / SIZE;
    //     if (CanPlaceShape(x, y, currentShape)) moves.Add(i);
    //   }
    // }

    public static int[] GetPossibleMoves(int[] field, int[][] shape) { 
      int[] start = new int[SIZE * SIZE];
      Array.Copy(state, start, state.Length);

      state = field;
      
      List<int> moves = new List<int>();
      for (int i = 0; i < SIZE*SIZE; i++) {
        var x = i % SIZE; var y = i / SIZE;
        if (CanPlaceShape(x, y, shape)) moves.Add(i);
      }

      state = start;
      return moves.ToArray();
    }
  } 
}