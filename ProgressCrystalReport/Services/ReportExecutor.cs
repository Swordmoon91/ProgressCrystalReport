// Services/ReportExecutor.cs (continuazione)
using System;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ProgressCrystalReport.Models;
using ProgressCrystalReport.Utils;
using System.IO;
using System.Diagnostics;

namespace ProgressCrystalReport.Services
{
    /// <summary>
    /// Gestore dell'esecuzione dei report
    /// </summary>
    public class ReportExecutor
    {
        private readonly ParameterManager _parameterManager;
        private readonly ReportExporter _reportExporter;

        public ReportExecutor()
        {
            _parameterManager = new ParameterManager();
            _reportExporter = new ReportExporter();
        }

        /// <summary>
        /// Esegue il report con i parametri specificati
        /// </summary>
        public void ExecuteReport(ApplicationParameters appParams)
        {
            LogHelper.LogInfo("Avvio del report in corso...");
            Console.WriteLine("Avvio del report in corso...");

            // Verifica i percorsi e i parametri
            ValidateParameters(appParams);

            // Carica parametri dal file se specificato
            _parameterManager.LoadParametersFromFile(appParams);

            // Crea l'istanza del report
            ReportDocument reportDocument = new ReportDocument();

            try
            {
                // Carica il file del report (.rpt)
                LogHelper.LogInfo($"Caricamento del report: {appParams.ReportPath}");
                Console.WriteLine($"Caricamento del report: {appParams.ReportPath}");
                reportDocument.Load(appParams.ReportPath);

                // Configura la connessione ODBC
                ConnectionInfo connectionInfo = CreateConnectionInfo(appParams);

                // Configura i parametri
                _parameterManager.SetReportParameters(reportDocument, appParams);

                // Configura la connessione ODBC
                LogHelper.LogInfo("Configurazione connessione ODBC...");
                Console.WriteLine("Configurazione connessione ODBC...");
                SetDBLogonForReport(connectionInfo, reportDocument);

                // Aggiorna il report (recupera i dati)
                LogHelper.LogInfo("Recupero dei dati...");
                Console.WriteLine("Recupero dei dati...");
                reportDocument.Refresh();

                // Gestisci l'output
                HandleOutput(reportDocument, appParams);

                LogHelper.LogInfo("Report completato con successo");
                Console.WriteLine("Operazione completata con successo.");
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Errore durante la generazione del report: {ex.Message}", ex);
                throw;
            }
            finally
            {
                // Libera le risorse
                LogHelper.LogDebug("Pulizia delle risorse del report");
                reportDocument?.Close();
                reportDocument?.Dispose();
            }
        }

        /// <summary>
        /// Verifica che i parametri siano validi
        /// </summary>
        private void ValidateParameters(ApplicationParameters appParams)
        {
            // Controlla che il file del report esista
            if (!FileHelper.FileExists(appParams.ReportPath))
            {
                LogHelper.LogError($"File del report non trovato: {appParams.ReportPath}");
                Console.WriteLine($"ERRORE: Il file del report '{appParams.ReportPath}' non esiste.");
                throw new FileNotFoundException($"Il file del report non esiste: {appParams.ReportPath}");
            }

            LogHelper.LogInfo($"Report path: {appParams.ReportPath}");
            LogHelper.LogInfo($"DSN ODBC: {appParams.OdbcDsn}");
            if (!string.IsNullOrEmpty(appParams.Username)) LogHelper.LogInfo($"Username ODBC: {appParams.Username}");
            if (!string.IsNullOrEmpty(appParams.OutputPath)) LogHelper.LogInfo($"Output path: {appParams.OutputPath}");
        }

        /// <summary>
        /// Crea l'oggetto ConnectionInfo
        /// </summary>
        private ConnectionInfo CreateConnectionInfo(ApplicationParameters appParams)
        {
            return new ConnectionInfo
            {
                ServerName = appParams.OdbcDsn,
                DatabaseName = "", // Potrebbe non essere necessario con ODBC
                UserID = appParams.Username,
                Password = appParams.Password
            };
        }

        /// <summary>
        /// Imposta le credenziali per tutte le tabelle nel report
        /// </summary>
        private void SetDBLogonForReport(ConnectionInfo connectionInfo, ReportDocument reportDocument)
        {
            LogHelper.LogDebug("Applicazione informazioni di connessione alle tabelle...");
            try
            {
                // Applica le informazioni di login a tutte le tabelle nel report
                foreach (Table table in reportDocument.Database.Tables)
                {
                    TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                    tableLogonInfo.ConnectionInfo = connectionInfo;
                    table.ApplyLogOnInfo(tableLogonInfo);
                    LogHelper.LogDebug($"Applicata connessione alla tabella: {table.Name}");
                }

                // Per i subreport
                foreach (ReportDocument subreport in reportDocument.Subreports)
                {
                    LogHelper.LogDebug($"Applicazione connessione al subreport: {subreport.Name}");
                    foreach (Table table in subreport.Database.Tables)
                    {
                        TableLogOnInfo tableLogonInfo = table.LogOnInfo;
                        tableLogonInfo.ConnectionInfo = connectionInfo;
                        table.ApplyLogOnInfo(tableLogonInfo);
                        LogHelper.LogDebug($"Applicata connessione alla tabella del subreport: {table.Name}");
                    }
                }
                LogHelper.LogInfo("Connessione applicata a tutte le tabelle");
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Errore nell'applicazione della connessione: {ex.Message}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gestisce l'output del report
        /// </summary>
        private void HandleOutput(ReportDocument reportDocument, ApplicationParameters appParams)
        {
            if (string.IsNullOrEmpty(appParams.OutputPath))
            {
                // Se non è stato specificato un percorso di output, esporta in PDF nella directory corrente
                string defaultPath = FileHelper.GenerateDefaultOutputPath();
                LogHelper.LogInfo($"Nessun percorso di output specificato. Esportazione in: {defaultPath}");
                Console.WriteLine($"Nessun percorso di output specificato. Esportazione in: {defaultPath}");
                _reportExporter.ExportReport(reportDocument, defaultPath, ExportFormatType.PortableDocFormat);

                // Apri il PDF con l'applicazione predefinita
                LogHelper.LogInfo("Apertura del PDF con l'applicazione predefinita");
                Console.WriteLine("Apertura del file...");
                Process.Start(new ProcessStartInfo
                {
                    FileName = defaultPath,
                    UseShellExecute = true
                });
            }
            else
            {
                // Se è stato specificato un percorso di output, esporta il report
                LogHelper.LogInfo($"Esportazione del report in: {appParams.OutputPath}");
                Console.WriteLine($"Esportazione del report in: {appParams.OutputPath}");

                // Assicurati che la directory di output esista
                FileHelper.EnsureDirectoryExists(appParams.OutputPath);

                // Determina il formato di esportazione in base all'estensione del file
                ExportFormatType formatType = _reportExporter.DetermineExportFormat(appParams.OutputPath);
                LogHelper.LogDebug($"Formato di esportazione: {formatType}");
                _reportExporter.ExportReport(reportDocument, appParams.OutputPath, formatType);

                LogHelper.LogInfo($"Report esportato con successo in: {appParams.OutputPath}");
                Console.WriteLine($"Report esportato con successo in: {appParams.OutputPath}");
            }
        }
    }
}
