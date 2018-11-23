using Coverlet.Core;

namespace Testeroid.Reports
{
    public interface IReport
    {
        void Generate(CoverageResult coverageResult);
    }
}