using Ct.Domain.Models;
using Ct.Domain.Models.Streams;

namespace Ct.Domain.Parsers
{
    public sealed class CsvFileStreamParser : ICsvFileStreamParser
    {
        public Dictionary<string, List<AsxListedCompany>> Parse(TemporaryFileStream tempFileStream)
        {
            var result = new Dictionary<string, List<AsxListedCompany>>(StringComparer.OrdinalIgnoreCase);

            if (!tempFileStream.TryCreateStreamReader(out var fileStream))
                return result;

            using (fileStream)
            {
                if (fileStream == null)
                    return result;

                using var reader = new StreamReader(fileStream);
                return GetCompanies(reader);
            }
        }

        private static Dictionary<string, List<AsxListedCompany>> GetCompanies(StreamReader reader)
        {
            var result = new Dictionary<string, List<AsxListedCompany>>(StringComparer.OrdinalIgnoreCase);

            var row = 0;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                if (row <= 2 || string.IsNullOrEmpty(line))
                {
                    row++;
                    continue;
                }

                var asxCompany = AsxListedCompany.CreateFrom(line);

                var isCompanyAddedToResult = result.TryAdd(asxCompany.AsxCode, new List<AsxListedCompany> { asxCompany });

                if (!isCompanyAddedToResult)
                {
                    var companies = result[asxCompany.AsxCode];

                    companies.Add(asxCompany);
                }

                row++;
            }

            return result;
        }
    }
}
