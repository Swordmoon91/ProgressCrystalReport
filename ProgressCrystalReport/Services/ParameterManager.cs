// Services/ParameterManager.cs
using System;
using System.Collections.Generic;
using CrystalDecisions.CrystalReports.Engine;
using ProgressCrystalReport.Models;
using ProgressCrystalReport.Utils;

namespace ProgressCrystalReport.Services
{
    /// <summary>
    /// Gestore dei parametri del report
    /// </summary>
    public class ParameterManager
    {
        /// <summary>
        /// Carica i parametri dal file se specificato
        /// </summary>
        public void LoadParametersFromFile(ApplicationParameters appParams)
        {
            if (!string.IsNullOrEmpty(appParams.ParameterFilePath))
            {
                LogHelper.LogInfo($"Lettura parametri dal file: {appParams.ParameterFilePath}");
                appParams.ReportParameters = FileHelper.ReadParametersFromFile(appParams.ParameterFilePath);
            }
        }

        /// <summary>
        /// Analizza e imposta i parametri del report
        /// </summary>
        public void SetReportParameters(ReportDocument reportDocument, ApplicationParameters appParams)
        {
            // Ottieni i parametri richiesti dal report
            ParameterFieldDefinitions paramFields = reportDocument.DataDefinition.ParameterFields;
            int requiredParamsCount = paramFields.Count;

            LogHelper.LogInfo($"Numero Parametri richiesti: {requiredParamsCount}");
            Console.WriteLine($"Numero Parametri richiesti: {requiredParamsCount}");

            // Estrai informazioni sui parametri
            List<ReportParameter> parameters = GetReportParameters(paramFields);

            // Mostra informazioni sui parametri
            DisplayParameterInfo(parameters);

            // Verifica se mancano parametri o ce ne sono troppi
            CheckParameterCount(appParams.ReportParameters, parameters);

            // Imposta i parametri
            SetParameters(reportDocument, parameters, appParams.ReportParameters);
        }

        /// <summary>
        /// Ottiene la lista dei parametri dal report
        /// </summary>
        private List<ReportParameter> GetReportParameters(ParameterFieldDefinitions paramFields)
        {
            List<ReportParameter> parameters = [];

            foreach (ParameterFieldDefinition param in paramFields)
            {
                ReportParameter reportParam = new()
                {
                    Name = param.Name,
                    Type = param.ParameterValueKind,
                    IsRequired = !param.IsOptionalPrompt,
                    IsMultiValue = param.EnableAllowMultipleValue,
                    Description = param.PromptText
                };

                // Se ci sono valori predefiniti, li salva
                if (param.DefaultValues != null && param.DefaultValues.Count > 0)
                {
                    reportParam.DefaultValue = param.DefaultValues[0].ToString();
                }

                parameters.Add(reportParam);

                LogHelper.LogDebug($"Parametro trovato: {reportParam.Name}, " +
                                   $"Tipo: {ParameterTypeConverter.GetParameterTypeName(reportParam.Type)}, " +
                                   $"Obbligatorio: {reportParam.IsRequired}, " +
                                   $"Multivalore: {reportParam.IsMultiValue}");
            }

            return parameters;
        }

        /// <summary>
        /// Visualizza informazioni sui parametri
        /// </summary>
        private void DisplayParameterInfo(List<ReportParameter> parameters)
        {
            Console.WriteLine("\nParametri richiesti dal report:");
            Console.WriteLine("{0,-25} {1,-15} {2,-10} {3,-10}", "Nome", "Tipo", "Obbligatorio", "Multivalore");
            Console.WriteLine(new string('-', 70));

            foreach (ReportParameter param in parameters)
            {
                Console.WriteLine("{0,-25} {1,-15} {2,-10} {3,-10}",
                                 param.Name,
                                 ParameterTypeConverter.GetParameterTypeName(param.Type),
                                 param.IsRequired ? "Sì" : "No",
                                 param.IsMultiValue ? "Sì" : "No");

                // Se ci sono valori predefiniti, mostrali
                if (!string.IsNullOrEmpty(param.DefaultValue))
                {
                    Console.WriteLine("  Valore predefinito: {0}", param.DefaultValue);
                }

                // Se ci sono descrizioni, mostrali
                if (!string.IsNullOrEmpty(param.Description))
                {
                    Console.WriteLine("  Descrizione: {0}", param.Description);
                }
            }
            Console.WriteLine();
        }

        /// <summary>
        /// Verifica se mancano parametri o ce ne sono troppi
        /// </summary>
        private void CheckParameterCount(List<string> reportParams, List<ReportParameter> parameters)
        {
            int requiredParamsCount = parameters.Count;

            if (reportParams.Count < requiredParamsCount)
            {
                int missingParams = requiredParamsCount - reportParams.Count;
                LogHelper.LogWarning($"Mancano {missingParams} parametri, verranno aggiunte stringhe vuote");
                Console.WriteLine($"ATTENZIONE: Mancano {missingParams} parametri, verranno aggiunte stringhe vuote");

                // Mostra quali parametri mancano
                for (int i = reportParams.Count; i < requiredParamsCount; i++)
                {
                    if (i < parameters.Count)
                    {
                        LogHelper.LogWarning($"Parametro mancante n. {i + 1}: {parameters[i].Name}");
                        Console.WriteLine($"  Parametro mancante n. {i + 1}: {parameters[i].Name}");
                    }
                    else
                    {
                        LogHelper.LogWarning($"Parametro mancante n. {i + 1}");
                        Console.WriteLine($"  Parametro mancante n. {i + 1}");
                    }

                    // Aggiungi stringa vuota
                    reportParams.Add(null);
                }
            }
            else if (reportParams.Count > requiredParamsCount)
            {
                LogHelper.LogWarning($"Sono stati forniti {reportParams.Count} parametri ma il report ne richiede solo {requiredParamsCount}. I parametri extra verranno ignorati.");
                Console.WriteLine($"ATTENZIONE: Sono stati forniti {reportParams.Count} parametri ma il report ne richiede solo {requiredParamsCount}. I parametri extra verranno ignorati.");
            }
        }

        /// <summary>
        /// Imposta i parametri sul report
        /// </summary>
        private void SetParameters(ReportDocument reportDocument, List<ReportParameter> parameters, List<string> reportParams)
        {
            int requiredParamsCount = parameters.Count;

            if (reportParams.Count > 0)
            {
                LogHelper.LogInfo($"Impostazione di {Math.Min(reportParams.Count, requiredParamsCount)} parametri del report");
                Console.WriteLine("Impostazione parametri del report:");

                for (int i = 0; i < Math.Min(reportParams.Count, requiredParamsCount); i++)
                {
                    string paramName = (i < parameters.Count) ? parameters[i].Name : $"Parametro {i + 1}";
                    string paramValue = reportParams[i];

                    LogHelper.LogDebug($"Parametro {i + 1} ({paramName}): {paramValue}");
                    Console.WriteLine($"  Parametro {i + 1} ({paramName}): {paramValue}");

                    try
                    {
                        // Converti il valore al tipo appropriato se possibile
                        if (i < parameters.Count)
                        {
                            object convertedValue = ParameterTypeConverter.ConvertParameterValue(paramValue, parameters[i].Type);
                            reportDocument.SetParameterValue(paramName, convertedValue);
                            LogHelper.LogDebug($"Valore convertito al tipo {parameters[i].Type}: {convertedValue}");
                        }
                        else
                        {
                            // Se non conosciamo il tipo, lasciamo che Crystal Reports gestisca la conversione
                            reportDocument.SetParameterValue(paramName, paramValue);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.LogWarning($"Errore durante l'impostazione del parametro {paramName}: {ex.Message}");
                        Console.WriteLine($"  ATTENZIONE: Problema con il parametro {paramName}: {ex.Message}");

                        // Prova un approccio alternativo - passa come stringa
                        try
                        {
                            LogHelper.LogDebug($"Tentativo di impostare il parametro {paramName} come stringa");
                            reportDocument.SetParameterValue(paramName, paramValue);
                        }
                        catch (Exception innerEx)
                        {
                            LogHelper.LogWarning($"Anche il tentativo alternativo è fallito: {innerEx.Message}");

                            // Ultimo tentativo - usa l'indice
                            try
                            {
                                LogHelper.LogDebug($"Ultimo tentativo usando l'indice {i}");
                                reportDocument.SetParameterValue(i, paramValue);
                            }
                            catch (Exception lastEx)
                            {
                                LogHelper.LogError($"Impossibile impostare il parametro {paramName} con nessun metodo: {lastEx.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}