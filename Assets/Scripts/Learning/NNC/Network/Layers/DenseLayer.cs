using System;
using System.Collections.Generic;
using System.Linq;


using NNC.Network.Tensors;

namespace NNC.Network.Layers
{
    public class DenseLayer : Layer
    {

        private List<Tensor1D> Weights;

        public int Neurons { get; private set; }




        public DenseLayer(int neurons,int inputnumber,Activation activation)
        {
            InputLayer = new List<Layer>();
            OutputLayer = new List<Layer>();

            LayerType = LayerType.Input;
            Neurons = neurons;
            InputNumber = inputnumber;
            OutputNumber = Neurons;
            Activation = activation;
        }

        public DenseLayer(Layer inputlayer, Activation activation, int neurons, LayerType type)
        {
            InputLayer = new List<Layer>();
            OutputLayer = new List<Layer>();

            InputLayer.Add(inputlayer);
            inputlayer.OutputLayer.Add(this);
            LayerType = type;
            Neurons = neurons;
            Activation = activation;
            InputNumber = inputlayer.OutputNumber;
            OutputNumber = Neurons;
        }

        public override void Backward(Tensor rootgradients)
        {
            var state = (Tuple<Tensor1D, Tensor1D>)State;

            var gradient = (Tensor1D)Activation.ComputeGradient(rootgradients, state.Item2);

            var propgrad = PropagateGradient((Tensor1D)rootgradients);

            for (var n = 0; n < Neurons; n++)
            {
                var input = state.Item1;

                Weights[n] = Weights[n] - 0.01f * gradient[n] * input;
            }

            if (LayerType != LayerType.Input)
                InputLayer[0].Backward(propgrad);
            else
            {
                var i = 2;
            }
        }

        private Tensor1D PropagateGradient(Tensor1D gradient)
        {
            var propgrad = new float[InputNumber + 1];

            for (var i = 0; i < InputNumber + 1; i++) {
                for (var n = 0; n < Neurons; n++)
                {
                    propgrad[i] += Weights[n][i] * gradient[n];
                }
            }
            return new Tensor1D(propgrad);
        }

        public override Tensor Forward(Tensor inputs)
        {
            if (InputLayer.Count > 1) throw new ArgumentException("Dense layer only takes one input!");
            if (InputLayer.Count < 1 && LayerType != LayerType.Input) throw new ArgumentException("No input provided to the dense layer !");

            if (!(inputs is Tensor1D)) throw new ArgumentException("Input must be a Tensor1D!");

            inputs = (Tensor)inputs.Clone();
            if(LayerType == LayerType.Input)
            {
                var outputvalues = ComputeOutput(inputs);
                State = new Tuple<Tensor1D, Tensor1D>((Tensor1D) inputs, (Tensor1D)outputvalues);
                return outputvalues;
            }

            var finputs = InputLayer[0].Forward(inputs);

            var output = ComputeOutput(finputs);

            State = new Tuple<Tensor1D, Tensor1D>((Tensor1D)finputs, (Tensor1D)output);

            return output;

        }

        private Tensor ComputeOutput(Tensor inputs)
        {
            var input = (Tensor1D)inputs;
            input.AddElement(1);
            var scalarprodukttensor = new Tensor1D(Neurons);
            for (var i = 0; i < Neurons; i++)
            {
                var product = Weights[i] * input;
                var productsum = product.ElementSum();
                scalarprodukttensor[i] = productsum;
            }

            var outputvalues = Activation.Compute(scalarprodukttensor);
            return outputvalues;
        }

        public override void Initilize()
        {
            var rnd = new Random();

            Weights = new List<Tensor1D>();

            for(var i = 0; i < Neurons; i++)
            {
                float[] initvalues = Enumerable
                .Repeat(0, InputNumber + 1)
                .Select(r => (float)rnd.NextDouble() - 0.5f)
                .ToArray();

                Weights.Add(new Tensor1D(initvalues));
                
            }

            foreach(var layer in InputLayer)
            {
                layer.Initilize();
            }
        }
    }
}
