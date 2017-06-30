using NNC.Network.Tensors;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transition{

    public Tensor1D InputA;
    public Tensor1D InputB;

    public Action Action;

    public float Reward;

    public Tensor1D OutputA;
    public Tensor1D OutputB;

}
