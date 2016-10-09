using System;

namespace FullInspector.Internal {
    public interface fiILoadedSerializers {
        Type DefaultSerializerProvider { get; }
        Type[] AllLoadedSerializerProviders { get; }
    }
}