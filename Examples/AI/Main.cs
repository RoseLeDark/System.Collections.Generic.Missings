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



using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// Deine SystemEX‑Imports
using SystemEx.AI;
using SystemEx.AI.Backend;
using SystemEx.AI.Tools;
using SystemEx.Algorithms.Compute.Interfaces;
using SystemEx.Collections.Generic;

namespace ExampleAIWindowsBackend {

    

    public class WinCopilotModel : Model<string, AITool> {
        public WinCopilotModel ( string stringModelName, WinCopilotBackend<string> backend )
            : base("ExampleAIObjerct", stringModelName,
            """
            You are an AI assistant with access to tools.

            BEHAVIOR:
            - Be concise but thorough
            - Use tools when appropriate instead of making up answers
            - Always respond in the user's language

            IMPORTANT:
            - For calculations, ALWAYS use the calculator tool
            - Never make up data: use tools to get real information
            """,
            backend) { }


        public void AddTools () {
            this.AddTool(new DateTimeTool());
            this.AddTool(new DateDifferenceTool());
            this.AddTool(new CalculatorTool());
        }

        public bool TryChangeModel ( string command ) {
            if ( !command.StartsWith("chg_model ", StringComparison.OrdinalIgnoreCase) )
                return false;

            var newModel = command.Substring("chg_model ".Length).Trim();

            if ( string.IsNullOrWhiteSpace(newModel) )
                return false;

            // Backend-Modell wechseln
            this.Backend.ModelName = newModel;

            return true;
        }

    }


    public static class Program {

        public static async Task Main ( string[] args ) {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===============================================");
            Console.WriteLine("   SystemEX – Windows AI Runtime Example");
            Console.WriteLine("===============================================\n");
            Console.ResetColor();

            // Backend + Factory
            var factory = new WinCopilotBackendFunctionFactory<string>();
            var backend = new WinCopilotBackend<string>(factory);

            // Model
            var model = new WinCopilotModel("phi-3-mini", backend);

            // Tools
            model.AddTools();

            Console.WriteLine("Tools registriert:");
            foreach ( var t in backend.ListTools() )
                Console.WriteLine($" - {t.Name}");

            Console.WriteLine("\nAgent bereit! Tippe 'exit' zum Beenden.\n");

            // Session
            string CLIENT_ID = Guid.NewGuid().ToString();
            bool running = model.Initialization(new Map<string, object>()
            {
                ["API_KEY_FOR_MODEL"] = "1234567890",
            });

            while ( running ) {
                string input = ReadInput();

                var sys = CheakSystemCommands(ref input, ref model);

                if ( sys == SystemEx.triple.False )
                    break;

                if ( sys == SystemEx.triple.Nin )
                    continue;

                var prompt = new ModelPromp<string>(input, CLIENT_ID);

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("Agent > ");
                Console.ResetColor();

                var result = await model.RunAsync(prompt);

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(result.Result);
                Console.ResetColor();
            }

            model.Release();
        }

        private static string ReadInput() {
            string? input = null;

            do {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("You > ");
                input = Console.ReadLine();
            } while ( string.IsNullOrWhiteSpace(input) );

            Console.ResetColor();

            return input;
        }

        private static SystemEx.triple CheakSystemCommands(ref string input, ref WinCopilotModel model) {

            if ( input.Equals("exit", StringComparison.OrdinalIgnoreCase) ) {
                return SystemEx.triple.False;
            }

            if ( model.TryChangeModel(input) ) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine($"System > Model changed to: {model.ModelName}");
                Console.ResetColor();

                return SystemEx.triple.Nin;
            }
            return SystemEx.triple.True;
        }
    }
}