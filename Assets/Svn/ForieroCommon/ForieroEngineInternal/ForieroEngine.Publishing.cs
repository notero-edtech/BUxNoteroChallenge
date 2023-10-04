namespace ForieroEngine
{
    public static class Publishing
    {
        public enum ReleaseVersions
        {
            NONE,
            DEMO,
            EARLY, 
            FULL,
        }

        public static ReleaseVersions ReleaseVersion 
        {
            get
            {
                #if RELEASE_VERSION_DEMO
                return ReleaseVersions.DEMO;
                #elif RELEASE_VERSION_EARLY
                return ReleaseVersions.EARLY;
                #elif RELEASE_VERSION_FULL
                return ReleaseVersions.FULL;
                #else
                return ReleaseVersions.NONE;
                #endif
            }
        }
    }
}
