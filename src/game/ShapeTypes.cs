namespace Game {
  public static class ShapeTypes { 
    public static int[][] DOT = new int[][] { 
        new [] { 1 },
      };

    public static int[][] SQUARE = new int[][] { 
        new [] { 1, 1 },
        new [] { 1, 1 },
      };
    
    public static int[][] HORIZONTAL_LINE_3 = new int[][] { 
        new [] { 1, 1, 1 },
      };
      
    public static int[][] HORIZONTAL_LINE_4 = new int[][] { 
        new [] { 1, 1, 1, 1 },
      };

    public static int[][] HORIZONTAL_LINE_5 = new int[][] { 
        new [] { 1, 1, 1, 1, 1 },
      };

    public static int[][] VERTICAL_LINE_3 = new int[][] { 
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
      };

    public static int[][] VERTICAL_LINE_4 = new int[][] { 
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
      };
    
    public static int[][] VERTICAL_LINE_5 = new int[][] { 
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
        new [] { 1 },
      };

    public static int[][] BIG_L_SHAPE_BL = new int[][] { 
        new [] { 1 },
        new [] { 1 },
        new [] { 1, 1, 1 },
      };

    public static int[][] BIG_L_SHAPE_TL = new int[][] { 
        new [] { 1, 1, 1 },
        new [] { 1 },
        new [] { 1 },
      };

    public static int[][] BIG_L_SHAPE_BR = new int[][] { 
        new [] { 0, 0, 1 },
        new [] { 0, 0, 1 },
        new [] { 1, 1, 1 },
      };

    public static int[][] BIG_L_SHAPE_TR = new int[][] { 
        new [] { 1, 1, 1 },
        new [] { 0, 0, 1 },
        new [] { 0, 0, 1 },
      };

    public static int[][] L_SHAPE_BL = new int[][] { 
        new [] { 1 },
        new [] { 1, 1 },
      };

    public static int[][] L_SHAPE_TL = new int[][] { 
        new [] { 1, 1 },
        new [] { 1 },
      };

    public static int[][] L_SHAPE_TR = new int[][] { 
        new [] { 1, 1 },
        new [] { 0, 1 },
      };

    public static int[][] L_SHAPE_BR = new int[][] { 
        new [] { 0, 1 },
        new [] { 1, 1 },
      };

    public static int[][] DIAG_R_2 = new int[][] { 
        new [] { 0, 1 },
        new [] { 1 },
      };
    public static int[][] DIAG_R_3 = new int[][] { 
        new [] { 0, 0, 1 },
        new [] { 0, 1 },
        new [] { 1 },
      };

    public static int[][] DIAG_L_2 = new int[][] { 
        new [] { 1 },
        new [] { 0, 1 },
      };

    public static int[][] DIAG_L_3 = new int[][] { 
        new [] { 1 },
        new [] { 0, 1 },
        new [] { 0, 0, 1 },
      };



    public static int[][][] SHAPES = new int[][][] { 
      DOT, SQUARE, HORIZONTAL_LINE_3, HORIZONTAL_LINE_4, HORIZONTAL_LINE_5,
      VERTICAL_LINE_3, VERTICAL_LINE_4, VERTICAL_LINE_5, 
      BIG_L_SHAPE_BL, BIG_L_SHAPE_BR, BIG_L_SHAPE_TL, BIG_L_SHAPE_TL, BIG_L_SHAPE_TR,
      L_SHAPE_BL, L_SHAPE_BR, L_SHAPE_TL, L_SHAPE_TR,
      DIAG_L_2, DIAG_R_2, DIAG_L_3, DIAG_R_3
    };

    public static int[][] GetRandomShape(Random r) { 
      return SHAPES[r.Next(SHAPES.Length)];
    }
  }
}