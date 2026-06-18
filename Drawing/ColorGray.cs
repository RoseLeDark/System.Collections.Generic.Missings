using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Drawing {
    public class ColorGray {
        private float m_v;

        public float Gray { get { return m_v; } }

        public ColorGray(float v) { m_v = v; }
    }
}}
