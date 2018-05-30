# Full Inspector

Full Inspector supercharges the Unity inspector.

- [Documentation](http://jacobdufault.github.io/fullinspector/guide/)

# Checking out the code

Full Inspector relies on a sparse submodule checkout of Full Serializer, which makes settings up the repro locally slightly convoluted. Here is the recommended method:

```bash
$ # Standard git clone with submodule
$ git clone --recursive https://github.com/jacobdufault/FullInspector
$ cd FullInspector
$ # Setup sparse checkout
$ git config -f .git/modules/Assets/FullSerializer/config --add core.sparsecheckout true
$ echo Assets/FullSerializer/Source > .git/modules/Assets/FullSerializer/info/sparse-checkout 
$ # Remove any content from submodule that is not in the sparse checkout
$ rm -rf Assets/FullSerializer && git checkout Assets && git submodule update
```
