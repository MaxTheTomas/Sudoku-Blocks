namespace Game {
  using SFML.Graphics;
  using SFML.Window;
  using SFML.System;
  using System.Threading;

  public class WindowRenderer { 
    static RenderWindow? window;
    public static RenderWindow? Window => window;

    public static readonly Vector2f size = new Vector2f(500, 500);
    public static readonly Vector2f startPos = new Vector2f(20, 20);
      
    Font font = new Font("c:/windows/fonts/arial.ttf");

    public void Start(bool threaded = true) { 
      if (threaded) { new Thread(() => Render()).Start(); }
      else Render();
    }

    void Render() { 
      window = new RenderWindow(new VideoMode(1280, 720), "Sudoku block game idk");
      window.Closed += (a, b) => { window.Close(); };
      window.Resized += (a, b) => { window.SetView(new View(new FloatRect(0, 0, b.Width, b.Height))); };

      PlayerControls.Initialize();

      while (window.IsOpen) { 
        GameState.OnFrame();

        window.DispatchEvents();
        window.Clear();
        
        RenderInterface();

        window.Display();
      }
    }

    void RenderInterface() { 
      DrawBoard();
      DrawUIs();
    }

    void DrawUIs() { 
      window?.Draw(new Text("weird sudoku-like game algorithm", font, 32) { Position = startPos + new Vector2f(size.X + 20, -10) });
      window?.Draw(new Text("selected shape - right-click on field to place", font, 16) { Position = startPos + new Vector2f(size.X + 20, 35) });
      window?.Draw(new Shape(PlayerControls.SelectedShape) { Scale = .7f, Position = startPos + new Vector2f(size.X + 22, 65) });

      window?.Draw(new Text($"S: {GameState.score}", font, 16) { Position = startPos + new Vector2f(size.X + 20, size.Y - 70), OutlineColor = Color.Black, OutlineThickness = 2 });
      
      window?.Draw(new RectangleShape(new Vector2f(100, 20)) { FillColor = GameState.AlgorithmSuccessful ? Color.Green : Color.Red, OutlineColor = Color.White, OutlineThickness = 2, Position = startPos + new Vector2f(size.X + 130, size.Y - 40) });
      window?.Draw(new Text($"M{GameState.SuccessfulMoves}", font, 16) { Position = startPos + new Vector2f(size.X + 135, size.Y - 40), OutlineColor = Color.Black, OutlineThickness = 1 });
      window?.Draw(new Text($"T: {AI.Algorithm.LastMoveTime.ToString(@"ss\.fff")}", font, 16) { Position = startPos + new Vector2f(size.X + 240, size.Y - 40), OutlineColor = Color.Black, OutlineThickness = 1 });
      
      window?.Draw(new RectangleShape(new Vector2f(100, 20)) { FillColor = GameState.AlgorithmPaused ? Color.Red : Color.Green, OutlineColor = Color.White, OutlineThickness = 2, Position = startPos + new Vector2f(size.X + 20, size.Y - 40) });
      window?.Draw(new Text(GameState.AlgorithmPaused ? "Paused" : "Running", font, 16) { Position = startPos + new Vector2f(size.X + 25, size.Y - 40), OutlineColor = Color.Black, OutlineThickness = 1 });
      window?.Draw(new Text("left-click on field to switch state", font, 16) { Position = startPos + new Vector2f(size.X + 20, size.Y - 13), OutlineColor = Color.Black, OutlineThickness = 2 });
      
      window?.Draw(new Text("--> shapes", font, 16) { Position = startPos + new Vector2f(0, size.Y + 15), OutlineColor = Color.Black, OutlineThickness = 2 });
      window?.Draw(new Shape(GameState.currentShape) { Scale = .6f, Position = startPos + new Vector2f(5, size.Y + 50) });
      window?.Draw(new Shape(GameState.upcomingShape) { Scale = .4f, Position = startPos + new Vector2f(200, size.Y + 50) });
      window?.Draw(new Shape(GameState.nextUpcomingShape) { Scale = .3f, Position = startPos + new Vector2f(300, size.Y + 50) });
    }

    void DrawBoard() { 
      // foreach (var m in GameState.GetPossibleMoves(GameState.state, GameState.currentShape)) { 
      //   window?.Draw(new RectangleShape(size / 9) { Position = startPos + new Vector2f((size.X / 9) * (m % 9), (size.Y / 9) * (m / 9)), FillColor = Color.Green });
      // }

      for (int y = 0; y < GameState.SIZE; y++) { 
        for (int x = 0; x < GameState.SIZE; x++) { 
          if (GameState.HasBlock(x, y))
            window?.Draw(new RectangleShape(size / 9) { Position = startPos + new Vector2f((size.X / 9) * x, (size.Y / 9) * y), FillColor = new Color(71, 58, 186) });
        }
      }

      DrawBoardLines();
    }

    void DrawBoardLines() { 
      window?.Draw(new RectangleShape(size) { Position = startPos, FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 6 });
      window?.Draw(new RectangleShape(new Vector2f(size.X / 3, size.Y)) { Position = startPos + new Vector2f(size.X / 3, 0), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 3 });
      window?.Draw(new RectangleShape(new Vector2f(size.X / 9, size.Y)) { Position = startPos + new Vector2f(size.X / 9, 0), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
      window?.Draw(new RectangleShape(new Vector2f(size.X / 9, size.Y)) { Position = startPos + new Vector2f((size.X / 9) * 4, 0), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
      window?.Draw(new RectangleShape(new Vector2f(size.X / 9, size.Y)) { Position = startPos + new Vector2f((size.X / 9) * 7, 0), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
      
      window?.Draw(new RectangleShape(new Vector2f(size.X, size.Y / 3)) { Position = startPos + new Vector2f(0, size.Y / 3), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 3 });
      window?.Draw(new RectangleShape(new Vector2f(size.X, size.Y / 9)) { Position = startPos + new Vector2f(0, (size.Y / 9)), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
      window?.Draw(new RectangleShape(new Vector2f(size.X, size.Y / 9)) { Position = startPos + new Vector2f(0, (size.Y / 9) * 4), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
      window?.Draw(new RectangleShape(new Vector2f(size.X, size.Y / 9)) { Position = startPos + new Vector2f(0, (size.Y / 9) * 7), FillColor = Color.Transparent, OutlineColor = Color.White, OutlineThickness = 1 });
    }

    // public 
  }
}