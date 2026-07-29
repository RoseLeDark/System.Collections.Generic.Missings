using System;
using System.Collections.Generic;
using System.Text;
using SystemEx.Collections.Generic;

namespace SystemEx.AI {
    public struct ModelResult<T> : IModelResult<T> {
        private readonly T m_result;
        private Map<string, object> m_metaData;
        private readonly Optional<Exception> m_exection;
        private readonly Optional<object> m_raw;
        private readonly DateTime m_time;
        private readonly bool m_okkay;

        public T Result => m_result;

        public Map<string, object> Metadata => m_metaData;

        public Optional<Exception> Error => m_exection;

        public Optional<object> Raw => m_raw;

        public bool Success => m_okkay;

        public DateTime Timestamp => m_time;


        public ModelResult ( T result, Map<string, object> metadata, Optional<Exception> error, Optional<object> raw ) {
            m_result = result;
            m_metaData = metadata;
            m_exection = error;
            m_raw = raw;
            m_time = DateTime.UtcNow;
        }
    }
}
