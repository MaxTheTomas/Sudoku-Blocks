namespace AI {
  using System.Text.Json.Nodes;
  public class NeuralNetwork { 
    Layer[] layers;
    int[] layerSizes;
    public NeuralNetwork(params int[] layerSizes) { 
      this.layerSizes = layerSizes;
      layers = new Layer[layerSizes.Length - 1];
      for (int i = 0; i < layerSizes.Length-1; i++) {
        layers[i] = new Layer(layerSizes[i], layerSizes[i + 1]);
      }
    }

    float[] CalculateOutputs(float[] inputs) { 
      foreach (var l in layers) 
        inputs = l.CalculateOutputs(inputs);

      return inputs;
    }
    
    /* 
      <summary>Returns index of largest value</summary>
     */
    int Classify(float[] inputs) { 
      float[] outputs = CalculateOutputs(inputs);
      int max_i = 0; float max_v = 0;
      for (int i = 0; i < outputs.Length; i++) {
        if (outputs[i] > max_v) { 
          max_v = outputs[i];
          max_i = i;
        }
      }
      return max_i;
    } 


    public string GetAsJSON() { 
      List<JsonObject> l = new List<JsonObject>();

      foreach (var lr in layers) { 
        var weights = new JsonObject() { };

        for (var i = 0; i < lr.neuronsIn; i++) {
          var jsonArray = new JsonArray();
          for (var o = 0; o < lr.neuronsOut; o++) {
            jsonArray.Add(lr.weights[i, o]);
          }
          weights[i.ToString()] = jsonArray;
        }

        var biases = new JsonArray();
        foreach(var b in lr.biases) biases.Add(b);

        l.Add(new JsonObject() { 
          ["in"] = lr.neuronsIn,
          ["out"] = lr.neuronsOut,
          ["weights"] = weights,
          ["biases"] = biases,
        });
      }
      
      var ls = new JsonArray();
      foreach(var s in layerSizes) ls.Add(s);
        
      var j = new JsonObject { 
        ["layer_sizes"] = ls,
        ["layer_count"] = layers.Length,
        ["layers"] = new JsonArray(l.ToArray())
      };

      return j.ToJsonString();
      // new JsonDocument(new JsonDocumentOptions() { })
    }

    public static NeuralNetwork LoadFromJSON(string json) { 
      throw new Exception();
    } 
  }

  public class Layer { 
    public int neuronsIn { get; private set; }
    public int neuronsOut { get; private set; }
    public float[,] weights;
    public float[] biases;

    public Layer(int nIn, int nOut) { 
      this.neuronsIn = nIn;
      this.neuronsOut = nOut;

      weights = new float[nIn, nOut];
      biases = new float[nOut];
    }
    
    public float[] CalculateOutputs(float[] inputs) { 
      float[] activations = new float[neuronsOut];

      for (int n = 0; n < neuronsOut; n++) {
        float input = biases[n];
        for (int i = 0; i < neuronsIn; i++) {
          input += inputs[i] * weights[i, n];
        }
        activations[n] = ActivationFunction(input);
      }

      return activations;
    }

    float ActivationFunction(float w) { 
      // return 1 / (1 + MathF.Exp(-w));
      // return MathF.Max(0, w);
      return w; 
    } 
  } 
}