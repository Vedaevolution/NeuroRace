using System;
using System.Collections.Generic;
using System.Linq;


namespace NNC.Network
{
    public abstract class Tensor : ICloneable
    {

        public int Length { get; protected set; }

        public int Dims { get; protected set; }

        public int[] Shape { get; protected set; }

        public abstract object Clone();
 
    
        public virtual bool EqualShape(Tensor othertensor)
        {
            if (Dims != othertensor.Dims) return false;

            for(var i = 0; i < Dims; i++)
            {
                if (Shape[i] != othertensor.Shape[i]) return false;
            }
            return true;
        }

    }
}
