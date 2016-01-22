.. highlight:: csharp

Q/A
===

This document contains answers to common questions regarding Full Inspector.

I cannot derive from BaseBehavior or BaseScriptableObject
---------------------------------------------------------


The easiest way to handle this scenario is to go and modify ``CommonBaseBehavior`` (in namespace FullInspector.Internal) so that it extends from your required parent instead of ``MonoBehavior``. You'll have to redo this per update, but ``CommonBaseBehavior`` is extremely unlikely to be removed (it performs type erasure so that we can get a custom editor for all ``BaseBehavior`` derived types).

A similar process can be done for ``BaseScriptableObject``; just modify ``CommonBaseScriptableObject`` (in namespace FullInspector.Internal).

If this does or cannot work for some really odd reason, then please send an email or create an issue.


Does Full Inspector impact runtime performance?
-----------------------------------------------

No! The only impact that Full Inspector has is a call to ``Awake`` for every object that requires special serialization support. The ``Awake`` call merely deserializes the object. Sometimes this can actually be faster than Unity deserialization, particularly with protobuf-net when the serialized object graph is large.

Full Inspector imposes no runtime impacts (such as a call to Update); in fact, your code will likely run faster will Full Inspector due to less GC pressure because you can now extensively use structs.

I want to use iOS or another AOT platform
-----------------------------------------

Awesome! Full Inspector Core supports AOT compilation quite well. On iOS, Full Inspector Core currently supports both Strip assemblies and Strip ByteCode.

The other big question w.r.t. AOT compilation is your chosen serializer. For a multitude of reasons, it is *strongly* recommended that you use Full Serializer. You can also use Json.NET for Unity if you have purchased it from the Asset Store.

===============
Full Serializer
===============

It just works :).

===============
BinaryFormatter
===============

You don't have to do anything! Full Inspector will automatically configure BinaryFormatter to use reflection on iOS.

========
Json.NET
========

Please follow these steps to use Json.NET on AOT platforms:

1. Purchase and install `Json.NET for Unity <http://u3d.as/5q2>`_ from the Asset Store.
2. Delete *"FullInspector2/Serializers/JsonNet/DLLS"*

That's it! You're now good to go for AOT platforms!

===================
protobuf-net (beta)
===================

Using protobuf-net on AOT compiled platforms is currently in alpha/beta. You just need to run *"Window/Full Inspector/Developer/Create protobuf-net precompiled serializer"* before an AOT build (while Unity is *not* in iOS or an AOT build environment).

Please note that if you're on OSX, you need to have a separate mono installation for the precompiled serializer. protobuf-net natively supports AOT DLL generation, but these AOT DLLs are not compatible with Unity's build system, so Full Inspector runs a decompiler over it. This decompiler requires .NET 4.0, and unfortunately Unity is a .NET 3.5 environment.


You should now be able to use protobuf-net on AOT platforms. However, this is still alpha/beta.