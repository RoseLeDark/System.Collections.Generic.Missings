using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.Drawing {


    /// <summary>
    /// 
    /// </summary>
    public interface ICanvas<T> {
        T GetPixel(int x, int y);

        /// <summary>
        /// 
        /// </summary>
        int Height { get;  }
        /// <summary>
        /// 
        /// </summary>
        int Width { get; }
        /// <summary>
        /// 
        /// </summary>
        IEnumerable<T> Buffer { get; }
        /// <summary>
        /// 
        /// </summary>
        bool Resize(int size);
        /// <summary>
        /// 
        /// </summary>
        ICanvas<T> CopyRegion(int x, int y, int width, int height);
        /// <summary>
        /// 
        /// </summary>
        ICanvas<T> Clone();
        /// <summary>
        /// 
        /// </summary>
        void Fill(T objcolor);
        /// <summary>
        /// 
        /// </summary>
        void FillRect(int x1, int y1, int x2, int y2);
        /// <summary>
        /// 
        /// </summary>
        void Clear();
        /// <summary>
        /// 
        /// </summary>
        Pair<int, int> Find(T color);
        /// <summary>
        /// 
        /// </summary>
        int FindLast(T color);
    }
}
