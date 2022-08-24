/* 
  Obsolete - Moving to AI 
  bruh
 */


// namespace AI {
//   using Game;

//   public static class Algorithm { 
//     public static TimeSpan LastMoveTime;

//     public static int GetNextMove() {
//       DateTime start = DateTime.Now;

//       int bestMovePrice = -1;
//       int bestMove = -1;
      
//       var m = GetMovesAndPrices(GameState.state, GameState.currentShape, out var p);
      
//       int bestMovesetCount = 0;
//       foreach (var i in m) { 
//         var price = p;
//         var fieldWithCurShape = GameState.GetFieldWithShape(i % 9, i / 9, GameState.currentShape);
//         var upcomingMoveset = GetMovesAndPrices(fieldWithCurShape, GameState.upcomingShape, out var p2);
        
//         if (upcomingMoveset.Length > bestMovesetCount) bestMovesetCount = upcomingMoveset.Length;
//         if (upcomingMoveset.Length == 0) p2 = -2000;

//         price += p2;

//         int bestPrice3 = 0;
//         int bestmoveset3 = 0;
//         foreach (var y in upcomingMoveset) { 
//           var fieldWithUpcoming = GameState.GetFieldWithShape(fieldWithCurShape, y % 9, y / 9, GameState.upcomingShape);
//           var m2 = GetMovesAndPrices(fieldWithCurShape, GameState.nextUpcomingShape, out var p3);
          
//           if (m2.Length == 0) p3 = -1000;
//           if (m2.Length > bestmoveset3) bestmoveset3 = m2.Length;

//           // if (p3 != 0) Console.WriteLine(p3);

//           if (p3 > bestPrice3) { 
//             bestPrice3 = p3 - (bestmoveset3 - m2.Length);
//           }
//         }

//         price += bestPrice3;

//         // if (price != 0)
//         //   Console.WriteLine(price);

//         if (price > bestMovePrice) { 
//           bestMove = i;
//           bestMovePrice = price - (bestMovesetCount - upcomingMoveset.Length);
//         }
//       }

//       LastMoveTime = DateTime.Now - start;
//       // Console.WriteLine($"price: { bestMovePrice } / time: {moveTime.TotalMilliseconds}");
      
//       if (bestMovePrice == -1) return -1;
//       return bestMove;
//     }



//     static int[] GetMovesAndPrices(int[] field, int[][] shape, out int price) { 
//       int bestPrice = -200;
//       List<int> moves = new List<int>();

//       foreach (var i in GameState.GetPossibleMoves(GameState.state, GameState.currentShape)) {
//         var fieldWithShape = GameState.GetFieldWithShape(i % GameState.SIZE, i / GameState.SIZE, 
//                                 GameState.currentShape);

//         var price_ = GameState.GetDestroyFor(fieldWithShape, out _);
//         // if (price_ != 0) Console.WriteLine(price_);

//         if (price_ > bestPrice) { moves.Clear(); moves.Add(i); bestPrice = price_; }
//         if (price_ == bestPrice) { moves.Add(i); }
//       }

//       price = bestPrice;
//       return moves.ToArray();
//     }
//   }
// }