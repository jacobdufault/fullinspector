using UnityEngine;

namespace FullInspector.Internal {
    /// <summary>
    /// Contains common functions to help manipulate rects.
    /// </summary>
    public static class fiRectUtility {
        public const float IndentHorizontal = 15f;
        public const float IndentVertical = 2f;

        /// <summary>
        /// Indents the given rect.
        /// </summary>
        public static Rect IndentedRect(Rect source) {
            return new Rect(source.x + IndentHorizontal, source.y + IndentVertical,
                source.width - IndentHorizontal, source.height - IndentVertical);
        }

        /// <summary>
        /// Moves the rect down (vertically) by the given amount. Returns an updated rect.
        /// </summary>
        public static Rect MoveDown(Rect rect, float amount) {
            rect.y += amount;
            rect.height -= amount;

            return rect;
        }

        /// <summary>
        /// Splits the rect into two horizontal ones, with the left rect set to an exact width.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="rightWidth">The width of the left rect.</param>
        /// <param name="margin">The amount of space between the two rects.</param>
        /// <param name="left">The new left rect.</param>
        /// <param name="right">The new right rect.</param>
        public static void SplitLeftHorizontalExact(Rect rect, float leftWidth, float margin,
            out Rect left, out Rect right) {

            left = rect;
            right = rect;

            left.width = leftWidth;

            right.x += leftWidth + margin;
            right.width -= leftWidth + margin;
        }

        public static void SplitRightHorizontalExact(Rect rect, float rightWidth, float margin, out Rect left, out Rect right) {
            left = new Rect(rect);
            left.width -= rightWidth + margin;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width = rightWidth;
        }

        /// <summary>
        /// Splits a rect into two, with the split occurring at a certain percentage of the rect's
        /// width.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="percentage">The percentage to split the rect at.</param>
        /// <param name="margin">The margin between the two split rects.</param>
        /// <param name="left">The new left rect.</param>
        /// <param name="right">The new right rect.</param>
        public static void SplitHorizontalPercentage(Rect rect, float percentage, float margin,
            out Rect left, out Rect right) {

            left = new Rect(rect);
            left.width *= percentage;

            right = new Rect(rect);
            right.x += left.width + margin;
            right.width -= left.width + margin;
        }

        public static void SplitHorizontalMiddleExact(Rect rect, float middleWidth, float margin, out Rect left,
            out Rect middle, out Rect right) {

            left = new Rect(rect);
            left.width = (rect.width - (2 * margin) - middleWidth) / 2;

            middle = new Rect(rect);
            middle.x += left.width + margin;
            middle.width = middleWidth;

            right = new Rect(rect);
            right.x += left.width + margin + middleWidth + margin;
            right.width = (rect.width - (2 * margin) - middleWidth) / 2;
        }

        public static void SplitHorizontalFlexibleMiddle(Rect rect, float leftWidth, float rightWidth, out Rect left,
            out Rect middle, out Rect right) {

            left = new Rect(rect);
            left.width = leftWidth;

            middle = new Rect(rect);
            middle.x += left.width;
            middle.width = rect.width - leftWidth - rightWidth;

            right = new Rect(rect);
            right.x += left.width + middle.width;
            right.width = rightWidth;
        }

        public static void CenterRect(Rect toCenter, float height, out Rect centered) {
            float extraHeight = toCenter.height - height;
            centered = new Rect(toCenter);
            centered.y += extraHeight / 2;
            centered.height = height;
        }

        public static void Margin(Rect container, float horizontalMargin, float verticalMargin, out Rect smaller) {
            smaller = container;
            smaller.x += horizontalMargin;
            smaller.width -= horizontalMargin * 2;
            smaller.y += verticalMargin;
            smaller.height -= verticalMargin * 2;
        }

        /// <summary>
        /// Splits a rect into two, with the split occurring at a certain percentage of the rect's
        /// height.
        /// </summary>
        /// <param name="rect">The rect to split.</param>
        /// <param name="percentage">The percentage to split the rect at.</param>
        /// <param name="margin">The margin between the two split rects.</param>
        /// <param name="top">The new top rect.</param>
        /// <param name="bottom">The new bottom rect.</param>
        public static void SplitVerticalPercentage(Rect rect, float percentage, float margin,
            out Rect top, out Rect bottom) {

            top = new Rect(rect);
            top.height *= percentage;

            bottom = new Rect(rect);
            bottom.y += top.height + margin;
            bottom.height -= top.height + margin;
        }
    }
}