ROOT_DIR=$(pwd)
mkdir "AutomationOutput"
cd "AutomationOutput"

# Make sure the compiler is installed.
sudo apt-get install mcs-mono --fix-missing

# Output DLL names.
OUTPUT_RUNTIME_NAME=FullInspector
OUTPUT_EDITOR_NAME=FullInspector-Editor

# Fetch sources to use.
ALL_SOURCES=$(find "$ROOT_DIR" -name *.cs | grep -v 'Test\|Generated')
RUNTIME_SOURCES=$(echo "$ALL_SOURCES" | grep -v 'Editor')
EDITOR_SOURCES=$(echo "$ALL_SOURCES" | grep 'Editor')
UNITY_DLL_FOLDER="$ROOT_DIR/Automation"

# Common compilation settings.
COMPILER=mcs \
  /lib:"$UNITY_DLL_FOLDER" \
  /reference:UnityEngine.dll \
  /reference:UnityEngine.Networking.dll \
  /nowarn:1591 \
  /target:library /debug /sdk:2

# Compile runtime and editor DLLs.
$COMPILER \
  /out:"$OUTPUT_RUNTIME_NAME.dll" /doc:"$OUTPUT_RUNTIME_NAME.xml" \
  $RUNTIME_SOURCES
$COMPILER \
  /reference:"$OUTPUT_RUNTIME_NAME.dll" \
  /out:"$OUTPUT_EDITOR_NAME.dll" /doc:"$OUTPUT_EDITOR_NAME.xml" \
  $EDITOR_SOURCES

# Put editor files into editor folder.
mkdir Editor
mv "$OUTPUT_EDITOR_NAME.*" Editor
cd $ROOT_DIR
zip --junk-paths -r "FullInspector-Latest.zip" $(find AutomationOutput)
