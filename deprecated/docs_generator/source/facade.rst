.. highlight:: csharp

Facade
======

The ``Facade<T>`` is best thought of as an inline template object that is stored directly in a ``BaseBehavior`` instance, without having to go through the hassle of creating a separate prefab asset.

Here is a (silly) example of usage. ``Facade<T>`` is most useful when you want a generic object definition but do not want to construct instances of that object type when editing it.

.. code::

    using FullInspector;

    public class FacadeSample : BaseBehavior {
        public Facade<DelayedProgram> DelayedProgram;

        public void OnEnable() {
            var delayedProgram = gameObject.AddComponent<DelayedProgram>();
            DelayedProgram.PopulateInstance(ref delayedProgram);

            delayedProgram.RunProgram();
        }
    }

.. code::

    using FullInspector;
    using System.Collections;
    using UnityEngine;

    public class DelayedProgram : BaseBehavior {
        public float Delay;
        public string OutputMessage;

        public void RunProgram() {
            StartCoroutine(PrintMessage());
        }

        protected IEnumerator PrintMessage() {
            yield return new WaitForSeconds(Delay);
            Debug.Log(OutputMessage);
        }
    }

Here is an example of the UI created by the facade:

.. image:: static/facade/facade_ux.png
