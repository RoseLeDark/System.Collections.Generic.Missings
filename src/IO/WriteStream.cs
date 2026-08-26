using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Text;
using SystemEx.Collections.Generic;

#if TEXTING_BUILD
namespace SystemEx.IO {
    public enum CacheHeat {
        Cold,
        Warm,
        Hot
    }
    /// <summary>
    /// Represents an entry in the stream cache policy.
    /// </summary>
    public struct StreamCachePolicyEntry {
        public int StreamPosition { get; set; }
        public int CachePosition { get; set; }

        public int Count { get; set; }

        public int StreamCount { get; set; }
        public int CacheCount { get; set; }
    }
    /// <summary>
    /// Represents the stream cache policy.
    /// </summary>
    public interface IStreamCachePolicy {
        // Write- und Read-Zugriffe melden
        Pair<Triple, StreamCachePolicyEntry> OnWrite(long streamPos, int offset, int count);
        Pair<Triple, StreamCachePolicyEntry> OnRead (long streamPos, int offset, int count);

        IReadOnlyList<StreamCachePolicyEntry> Select ();


        // Liefert eine Bewertung des Blocks (z.B. für ARC)
        CacheHeat Evaluate(int cacheIndex);
        // return true then m_stream can update length
        bool OnSetLength ( long value, out bool requiresFlush );
    }


    /// <summary>
    /// Represents a write stream with caching capabilities.
    /// </summary>
    /// <typeparam name="TStreamCache">The stream cache policy.</typeparam>
    public class WriteCaheStream<TStreamCache> : Stream 
        where TStreamCache : IStreamCachePolicy {
        private Stream m_stream;
        private Endian m_endian;
        private Cache m_cache;
        private TStreamCache m_scPolicy;
        private object m_lock;

        /// <summary>
        /// Represents an action that can be performed on the write stream.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="sender">The sender.</param>
        /// <param name="value">The value.</param>
        public delegate void WriteStreamAction<T>( WriteCaheStream<TStreamCache> sender, T value) where T : allows ref struct;
      

        public override bool CanRead => m_stream.CanRead;

        public override bool CanSeek => m_stream.CanSeek;

        public override bool CanWrite => m_stream.CanWrite;

        public override long Length => m_stream.Length;

        public WriteStreamAction<int> OnCommit { get; set;  }

        public WriteStreamAction<int> OnIsCommit { get; set; }

        public override long Position {
            get => m_stream.Position;
            set => m_stream.Position = value;
        }

        public long Used {
            get {
                return Length - Position;
            }
        }

        public WriteCaheStream ( int WriteChacheSize, Stream backendStream) {
            m_stream = backendStream;
            m_cache = new Cache(WriteChacheSize, CacheType.OnlySystem);
        }
        

        public override void Flush () {

            lock ( m_lock ) {
                Commit();
                m_stream.Flush();

            }
        }

        public override int Read ( byte[] buffer, int offset, int count ) {
            int totalRead = -1;

            lock ( m_lock ) {
                // Policy entscheidet, ob Cache, Stream oder beides
                var r = m_scPolicy.OnRead(m_stream.Position, offset, count);

                Triple is_in = r.First;
                StreamCachePolicyEntry s = r.Second;

                

                if ( is_in == true ) {
                    // Alles im Cache
                    m_cache.Seek(SeekOrigin.Begin, s.CachePosition);
                    totalRead = m_cache.Read(buffer, offset, count);

                } else if ( is_in == false ) {
                    // Alles im Stream
                    totalRead = m_stream.Read(buffer, offset, count);

                } else {
                    // Teilweise im Cache, teilweise im Stream

                    // 1. Cache-Teil
                    m_cache.Seek(SeekOrigin.Begin, s.CachePosition);
                    int cacheRead = m_cache.Read(buffer, offset, s.CacheCount);

                    // 2. Stream-Teil
                    m_stream.Seek(s.StreamPosition + s.CacheCount, SeekOrigin.Begin);
                    int streamRead = m_stream.Read(buffer, offset + cacheRead, count - cacheRead);

                    totalRead = cacheRead + streamRead;
                }

            }
            return totalRead;
        }

        public override long Seek ( long offset, SeekOrigin origin ) 
            => m_stream.Seek(offset, origin);

        public override void SetLength ( long value ) {
            bool _requiresFlush = false;
            bool _plice = false;

            lock ( m_lock ) {
                _plice = m_scPolicy.OnSetLength(value, out _requiresFlush);
            }
            if ( _plice ) { 
               if ( _requiresFlush )
                    Flush();
               m_stream.SetLength(value);
            }
        }

        public override void Write(byte[] buffer, int offset, int count) {
            if ( (ulong)count > m_cache.Length ) {
                Flush();
                m_stream.Write(buffer, offset, count);
                return;
            }
            if ( m_cache.Free < (ulong)count )
                Flush();

            lock ( m_lock ) {
                m_scPolicy.OnWrite(m_stream.Position, offset, count);
                m_cache.Write(buffer, offset, count);
            }
        }
        private void Commit () {

            var entries = m_scPolicy.Select();
            OnCommit?.Invoke(this, entries.Count);

            // 3. Commit durchführen
            if ( entries.Count > 0 ) {

                foreach ( var item in entries ) {
                    byte[] buffer = new byte[item.Count];
                    m_cache.Seek(SeekOrigin.Begin, item.CachePosition);
                    m_cache.Read(buffer, 0, item.Count);
                    m_stream.Seek(item.StreamPosition, SeekOrigin.Begin);
                    m_stream.Write(buffer, 0, item.Count);
                    m_stream.Flush();
                }
            }

            OnIsCommit.Invoke(this, entries.Count);
        }
    }
    /// @}
}
#endif