# Full Inspector

Full Inspector supercharges the Unity inspector.

- [Documentation](http://jacobdufault.github.io/fullinspector/guide/)
- Free license (GPLv3, this GitHub repro)
- [Commercial license](https://www.assetstore.unity3d.com/en/#!/content/14913)

# Licensing

Full Inspector is available under two different licenses. You can use either one, depending on your needs.
- GPLv3: This can be used so long as you abide by the its terms and publish your source code.
- Commercial: If you do not wish to publish your game's source code, please purchase a commerical license from the Unity Asset Store.

## What this means

If you just want to try Full Inspector out, the GPLv3 license is fine. If you distribute your game in any way, then you need to make the source code available if you continue to use the GPLv3 licensed version.

If you want to use Full Inspector in a project where you do not publish source code, then you need to purchase a license from the Unity Asset Store.

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
