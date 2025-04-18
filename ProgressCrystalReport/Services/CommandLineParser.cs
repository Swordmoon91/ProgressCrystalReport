// Services/CommandLineParser.cs
using System;
using System.Collections.Generic;
using ProgressCrystalReport.Models;
using ProgressCrystalReport.Utils;
using System.Configuration;

namespace ProgressCrystalReport.Services
{
    /// <summary>
    /// Gestore del parsing dei parametri di riga di comando
    /// </summary>
    public class CommandLineParser
    {
        /// <summary>
        /// Analizza gli argomenti della riga di comando
        /// </summary>
        public ApplicationParameters Parse(string[] args)
        {
            LogHelper.LogDebug($"Parsing di {args.Length} parametri");

            Dictionary<string, string> parsedParams = ParseParametri(args);
            ApplicationParameters appParams = new ();

            // Verifica presenza dei parametri obbligatori o recupero dei valori di default
            ResolveReportPath(parsedParams, appParams);
            ResolveOdbcDsn(parsedParams, appParams);

            // Parametri opzionali
            appParams.Username = parsedParams.ContainsKey("-u") ? parsedParams["-u"] : "";
            appParams.Password = parsedParams.ContainsKey("-p") ? parsedParams["-p"] : "";

            // Output path
            ResolveOutputPath(parsedParams, appParams);

            // Parametri del file
            if (parsedParams.ContainsKey("-f"))
            {
                appParams.ParameterFilePath = parsedParams["-f"];
                // I parametri effettivi verranno caricati successivamente
            }

            return appParams;
        }

        /// <summary>
        /// Mostra il messaggio di aiuto
        /// </summary>
        public void ShowHelp()
        {
            LogHelper.LogInfo("Visualizzazione messaggio di aiuto");
            Console.WriteLine("=== PROGRESSCRYSTALREPORT - AIUTO ===");
            Console.WriteLine("Utilizzo: ProgressCrystalReport.exe [opzioni]");
            Console.WriteLine("\nOpzioni:");
            Console.WriteLine("  -r <percorso>       Percorso completo del file report (.rpt) [OBBLIGATORIO]");
            Console.WriteLine("  -d <nome>           Nome DSN ODBC [OBBLIGATORIO]");
            Console.WriteLine("  -u <username>       Username per la connessione ODBC");
            Console.WriteLine("  -p <password>       Password per la connessione ODBC");
            Console.WriteLine("  -o <percorso>       Percorso di output per il report esportato");
            Console.WriteLine("  -P:<nome> <valore>  Parametro per il report (es. -P:CustomerID 12345)");
            Console.WriteLine("  -f <file>           File di parametri separati da CHAR(1)");
            Console.WriteLine("  -h, --help          Mostra questo messaggio di aiuto");
            Console.WriteLine("\nEsempio:");
            Console.WriteLine("  ProgressCrystalReport.exe -r \"C:\\report.rpt\" -d \"MioDSN\" -u \"admin\" -p \"password\" -o \"C:\\output.pdf\" -P:Data \"2023-01-01\"");
            Console.WriteLine("  ProgressCrystalReport.exe -r \"C:\\report.rpt\" -d \"MioDSN\" -f \"parametri.txt\"");
        }

        /// <summary>
        /// Parsing dei parametri della riga di comando
        /// </summary>
        private Dictionary<string, string> ParseParametri(string[] args)
        {
            Dictionary<string, string> parametri = [];

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    string chiave = args[i];
                    string valore = "";

                    // Controlla se c'è un valore dopo la chiave
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        valore = args[i + 1];
                        i++; // Avanza all'argomento successivo
                    }

                    parametri[chiave] = valore;
                    LogHelper.LogDebug($"Parametro trovato: {chiave}={valore}");
                }
            }

            return parametri;
        }

        /// <summary>
        /// Risolve il percorso del report
        /// </summary>
        private void ResolveReportPath(Dictionary<string, string> parsedParams, ApplicationParameters appParams)
        {
            if (parsedParams.ContainsKey("-r"))
            {
                appParams.ReportPath = parsedParams["-r"];
            }
            else
            {
                // Cerca il valore di default nel config
                string defaultReportPath = ConfigurationManager.AppSettings["DefaultReportPath"];
                if (!string.IsNullOrEmpty(defaultReportPath))
                {
                    LogHelper.LogInfo($"Nessun parametro -r specificato, utilizzo valore predefinito: {defaultReportPath}");
                    appParams.ReportPath = defaultReportPath;
                }
                else
                {
                    LogHelper.LogError("Parametro obbligatorio -r mancante e nessun valore predefinito");
                    Console.WriteLine("ERRORE: Il parametro -r (report) è obbligatorio.");
                }
            }
        }

        /// <summary>
        /// Risolve il DSN ODBC
        /// </summary>
        private void ResolveOdbcDsn(Dictionary<string, string> parsedParams, ApplicationParameters appParams)
        {
            if (parsedParams.ContainsKey("-d"))
            {
                appParams.OdbcDsn = parsedParams["-d"];
            }
            else
            {
                // Cerca il valore di default nel config
                string defaultOdbcDsn = ConfigurationManager.AppSettings["DefaultOdbcDsn"];
                if (!string.IsNullOrEmpty(defaultOdbcDsn))
                {
                    LogHelper.LogInfo($"Nessun parametro -d specificato, utilizzo valore predefinito: {defaultOdbcDsn}");
                    appParams.OdbcDsn = defaultOdbcDsn;
                }
                else
                {
                    LogHelper.LogError("Parametro obbligatorio -d mancante e nessun valore predefinito");
                    Console.WriteLine("ERRORE: Il parametro -d (DSN) è obbligatorio.");
                }
            }
        }

        /// <summary>
        /// Risolve il percorso di output
        /// </summary>
        private void ResolveOutputPath(Dictionary<string, string> parsedParams, ApplicationParameters appParams)
        {
            if (parsedParams.ContainsKey("-o"))
            {
                appParams.OutputPath = parsedParams["-o"];
            }
            else
            {
                // Cerca il valore di default nel config
                string defaultOutputPath = ConfigurationManager.AppSettings["DefaultOutputPath"];
                if (!string.IsNullOrEmpty(defaultOutputPath))
                {
                    LogHelper.LogInfo($"Nessun parametro -o specificato, utilizzo valore predefinito: {defaultOutputPath}");
                    appParams.OutputPath = defaultOutputPath;
                }
            }
        }
    }
}