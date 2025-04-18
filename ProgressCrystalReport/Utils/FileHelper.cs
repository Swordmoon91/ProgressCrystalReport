using System;
using System.IO;
using ProgressCrystalReport.Models;
using System.Collections.Generic;

namespace ProgressCrystalReport.Utils
{
    /// <summary>
    /// Classe di utilità per operazioni sui file
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// Verifica che il file esista
        /// </summary>
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Crea la directory se non esiste
        /// </summary>
        public static void EnsureDirectoryExists(string path)
        {
            string directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                LogHelper.LogDebug($"Creazione directory: {directory}");
                Directory.CreateDirectory(directory);
            }
        }

        /// <summary>
        /// Legge i parametri da un file separati da CHAR(1)
        /// </summary>
        public static List<string> ReadParametersFromFile(string paramFilePath)
        {
            List<string> parameters = new List<string>();

            try
            {
                if (FileExists(paramFilePath))
                {
                    string fileContent = File.ReadAllText(paramFilePath);
                    char[] separator = new char[] { (char)1 };
                    string[] fileParams = fileContent.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    LogHelper.LogDebug($"Trovati {fileParams.Length} parametri nel file");

                    foreach (string paramValue in fileParams)
                    {
                        parameters.Add(paramValue);
                    }
                }
                else
                {
                    LogHelper.LogError($"File dei parametri non trovato: {paramFilePath}");
                    Console.WriteLine($"ERRORE: Il file dei parametri '{paramFilePath}' non esiste.");
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogError($"Errore durante la lettura del file dei parametri: {ex.Message}", ex);
                Console.WriteLine($"ERRORE: Impossibile leggere il file dei parametri: {ex.Message}");
            }

            return parameters;
        }

        /// <summary>
        /// Genera un nome file di default per l'output
        /// </summary>
        public static string GenerateDefaultOutputPath(string extension = "pdf")
        {
            return Path.Combine(Environment.CurrentDirectory,
                $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.{extension}");
        }
    }
}