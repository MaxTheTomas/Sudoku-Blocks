namespace AI {
  using System.Text.Json.Nodes;
  public class NeuralNetwork { 
    Layer[] layers;
    int[] layerSizes;
    public NeuralNetwork(params int[] layerSizes) { 
      this.layerSizes = layerSizes;
      layers = new Layer[layerSizes.Length - 1];

      // Todo review
      for (int i = 0; i < layerSizes.Length - 1; i++) {
        layers[i] = new Layer(layerSizes[i], layerSizes[i + 1]);
      }
    }

    public float[] CalculateOutputs(float[] inputs) { 
      if (inputs.Length != layerSizes[0]) { Console.Error.WriteLine("!!! Number of inputs != layerSizes[0]"); } 
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

    public static NeuralNetwork LoadFromJSON(string json_string) { 
      var json = JsonObject.Parse(json_string);
      if (json == null) throw new NullReferenceException("Bad json");

      var sizes = json["layer_sizes"]?.AsArray();
      int[] szs = new int[sizes.Count];
      for (int s = 0; s < sizes.Count; s++) { szs[s] = (int) sizes[s]; }
      var neural = new NeuralNetwork(layerSizes: szs);

      var layers = json["layers"].AsArray();
      for (int l = 0; l < layers.Count; l++) {
        var layer = layers[l].AsObject();

        var @in = (int) layer["in"];
        var @out = (int) layer["out"];

        var @weightsO = layer["weights"].AsObject();
        var @biasesO = layer["biases"].AsArray();

        float[,] weights = new float[@in, @out];
        float[] biases = new float[@out];

        for (int b = 0; b < biasesO.Count; b++) {
          biases[b] = (float) biasesO[b];
        }

        for (int wIn = 0; wIn < @in; wIn++) {
          for (int wOut = 0; wOut < @out; wOut++) {
            weights[wIn, wOut] = 
              (float) weightsO[wIn.ToString()] .AsArray()[wOut];
          }
        }
      }

      return neural;
    } 

    public NeuralNetwork Clone() { 
      var n = new NeuralNetwork(layerSizes: layerSizes);
      
      for (int lIndex = 0; lIndex < n.layers.Length; lIndex++) {
        layers[lIndex].biases.CopyTo(n.layers[lIndex].biases, 0);
        for (int @in = 0; @in < layers[lIndex].neuronsIn; @in++) { 
          for (int @out = 0; @out < layers[lIndex].neuronsOut; @out++) {
            n.layers[lIndex].weights[@in, @out] = layers[lIndex].weights[@in, @out];
          }
        }
      }

      return n;
    }

    public void RandomizeAll(float weights = .1f, float biases = .1f, Random? r = null) { 
      foreach (var l in layers) 
        { l.Randomize(weights, biases, r); }
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
      return 1 / (1 + MathF.Exp(-w));
      // return MathF.Max(0, w);
      // return w; 
    } 

    public void Randomize(float magnitudeWeights = .01f, float magnitudeBiases = .01f, Random? r = null) { 
      if (r == null) r = new Random();

      for (var i = 0; i < neuronsIn; i++) {
        for (var o = 0; o < neuronsOut; o++) {
          weights[i, o] += (r.NextSingle() - .5f) * magnitudeWeights; 
        }
      }

      for (var o = 0; o < neuronsOut; o++) {
        biases[o] += (r.NextSingle() - .5f) * magnitudeBiases; 
      }
    }
  } 
}