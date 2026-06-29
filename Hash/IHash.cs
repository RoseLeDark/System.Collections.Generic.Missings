using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using SystemEx.Collections.Generic;
using SystemEx.IO.Provider;

namespace SystemEx.Hash {
    // Endian enum (falls noch nicht vorhanden)

    // Attribute zum Binden eines Hasher-Typs und Endian
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class HashAlgorithmAttribute : Attribute {
        public Type HasherType { get; }
        public Endian Endian { get; }

        public HashAlgorithmAttribute ( Type hasherType, Endian endian ) {
            HasherType = hasherType;
            Endian = endian;
        }
    }

    // Ergebnis-Typen
    public readonly struct Hash32 {
        public readonly int Value;
        public Hash32 ( int value ) => Value = value;
    }

    public readonly struct Hash64 {
        public readonly long Value;
        public Hash64 ( long value ) => Value = value;
    }

    // Hasher-Interface (arbeitet mit bytes und Endian)
    public interface IHasher {
        Hash32 Compute ( Array<byte> input, Endian endian );
        Hash64 ComputeLong ( Array<byte> input, Endian endian );
    }

    // Die einfache Basisklasse für hashbare Objekte
    public abstract class HashableObject {
        // Muss vom Typ implementiert werden: deterministische Byte-Repräsentation
        public abstract Array<byte> ToBytes ();

        public override int GetHashCode () {
            int _hash = 0;

            // Attribut vom konkreten Typ lesen (nicht typeof(HashableObject))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(this.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr == null ) {
                _hash = base.GetHashCode();
            } else {
                // Bytes erzeugen
                Array<byte> input = ToBytes();

                // Hasher transient erzeugen: zuerst versuchen, Konstruktor mit Endian, sonst parameterlos
                object? inst = null;
                try {
                    inst = Activator.CreateInstance(attr.HasherType, attr.Endian);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }

                if ( inst is IHasher hasher ) {
                    var h = hasher.Compute(input, attr.Endian);
                    _hash = h.Value;
                } else {
                   _hash = base.GetHashCode(); 
                }
            }
            
            return _hash;
        }
        public virtual long GetHashCodeLong () {
            long _hash = 0;

            // Attribut vom konkreten Typ lesen (nicht typeof(HashableObject))
            var attr = (HashAlgorithmAttribute?)Attribute.GetCustomAttribute(this.GetType(), typeof(HashAlgorithmAttribute));
            if ( attr == null ) {
                _hash = base.GetHashCode();
            } else {
                // Bytes erzeugen
                Array<byte> input = ToBytes();

                // Hasher transient erzeugen: zuerst versuchen, Konstruktor mit Endian, sonst parameterlos
                object? inst = null;
                try {
                    inst = Activator.CreateInstance(attr.HasherType, attr.Endian);
                } catch {
                    try {
                        inst = Activator.CreateInstance(attr.HasherType);
                    } catch {
                        inst = null;
                    }
                }

                if ( inst is IHasher hasher ) {
                    var h = hasher.ComputeLong(input, attr.Endian);
                    _hash = h.Value;
                } else {
                    _hash = base.GetHashCode();
                }
            }

            return _hash;
        }
    }

}
