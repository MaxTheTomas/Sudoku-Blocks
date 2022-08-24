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

    int _current_game = 0;

    public void Start(bool threaded = true) { 
      if (threaded) { new Thread(() => Render()).Start(); }
      else Render();
    }

    bool draw_board = true;

    void Render() { 
      window = new RenderWindow(new VideoMode(1280, 720), "Sudoku block game idk");
      window.Closed += (a, b) => { window.Close(); };
      window.Resized += (a, b) => { window.SetView(new View(new FloatRect(0, 0, b.Width, b.Height))); };
      window.KeyPressed += (a, b) => { 
        switch (b.Code) { 
          case Keyboard.Key.Escape: 
            window.Close();
            break;
            
          case Keyboard.Key.R: 
            GameManager.ResetAll();
            break;
            
          case Keyboard.Key.H: 
            // GameManager.ResetAll();
            draw_board = !draw_board;
            break;

          case Keyboard.Key.S: 
            GameManager.StartAll();
            break;

          case Keyboard.Key.Space: 
            GameManager.Learning = !GameManager.Learning;
            break;

          case Keyboard.Key.E: 
            GameManager.TickAll();
            break;

          case Keyboard.Key.M: 
            GameManager.MutateAll();
            break;

          case Keyboard.Key.I: 
            GameManager.Learning = false;
            while (GameManager.has_running_games()) {  }
            var json = GameManager.Games[0].GetAsJSON();
            File.WriteAllText("best_network_report.json", json);
            break;

          case Keyboard.Key.Left:
            _current_game--;
            if (_current_game < 0) _current_game = GameManager.Games.Count - 1;
            break;

          case Keyboard.Key.Right:
            if (GameManager.Games.Count == 0) break;

            _current_game = (_current_game + 1) % GameManager.Games.Count;
            break;

          case Keyboard.Key.Up:
            GameManager.LearningRate += (0.00001m) * (b.Control ? 10 : 1) * (b.Shift ? 100 : 1);
            break;

          case Keyboard.Key.Down:
            GameManager.LearningRate -= 0.00001m * (b.Control ? 10 : 1) * (b.Shift ? 100 : 1);
            break;
        }
      };

      // PlayerControls.Initialize();

      while (window.IsOpen) { 
        window.DispatchEvents();
        window.Clear();
        
        try { 
          if (draw_board)
            DrawBoard();
          DrawUIs();
          window.Display();
        } catch (Exception e) { }
      }

      Environment.Exit(Environment.ExitCode);
    }

    void DrawUIs() { 
      window?.Draw(new Text("weird sudoku-like game ai", font, 32) { Position = startPos + new Vector2f(size.X + 20, -10) });
      window?.Draw(new RectangleShape(new Vector2f(130, 25)) { Position = startPos + new Vector2f(size.X + 20, 40), FillColor = GameManager.Learning ? Color.Green : Color.Red, OutlineColor = Color.White, OutlineThickness = 2 });
      window?.Draw(new Text("Learning status", font, 15) { Position = startPos + new Vector2f(size.X + 25, 43), OutlineColor = Color.Black, OutlineThickness = 1 });

      
      window?.Draw(new Text($"iter-s:", font, 16) { Position = startPos + new Vector2f(size.X + 20, 87) });
      window?.Draw(new Text($"{GameManager.Iterations}", font, 24) { Position = startPos + new Vector2f(size.X + 90, 80) });
      
      window?.Draw(new Text($"rate:", font, 16) { Position = startPos + new Vector2f(size.X + 20, 117) });
      window?.Draw(new Text($"{GameManager.LearningRate}", font, 24) { Position = startPos + new Vector2f(size.X + 90, 110) });
      
      window?.Draw(new Text($"moves:", font, 16) { Position = startPos + new Vector2f(size.X + 20, 147) });
      window?.Draw(new Text($"{GameManager.BestMoves} / {GameManager.WorstMoves_G}-{GameManager.BestMoves_G}", font, 24) { Position = startPos + new Vector2f(size.X + 90, 140) });
      
      window?.Draw(new Text($"score:", font, 16) { Position = startPos + new Vector2f(size.X + 20, 177) });
      window?.Draw(new Text($"{GameManager.BestScore / 100} / {GameManager.WorstScore_G / 100}-{GameManager.BestScore_G / 100}", font, 24) { Position = startPos + new Vector2f(size.X + 90, 170) });
      
      
      var end_x = window.Size.X;

      window?.Draw(new Text($"N", font, 16) { Position = startPos + new Vector2f(end_x - 250, 30) });
      window?.Draw(new Text($"M", font, 16) { Position = startPos + new Vector2f(end_x - 190, 30) });
      window?.Draw(new Text($"S", font, 16) { Position = startPos + new Vector2f(end_x - 100, 30) });
      
      var g = GameManager.Games.ToList();
      g.Sort((a, b) => b.SuccessfulMoves - a.SuccessfulMoves);

      for (int i = 0; i < g.Count; i++) {
        var game = g[i];
        var index = GameManager.Games.IndexOf(game);

        window?.Draw(new Text($"{index}", font, 16) { Position = startPos + new Vector2f(end_x - 250, 50 + 20*i),
          FillColor = game.Finished ? Color.Red : Color.Green
        });
        window?.Draw(new Text($"{game.SuccessfulMoves} /{game.UnsuccessfulMoveTries}", font, 16) { Position = startPos + new Vector2f(end_x - 190, 50 + 20*i) });
        window?.Draw(new Text($"{game.Score / 100}", font, 16) { Position = startPos + new Vector2f(end_x - 100, 50 + 20*i) }); 
      }

      window?.Draw(new Text($"Inspecting: {_current_game}", font, 24) { Position = startPos + new Vector2f(size.X + 20, size.Y - 80), OutlineColor = Color.Black, OutlineThickness = 1 });

      if (_current_game >= GameManager.Games.Count) return;
      window?.Draw(new Text($"Currently running: {GameManager.GetRunningGames()} / {GameManager.Games.Count}", font, 14) { Position = startPos + new Vector2f(size.X + 20, size.Y - 50), OutlineColor = Color.Black, OutlineThickness = 1 });
      window?.Draw(new Text($"Score {GameManager.Games[_current_game].Score / 100}", font, 16) { Position = startPos + new Vector2f(size.X + 105, size.Y - 20), OutlineColor = Color.Black, OutlineThickness = 1 });
      window?.Draw(new Text($"Move {GameManager.Games[_current_game].SuccessfulMoves}", font, 16) { Position = startPos + new Vector2f(size.X + 20, size.Y - 20), OutlineColor = Color.Black, OutlineThickness = 1 }); 
      window?.Draw(new Text($"Bad moves {GameManager.Games[_current_game].UnsuccessfulMoveTries}", font, 16) { Position = startPos + new Vector2f(size.X + 20, size.Y), OutlineColor = Color.Black, OutlineThickness = 1 }); 
    }

    void DrawBoard() { 
      if (_current_game >= GameManager.Games.Count) return;
      // foreach (var m in GameManager.Games[_current_game].state.GetPossibleMoves(GameManager.Games[_current_game].state.state, GameManager.Games[_current_game].state.currentShape)) { 
      //   window?.Draw(new RectangleShape(size / 9) { Position = startPos + new Vector2f((size.X / 9) * (m % 9), (size.Y / 9) * (m / 9)), FillColor = Color.Green });
      // }

      window?.Draw(new Text("--> shapes", font, 16) { Position = startPos + new Vector2f(0, size.Y + 15), OutlineColor = Color.Black, OutlineThickness = 2 });
      window?.Draw(new Shape(GameManager.Games[_current_game].state.currentShape) { Scale = .6f, Position = startPos + new Vector2f(5, size.Y + 50) });
      window?.Draw(new Shape(GameManager.Games[_current_game].state.upcomingShape) { Scale = .4f, Position = startPos + new Vector2f(200, size.Y + 50) });
      window?.Draw(new Shape(GameManager.Games[_current_game].state.nextUpcomingShape) { Scale = .3f, Position = startPos + new Vector2f(300, size.Y + 50) });


      for (int y = 0; y < GameState.SIZE; y++) { 
        for (int x = 0; x < GameState.SIZE; x++) { 
          if (GameManager.Games[_current_game].state.HasBlock(x, y))
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