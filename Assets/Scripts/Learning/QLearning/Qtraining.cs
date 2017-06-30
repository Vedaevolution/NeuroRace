using NNC.Network;
using NNC.Network.Tensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Learning.QLearning
{
    public class QTraining
    {
        private Layer _Network;
        private Random _Rnd;

        public QTraining()
        {
            _Rnd = new Random();
        }

        public void ComputeErrorTensor(Transition tr, Layer Network)
        {
            var outputblist = ((Tensor1D)Network.Forward(tr.InputB)).Tolist();
            var maxrewardindexnextstep = outputblist.IndexOf(outputblist.Max());

   


            var correctreward = tr.Reward + 0.9f * maxrewardindexnextstep;

            var errorarray = new float[tr.OutputA.Length];

            var errortensor = new Tensor1D(errorarray);

            var output = (Tensor1D)Network.Forward(tr.InputA);
            

            var index = tr.Action.SteeringCount * tr.Action.SteeringNumber + tr.Action.TourqueNumber;

            errortensor[index] = output[index] - correctreward;

            Network.Backward(errortensor);
        }

        public void TrainAction(List<Transition> transitions, Layer Network)
        {
            foreach(var trans in transitions)
            {
                ComputeErrorTensor(trans, Network);
            }


        }
    }
}
