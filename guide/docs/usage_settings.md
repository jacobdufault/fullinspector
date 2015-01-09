# Settings

You can easily customize how Full Inspector operates by modifying the settings. This is easy to do.

<note>
You can chose to directly modify fiSettings.cs itself, but this is highly discouraged as your modifications will not survive when you upgrade Full Inspector to the next version.
</note>

Full Inspector will automatically instantiate types which derive from `fiSettingsProcessor` and call the `Process` override method. This means that you can place the following content into a file anywhere inside of your project and `Process` will be invoked at an appropriate time to customize the settings instance. This is guaranteed to happen before any setting is actually used.

Here is an example `fiSettingsProcessor` that will disable animation and make labels occupy half of the available width.

```c#
using FullInspector;

public class MySettings : fiSettingsProcessor {
    public void Process() {
        fiSettings.EnableAnimation = false;
        fiSettings.LabelWidthPercentage = .5f;
    }
}
```

<important>
Please note that `Process` may not be called on the main Unity thread - make sure you do not make any Unity API calls (such as calling `==` on two `UnityObject` references).
</important>