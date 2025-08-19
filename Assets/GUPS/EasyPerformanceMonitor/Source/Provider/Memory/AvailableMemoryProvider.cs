// Unity
using UnityEngine;

namespace GUPS.EasyPerformanceMonitor.Provider
{
    /// <summary>
    /// Fetches the total system memory available to the application. This represents the total physical RAM installed on the device.  
    /// Note that this does not reflect the actual memory usage of the application.
    /// </summary>
    /// <remarks>
    /// The <see cref="AvailableMemoryProvider"/> class provides information about the total system memory (RAM) available on the device.  
    /// This component is supported on all platforms and retrieves the total system memory using <see cref="SystemInfo.systemMemorySize"/>.
    /// </remarks>
    public class AvailableMemoryProvider : APerformanceProvider
    {
        /// <summary>
        /// The performance component name.
        /// </summary>
        public const string CName = "Available Memory";

        /// <summary>
        /// Returns the performance component name.
        /// </summary>
        public override string Name => CName;

        /// <summary>
        /// Returns true if the performance component is supported on the current platform.
        /// The Available Memory component is supported on all platforms.
        /// </summary>
        public override bool IsSupported => true;

        /// <summary>
        /// The default unit is megabytes.
        /// </summary>
        public override string Unit => "MB";

        /// <summary>
        /// Retrieves the total system memory available on the device.
        /// </summary>
        /// <returns>Total system memory in megabytes (MB).</returns>
        protected override float GetNextValue()
        {
            // Get the total system memory in megabytes.
            return SystemInfo.systemMemorySize;
        }
    }
}
