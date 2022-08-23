namespace Game {
  using SFML.Graphics;
  using SFML.System;
  public class Shape : Drawable { 
    public bool IsCorrect = true;
    public Vector2f Position;
    public float Scale = 1;
    
    int[][] shape;
    Vector2f[] rectanglesAt;
    public int sizeX { get; private set; }
    public int sizeY  => shape.Length;

    public Shape(int[][] shape) { 
      this.shape = shape;

      var size = WindowRenderer.size / 9;
      int longest = 0;

      List<Vector2f> vectors = new List<Vector2f>(); 
      for (int y = 0; y < shape.Length; y++) {
        if (shape[y].Length > longest) longest = shape[y].Length;

        for (int x = 0; x < shape[y].Length; x++) {
          if (shape[y][x] == 1) { 
            var pos = new Vector2f(size.X * x, size.Y * y);
            vectors.Add(pos);
          }
        }
      }

      sizeX = longest;
      rectanglesAt = vectors.ToArray();
    }

    public void Draw(RenderTarget t, RenderStates s) { 
      var size = WindowRenderer.size / 9;
      foreach (var p in rectanglesAt) { 
        if (IsCorrect)
          t.Draw(new RectangleShape((size - new Vector2f(6, 6)) * Scale) { Position = Position + (p * Scale), FillColor = new Color(58, 163, 186), OutlineColor = new Color(148, 201, 255), OutlineThickness = 2 });
        else 
          t.Draw(new RectangleShape((size - new Vector2f(6, 6)) * Scale) { Position = Position + (p * Scale), FillColor = new Color(224, 40, 40), OutlineColor = new Color(250, 100, 100), OutlineThickness = 2 });
      }
    }
  }
}