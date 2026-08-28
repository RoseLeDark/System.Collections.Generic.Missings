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

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
	/// @{

	/// <summary>
	/// Represents a cluster node in a weighted graph structure.
	/// </summary>
	public interface ICluster {
        /// <summary>
        /// Number of direct children (outgoing edges) of this node.
        /// </summary>
        public long Size { get; }

        /// <summary>
        /// Have this node a Child
        /// </summary>
        public bool HaveChild { get; }
        /// <summary>
        /// Have this Node a Parent
        /// </summary>
        public bool HaveParent { get; }

        /// <summary>
        /// Get Cost of the element at position N
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The costs the elment on position N</returns>
        public uint CostAt ( long index );
    }

    /// <summary>
    /// Represents a cluster node in a weighted graph structure.
    /// </summary>
    /// <typeparam name="T">Type of the value stored in the cluster node.</typeparam>
    public interface ICluster<T> : ICluster {
        /// <summary>
        /// The value stored in this cluster node. May be null.
        /// </summary>
        public T? Value { get;  }

        
        /// <summary>
        /// Mapping of child node to edge weight.
        /// </summary>
        public Map<ICluster<T>, uint> Child {  get; }
        /// <summary>
        /// Optional parent pointer and the weight of the edge to the parent.
        /// </summary>
        public Pair<ICluster<T>, uint>? Parent { get; }
        /// <summary>
        /// Add a directed edge from this node to <paramref name="other"/> with the given weight.
        /// </summary>
        /// <param name="other">Target child node.</param>
        /// <param name="gewicht">Edge weight.</param>
        /// <returns>True if the child was added; otherwise false.</returns>
        public bool Add(ICluster<T> other, uint gewicht);
        /// <summary>
        /// Remove the directed edge to <paramref name="other"/>.
        /// </summary>
        /// <param name="other">Child node to remove.</param>
        /// <returns>True if the child was removed; otherwise false.</returns>
        public bool Remove(ICluster<T> other);
        /// <summary>
        /// Determines whether <paramref name="other"/> is a direct child of this node.
        /// </summary>
        /// <param name="other">Node to check.</param>
        /// <returns>True if <paramref name="other"/> is a child; otherwise false.</returns>
        public bool Contains(ICluster<T> other);

        /// <summary>
        /// Search for a node whose Value equals <paramref name="value"/> using a cost-limited Dijkstra-like search.
        /// If found, sets <paramref name="cost"/> to the accumulated cost.
        /// </summary>
        /// <param name="value">Value to search for.</param>
        /// <param name="cost">Reference to store the found cost.</param>
        /// <param name="budget">Maximum allowed cost for the search.</param>
        /// <returns>True if a matching node was found within budget; otherwise false.</returns>
        public bool Cost(T value, ref ulong cost, ulong budget);

        /// <summary>
        /// General search that propagates either remaining energy or accumulated cost according to <paramref name="type"/>.
        /// Fills <paramref name="steps"/> with the remaining energy for visited nodes.
        /// </summary>
        /// <param name="pradicat">Predicate value to match against node values.</param>
        /// <param name="type">Search mode that controls how weights affect energy/cost.</param>
        /// <param name="budget">Energy or cost budget depending on the search type.</param>
        /// <param name="steps">Map to record remaining energy per visited node.</param>
        /// <returns>True if a matching node was found within constraints; otherwise false.</returns>
        public bool find(T pradicat, SearchType type, ulong budget, ref Map<ICluster<T>, uint> steps);


        /// <summary>
        /// Get Element at Position N
        /// </summary>
        ICluster<T> ElementAt ( long index );
    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
