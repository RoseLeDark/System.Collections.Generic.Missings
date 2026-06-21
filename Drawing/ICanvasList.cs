using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Drawing {
    public enum BlendMode {
        Add,
        Subtract,
        Multiply,
        Screen,
        Overlay,
        Replace,
        Divide,
        Light,
        Dark
    }

    public interface ISubCanvas<T> : ICanvas<T> {
        bool Enable { get; set;  }
        bool Showíng { get;set;  }
        bool IsDirty { get; set; }

        string Name { get; set; }

        byte Visible { get; set; }

        bool HasMask { get; set; }
        ICanvas<ColorGray> Mask { get; set; }
    }


    public interface ICanvasList<T> : ICanvas<T> {
        IReadOnlyMap<ISubCanvas<T>, BlendMode>  Layers {  get;  }
        ICanvas<T> this[int index] { get; }

        int AddLayer(ISubCanvas<T> layer, BlendMode mode);

        ISubCanvas<T> GetLayer(int index);

        bool SetShowing(bool show);

        bool IsShowing(int index);

        T GetPixel(int layer, int x, int y);

        int SwapIn(int x, int y, int width, int height, ref ICanvas<T> toDraw);

        int SwapIn(ref ICanvas<T> toDraw);
    }
}
