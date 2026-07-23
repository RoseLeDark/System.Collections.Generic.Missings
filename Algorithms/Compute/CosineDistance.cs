
using SystemEx.Algorithms.Compute.Interfaces;
using SystemEx.Numeric;

namespace SystemEx.Algorithms.Compute {

    public sealed class CosineDistanceF : ICDistance {
        public float Compute ( ICVector a, ICVector b ) {
            float dot = 0f, magA = 0f, magB = 0f;

            for ( int i = 0 ; i < a.Dimension ; i++ ) {
                float x = a[i];
                float y = b[i];

                dot += x * y;
                magA += x * x;
                magB += y * y;
            }

            return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
        }
    }

    #if TEST
    public sealed class CosineDistanceH16 : ICDistance {
        public Half16 Compute ( ICVector a, ICVector b ) {
            Half16 dot = 0, magA = 0, magB = 0;

            for ( int i = 0 ; i < a.Dimension ; i++ ) {
                float x = a[i];
                float y = b[i];

                dot += x * y;
                magA += x * x;
                magB += y * y;
            }

            return dot / (MathF.Sqrt(magA) * MathF.Sqrt(magB));
        }
    }
    #endif
}
