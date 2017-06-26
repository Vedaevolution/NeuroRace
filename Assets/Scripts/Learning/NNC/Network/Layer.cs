using System;
using System.Collections.Generic;
using System.Linq;


namespace NNC.Network
{
    public abstract class Layer
    {

        public int InputNumber { get; protected set; }

        public int OutputNumber { get; protected set; }

        public List<Layer> InputLayer { get; protected set; }

        public List<Layer> OutputLayer { get; protected set; }

        public Activation Activation { get; protected set; }

        public LayerType LayerType { get; protected set; }

        public Object State { get; set; }

        public abstract void Initilize();

        public abstract Tensor Forward(Tensor inputs);

        public abstract void Backward(Tensor rootgradients);
    }
}
