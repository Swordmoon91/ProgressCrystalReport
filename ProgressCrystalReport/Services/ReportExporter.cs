using System;
using System.IO;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using ProgressCrystalReport.Utils;

namespace ProgressCrystalReport.Services
{
    /// <summary>
    /// Gestore dell'esportazione dei report
    /// </summary>
    public class ReportExporter
    {
        /// <summary>
        /// Determina il formato di esportazione in base all'estensione del file
        /// </summary>
        public ExportFormatType DetermineExportFormat(string outputPath)
        {
            string extension = Path.GetExtension(outputPath).ToLower();
            LogHelper.LogDebug($"Determinazione formato dal file: {outputPath}, estensione: {extension}");

            ExportFormatType formatType;
            switch (extension)
            {
                case ".pdf":
                    formatType = ExportFormatType.PortableDocFormat;
                    break;
                case ".doc":
                case ".docx":
                    formatType = ExportFormatType.WordForWindows;
                    break;
                case ".xls":
                case ".xlsx":
                    formatType = ExportFormatType.Excel;
                    break;
                case ".rtf":
                    formatType = ExportFormatType.RichText;
                    break;
                case ".html":
                case ".htm":
                    formatType = ExportFormatType.HTML40;
                    break;
                case ".csv":
                    formatType = ExportFormatType.CharacterSeparatedValues;
                    break;
                default:
                    LogHelper.LogWarning($"Estensione non riconosciuta: {extension}. Utilizzo formato PDF predefinito.");
                    formatType = ExportFormatType.PortableDocFormat;
                    break;
            }

            LogHelper.LogInfo($"Formato di esportazione selezionato: {formatType}");
            return formatType;
        }

        /// <summary>
        /// Esporta il report nel formato specificato
        /// </summary>
        public void ExportReport(ReportDocument reportDocument, string outputPath, ExportFormatType formatType)
        {
            try
            {
                LogHelper.LogInfo($"Inizio esportazione in formato {formatType} su {outputPath}");

                // Configura le opzioni di esportazione
                ExportOptions exportOptions = reportDocument.ExportOptions;
                exportOptions.ExportFormatType = formatType;

                // Configura la destinazione
                DiskFileDestinationOptions diskOptions = new DiskFileDestinationOptions();
                diskOptions.DiskFileName = outputPath;
                exportOptions.DestinationOptions = diskOptions;

                // Esporta il report
                LogHelper.LogDebug("Esecuzione export...");
                reportDocument.Export();
                LogHelper.LogInfo("Esportazione completata con successo");
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Errore durante l'esportazione: {ex.Message}", ex);
                throw new Exception($"Errore durante l'esportazione del report: {ex.Message}", ex);
            }
        }
    }
}