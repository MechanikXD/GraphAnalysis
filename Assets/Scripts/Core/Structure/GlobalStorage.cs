namespace Core.Structure
{
    public static class GlobalStorage
    {
        public const float MAX_ZOOM = 10f;
        public const float MOVE_SPEED = 10f;
        public const float SNAP_DISTANCE = 0.1f;
        public const float PIXEL_TO_UNIT_RATIO = 100f;
        public const float SCALE_SNAP_DISTANCE = 0.01f;
        
        public static class InfoKeys
        {
            public const string LOG_PROCESSING_STATS = "LOG_PROCESSING_STATS";
            public const string LOG_PROCESSING_STATS_FINISHED = "LOG_PROCESSING_STATS_FINISHED";
            public const string LOG_DATA_LOAD_SUCCESS = "LOG_DATA_LOAD_SUCCESS";
            public const string WARNING_NODE_REMOVED = "WARNING_NODE_REMOVED";
            public const string LOG_BACKGROUND_UPDATED = "LOG_BACKGROUND_UPDATED";
            public const string LOG_SETTINGS_UPDATED = "LOG_SETTINGS_UPDATED";
            public const string WARNING_FILE_NOT_FOUND = "WARNING_FILE_NOT_FOUND";
            public const string WARNING_IMAGE_NOT_VALID = "WARNING_IMAGE_NOT_VALID";
            public const string FILE_EXPORT_SUCCESS = "FILE_EXPORT_SUCCESS";
        }

        public static class EdgeData
        {
            public const float CROP_WHEN_TWO_SIDED = 0.6f;
            public const float CROP_WHEN_ONE_SIDED = 0.4f;
            public const float OFFSET_WHEN_ONE_SIDED = 0.1f;
            public const float ARROW_WIDTH = 0.05f;
        }
    }
}