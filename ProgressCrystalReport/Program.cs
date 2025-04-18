using ProgressCrystalReport.Services;
using ProgressCrystalReport.Utils;
using System;

namespace ProgressCrystalReport
{
    /// <summary>
    /// Classe principale dell'applicazione
    /// </summary>
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                // Inizializza il logger
                LogHelper.Initialize();
                LogHelper.LogInfo("=== ProgressCrystalReport avviato ===");

                // Verifica se ci sono argomenti o se è stato richiesto l'aiuto
                CommandLineParser parser = new();
                if (args.Length == 0 || (args.Length == 1 && (args[0] == "-h" || args[0] == "--help")))
                {
                    parser.ShowHelp();
                    return 0;
                }

                // Visualizza intestazione
                Console.WriteLine("=== LANCIATORE CRYSTAL REPORT ===");

                // Inizializza i servizi
                ReportExecutor executor = new ();

                // Analizza i parametri della riga di comando
                var applicationParameters = parser.Parse(args);

                // Verifica che i parametri siano validi
                if (!applicationParameters.IsValid())
                {
                    parser.ShowHelp();
                    return 1;
                }

                // Stampa informazioni di base
                Console.WriteLine($"Report: {applicationParameters.ReportPath}");
                Console.WriteLine($"DSN ODBC: {applicationParameters.OdbcDsn}");

                // Esegui il report
                executor.ExecuteReport(applicationParameters);

                return 0;
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Errore durante l'esecuzione: {ex.Message}", ex);
                Console.WriteLine($"ERRORE: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                return 1;
            }
            finally
            {
                LogHelper.LogInfo("=== ProgressCrystalReport terminato ===");
            }
        }
    }
}