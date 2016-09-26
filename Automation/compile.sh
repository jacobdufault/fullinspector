ROOT_DIR=$(pwd)
WORK_DIR="Output"
mkdir "$WORK_DIR"
cd "$WORK_DIR"

# Output DLL names.
OUTPUT_RUNTIME_NAME=FullInspector
OUTPUT_EDITOR_NAME=FullInspector-Editor

# Fetch sources to use.
ALL_SOURCES=$(find "$ROOT_DIR" -name *.cs | grep -v 'Test\|Generated\|JsonNet')
RUNTIME_SOURCES=$(echo "$ALL_SOURCES" | grep -v 'Editor/')
EDITOR_SOURCES=$(echo "$ALL_SOURCES" | grep 'Editor/')
UNITY_DLL_FOLDER="$ROOT_DIR/Automation"

# Common compilation settings.
COMPILER="mcs
  /lib:$ROOT_DIR/Automation
  /reference:UnityEngine.dll
  /reference:UnityEngine.Networking.dll
  /nowarn:1591
  /target:library /debug /sdk:2"

# Compile runtime and editor DLLs.
$COMPILER \
  /out:"$OUTPUT_RUNTIME_NAME.dll" /doc:"$OUTPUT_RUNTIME_NAME.xml" \
  $RUNTIME_SOURCES
$COMPILER \
  /reference:"$OUTPUT_RUNTIME_NAME.dll" \
  /reference:UnityEditor.dll \
  /out:"$OUTPUT_EDITOR_NAME.dll" /doc:"$OUTPUT_EDITOR_NAME.xml" \
  $EDITOR_SOURCES

# Put editor files into editor folder.
mkdir Editor
mv "$OUTPUT_EDITOR_NAME.*" Editor
cd $ROOT_DIR
zip --junk-paths -r "FullInspector-DLLs.zip" $(find $WORK_DIR)
rm -rf "$WORK_DIR"
