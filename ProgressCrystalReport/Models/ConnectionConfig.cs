// Models/ConnectionConfig.cs
namespace ProgressCrystalReport.Models
{
    /// <summary>
    /// Rappresenta la configurazione di connessione per Crystal Reports
    /// </summary>
    public class ConnectionConfig
    {
        public string ServerName { get; set; }
        public string DatabaseName { get; set; }
        public string UserID { get; set; }
        public string Password { get; set; }
    }
}