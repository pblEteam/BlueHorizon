#if GRIFFIN
namespace Pinwheel.Griffin
{
    public enum GTerrainResourceFlag
    {
        HeightMap, AlbedoMap, MetallicMap, SplatControlMaps, MaskMap, TreeInstances, GrassInstances,
#if __MICROSPLAT_POLARIS__ && __MICROSPLAT_STREAMS__
        StreamMap
#endif
    }
}
#endif
