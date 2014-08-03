.. highlight:: csharp

Shared Instance
===============

Have you ever wanted to share a single object instance across multiple ``BaseBehaviors`` without having to deal with the
hassle and non-intuitive UX of ``ScriptableObjects``? ``SharedInstance<T>`` is here for you!

.. code:: c#

    using FullInspector;

    public class MySharedInstance {
        public int A, B, C;
    }

    public class MyDemoBehavior1 : BaseBehavior {
        public SharedInstance<MySharedInstance> Shared; 
    }

    public class MyDemoBehavior2 : BaseBehavior {
        public SharedInstance<MySharedInstance> Shared; 
    }

Now, instances of ``MyDemoBehavior1`` and ``MyDemoBehavior2`` can share the exact same object instance.

Example User Story - Prefab Variants
------------------------------------

Notice that ``SharedInstance<T>`` does a lot more than it seems. It enables extremely easy data-sharing between prefabs, providing
a nice solution to Unity's current lack of nested prefabs. For example, let's say that we have multiple variants of *RomboidZombie*.

.. code:: c#

    using FullInspector;
    using UnityEngine;

    public class ZombieStats {
        public float Health;
        public float AttackStrength;
        public float RegenerationRate;
    }

    public class Zombie : BaseBehavior {
        public SharedInstance<ZombieStats> Stats;

        protected void OnEnable() {
            Debug.Log("This zombie has " + Stats.Instance.Health + " health!");
        }
    }


We want to have variants *RomboidZombie_Small*, *RomboidZombie_Big*, and *RomboidZombie_Fast*.

Without Full Inspector, we would need to either have one *RomboidZombie* prefab and then have three prefab instances (one for *small*, *big*, and *fast*). However, this is hard to maintain in large projects.

If we think about it, the only data that is really shared across the three zombie types is the ``ZombieStats`` instance. Since the goal for prefabs is reuse, we can just use ``SharedInstance<ZombieStats>`` to reuse the same ``ZombieStats`` instance across three new prefabs!

Here we create a new ``ZombieStats`` instance for one of our zombies. We shall share this ``ZombieStats`` instance across our three prefabs.

.. image:: static/shared_instance/zombie_create_instance.gif

Here we demonstrate that the ``ZombieStats`` instance is actually being shared, and how easy it is to find the instance you want and quickly view or make changes to it.

.. image:: static/shared_instance/zombie_instance_selection.gif
