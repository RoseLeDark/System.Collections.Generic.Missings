/* 
 * SPDX-License-Identifier: EUPL-1.2
 *
 * Copyright (c) 2026 Amber-Sophia Schröck <ambersophia.schroeck@mail.de>
 *
 * This file is licensed under the European Union Public Licence (EUPL) version 1.2.
 * You can obtain a copy of the licence at:
 *   https://joinup.ec.europa.eu/collection/eupl/eupl-text-eupl-12
 *
 * Unless required by applicable law or agreed to in writing, software distributed
 * under the Licence is distributed on an "AS IS" basis, WITHOUT WARRANTIES OR
 * CONDITIONS OF ANY KIND, either express or implied.
 *
 * If you modify this file, retain this notice and add a short description of your
 * changes and the date.
 */

using SystemEx.Collections.Generic;
using SystemEx.Collections.Generic.Interfaces;

namespace SystemEx.Drawing {
    /// \addtogroup color
    /// @{
    /// <summary>
    /// Specifies the mathematical blend operation used when combining a layer
    /// with the layers beneath it. These modes do not perform graphical drawing;
    /// instead, each mode defines a deterministic color transformation applied
    /// during layer composition when <see cref="ICanvasList{T}.GetPixels"/> or
    /// <see cref="ICanvasList{T}.SwapIn"/> is invoked.
    /// </summary>
    public enum BlendMode
    {
        /// <summary>
        /// Adds the layer’s color value to the underlying color. Useful for
        /// additive accumulation or brightness‑increasing transformations.
        /// </summary>
        Add,

        /// <summary>
        /// Subtracts the layer’s color value from the underlying color. Produces
        /// darker results and is often used for inverse or removal‑style effects.
        /// </summary>
        Subtract,

        /// <summary>
        /// Multiplies the layer’s color value with the underlying color. This
        /// darkens the result and is commonly used for intensity modulation.
        /// </summary>
        Multiply,

        /// <summary>
        /// Applies the mathematical inverse of Multiply. Brightens the result by
        /// combining colors through reciprocal multiplication rules.
        /// </summary>
        Screen,

        /// <summary>
        /// Applies a conditional combination of Multiply and Screen depending on
        /// the underlying brightness. Produces contrast‑enhancing transformations.
        /// </summary>
        Overlay,

        /// <summary>
        /// Replaces the underlying color entirely with the layer’s color. This
        /// mode ignores all lower layers and acts as a direct override.
        /// </summary>
        Replace,

        /// <summary>
        /// Divides the underlying color by the layer’s color. Produces brightening
        /// or contrast‑shifting effects depending on the color domain.
        /// </summary>
        Divide,

        /// <summary>
        /// Selects the lighter value between the layer and the underlying color.
        /// Useful for highlight‑style transformations.
        /// </summary>
        Light,

        /// <summary>
        /// Selects the darker value between the layer and the underlying color.
        /// Useful for shadow‑style or depth‑enhancing transformations.
        /// </summary>
        Dark
    }


    /// <summary>
    /// Represents a mathematical sub‑canvas that participates as a layer within
    /// a layered canvas system. A sub‑canvas does not perform graphical drawing;
    /// instead, it defines deterministic transformation rules, visibility states,
    /// and optional masking that influence the final composed pixel buffer when
    /// requested through <see cref="ICanvasList{T}.GetPixels"/> or <see cref="ICanvasList{T}.SwapIn"/>.
    ///
    /// Sub‑canvases act as structural components in a purely mathematical model:
    /// they contribute color values, transformations, or blend interactions, but
    /// no pixels are rendered until the composition pipeline is evaluated.
    /// </summary>
    /// <typeparam name="T">The color type used by the canvas.</typeparam>
    public interface ISubCanvas<T> : ICanvas<T>
    {
        /// <summary>
        /// Gets or sets whether this sub‑canvas is enabled. Disabled sub‑canvases
        /// do not participate in mathematical composition and behave as if they
        /// were absent from the layer stack.
        /// </summary>
        bool Enable { get; set; }

        /// <summary>
        /// Gets or sets whether this sub‑canvas is visible. A hidden sub‑canvas
        /// remains present in the layer structure but does not contribute to the
        /// composed pixel buffer.
        /// </summary>
        bool Showing { get; set; }

        /// <summary>
        /// Gets or sets whether the sub‑canvas is marked as dirty. A dirty state
        /// indicates that its mathematical content has changed and that dependent
        /// compositions may need to be recalculated.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// Gets or sets the name of the sub‑canvas. This is a user‑defined
        /// identifier useful for organizing, labeling, or referencing layers
        /// within a canvas list.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets the visibility weight of the sub‑canvas. This value
        /// represents a mathematical transparency factor used during layer
        /// blending. Higher values increase the influence of the sub‑canvas
        /// on the final composition.
        /// </summary>
        byte Visible { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the sub‑canvas uses a mask.
        /// When enabled, the mask determines which regions of the sub‑canvas
        /// contribute to the mathematical composition.
        /// </summary>
        bool HasMask { get; set; }

        /// <summary>
        /// Gets or sets the grayscale mask applied to this sub‑canvas. The mask
        /// defines per‑pixel visibility weights, allowing selective inclusion or
        /// exclusion of regions during composition. A mask does not draw pixels;
        /// it modifies how the sub‑canvas mathematically interacts with underlying
        /// layers.
        /// </summary>
        ICanvas<ColorGray> Mask { get; set; }
    }


    /// <summary>
    /// Represents a layered mathematical canvas system composed of multiple
    /// sub‑canvases. Each layer is a deterministic transformation applied on
    /// top of the base canvas, forming a purely mathematical composition model
    /// rather than a graphical drawing system.
    ///
    /// Layers do not store rendered pixels. Instead, each layer contributes
    /// transformation rules, blend operations, or color modifications that are
    /// applied when the final pixel buffer is requested through <see cref="GetPixels"/>
    /// or when swapped into another canvas via <see cref="SwapIn"/>.
    ///
    /// The system is fully deterministic: the same layer configuration always
    /// produces the same composed result.
    /// </summary>
    /// <typeparam name="T">The color type used by the canvas.</typeparam>
    public interface ICanvasList<T> : ICanvas<T> {

        /// <summary>
        /// Gets the collection of layers associated with this canvas list.
        /// Each layer is represented as an <see cref="ISubCanvas{T}"/> paired
        /// with a <see cref="BlendMode"/> describing how the layer mathematically
        /// interacts with the layers beneath it.
        ///
        /// Layers do not contain rendered pixel data; they define transformation
        /// rules that are applied when the composed buffer is requested.
        /// </summary>
        IReadOnlyMap<ISubCanvas<T>, BlendMode>  Layers {  get;  }

        /// <summary>
        /// Gets the sub‑canvas at the specified index.
        /// </summary>
        /// <param name="index">The index of the sub‑canvas to retrieve.</param>
        /// <returns>The sub‑canvas at the given index.</returns>
        ISubCanvas<T> this[int index] { get; }

        /// <summary>
        /// Adds a new layer to the canvas list.
        /// The layer becomes part of the mathematical composition pipeline and
        /// will influence the final pixel buffer depending on its blend mode.
        /// </summary>
        /// <param name="layer">The sub‑canvas to add as a layer.</param>
        /// <param name="mode">The blend mode describing how the layer interacts with lower layers.</param>
        /// <returns>The index at which the layer was added.</returns>
        int AddLayer(ISubCanvas<T> layer, BlendMode mode);

        /// <summary>
        /// Removes the layer at the specified index.
        /// </summary>
        /// <param name="index">The index of the layer to remove.</param>
        /// <returns>The removed layer instance.</returns>
        ISubCanvas<T> RemoveLayer(int index);


        /// <summary>
        /// Retrieves the sub‑canvas at the specified index.
        /// </summary>
        /// <param name="index">The index of the layer to retrieve.</param>
        /// <returns>The sub‑canvas at the given index.</returns>
        ISubCanvas<T> GetLayer(int index);
    
        /// <summary>
        /// Sets the visibility state of the specified layer.
        /// A hidden layer does not participate in the mathematical composition
        /// when <see cref="GetPixels"/> is invoked.
        ///
        /// Layer index semantics:
        /// <c>0</c> refers to the base canvas,
        /// <c>1</c> refers to the first added layer,
        /// <c>2</c> to the second, and so on.
        /// </summary>
        /// <param name="show">Whether the layer should be included in composition.</param>
        /// <param name="index">The layer index whose visibility is being changed.</param>
        /// <returns><c>true</c> if the visibility was changed; otherwise <c>false</c>.</returns>
        bool SetShowing(bool show, int index);

        /// <summary>
        /// Gets the visibility state of the specified layer.
        /// Hidden layers do not contribute to the mathematical composition.
        /// </summary>
        /// <param name="index">
        /// The layer index to query. <c>0</c> refers to the base canvas.
        /// </param>
        /// <returns><c>true</c> if the layer is visible; otherwise <c>false</c>.</returns>
        bool IsShowing(int index = 0);

        /// <summary>
        /// Retrieves the mathematically composed pixel value at the specified
        /// coordinates for the given layer index.
        ///
        /// The returned pixel is computed by folding the transformations of
        /// all layers from the specified layer down to the base canvas.
        /// </summary>
        /// <param name="layer">The layer index to compose from.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns>The composed pixel value at the given coordinates.</returns>
        T GetPixel(int layer, int x, int y);

        /// <summary>
        /// Returns the mathematically composed pixel buffer for the specified
        /// layer index. This canvas system is purely mathematical and does not
        /// perform real drawing; each layer represents a deterministic transformation
        /// applied on top of the layers beneath it.
        ///
        /// Layer index semantics:
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// <c>layer = Length</c> — Returns the fully composed pixel buffer of this
        /// canvas, including all layers that have been added. This represents the
        /// final accumulated state of the entire layered canvas.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// <c>layer = N</c> — Returns the pixel buffer resulting from combining
        /// all layers from <c>N</c> down to <c>1</c>, in order. The returned buffer
        /// reflects the mathematical accumulation of transformations up to the
        /// specified layer index, starting from that layer and folding downward
        /// into the base canvas.
        /// </description>
        /// </item>
        /// </list>
        ///
        /// Example:
        /// If the canvas contains 10 layers, calling <c>GetPixels(10)</c>
        /// produces the composed buffer of layers <c>10 + 9 + 8 + 7 + 6 + 5 + 4 + 3 + 2 + 1</c>.
        /// No rendering is performed; the result is computed entirely through
        /// deterministic layer composition rules.
        /// </summary>
        /// <param name="layer">
        /// The layer index to compose from. <c>Length</c> returns the complete
        /// final canvas; values greater than zero return the accumulated
        /// mathematical result of all layers from the specified index down to
        /// the base canvas.
        /// </param>
        /// <returns>
        /// An <see cref="Array{T}"/> containing the composed pixel data for the
        /// requested layer range.
        /// </returns>
        Array<T> GetPixels(int layer); 

        /// <summary>
        /// Swaps the mathematically composed region of this canvas into another
        /// canvas. The region defined by <paramref name="x"/>, <paramref name="y"/>,
        /// <paramref name="width"/>, and <paramref name="height"/> is computed
        /// mathematically and then written into <paramref name="toDraw"/>.
        ///
        /// No drawing occurs; the operation transfers computed pixel values.
        /// </summary>
        /// <param name="x">The X coordinate of the region.</param>
        /// <param name="y">The Y coordinate of the region.</param>
        /// <param name="width">The width of the region.</param>
        /// <param name="height">The height of the region.</param>
        /// <param name="toDraw">The target canvas receiving the computed pixels.</param>
        /// <returns>The number of pixels swapped.</returns>
        int SwapIn(int x, int y, int width, int height, ref ICanvas<T> toDraw);

        /// <summary>
        /// Swaps the entire mathematically composed canvas into another canvas.
        /// This transfers the full computed pixel buffer into <paramref name="toDraw"/>.
        /// </summary>
        /// <param name="toDraw">The target canvas receiving the composed pixel buffer.</param>
        /// <returns>The number of pixels swapped.</returns> 
        int SwapIn(ref ICanvas<T> toDraw);
    }
    // @}
}
