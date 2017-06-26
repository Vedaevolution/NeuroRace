using NNC.Network.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NNC.Network.Activations
{
    public class LinearActivation : Activation
    {
        public override Tensor Compute(Tensor input)
        {
            var value = (Tensor)input;
            return value;
        }

        public override Tensor ComputeGradient(Tensor rootgradients, Tensor lastoutput)
        {
            return (Tensor1D)rootgradients.Clone();
        }
    }
}
