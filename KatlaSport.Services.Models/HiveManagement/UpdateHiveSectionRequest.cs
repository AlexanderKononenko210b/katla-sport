
namespace KatlaSport.Services.HiveManagement
{
    /// <summary>
    /// Present info about about HiveSection
    /// </summary>
    public class UpdateHiveSectionRequest
    {
        /// <summary>
        /// Gets or sets a store hive name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a store hive code.
        /// </summary>
        public string Code { get; set; }
    }
}
