namespace Core.Structure
{
    public static class GlobalStorage
    {
        public const float MAX_ZOOM = 10f;
        public const float MOVE_SPEED = 10f;
        public const float SNAP_DISTANCE = 0.1f;
        public const float PIXEL_TO_UNIT_RATIO = 100f;
        public const float SCALE_SNAP_DISTANCE = 0.01f;
        public const string EMPTY_ENTRY_KEY = "EMPTY";
        
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
        
        public static class SettingKeys
        {
            public static class Display
            {
                public const string LANGUAGE = "Language";
                public const string SCREEN_MODE = "Screen mode";
                public const string FRAME_RATE_LIMIT = "Frame Rate Limit";
                public const string V_SYNC = "VSync";
                public const string ANTI_ALIASING = "Anti-aliasing";
            }

            public static class Controls
            {
                public const string COLOR_CHANGE_SPEED = "Color change speed";
                public const string DRAG_SPEED = "Drag speed";
                public const string SCROLL_SPEED = "Scroll speed";
                public const string RESTRICT_MOVEMENT_TO_BACKGROUND_SIZE = "Restrict movement to background size";
                public const string DELETE_NODES_OUTSIDE_BACKGROUND = "Delete nodes outside background";
            }

            public static class Graph
            {
                public const string TARGET_METRIC = "Target metric";
                public const string LOW_VALUE_COLOR = "Low value color";
                public const string HIGH_VALUE_COLOR = "High value color";
            }
        }
    }
}