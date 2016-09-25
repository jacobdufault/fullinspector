// Copyright (c) 2012-2013 Rotorz Limited. All rights reserved. Use of this source code is governed
// by a BSD-style license that can be found in the LICENSE file.

using UnityEngine;
using UnityEditor;

using System;
using System.Reflection;

namespace FullInspector.Rotorz.ReorderableList.Internal {

    /// <summary>
    /// Utility functions to assist with GUIs.
    /// </summary>
    internal static class RotorzGUIHelper {

        static RotorzGUIHelper() {
            var tyGUIClip = typeof(GUI).Assembly.GetType("UnityEngine.GUIClip");
            if (tyGUIClip != null) {
#if false
                var piEnabled = tyGUIClip.GetProperty("enabled", BindingFlags.Static | BindingFlags.Public);
                if (piEnabled != null) {
                    var getGetMethod = piEnabled.GetGetMethod();
                    _guiClipEnabled = () => (bool)getGetMethod.Invoke(null, null);
                }
#endif

                var piVisibleRect = tyGUIClip.GetProperty("visibleRect", BindingFlags.Static | BindingFlags.Public);
                if (piVisibleRect != null) {
                    var getGetMethod = piVisibleRect.GetGetMethod();
                    _guiClipVisibleRect = () => (Rect)getGetMethod.Invoke(null, null);
                }
            }

            var miFocusTextInControl = typeof(EditorGUI).GetMethod("FocusTextInControl", BindingFlags.Static | BindingFlags.Public);
            if (miFocusTextInControl == null)
                miFocusTextInControl = typeof(GUI).GetMethod("FocusControl", BindingFlags.Static | BindingFlags.Public);

            FocusTextInControl = str => miFocusTextInControl.Invoke(null, new object[] { str });
        }

        public static bool VisibleRectEnabled {
            get {
                // TODO: Rotorz clipping appears to be broken at the moment. Since we have the large-collection editor,
                //       we can likely get away with not supporting it, but we really should at some-point in the future.
                return false;

#if false
                if (_guiClipEnabled == null) return true;

                // We are disabled if the query to get the visible rect fails
                try {
                    _guiClipVisibleRect();
                } catch (Exception) {
                    return false;
                }

                return _guiClipEnabled();
#endif
            }
        }
#if false
        private static Func<bool> _guiClipEnabled;
#endif

        /// <summary>
        /// Gets visible rectangle within GUI.
        /// </summary>
        /// <remarks>
        /// <para>VisibleRect = TopmostRect + scrollViewOffsets</para>
        /// </remarks>
        public static Rect VisibleRect {
            get {
                return _guiClipVisibleRect();
            }
        }
        private static Func<Rect> _guiClipVisibleRect;

        /// <summary>
        /// Focus control and text editor where applicable.
        /// </summary>
        public static Action<string> FocusTextInControl;

    }

}