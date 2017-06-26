using Assets.Scripts.Learning.NNC.Network.Activations;
using NNC.Network.Activations;


namespace NNC.Network
{
    public abstract class Activation
    {
        public abstract Tensor Compute(Tensor input);

        public abstract Tensor ComputeGradient(Tensor rootgradients, Tensor lastoutput);

        public static SigmoidActivation Sigmoid()
        {
            return new SigmoidActivation();
        }

        public static LinearActivation Linear()
        {
            return new LinearActivation();
        }

        public static TangensHyperbolic TangesHyperbolic()
        {
            return new TangensHyperbolic();
        }
    }
}
