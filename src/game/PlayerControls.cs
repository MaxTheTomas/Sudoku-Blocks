namespace Game {
  using SFML.Window;
  using SFML.System;
  using SFML.Graphics;
  public static class PlayerControls { 
    public static Vector2f MousePosition { get; private set; }

    static int _selected_shape = 0;
    public static int[][] SelectedShape => ShapeTypes.SHAPES[_selected_shape];

    static bool isPressed = false;
    public static void Initialize() { 
      if (WindowRenderer.Window == null) 
        throw new NullReferenceException("Trying to PlayerControls.Initialize without active window.");  
      
      WindowRenderer.Window.MouseMoved += MouseMoved;
      WindowRenderer.Window.MouseButtonPressed += MousePressed;
      WindowRenderer.Window.MouseButtonReleased += MouseReleased;
      WindowRenderer.Window.KeyPressed += KeyPressed;
    }

    static void MouseMoved(object? a, MouseMoveEventArgs args) { 
      MousePosition = new Vector2f(args.X, args.Y);
    }

    static void MousePressed(object? a, MouseButtonEventArgs args) { 
      var f = new FloatRect(WindowRenderer.startPos, WindowRenderer.size);
      if (!f.Contains(args.X, args.Y)) return;

      var pos = new Vector2f(args.X, args.Y);
      pos -= WindowRenderer.startPos;
      var coords = (Vector2i) (pos / (WindowRenderer.size.X / 9));

      if (args.Button == Mouse.Button.Left) {  
        GameState.SetBlock(coords.X, coords.Y, !GameState.HasBlock(coords.X, coords.Y));
      }
      if (args.Button == Mouse.Button.Right) {  
        GameState.PlaceShapeForcefully(coords.X, coords.Y, SelectedShape);
      }
    }

    static void MouseReleased(object? a, MouseButtonEventArgs args) { }
    static void KeyPressed(object? a, KeyEventArgs args) { 

      // todo pause
      switch (args.Code) { 
        case Keyboard.Key.Right:
          NextShape(); 
          break;
          
        case Keyboard.Key.Left: 
          PreviousShape();
          break;
          
        case Keyboard.Key.R: 
          GameState.Reset();
          break;

        case Keyboard.Key.E: 
          GameState.DoAIStep();
          break;
          
        case Keyboard.Key.Space: 
          GameState.AlgorithmPaused = !GameState.AlgorithmPaused;
          break;
      }
    }

    public static void NextShape() { _selected_shape = (_selected_shape + 1) % ShapeTypes.SHAPES.Length; }
    public static void PreviousShape() { _selected_shape--; if (_selected_shape < 0) _selected_shape = ShapeTypes.SHAPES.Length - 1; }
  }
}