using System.Threading.Tasks;

namespace ScriptService.Tests.Mocks {
    /// <summary>
    /// api to performance endpoints
    /// </summary>
    public interface IPerformanceApi {
        
        /// <summary>
        /// accumulates page performance data
        /// </summary>
        /// <param name="filter">criterias for performance accumulation</param>
        /// <returns>item performance entries</returns>
        Task<object[]> PagePerformance(object filter = null);
        
        /// <summary>
        /// accumulates click performance data
        /// </summary>
        /// <param name="filter">criterias for performance accumulation</param>
        /// <returns>click performance entries</returns>
        Task<object[]> ClickPerformance(object filter = null);

        /// <summary>
        /// determines expected page performance based on buzzword data
        /// </summary>
        /// <param name="data">buzz string containing buzz terms</param>
        /// <returns>computed page performance</returns>
        Task<object[]> DeterminePagePerformance(string data);

        /// <summary>
        /// determines expected page performance based on buzzword data
        /// </summary>
        /// <param name="context">context buzz string is located in</param>
        /// <param name="data">buzz string containing buzz terms</param>
        /// <returns>computed page performance</returns>
        Task<object[]> DeterminePagePerformance(string context, string data);

        /// <summary>
        /// determines expected click performance based on buzzword data
        /// </summary>
        /// <param name="data">buzz string containing buzz terms</param>
        /// <returns>computed click performance</returns>
        Task<object[]> DetermineClickPerformance(string data);

        /// <summary>
        /// determines expected click performance based on buzzword data
        /// </summary>
        /// <param name="context">context buzz string is located in</param>
        /// <param name="data">buzz string containing buzz terms</param>
        /// <returns>computed click performance</returns>
        Task<object[]> DetermineClickPerformance(string context, string data);
    }}