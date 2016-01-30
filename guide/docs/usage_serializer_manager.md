# Serializer Manager

Please click `Window/Full Inspector/Developer/Show Serializer Manager` to open up the serializer manager. The following window will then display:

![](docs/images/serializer_manager.png)

You can easily manage which serializers you have installed using this window. To remove a serializer, simply hit the red `Remove` button. Don't worry - you'll be able to reimport it using the `Import` button.

You can also change the default serializer using the `Set Default` button. Full Serializer is recommended as the default serializer for a wide variety of reasons (see [here](http://localhost:5000/#docs/platforms_serializers?full_serializer) for why).

## Why can't I remove Full Serializer?

Full Serializer is used internally for a number of features like the [backup service](#docs/workflow_backups) and the [graph metadata engine](#docs/extending_metadata). Don't worry though - Full Serializer works everywhere.
