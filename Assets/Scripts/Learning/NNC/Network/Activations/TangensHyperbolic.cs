using NNC.Network;
using NNC.Network.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Learning.NNC.Network.Activations
{
    public class TangensHyperbolic : Activation
    {
        public override Tensor Compute(Tensor input)
        {
            var values = (Tensor1D)input;
            var results = new Tensor1D(values.Length);
            for (var i = 0; i < values.Length; i++)
            {
                results[i] = 2f / (1 + (float)Math.Exp(-2 * values[i])) - 1;
            }
            return results;
        }

        public override Tensor ComputeGradient(Tensor rootgradients, Tensor lastoutput)
        {
            var rg = (Tensor1D)rootgradients;
            var la = (Tensor1D)lastoutput;
            var res = rg * (1 - la * la);
            return res;
        }
    }
}
