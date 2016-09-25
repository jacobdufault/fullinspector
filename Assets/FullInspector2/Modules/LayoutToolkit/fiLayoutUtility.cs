namespace FullInspector.LayoutToolkit {
    public static class fiLayoutUtility {
        /// <summary>
        /// Returns a layout that surrounds the given layout with a margin on all sides of the
        /// given size.
        /// </summary>
        public static fiLayout Margin(float margin, fiLayout layout) {
            return new fiHorizontalLayout {
                margin,
                new fiVerticalLayout {
                    margin,
                    layout,
                    margin
                },
                margin
            };
        }
    }
}