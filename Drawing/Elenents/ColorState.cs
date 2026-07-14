using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Drawing.Elenents {

    public enum ColorStateChange {
        Darken,
        Lighten,
        NewColor,


        User
    }
    public class ColorState {
        private ColorHSV m_baseColor;
        private Array<Pair<ColorStateChange, object>> m_states;
        private int m_currentState = -1;
        private bool m_isDirty = false;

        public bool IsDirty => m_isDirty;

        public ColorR8G8B8 Current =>
            (m_currentState == -1) ? m_baseColor.ToColorR8G8B8() : GetColor( (byte)m_currentState);

        public ColorState(ColorR8G8B8 baseColor, byte states) {
            m_baseColor = baseColor.ToColorHSV();
            m_states = new Array<Pair<ColorStateChange, object>>(states);
        }

        public bool AddDarkenState (byte n, float darkFactor)  => 
            SetState(n, ColorStateChange.Darken, -System.Math.Abs(darkFactor));
        public bool AddLightenState ( byte n, float lightenFactor ) 
            => SetState(n, ColorStateChange.Lighten, System.Math.Abs(lightenFactor));
        public bool AddNewColorState ( byte n, ColorR8G8B8 newColor ) 
            => SetState(n, ColorStateChange.NewColor, newColor.ToColorHSV());
        public bool AddUserState ( byte n, object args )  
            => SetState(n, ColorStateChange.User, args);

        public bool SetState ( int n ) {
            bool _ret = true;

            if ( n < 0 ) {
                m_currentState = -1;
                m_isDirty = true;
            } else {
                if ( n >= m_states.Count ) _ret = false;
                else {
                    m_currentState = n;
                    m_isDirty = true;
                }
            }
            return _ret;
        }

        

        public ColorR8G8B8 GetColor ( byte n) {
            var pair = m_states.ElementAt(n);

            // ALWAYS start from a fresh copy of the base color
            ColorHSV newColor = new ColorHSV(m_baseColor.H, m_baseColor.S, m_baseColor.V);

            switch ( pair.First ) {
            case ColorStateChange.Darken:
            newColor = GetColorDarker((float)pair.Second);
            break;

            case ColorStateChange.Lighten:
            newColor = GetColorLighter((float)pair.Second);
            break;

            case ColorStateChange.NewColor:
            newColor = ((ColorHSV)pair.Second);
            break;
            case ColorStateChange.User:
            newColor = OnGetCostumColor(n, pair.Second);
            break;
            default:
            break;
            }

            return newColor.ToColorR8G8B8();
        }

        protected virtual ColorHSV OnGetCostumColor ( byte state, object parameter ) {
            return m_baseColor;
        }

        private bool SetState ( byte n, ColorStateChange change, object parameter ) {
            return m_states.Insert(n, new Pair<ColorStateChange, object>(change, parameter));
        }

        private ColorHSV GetColorDarker (float parameter) {
            ColorHSV newColor = new ColorHSV(m_baseColor.H, m_baseColor.S, m_baseColor.V);
            parameter = -System.Math.Abs(parameter);

            return newColor.Brightness(parameter);
        }
        private ColorHSV GetColorLighter ( float parameter ) {
            ColorHSV newColor = new ColorHSV(m_baseColor.H, m_baseColor.S, m_baseColor.V);
            parameter = System.Math.Abs(parameter);
            return newColor.Brightness(parameter);
        }

    }
}
