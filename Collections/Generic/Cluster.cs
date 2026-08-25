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

using System.Numerics;
using System.Reflection;

namespace SystemEx.Collections.Generic {
	/// \addtogroup Collections
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

    public struct CluserIterrator <TElemet> : Iterrator<ICluster<TElemet>>  {
        private long m_index, m_end;
        private ICluster<TElemet> m_root;
        private ICluster<TElemet> m_current;

        public bool IsEnd => !HaveNext;

        public bool HaveNext => m_index < m_end;

        public long Index { get => m_index; set => m_index = value; }

        public Optional<ICluster<TElemet>> Current => new Optional<ICluster<TElemet>>(m_current);

        public CluserIterrator ( ICluster<TElemet> cluster, long index , long end) {
            m_index = index;
            m_root = cluster;
            m_current = m_root;
            m_end = end;
        }

        public void Forward () {
            if ( HaveNext ) {
                m_index++;
                m_current = m_current.ElementAt(m_index);
            }
        }
    }

    // TODO: Umbauen as Tree with N Childs!
    /// <summary>
    /// Basic implementation of <see cref="ICluster{T}"/> representing a node with weighted children.
    /// </summary>
    /// <typeparam name="T">Type of the stored value.</typeparam>
    public class Cluster<T> : ICluster<T>, IUsedIterrator<ICluster<T>, CluserIterrator<T> >  {
#pragma warning disable CA1051
        /// <summary>Stored node value.</summary>
        protected T m_value;

        /// <summary>Map of child node to edge weight.</summary>
        protected Map<ICluster<T>, uint> m_childs;
#pragma warning restore CA1051

        /// <summary>Gets the stored value.</summary>
        public T Value => m_value;

        /// <summary>Gets the number of children.</summary>
        public long Size => Child.Size;

        /// <summary>Gets the child map.</summary>
        public Map<ICluster<T>, uint> Child => m_childs;

        /// <summary>Optional parent pointer with edge weight.</summary>
        public Pair<ICluster<T>, uint>? Parent { get; set; }

        /// <summary>
        /// Begin IT on current layer
        /// </summary>
        public CluserIterrator<T> Begin => new CluserIterrator<T>(this, 0, Size);
        /// <summary>
        /// End IT im current layer
        /// </summary>
        public CluserIterrator<T> End => new CluserIterrator<T>(this, Size+1, Size);

        /// <inheritdoc/>
        public bool HaveChild { get => !m_childs.IsEmpty; }

        /// <inheritdoc/>
        public bool HaveParent  { get => Parent != null; }

        /// <summary>
        /// Indexer by integer index into the child map.
        /// </summary>
        public Pair<ICluster<T>, uint> this[int i] {
            get => m_childs.ElementAt(i);
            protected set => m_childs.Replace(i, value);
        }

        /// <summary>
        /// Indexer by child node key to access the edge weight.
        /// </summary>
        public Optional<uint> this[ICluster<T> key] {
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
            return m_childs.PushBack(other, gewicht);
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

            m_childs.Remove(other);
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
            var visited = new Vector<ICluster<T>>(128, 16);
            var costs = new Map<ICluster<T>, ulong>();

            var finder = Vector<ICluster<T>>.AsFinder(ref visited);

            // Start at this node with cost 0
            pq.Enqueue(this, 0);
            costs[this] = 0;

            while ( pq.Count > 0 ) {
                Optional<ICluster<T>> current = pq.Dequeue();
                if ( current.IsNull ) continue;

                // Skip if already visited
                if ( finder.Exists(current.Value!) )
                    continue;

                visited.PushBack(current.Value!);
                Optional<ulong> currentCost = costs[current.Value!];

                // If current cost exceeds budget, abandon this path
                if ( currentCost.Value! > budget )
                    continue;

                // Found target?
                if ( current.Value != null && current.Value.Equals(value) ) {
                    cost = currentCost.Value!;
                    _ret = true;
                    break;
                }

                // Traverse children
                for ( int i = 0; i < current.Value!.Child.Size; i++ ) {
                    var childPair = current.Value!.Child.ElementAt(i); // Pair<ICluster<T>, int>
                    ICluster<T> childNode = childPair.First;
                    uint edgeCost = childPair.Second;

                    if ( finder.Exists(childNode) )
                        continue;

                    ulong newCost = currentCost.Value! + (ulong)edgeCost;

                    // Only continue if within budget
                    if ( newCost > budget )
                        continue;

                    // Found a better path?
                    var newC = costs[childNode];

                    if ( !costs.ContainsKey(childNode) || newCost < newC.Value! ) {
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
            var pq = new PriorityQueue<Triple<ICluster<T>, ulong, ulong>, ulong>();
            var visited = new Vector<ICluster<T>>(128, 16);
            var finder = Vector<ICluster<T>>.AsFinder(ref visited);

            // Start: energie = budget, cost = 0
            pq.Enqueue( new Triple<ICluster<T>, ulong, ulong>(this, budget, 0), 0);

            while ( pq.Count > 0 ) {
                var item = pq.Dequeue();
                if ( item.IsNull ) continue;

                ICluster<T> current = item.Value!.First;
                ulong energie = item.Value!.Second;
                ulong cost = item.Value!.Third;

                if ( finder.Exists(current) )
                    continue;

                visited.PushBack(current);

                // Schritt speichern
                steps[current] = (uint)energie;

                // Ziel gefunden?
                if ( current.Value != null && (current.Value.Equals(pradicat) ) )
                    return true;

                // Kinder durchgehen
                for ( int i = 0; i < current.Child.Size; i++ ) {
                    var pair = current.Child.ElementAt(i);

                    var child = pair.First;
                    ulong gewicht = pair.Second;

                    if ( finder.Exists(child) )
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

        public ICluster<T> ElementAt ( long index ) => m_childs.ElementAt(index).First;

        public uint CostAt ( long index ) => m_childs.ElementAt(index).Second;

    }
#pragma warning disable CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
    /// @}
#pragma warning restore CS1587 // Der XML-Kommentar ist auf keinem gültigen Sprachelement abgelegt.
}
