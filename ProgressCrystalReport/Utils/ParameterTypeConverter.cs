using System;
using CrystalDecisions.Shared;
using System.Globalization;

namespace ProgressCrystalReport.Utils
{
    /// <summary>
    /// Classe di utilità per la conversione dei parametri
    /// </summary>
    public static class ParameterTypeConverter
    {
        /// <summary>
        /// Restituisce il nome leggibile del tipo di parametro
        /// </summary>
        public static string GetParameterTypeName(ParameterValueKind valueKind)
        {
            switch (valueKind)
            {
                case ParameterValueKind.BooleanParameter:
                    return "Boolean";
                case ParameterValueKind.CurrencyParameter:
                    return "Currency";
                case ParameterValueKind.DateParameter:
                    return "Date";
                case ParameterValueKind.DateTimeParameter:
                    return "DateTime";
                case ParameterValueKind.NumberParameter:
                    return "Number";
                case ParameterValueKind.StringParameter:
                    return "String";
                case ParameterValueKind.TimeParameter:
                    return "Time";
                default:
                    return valueKind.ToString();
            }
        }

        /// <summary>
        /// Converte una stringa nel tipo appropriato per un parametro Crystal Reports
        /// </summary>
        public static object ConvertParameterValue(string value, ParameterValueKind parameterType)
        {
            if (string.IsNullOrEmpty(value))
            {
                // Per i parametri vuoti, usa DBNull per i tipi non stringa
                if (parameterType != ParameterValueKind.StringParameter)
                {
                    return DBNull.Value;
                }
                return string.Empty;
            }

            try
            {
                switch (parameterType)
                {
                    case ParameterValueKind.BooleanParameter:
                        if (bool.TryParse(value, out bool boolResult))
                            return boolResult;
                        // Controlla anche per 0/1, SI/NO, etc.
                        if (value.Trim().ToUpper() == "SI" || value.Trim().ToUpper() == "S" ||
                            value.Trim() == "1" || value.Trim().ToUpper() == "TRUE" ||
                            value.Trim().ToUpper() == "T" || value.Trim().ToUpper() == "Y" ||
                            value.Trim().ToUpper() == "YES")
                            return true;
                        if (value.Trim().ToUpper() == "NO" || value.Trim().ToUpper() == "N" ||
                            value.Trim() == "0" || value.Trim().ToUpper() == "FALSE" ||
                            value.Trim().ToUpper() == "F")
                            return false;
                        return false; // Default

                    case ParameterValueKind.NumberParameter:
                        if (double.TryParse(value, out double doubleResult))
                            return doubleResult;
                        return 0;

                    case ParameterValueKind.CurrencyParameter:
                        if (decimal.TryParse(value, out decimal decimalResult))
                            return decimalResult;
                        return 0m;

                    case ParameterValueKind.DateParameter:
                        if (DateTime.TryParse(value, out DateTime dateResult))
                            return dateResult.Date;
                        return DBNull.Value;

                    case ParameterValueKind.DateTimeParameter:
                        if (DateTime.TryParse(value, out DateTime dateTimeResult))
                            return dateTimeResult;
                        return DBNull.Value;

                    case ParameterValueKind.TimeParameter:
                        if (DateTime.TryParse(value, out DateTime timeResult))
                            return timeResult.TimeOfDay;
                        return DBNull.Value;

                    case ParameterValueKind.StringParameter:
                    default:
                        return value;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogWarning($"Errore durante la conversione del valore '{value}' al tipo {parameterType}: {ex.Message}");
                // In caso di errore di conversione, torna alla stringa originale
                return value;
            }
        }
    }
}