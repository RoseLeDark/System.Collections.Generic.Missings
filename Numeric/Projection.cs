using System;
using System.Collections.Generic;
using System.Text;

namespace SystemEx.Numeric {
    public struct Projection {
        float m_fFov;
        float m_fAspect;
        float m_fNearPlane;
        float m_fFarPlane;

        public Projection ( float fFov, float fAspect, float fNearPlane, float fFarPlane ) {
            m_fFov = fFov;
            m_fAspect = fAspect;
            m_fNearPlane = fNearPlane;
            m_fFarPlane = fFarPlane;
        }
    }
}
