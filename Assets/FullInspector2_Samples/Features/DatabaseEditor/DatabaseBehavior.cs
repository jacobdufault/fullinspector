using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector.Samples.DatabaseEditor {
    public interface ISkillActivator {
    }
    public abstract class BaseSkillEffect {
        public string Name;
    }

    public class TimedActivator : ISkillActivator {
        public float Delay;
    }

    [Flags]
    public enum EventMask {
        EventA, EventB, EventC
    }
    public class EventActivator : ISkillActivator {
        public EventMask EventFilter;
    }

    public class DamagePlayerEffect : BaseSkillEffect {
        public float Damage;
    }
    public class HealPlayerEffect : BaseSkillEffect {
        public float RestoredHealth;
    }
    public class StatDecreaseEffect : BaseSkillEffect {
        public float Armor;
        public float Endurance;
        public float Strength;
        public float Agility;
        public float Duration;
    }

    public class Ability {
        public string Name;
        public int UnlockGoldCost;
        public Texture Image;
        [InspectorTextArea]
        public string Description;

        public List<ISkillActivator> ActivationRequirements;
        public List<BaseSkillEffect> Effects;
    }

    [AddComponentMenu("Full Inspector Samples/Other/Database Behavior")]
    public class DatabaseBehavior : BaseBehavior<FullSerializerSerializer> {
        [InspectorComment(CommentType.Info, "If you have a huge collection of items, for example, a set " +
            "of skills, then the [SingleItemListEditor] attribute can be extremely useful. It " +
            "activates an editor that only shows you one item at a time.")]
        [ShowInInspector]
        [InspectorHidePrimary]
        private int _comment;

        [InspectorDatabaseEditor]
        public List<Ability> Abilities;
    }
}