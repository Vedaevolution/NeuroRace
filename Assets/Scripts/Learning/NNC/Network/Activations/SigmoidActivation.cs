using System;


using NNC.Network;
using NNC.Network.Tensors;

namespace NNC.Network.Activations
{
    public class SigmoidActivation : Activation
    {
        public override Tensor Compute(Tensor input)
        {
            var values = (Tensor1D)input;
            var results = new Tensor1D(values.Length);
            for(var i = 0; i < values.Length; i++)
            {
                results[i] = 1f / (1 + (float)Math.Exp(-values[i]));
            }
            return results;
        }

        public override Tensor ComputeGradient(Tensor rootgradients, Tensor lastoutput)
        {
            var rg = (Tensor1D)rootgradients;
            var la = (Tensor1D)lastoutput;
            var res = rg * la * (1 - la);
            //res.ZeroDistance(0.001f);
            return res;
        }
    }
}
