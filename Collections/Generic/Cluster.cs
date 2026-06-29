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
    /// \addtogroup collections
    /// @{
    /// <summary>
    /// Defines how the search algorithm treats edge weights during traversal.
    /// </summary>
    public enum SearchType {
        /// <summary>
        /// Subtract the edge weight from the current energy: energy = energy - weight.
        /// </summary>
        SubtractWeight,
        /// <summary>
        /// Replace the current energy with the edge weight: energy = weight.
        /// </summary>
        SetToWeight,
        /// <summary>
        /// Do not consume energy; instead accumulate the edge weight into the cost: cost += weight.
        /// </summary>
        AccumulateCost
    }
    /// <summary>
    /// Represents a cluster node in a weighted graph structure.
    /// </summary>
    /// <typeparam name="T">Type of the value stored in the cluster node.</typeparam>
    public interface ICluster<T> {
        /// <summary>
        /// The value stored in this cluster node. May be null.
        /// </summary>
        public T? Value { get;  }
        /// <summary>
        /// Number of direct children (outgoing edges) of this node.
        /// </summary>
        public int Size { get; }
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
    }
    /// <summary>
    /// Basic implementation of <see cref="ICluster{T}"/> representing a node with weighted children.
    /// </summary>
    /// <typeparam name="T">Type of the stored value.</typeparam>
    public class Cluster<T> : ICluster<T> {
#pragma warning disable CA1051
        /// <summary>Stored node value.</summary>
        protected T m_value;

        /// <summary>Map of child node to edge weight.</summary>
        protected Map<ICluster<T>, uint> m_childs;
#pragma warning restore CA1051

        /// <summary>Gets the stored value.</summary>
        public T Value => m_value;

        /// <summary>Gets the number of children.</summary>
        public int Size => Child.Size;

        /// <summary>Gets the child map.</summary>
        public Map<ICluster<T>, uint> Child => m_childs;

        /// <summary>Optional parent pointer with edge weight.</summary>
        public Pair<ICluster<T>, uint>? Parent { get; set; }

        /// <summary>
        /// Indexer by integer index into the child map.
        /// </summary>
        public Pair<ICluster<T>, uint> this[int i] {
            get => m_childs[i];
            protected set => m_childs[i] = value;
        }

        /// <summary>
        /// Indexer by child node key to access the edge weight.
        /// </summary>
        public uint this[ICluster<T> key] {
            get => m_childs[key];
            protected set => m_childs[key] = value;
        }

        /// <summary>
        /// Create a new cluster node with an initial capacity for children.
        /// </summary>
        /// <param name="value">Value to store in the node.</param>
        /// <param name="childs">Initial child capacity (optional).</param>
        public Cluster(T value, int childs = 100) {
            m_value = value;
            m_childs = new Map<ICluster<T>, uint>();
            Parent = null;
        }

        /// <summary>
        /// Add a child node with the specified weight.
        /// </summary>
        public bool Add(ICluster<T> other, uint gewicht) {
            return m_childs.TryAdd(other, gewicht);
        }
        /// <summary>
        /// Check whether the specified node is a direct child.
        /// </summary>
        public bool Contains(ICluster<T> other) {
            return m_childs.ContainsKey(other);
        }

        /// <summary>
        /// Remove the specified child node if present.
        /// </summary>
        public bool Remove(ICluster<T> other) {
            bool _ret = false;
            Pair<ICluster<T>, uint>? pair = m_childs.FindFirst(other);
            if ( pair.HasValue ) _ret = m_childs.Remove(pair.Value);
            return _ret;
        }

        /// <summary>
        /// Perform a cost-limited search (Dijkstra-like) to find a node with the given value.
        /// Returns true and sets <paramref name="cost"/> when a path within <paramref name="budget"/> is found.
        /// </summary>
        public bool Cost(T value, ref ulong cost, ulong budget) {
            bool _ret = false;
            // Priority Queue: (current cost, node)
            var pq = new PriorityQueue<ICluster<T>, ulong>();
            var visited = new Array<ICluster<T>>(128, 16);
            var costs = new Map<ICluster<T>, ulong>();

            // Start at this node with cost 0
            pq.Enqueue(this, 0);
            costs[this] = 0;

            while ( pq.Count > 0 ) {
                var current = pq.Dequeue();

                // Skip if already visited
                if ( visited.Contains(current) )
                    continue;

                visited.Add(current);
                ulong currentCost = costs[current];

                // If current cost exceeds budget, abandon this path
                if ( currentCost > budget )
                    continue;

                // Found target?
                if ( current.Value != null && current.Value.Equals(value) ) {
                    cost = currentCost;
                    _ret = true;
                    break;
                }

                // Traverse children
                for ( int i = 0; i < current.Child.Size; i++ ) {
                    var childPair = current.Child[i]; // Pair<ICluster<T>, int>
                    ICluster<T> childNode = childPair.First;
                    uint edgeCost = childPair.Second;

                    if ( visited.Contains(childNode) )
                        continue;

                    ulong newCost = currentCost + (ulong)edgeCost;

                    // Only continue if within budget
                    if ( newCost > budget )
                        continue;

                    // Found a better path?
                    if ( !costs.ContainsKey(childNode) || newCost < costs[childNode] ) {
                        costs[childNode] = newCost;
                        pq.Enqueue(childNode, newCost);
                    }
                }
            }

            // Return whether target was found within budget
            return _ret;
        }
        /// <summary>
        /// General search that propagates remaining energy or accumulates cost depending on <paramref name="type"/>.
        /// Records remaining energy per visited node in <paramref name="steps"/>.
        /// </summary>
        public bool find(T pradicat, SearchType type, ulong budget, ref Map<ICluster<T>, uint> steps) {
            // PriorityQueue: (cost, node)
            var pq = new PriorityQueueEx<Triple<ICluster<T>, ulong, ulong>, ulong>();
            var visited = new Array<ICluster<T>>(128, 16);

             
            // Start: energie = budget, cost = 0
            pq.Enqueue( new Triple<ICluster<T>, ulong, ulong>(this, budget, 0), 0);

            while ( pq.Count > 0 ) {
                var item = pq.Dequeue();
                var current = item.First;
                var energie = item.Second;
                var cost = item.Third;

                if ( visited.Contains(current) )
                    continue;

                visited.Add(current);

                // Schritt speichern
                steps[current] = (uint)energie;

                // Ziel gefunden?
                if ( current.Value != null && (current.Value.Equals(pradicat) ) )
                    return true;

                // Kinder durchgehen
                for ( int i = 0; i < current.Child.Size; i++ ) {
                    var pair = current.Child[i];
                    var child = pair.First;
                    ulong gewicht = pair.Second;

                    if ( visited.Contains(child) )
                        continue;

                    ulong newEnergie = energie;
                    ulong newCost = cost;

                    switch ( type ) {
                    case SearchType.SubtractWeight:
                        if ( gewicht > newEnergie )
                            continue;
                        newEnergie -= (ulong)gewicht;
                        break;

                    case SearchType.SetToWeight:
                        newEnergie = (ulong)gewicht;
                        if ( newEnergie > budget )
                            continue;
                        break;

                    case SearchType.AccumulateCost:
                        newCost += (ulong)gewicht;
                        if ( newCost > budget )
                            continue;
                        break;
                    }

                    // Enqueue child with priority = accumulated cost
                    pq.Enqueue(new Triple<ICluster<T>, ulong, ulong>(child, newEnergie, newCost), newCost);
                }
            }
            // Not found within budget/constraints
            return false;
        }

    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
