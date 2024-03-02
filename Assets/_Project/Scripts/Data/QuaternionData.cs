using System;

namespace Data
{
    [Serializable]
    public class QuaternionData
    {
        public float X;
        public float Y;
        public float Z;
        public float W;

        public QuaternionData()
        {
            X = 0;
            Y = 0;
            Z = 0;
            W = 0;
        }
        public QuaternionData(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }
    }
}