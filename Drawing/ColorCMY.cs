using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {
    /// <summary>
    /// 
    /// </summary>
    public class ColorCMY {
        private float m_c;
        private float m_m;
        private float m_y;
        /// <summary>
        /// 
        /// </summary>
        public float C => m_c;
        /// <summary>
        /// 
        /// </summary>
        public float M => m_m;
        /// <summary>
        /// 
        /// </summary>
        public float Y => m_y;
        /// <summary>
        /// 
        /// </summary>
        public ColorCMY(float[] x) {
            m_c = x[0];
            m_m = x[1];
            m_y = x[3];
        }
        /// <summary>
        /// 
        /// </summary>
        public ColorCMY(float c, float m, float y) {
            m_c = c;
            m_m = m;
            m_y = y;
        }
    }
}
