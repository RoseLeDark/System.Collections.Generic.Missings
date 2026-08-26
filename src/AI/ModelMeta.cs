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


namespace SystemEx.AI {
	/// \addtogroup AI
	/// @{

	/// <summary>
	/// Defines the data format returned by a backend during model execution.
	/// 
	/// This enum follows the AI_FORMAT_XXX naming convention and provides
	/// stable integer identifiers for all supported formats.
	/// </summary>
	public enum MetaFormat : int {
        // ---------------------------------------------------------------------
        // Standard Web/API formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// JSON object or array. The default format for most Web APIs.
        /// </summary>
        AI_FORMAT_JSON = 1,

        /// <summary>
        /// Plain text or raw string output.
        /// </summary>
        AI_FORMAT_TEXT = 2,


        // ---------------------------------------------------------------------
        // Structured formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// XML document format.
        /// </summary>
        AI_FORMAT_XML = 3,

        /// <summary>
        /// YAML document format, commonly used for configuration files.
        /// </summary>
        AI_FORMAT_YAML = 4,

        /// <summary>
        /// INI-style key-value configuration format.
        /// </summary>
        AI_FORMAT_INI = 5,


        // ---------------------------------------------------------------------
        // Binary / compact formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// Raw binary data without a predefined structure.
        /// </summary>
        AI_FORMAT_BINARY = 6,

        /// <summary>
        /// Google Protocol Buffers (Protobuf) binary format.
        /// </summary>
        AI_FORMAT_PROTOBUF = 7,

        /// <summary>
        /// MessagePack binary JSON format.
        /// </summary>
        AI_FORMAT_MSGPACK = 8,


        // ---------------------------------------------------------------------
        // Document / markup formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// HTML document format.
        /// </summary>
        AI_FORMAT_HTML = 9,

        /// <summary>
        /// Markdown text format.
        /// </summary>
        AI_FORMAT_MARKDOWN = 10,


        // ---------------------------------------------------------------------
        // AI-specific formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// Token arrays or token streams produced by language models.
        /// </summary>
        AI_FORMAT_TOKENS = 11,

        /// <summary>
        /// Vector embeddings (float arrays) produced by embedding models.
        /// </summary>
        AI_FORMAT_EMBEDDING = 12,


        // ---------------------------------------------------------------------
        // Media formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// Image data (PNG, JPG, BMP, WebP, etc.).
        /// </summary>
        AI_FORMAT_IMAGE = 13,

        /// <summary>
        /// Audio data (WAV, MP3, OGG, PCM, etc.).
        /// </summary>
        AI_FORMAT_AUDIO = 14,

        /// <summary>
        /// Video data (MP4, WebM, etc.).
        /// </summary>
        AI_FORMAT_VIDEO = 15,


        // ---------------------------------------------------------------------
        // Custom / developer-defined formats
        // ---------------------------------------------------------------------

        /// <summary>
        /// Any custom developer-defined format not covered by the predefined types.
        /// </summary>
        AI_FORMAT_CUSTOM = 100
    }




    /// <summary>
    /// Defines strongly typed metadata keys for model execution.
    /// 
    /// Naming follows the AI_META_XXX convention, Values are grouped logically and 
    /// designed to remain stable for long-term API usage.
    /// 
    /// These metadata keys describe backend information, model details,
    /// request/response properties, execution metrics, tool usage, and
    /// error diagnostics.
    /// </summary>
    public enum ModelMeta : int {
        // ---------------------------------------------------------------------
        // Backend Information (0x1000 - 0x10FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Name of the backend used to execute the model.
        /// </summary>
        AI_META_BACKEND = 0x1001,

        /// <summary>
        /// Version string of the backend implementation.
        /// </summary>
        AI_META_BACKEND_VERSION = 0x1002,

        /// <summary>
        /// Type of backend (e.g., Local, Remote, WebAPI, WindowsAI).
        /// </summary>
        AI_META_BACKEND_TYPE = 0x1003,

        /// <summary>
        /// Measured backend latency in milliseconds.
        /// </summary>
        AI_META_BACKEND_LATENCY = 0x1004,
        /// <summary>
        /// The Platform where run the backend
        /// </summary>
        AI_META_PLATFORM = 0x1005,

        // ---------------------------------------------------------------------
        // Model Information (0x1100 - 0x11FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Identifier of the model used (e.g., "gpt-4", "llama3").
        /// </summary>
        AI_META_MODEL = 0x1101,

        /// <summary>
        /// Version of the model, if provided by the backend.
        /// </summary>
        AI_META_MODEL_VERSION = 0x1102,

        /// <summary>
        /// Provider of the model (OpenAI, Microsoft, HuggingFace, FreeWeb, etc.).
        /// </summary>
        AI_META_MODEL_PROVIDER = 0x1103,

        /// <summary>
        /// Capability flags describing supported features of the model.
        /// </summary>
        AI_META_MODEL_CAPS = 0x1104,


        // ---------------------------------------------------------------------
        // Request Information (0x1200 - 0x12FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// URL used to send the request to the backend.
        /// </summary>
        AI_META_URL = 0x1201,

        /// <summary>
        /// HTTP status code returned by the backend.
        /// </summary>
        AI_META_HTTP_STATUS = 0x1202,

        /// <summary>
        /// HTTP response headers returned by the backend.
        /// </summary>
        AI_META_HTTP_HEADERS = 0x1203,

        /// <summary>
        /// Size of the request payload in bytes.
        /// </summary>
        AI_META_REQUEST_SIZE = 0x1204,

        /// <summary>
        /// Size of the response payload in bytes.
        /// </summary>
        AI_META_RESPONSE_SIZE = 0x1205,


        // ---------------------------------------------------------------------
        // Raw Data (0x1300 - 0x13FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Raw content returned by the backend (JSON, text, binary, etc.).
        /// </summary>
        AI_META_RAW = 0x1301,

        /// <summary>
        /// Parsed output content (JSON element, XML DOM, text, binary blob, etc.).
        /// </summary>
        AI_META_CONTENT_OUT = 0x1302,

        /// <summary>
        /// Serialized input content sent to the backend.
        /// </summary>
        AI_META_CONTENT_IN = 0x1303,

        /// <summary>
        /// Trace identifier used for debugging or correlation.
        /// </summary>
        AI_META_TRACE_ID = 0x1304,

        /// <summary>
        /// Format of the returned content. See <see cref="MetaFormat"/>.
        /// </summary>
        AI_META_FORMAT = 0x1300,


        // ---------------------------------------------------------------------
        // Execution Metrics (0x1400 - 0x14FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Total execution time in milliseconds.
        /// </summary>
        AI_META_EXEC_TIME = 0x1401,

        /// <summary>
        /// Timestamp marking the start of execution.
        /// </summary>
        AI_META_EXEC_START = 0x1402,

        /// <summary>
        /// Timestamp marking the end of execution.
        /// </summary>
        AI_META_EXEC_END = 0x1403,

        /// <summary>
        /// Number of input tokens processed by the model.
        /// </summary>
        AI_META_TOKENS_IN = 0x1404,

        /// <summary>
        /// Number of output tokens generated by the model.
        /// </summary>
        AI_META_TOKENS_OUT = 0x1405,

        /// <summary>
        /// Total number of tokens involved in the request.
        /// </summary>
        AI_META_TOKENS_TOTAL = 0x1406,


        // ---------------------------------------------------------------------
        // Tool Information (0x1500 - 0x15FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Name of the tool used during execution, if any.
        /// </summary>
        AI_META_TOOL_USED = 0x1501,

        /// <summary>
        /// Number of tools invoked during execution.
        /// </summary>
        AI_META_TOOL_COUNT = 0x1502,

        /// <summary>
        /// Execution time of the tool in milliseconds.
        /// </summary>
        AI_META_TOOL_LATENCY = 0x1503,


        // ---------------------------------------------------------------------
        // Error Information (0x1600 - 0x16FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Type of exception thrown during execution.
        /// </summary>
        AI_META_ERROR_TYPE = 0x1601,

        /// <summary>
        /// Error message describing the failure.
        /// </summary>
        AI_META_ERROR_MESSAGE = 0x1602,

        /// <summary>
        /// Stack trace of the exception, if available.
        /// </summary>
        AI_META_ERROR_STACK = 0x1603,

        /// <summary>
        /// Backend-specific error code or HResult.
        /// </summary>
        AI_META_ERROR_CODE = 0x1604,



        

        /// <summary>
        /// Session identifier used for conversational or stateful backends.
        /// </summary>
        AI_META_SESSION_ID = 0x1701,

        /// <summary>
        /// List of tool names used during execution.
        /// </summary>
        AI_META_TOOLS_USED = 0x1702,

        /// <summary>
        /// Hash of the system prompt for debugging or caching.
        /// </summary>
        AI_META_SYSTEMPROMPT_HASH = 0x1703,

        /// <summary>
        /// Model parameters passed to the backend (temperature, top_p, etc.).
        /// </summary>
        AI_META_MODEL_PARAMETERS = 0x1704,

        /// <summary>
        /// Backend configuration snapshot used during execution.
        /// </summary>
        AI_META_BACKEND_CONFIG = 0x1705,


        AI_META_DURATION = 0x1707,
        AI_META_EXECUTION_MODE = 0x1706,
        AI_META_RUNTIME_VERSION = 0x1707,
        AI_META_CLIENT_VERSION = 0x1708,



    }
    /// @}

}
