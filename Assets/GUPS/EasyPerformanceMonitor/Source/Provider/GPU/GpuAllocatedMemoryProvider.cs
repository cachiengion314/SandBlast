// Unity
using UnityEngine.Profiling;

namespace GUPS.EasyPerformanceMonitor.Provider
{
    /// <summary>
    /// Fetches the total GPU memory allocated by Unity for the graphics driver.  
    /// This represents the amount of memory currently used by Unity's rendering system.  
    /// Note that this does not reflect the total VRAM or the free GPU memory.
    /// </summary>
    /// <remarks>
    /// The <see cref="GpuAllocatedMemoryProvider"/> class provides information about the GPU memory allocated by Unity's graphics driver.  
    /// This component is only supported in development builds and the Unity Editor.  
    /// It retrieves the allocated GPU memory using <see cref="Profiler.GetAllocatedMemoryForGraphicsDriver"/> and returns the value in bytes (B).
    /// </remarks>
    public class GpuAllocatedMemoryProvider : APerformanceProvider
    {
        /// <summary>
        /// The performance component name.
        /// </summary>
        public const string CName = "GPU Allocated Memory";

        /// <summary>
        /// Returns the performance component name.
        /// </summary>
        public override string Name => CName;

        /// <summary>
        /// Returns true if the performance component is supported on the current platform.
        /// The GPU Allocated Memory component is only supported in development builds and the Unity Editor.
        /// </summary>
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        public override bool IsSupported => true;
#else
        public override bool IsSupported => false;
#endif

        /// <summary>
        /// The default unit is bytes (B).
        /// </summary>
        public override string Unit => "B";

        /// <summary>
        /// Retrieves the total GPU memory allocated by Unity for the graphics driver.
        /// </summary>
        /// <returns>GPU allocated memory in bytes (B).</returns>
        protected override float GetNextValue()
        {
            // Get the allocated GPU memory in bytes.
            return Profiler.GetAllocatedMemoryForGraphicsDriver();
        }
    }
}
