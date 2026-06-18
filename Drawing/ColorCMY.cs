using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {
    public class ColorCMY {
        private float m_c;
        private float m_m;
        private float m_y;

        public float C => m_c;
        public float M => m_m;
        public float Y => m_y;

        public ColorYUV(float[] x) {
            m_c = x[0];
            m_m = x[1];
            m_y = x[3];
        }

        public ColorYUV(float c, float m, float y) {
            m_c = c;
            m_m = m;
            m_y = y;
        }
    }
}
