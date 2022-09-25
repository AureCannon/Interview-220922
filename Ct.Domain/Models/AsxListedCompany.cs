namespace Ct.Domain.Models
{
    public class AsxListedCompany
    {
        private const char CsvDelimeter = ',';
        private const char ValueSymbol = '"';

        private AsxListedCompany() : this("", "", "", true)
        {
        }

        private AsxListedCompany(string companyName, string asxCode, string gicsIndustryGroup, bool isEmpty = false)
        {
            CompanyName = companyName;
            AsxCode = asxCode;
            GicsIndustryGroup = gicsIndustryGroup;
            IsEmpty = isEmpty;
        }

        public string CompanyName { get; }
        public string AsxCode { get; }
        public string GicsIndustryGroup { get; }

        public bool IsEmpty { get; }
        public static AsxListedCompany Empty => new();

        public static AsxListedCompany CreateFrom(string csv)
        {
            var values = csv.Split(CsvDelimeter)
                            .Select(Trim)
                            .ToList();

            var isEmptyValues = values.All(x => string.IsNullOrEmpty(x));

            if (isEmptyValues)
                return Empty;

            return new AsxListedCompany(values[0], values[1], values[2]);
        }

        private static string Trim(string value)
        {
            return value.TrimStart(ValueSymbol).TrimEnd(ValueSymbol).Trim();
        }
    }
}
