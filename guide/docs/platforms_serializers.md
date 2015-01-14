# Serializer Platform Support

The core Full Inspector engine supports every platform. Your select serializer will determine which platforms you can deploy to. For numerous reasons, Full Serializer is strongly recommended over every other serializer.

## Full Serializer

It'll work everywhere without any modifications. Full Serializer will always support every platform that Full Inspector Core supports.

## Json.NET

Full Inspector ships with an essentially vanilla version of Json.NET. This supports the easier platforms (Android, Windows/Linux/OSX) but does not work for the more complex ones like WinRT, iOS, WebGL, or WebPlayer.

Please follow these steps to use Json.NET on all platforms:

- Purchase and install [Json.NET for Unity](http://u3d.as/5q2) from the Asset Store.
- Delete `FullInspector2/Serializers/JsonNet/DLLS`

That’s it! You’re now good to go for AOT platforms!

## protobuf-net (unstable)

Using protobuf-net on AOT compiled platforms is currently in alpha/beta. You just need to run `Window/Full Inspector/Developer/Create protobuf-net precompiled serializer` before an AOT build (while Unity is not in iOS or an AOT build environment).

Please note that if you’re on OSX, you need to have a separate mono installation for the precompiled serializer. protobuf-net natively supports AOT DLL generation, but these AOT DLLs are not compatible with Unity’s build system, so Full Inspector runs a decompiler over it. This decompiler requires .NET 4.0, and unfortunately Unity is a .NET 3.5 environment.

You should now be able to use protobuf-net on AOT platforms. However, this is still unstable software and may have issues.

## BinaryFormatter

Best to stick with only desktop builds. WebPlayer and iOS have some glitches. Full Inspector will automatically configure BinaryFormatter to not use its JIT on AOT platforms.