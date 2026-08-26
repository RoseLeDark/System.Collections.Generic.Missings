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
	/// Converts an <see cref="IModelTool{T}"/> definition into a backend‑specific 
	/// function object that can be exposed to an AI model.  
	/// 
	/// <para>
	/// A factory implementing <see cref="IAIFunctionFactory{T, TAITOOL}"/> is used 
	/// by model backends to translate internal tool definitions into the concrete 
	/// function format required by the underlying AI runtime.  
	/// This enables consistent tool‑exposure across different model backends, 
	/// runtimes, and execution environments.
	/// </para>
	/// </summary>
	/// <typeparam name="T">
	/// The prompt type used by the model backend.
	/// </typeparam>
	/// <typeparam name="TAITOOL">
	/// The backend‑specific function type produced by the factory.
	/// </typeparam>
	public interface IAIFunctionFactory<T, TAITOOL> {

		/// <summary>
		/// Converts the given <see cref="IModelTool{T}"/> into a backend‑compatible 
		/// function object.  
		/// Returns <c>true</c> if conversion succeeded; otherwise <c>false</c>.
		/// </summary>
		/// <param name="tool">
		/// The model tool definition to convert.
		/// </param>
		/// <param name="converted">
		/// When the method returns, contains the converted backend‑specific tool 
		/// object if conversion succeeded; otherwise <c>null</c>.
		/// </param>
		/// <returns>
		/// <c>true</c> if the tool was successfully converted; otherwise <c>false</c>.
		/// </returns>
		bool Convert ( IModelTool<T> tool, out TAITOOL converted );
	}
	/// @}

}
