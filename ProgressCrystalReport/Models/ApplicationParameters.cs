
// Models/ApplicationParameters.cs
using System.Collections.Generic;

namespace ProgressCrystalReport.Models
{
    /// <summary>
    /// Rappresenta i parametri dell'applicazione forniti da riga di comando
    /// </summary>
    public class ApplicationParameters
    {
        // Parametri obbligatori
        public string ReportPath { get; set; }
        public string OdbcDsn { get; set; }

        // Parametri opzionali
        public string Username { get; set; }
        public string Password { get; set; }
        public string OutputPath { get; set; }
        public string ParameterFilePath { get; set; }

        // Parametri del report
        public List<string> ReportParameters { get; set; } = new List<string>();

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ReportPath) && !string.IsNullOrEmpty(OdbcDsn);
        }
    }
}