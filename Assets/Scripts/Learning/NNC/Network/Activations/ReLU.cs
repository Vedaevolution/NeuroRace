using NNC.Network.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NNC.Network.Activations
{
    public class ReLU : Activation
    {
        public override Tensor Compute(Tensor input)
        {
            var value = (Tensor1D)input;
            for(var i = 0; i < value.Length; i++)
            {
                value[i] = value[i] > 0 ? value[i] : 0;
            }

            return value;
        }

        public override Tensor ComputeGradient(Tensor rootgradients, Tensor lastoutput)
        {
            var rg = (Tensor1D)rootgradients.Clone();
            var la = (Tensor1D) lastoutput;
            for (var i = 0; i < rg.Length; i++)
            {
                rg[i] = la[i] > 0 ? rg[i] : 0;
            }

            return rg;
        }
    }
}
