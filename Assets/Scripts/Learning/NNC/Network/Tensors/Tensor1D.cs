using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace NNC.Network.Tensors
{
    public class Tensor1D : Tensor
    {

        private float[] _Data;

        public Tensor1D(int size)
        {
            Dims = 1;
            Shape = new int[] { size };
            _Data = new float[size];
            Length = size;
        }

        public Tensor1D(float[] data) : this(data.Length)
        {
            Array.Copy(data, _Data, data.Length);
        }

        public float this[int pos]
        {
            get
            {
                return _Data[pos];
            }
            set
            {
                _Data[pos] = value;
            }
        }

        public float ElementSum()
        {
            float sum = 0;
            for (var i = 0; i < Length; i++)
            {
                sum += _Data[i];
            }
            return sum;
        }

        public void AddElement(float input)
        {
            var olddata = _Data;
            _Data = new float[olddata.Length + 1];
            _Data[olddata.Length] = input;
            Array.Copy(olddata, _Data, olddata.Length);
            Length += 1;
            Shape = new int[] { Length };
        }

        public override object Clone()
        {
            var data = (float[])_Data.Clone();
            var nobj = new Tensor1D(data);
            return nobj;
        }

        public static Tensor1D operator *(Tensor1D t1, Tensor1D t2)
        {
            if (!t1.EqualShape(t2)) throw new ArgumentException();

            var size = t1.Length;

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = t1[i] * t2[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator *(float f, Tensor1D t)
        {

            var size = t.Length;

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = f * t[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator /(Tensor1D t1, Tensor1D t2)
        {
            if (!t1.EqualShape(t2)) throw new ArgumentException();

            var size = t1.Shape[0];

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = t1[i] / t2[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator /(float f, Tensor1D t)
        {

            var size = t.Shape[0];

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = f / t[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator +(Tensor1D t1, Tensor1D t2)
        {
            if (!t1.EqualShape(t2)) throw new ArgumentException();

            var size = t1.Length;

            var resulttensor = new Tensor1D(size);

            for(var i = 0; i < size; i++)
            {
                resulttensor[i] = t1[i] + t2[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator -(Tensor1D t1, Tensor1D t2)
        {
            if (!t1.EqualShape(t2)) throw new ArgumentException();

            var size = t1.Length;

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = t1[i] - t2[i];
            }
            return resulttensor;
        }


        public static Tensor1D operator +(float f, Tensor1D t)
        {

            var size = t.Length;

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = f + t[i];
            }
            return resulttensor;
        }

        public static Tensor1D operator -(float f, Tensor1D t)
        {

            var size = t.Length;

            var resulttensor = new Tensor1D(size);

            for (var i = 0; i < size; i++)
            {
                resulttensor[i] = f - t[i];
            }
            return resulttensor;
        }

        public void ZeroDistance(float distance)
        {
            for( var i = 0; i < _Data.Length; i++)
            {
                _Data[i] = _Data[i] < 0 ? Math.Min(_Data[i], distance) : Math.Max(_Data[i], distance);    
            }
        }



    }
}
