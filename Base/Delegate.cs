using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.Utils;

namespace SystemEx {

    /// <summary>
    /// Represents a delegate container that can hold multiple callback functions.
    /// Each callback receives the delegate instance itself and an argument of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The argument type passed to all subscribed callbacks.</typeparam>
    public interface IDelegate<T> {
        /// <summary>
        /// Invokes all subscribed callback functions with the specified argument.
        /// </summary>
        /// <param name="arg">The argument passed to each callback.</param>
        void Invoke ( T arg );

        /// <summary>
        /// Subscribes a new callback function to this delegate.
        /// </summary>
        /// <param name="func">The callback function to add.</param>
        void Subscribe ( Action<IDelegate<T>, T> func );

        /// <summary>
        /// Removes a previously subscribed callback function.
        /// </summary>
        /// <param name="func">The callback function to remove.</param>
        void UnSubscribe ( Action<IDelegate<T>, T> func );

        /// <summary>
        /// Removes all subscribed callback functions.
        /// </summary>
        void Clear ();
    }

    /// <summary>
    /// A delegate container that stores multiple callback functions and allows invocation,
    /// subscription, unsubscription, enumeration, and basic comparison.
    /// </summary>
    /// <typeparam name="T">The argument type passed to all subscribed callbacks.</typeparam>
    public class Delegate<T> : IDelegate<T> , IComparable<Delegate<T>>, IEquatable< Delegate<T> >, IEnumerable<Action<IDelegate<T>, T>> {
        /// <summary>
        /// Internal sparse storage of callback functions.
        /// </summary>
        private Sparsed< Action<IDelegate<T>, T > > m_functions;

        /// <summary>
        /// Initializes a new delegate container with an optional initial callback.
        /// </summary>
        /// <param name="func">An optional callback function to subscribe immediately.</param>
        public Delegate ( Action<IDelegate<T>, T> func ) {
            m_functions = new Sparsed< Action<IDelegate<T>, T>   >();
            if(func != null) m_functions.Push(func);
        }

        /// <summary>
        /// Invokes all subscribed callback functions with the specified argument.
        /// </summary>
        /// <param name="arg">The argument passed to each callback.</param>
        public void Invoke ( T arg ) {

            foreach ( var item in m_functions ) {
               if( item != null) item?.Invoke(this, arg);
            }
  
        }
        /// <summary>
        /// Subscribes a new callback function to this delegate.
        /// </summary>
        /// <param name="func">The callback function to add.</param>
        public void Subscribe ( Action<IDelegate<T>, T> func ) {
            m_functions.Push(func);
        }
        /// <summary>
        /// Removes a previously subscribed callback function.
        /// </summary>
        /// <param name="func">The callback function to remove.</param>
        public void UnSubscribe ( Action<IDelegate<T>, T> func ) {
            m_functions.Erase(func);
        }
        /// <summary>
        /// Removes all subscribed callback functions.
        /// </summary>
        public void Clear () {
            m_functions.Clear();
        }
        /// <summary>
        /// Compares this delegate to another based on the number of subscribed functions.
        /// </summary>
        /// <param name="other">The other delegate to compare against.</param>
        /// <returns>
        /// A value indicating whether this delegate has fewer, equal, or more subscribed functions.
        /// </returns>
        public int CompareTo ( Delegate<T> other ) {
 
            long A = m_functions.Count;
            long B = other.m_functions.Count;


            if (A > B) return (int)CompareResult.Greater;
            if (A < B ) return (int)CompareResult.Less;
            
            return 0;
        }
        /// <summary>
        /// Returns an enumerator that iterates through all subscribed callback functions.
        /// </summary>
        public IEnumerator<Action<IDelegate<T>, T>> GetEnumerator () {
            return m_functions.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator () {
            return GetEnumerator();
        }

        /// <summary>
        /// Determines whether this delegate is equal to another delegate.
        /// Equality is based on reference equality of the internal storage container.
        /// </summary>
        /// <param name="other">The other delegate to compare with.</param>
        /// <returns>True if both delegates reference the same storage container; otherwise false.</returns>
        public bool Equals ( Delegate<T>? other ) {
            return m_functions.Equals(other);
        }

        /// <summary>
        /// Determines whether two delegates are equal.
        /// </summary>
        public static bool operator == ( Delegate<T> a, Delegate<T> b ) {
            return a.Equals(b);
        }

        /// <summary>
        /// Determines whether two delegates are not equal.
        /// </summary>
        public static bool operator != ( Delegate<T> a, Delegate<T> b ) {
            return !a.Equals(b);
        }

        
    }

}
