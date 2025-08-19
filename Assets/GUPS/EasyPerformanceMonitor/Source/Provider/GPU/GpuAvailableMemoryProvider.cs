// Unity
using UnityEngine;

namespace GUPS.EasyPerformanceMonitor.Provider
{
    /// <summary>
    /// Fetches the total GPU memory available on the device.
    /// This represents the total video memory (VRAM) that the GPU has access to.  
    /// Note that this does not reflect the actual available or used GPU memory.
    /// </summary>
    /// <remarks>
    /// The <see cref="GpuAvailableMemoryProvider"/> class provides information about the total GPU memory (VRAM) available on the device.  
    /// This component is supported on all platforms and retrieves the total graphics memory using <see cref="SystemInfo.graphicsMemorySize"/>.
    /// </remarks>
    public class GpuAvailableMemoryProvider : APerformanceProvider
    {
        /// <summary>
        /// The performance component name.
        /// </summary>
        public const string CName = "Available GPU Memory";

        /// <summary>
        /// Returns the performance component name.
        /// </summary>
        public override string Name => CName;

        /// <summary>
        /// Returns true if the performance component is supported on the current platform.
        /// The Available GPU Memory component is supported on all platforms.
        /// </summary>
        public override bool IsSupported => true;

        /// <summary>
        /// The default unit is megabytes (MB).
        /// </summary>
        public override string Unit => "MB";

        /// <summary>
        /// Retrieves the total GPU memory available on the device.
        /// </summary>
        /// <returns>Total GPU memory in megabytes (MB).</returns>
        protected override float GetNextValue()
        {
            // Get the total GPU memory in megabytes.
            return SystemInfo.graphicsMemorySize;
        }
    }
}
