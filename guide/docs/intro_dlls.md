# Using Full Inspector in DLL Format

It's really easy to deploy Full Inspector in DLL format (versus using the source code). DLLs have a few benefits, such as reduced compile time. You can import Full Inspector as a DLL via one of two ways:

## Method 1: Using the precompiled DLLs

Full Inspector ships with DLLs that are already built for you. You can use them by importing the `FullInspector2/PrecompiledDLLs.unitypackage` package and then removing the `FullInspector2/Core` and `FullInspector2/Modules` folders.

## Method 2: Building the DLLs manually

You can also build the Full Inspector DLLs yourself if you wish. The builder is included as a set of csproj files and a solution. These files are already included as `FullInspector2/PrecompiledDLLs-ProjFiles.zip`, but you can download them [here](assets/FullInspector-DLL-ProjFiles.zip) as well.

Assuming that the standard Unity project looks like this:

```
Assets\*
Library\*
```

unzip the project files so that it is next to assets, like so:

```
Assets\*
Library\*
FullInspector-DLL-ProjFiles\*
```

You can then open up `FullInspector-DLL-ProjFiles/FullInspector.sln` and build it. You will likely have to build this solution twice; the first build will fail because the editor assembly depends on the runtime assembly which hasn't been built yet.

You will then find the actual DLL files at `FullInspector-DLL-ProjFiles/DLLs`. Copy these files into your Assets folder and delete `Assets/FullInspector2/Core` and `Assets/FullInspector2/Modules`.

### Helper Batch Script for Building the DLLs

If you're making modifications to Full Inspector but are using it in a separate project in DLL format, here is an example batch script that will automatically build it as a DLL and then copy it over to your target project.

```bash
@echo off

REM Setup variables
set FI2_SLN="C:\Users\jacob\Full Inspector\FullInspector-DLL-ProjFiles\FullInspector.sln"
set MSBUILD_DIR="C:\Program Files (x86)\MSBuild\12.0\Bin\MSBuild.exe"
set BUILD_DLL_DIR="C:\Users\jacob\Full Inspector\FullInspector-DLL-ProjFiles\DLLs"
set OUTPUT_DLL_DIR="C:\Users\jacob\Game\Assets\External\FullInspector2\"

REM Build it twice to avoid any circular dependency issues
%MSBUILD_DIR% %FI2_SLN%
%MSBUILD_DIR% %FI2_SLN%

REM Copy the DLL files over
robocopy /S %BUILD_DLL_DIR% %OUTPUT_DLL_DIR%
```