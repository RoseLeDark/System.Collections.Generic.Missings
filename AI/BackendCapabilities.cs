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
    /// <summary>
    /// Defines capability flags for AI backends.
    /// 
    /// Naming follows the AI_BACKEND_CAPS_XXX convention.
    /// These flags describe what features a backend supports, such as
    /// text generation, audio processing, vision models, tool execution,
    /// platform integration, and hardware acceleration.
    /// </summary>
    /// <summary>
    /// Describes capabilities supported by an AI backend.
    /// 
    /// This enum is intentionally broad and future-proof. It covers:
    /// - Local and remote execution
    /// - GPU/CPU/WASM/native runtimes
    /// - Text, chat, audio, vision, video, multimodal
    /// - Tools, agents, function calling
    /// - Embeddings, vector search, batch processing
    /// - Platform-specific capabilities (Windows, Web, Mobile)
    /// - Custom developer-defined capabilities
    /// 
    /// Naming follows AI_BACKEND_CAPS_XXX.
    /// </summary>
    public enum BackendCapabilities : int {
        // ---------------------------------------------------------------------
        // Core Text & Chat Capabilities (0x1000 - 0x10FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports text generation or completion.
        /// </summary>
        AI_BACKEND_CAPS_TEXT = 0x1001,

        /// <summary>
        /// Backend supports chat-style conversational models.
        /// </summary>
        AI_BACKEND_CAPS_CHAT = 0x1002,

        /// <summary>
        /// Backend supports structured JSON output.
        /// </summary>
        AI_BACKEND_CAPS_JSON = 0x1003,

        /// <summary>
        /// Backend supports instruction-following models.
        /// </summary>
        AI_BACKEND_CAPS_INSTRUCT = 0x1004,


        // ---------------------------------------------------------------------
        // Media Capabilities (0x1100 - 0x11FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports image generation or vision models.
        /// </summary>
        AI_BACKEND_CAPS_VISION = 0x1101,

        /// <summary>
        /// Backend supports audio input or audio generation.
        /// </summary>
        AI_BACKEND_CAPS_AUDIO = 0x1102,

        /// <summary>
        /// Backend supports video processing or video generation.
        /// </summary>
        AI_BACKEND_CAPS_VIDEO = 0x1103,

        /// <summary>
        /// Backend supports multimodal input (text + image + audio).
        /// </summary>
        AI_BACKEND_CAPS_MULTIMODAL = 0x1104,


        // ---------------------------------------------------------------------
        // Tooling & Agent Capabilities (0x1200 - 0x12FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports tool execution or function calling.
        /// </summary>
        AI_BACKEND_CAPS_TOOLS = 0x1201,

        /// <summary>
        /// Backend supports autonomous agent behavior.
        /// </summary>
        AI_BACKEND_CAPS_AGENT = 0x1202,

        /// <summary>
        /// Backend supports code execution or code generation tools.
        /// </summary>
        AI_BACKEND_CAPS_CODE = 0x1203,


        // ---------------------------------------------------------------------
        // Embeddings & Vector Search (0x1300 - 0x13FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports embedding generation.
        /// </summary>
        AI_BACKEND_CAPS_EMBEDDING = 0x1301,

        /// <summary>
        /// Backend supports vector search or similarity queries.
        /// </summary>
        AI_BACKEND_CAPS_VECTOR_SEARCH = 0x1302,


        // ---------------------------------------------------------------------
        // Execution Environment (0x1400 - 0x14FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend runs locally on the user's machine.
        /// </summary>
        AI_BACKEND_CAPS_LOCAL = 0x1401,

        /// <summary>
        /// Backend runs remotely (Web API, cloud service).
        /// </summary>
        AI_BACKEND_CAPS_REMOTE = 0x1402,

        /// <summary>
        /// Backend supports GPU acceleration.
        /// </summary>
        AI_BACKEND_CAPS_GPU = 0x1403,

        /// <summary>
        /// Backend supports CPU-only execution.
        /// </summary>
        AI_BACKEND_CAPS_CPU = 0x1404,

        /// <summary>
        /// Backend supports WebAssembly execution.
        /// </summary>
        AI_BACKEND_CAPS_WASM = 0x1405,

        /// <summary>
        /// Backend supports native OS-level execution.
        /// </summary>
        AI_BACKEND_CAPS_NATIVE = 0x1406,


        // ---------------------------------------------------------------------
        // Platform Capabilities (0x1500 - 0x15FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend is part of the Windows AI platform.
        /// </summary>
        AI_PLATFORM_CAP_WINDOWS = 0x1501,

        /// <summary>
        /// Backend is available on all platforms.
        /// </summary>
        AI_PLATFORM_CAP_ALL = 0x1502,

        /// <summary>
        /// Backend is part of a free or open platform.
        /// </summary>
        AI_CAP_FREE = 0x1503,

        /// <summary>
        /// Backend is optimized for mobile devices.
        /// </summary>
        AI_PLATFORM_CAP_MOBILE = 0x1504,


        // ---------------------------------------------------------------------
        // Performance & Execution Modes (0x1600 - 0x16FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports streaming output.
        /// </summary>
        AI_BACKEND_CAPS_STREAMING = 0x1601,

        /// <summary>
        /// Backend supports batch processing.
        /// </summary>
        AI_BACKEND_CAPS_BATCH = 0x1602,

        /// <summary>
        /// Backend supports low-latency execution.
        /// </summary>
        AI_BACKEND_CAPS_LOW_LATENCY = 0x1603,

        // ---------------------------------------------------------------------
        // Erweiterung: Backend‑Requirements (0x1700 – 0x17FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend requires an active internet connection.
        /// </summary>
        AI_BACKEND_CAPS_NEEDS_INTERNET = 0x1701,

        /// <summary>
        /// Backend requires an API key for authentication.
        /// </summary>
        AI_BACKEND_CAPS_NEEDS_API_KEY = 0x1702,

        /// <summary>
        /// Backend requires local configuration files.
        /// </summary>
        AI_BACKEND_CAPS_NEEDS_LOCAL_CONFIG = 0x1703,

        /// <summary>
        /// Backend requires access to local hardware (GPU, microphone, camera).
        /// </summary>
        AI_BACKEND_CAPS_NEEDS_HARDWARE = 0x1704,

        /// <summary>
        /// Backend requires user login or OAuth authentication.
        /// </summary>
        AI_BACKEND_CAPS_NEEDS_LOGIN = 0x1705,

        // ---------------------------------------------------------------------
        //Backend‑Transport / Communication (0x1800 – 0x18FF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend communicates via REST (HTTP/JSON).
        /// </summary>
        AI_BACKEND_CAPS_TRANSPORT_REST = 0x1801,

        /// <summary>
        /// Backend communicates via WebSocket (streaming, HTML push).
        /// </summary>
        AI_BACKEND_CAPS_TRANSPORT_WS = 0x1802,

        /// <summary>
        /// Backend communicates via native DLL calls.
        /// </summary>
        AI_BACKEND_CAPS_TRANSPORT_NATIVE = 0x1803,

        /// <summary>
        /// Backend communicates via local process IPC (pipes, shared memory).
        /// </summary>
        AI_BACKEND_CAPS_TRANSPORT_IPC = 0x1804,

        /// <summary>
        /// Backend communicates via WASM runtime (browser or sandbox).
        /// </summary>
        AI_BACKEND_CAPS_TRANSPORT_WASM = 0x1805,

        // ---------------------------------------------------------------------
        // Backend‑Runtime (0x1900 – 0x19FF)
        // ---------------------------------------------------------------------
        /// <summary>
        /// Backend runs inside a local OS runtime (WindowsAI, DirectML).
        /// </summary>
        AI_BACKEND_CAPS_RUNTIME_LOCAL = 0x1901,

        /// <summary>
        /// Backend runs inside a cloud runtime (OpenAI, Anthropic, etc.).
        /// </summary>
        AI_BACKEND_CAPS_RUNTIME_CLOUD = 0x1902,

        /// <summary>
        /// Backend runs inside a browser runtime (JavaScript/WASM).
        /// </summary>
        AI_BACKEND_CAPS_RUNTIME_BROWSER = 0x1903,

        /// <summary>
        /// Backend runs inside a native runtime (C++, CUDA, Vulkan).
        /// </summary>
        AI_BACKEND_CAPS_RUNTIME_NATIVE = 0x1904,

        // ---------------------------------------------------------------------
        // Backend‑Config / Customization (0x1A00 – 0x1AFF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Backend supports custom configuration parameters.
        /// </summary>
        AI_BACKEND_CAPS_CONFIGURABLE = 0x1A01,

        /// <summary>
        /// Backend supports dynamic runtime configuration changes.
        /// </summary>
        AI_BACKEND_CAPS_CONFIG_DYNAMIC = 0x1A02,

        /// <summary>
        /// Backend supports environment variables for configuration.
        /// </summary>
        AI_BACKEND_CAPS_CONFIG_ENV = 0x1A03,


        // ---------------------------------------------------------------------
        // Custom / Developer-defined (0x1FFF)
        // ---------------------------------------------------------------------

        /// <summary>
        /// Custom capability defined by the developer.
        /// </summary>
        AI_BACKEND_CAPS_CUSTOM = 0x1FFF
    }


}
