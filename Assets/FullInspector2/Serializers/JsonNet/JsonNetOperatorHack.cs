namespace FullInspector.Serializers.JsonNet {
    /// <summary>
    /// Normally we would use the JsonNetSerializer context object to fetch the active ISerializationOperator
    /// instance, but WinRT breaks the StreamingContext object. We use this hack to fix the WinRT build.
    /// </summary>
    public static class JsonNetOperatorHack {
        public static ISerializationOperator ActivateOperator;
    }
}