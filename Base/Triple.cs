

using System.Diagnostics.CodeAnalysis;

namespace SystemEx {

    public enum triple : sbyte {
        True = 1,
        False = 0,
        Nin = -1
    }

    public readonly struct Triple :
          IEquatable<triple> {

        private readonly triple m_value; 


        internal const triple True = triple.True;

        internal const triple False = triple.False;

        internal const triple Nin = triple.Nin;

  
        public static readonly string TrueString = "True";
        public static readonly string FalseString = "False";
        public static readonly string NinString = "Nin";

        public override int GetHashCode () {
            if ( m_value == True ) return 1;
            if ( m_value == False ) return 0;
            return -1;
        }

        public override string ToString () {
            string _ret = NinString;

            if ( m_value == triple.False) {
                _ret = FalseString;
            } else if ( m_value == triple.True )
                _ret = TrueString;

            return _ret;
        }
        public Triple() {
            m_value = triple.Nin;
        }
        public Triple(bool v) {
            m_value = v ? triple.True : triple.False;
        }
        public Triple ( triple v ) {
            m_value = v;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals ( [NotNullWhen(true)] object? obj ) {
            bool _ret = false;

            if( (obj is Boolean) )       _ret = ((Boolean)obj == this.ToBoolean());
            else if ( (obj is Triple) )  _ret = m_value == ((Triple)obj).m_value;

            return _ret;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals ( bool obj ) {
            return ToBoolean() == obj;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean () {
            return m_value == triple.True;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool Equals ( triple obj ) {
            return m_value == obj;
        }

        public static bool operator == ( Triple left, Triple right ) {
            return left.Equals(right);
        }
        public static bool operator == ( Triple left, bool right ) {
            return left.Equals(right);
        }
        public static bool operator == ( bool left, Triple right ) {
            return right.Equals(left);
        }

        public static bool operator != ( Triple left, Triple right ) {
            return !(left.Equals(right));
        }
        public static bool operator != ( Triple left, bool right ) {
            return !(left.Equals(right));
        }
        public static bool operator != ( bool left, Triple right ) {
            return !(right.Equals(left));
        }
    }
}
