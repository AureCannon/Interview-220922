using Ct.Domain.Models;
using Ct.Domain.Models.Streams;

namespace Ct.Domain.Parsers
{
    public interface ICsvFileStreamParser
    {
        Dictionary<string, List<AsxListedCompany>> Parse(TemporaryFileStream tempFileStream);
    }
}