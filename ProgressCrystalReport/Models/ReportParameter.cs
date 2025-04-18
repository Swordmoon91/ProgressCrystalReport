using CrystalDecisions.Shared;

namespace ProgressCrystalReport.Models
{
    /// <summary>
    /// Rappresenta un parametro di un report Crystal
    /// </summary>
    public class ReportParameter
    {
        public string Name { get; set; }
        public ParameterValueKind Type { get; set; }
        public bool IsRequired { get; set; }
        public bool IsMultiValue { get; set; }
        public string Description { get; set; }
        public string DefaultValue { get; set; }
        public object Value { get; set; }
    }
}