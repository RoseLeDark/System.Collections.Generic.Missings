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

namespace SystemEx {
    /// <summary>
    /// A base class for reference-counted objects.
    /// </summary>
    public class Reference : IDisposable {
        private int m_refCount;

        /// <summary>
        /// Gets the current reference count of the object.
        /// </summary>
        public int RefCount {
            get { return m_refCount; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Reference"/> class with a reference count of zero.
        /// </summary>
        public Reference() {
            m_refCount = 0;
        }

        /// <summary>
        /// Increments the reference count of the object.
        /// </summary>
        public void AddRef() {
            m_refCount++;
        }

        /// <summary>
        /// Decrements the reference count of the object and disposes it if the count reaches zero.
        /// </summary>
        public void Release() {
            m_refCount--;
            if (m_refCount <= 0) {
                Dispose();
            }
        }

        public Reference Clone() {
            Reference clone = (Reference)this.MemberwiseClone();
            clone.m_refCount = 0; // Reset reference count for the clone
            return clone;
        }
        

        /// <summary>
        /// Disposes the object and releases any resources it holds.
        /// </summary>
        public static void AddRef(ref Reference reference) {
            reference.AddRef();
        }

        public static void Release(ref Reference reference) {
            reference.Release();
            if(reference.RefCount <= 0) {
                reference.Dispose();
                reference = null;
            }
            
        }
        /// <summary>
        /// Creates an instance of a class derived from <see cref="Reference"/> and initializes it with the specified arguments.
        /// </summary>
        public static bool CreateInstance<T>(out T instance, object args) where T : Reference, new() {
            instance = new T();
            bool _ret = false;
            if (instance != null) {
                instance.AddRef();
                _ret = instance.OnCreate(args);
            }
            return _ret;
        }

        /// <summary>
        /// Disposes the object and releases any resources it holds. Override this method in derived classes to clean up resources.
        /// </summary>
        public virtual void Dispose() {
            // Override in derived classes to clean up resources
        }

        /// <summary>
        /// Called when the object is created. Override this method in derived classes to perform initialization with the specified arguments.
        /// </summary>
        protected virtual bool OnCreate(object args) {
            // Override in derived classes to clean up resources
            return true;
        }

    }

}