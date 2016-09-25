#if UNITY_IOS || UNITY_WEBPLAYER || UNITY_WINRT
#error The default Json.NET implementation does not support this export platform. Please open up the serializer manager to remove it. You can open the serializer manager by clicking the menu item "Window/Full Inspector/Developer/Show Serializer Manager"; switch to the desktop export if you do not see the menu item.
#endif