using System;
using System.Collections.Generic;
using UnityEngine;

namespace FullInspector {


    /// <summary>
    ///     Attribute for customized curves.<br />
    ///     To use on collections, add <br />
    ///     [<see cref="InspectorCollectionItemAttributesAttribute"/>(typeof(<see cref="InspectorMovementCurvesDefault"/>))]
    ///     <br />
    ///     Or create a custom class, similar to <see cref="InspectorMovementCurvesDefault" />
    ///     instead.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class InspectorCurveAttribute : Attribute {
        public float TimeStart;
        public float TimeEnd;

        public float ValueStart;
        public float ValueEnd;

        public InspectorCurveAttribute(float timeStart = 0, float valueStart = 1, float timeEnd = 1, float valueEnd = 1) {
            TimeStart = Mathf.Max(0, timeStart);
            ValueStart = valueStart;
            TimeEnd = timeEnd;
            ValueEnd = valueEnd;
        }
    }


    /// <summary>
    ///     Uses the default constructor from <see cref="InspectorCurveAttribute" /><br />
    ///     Usage:<br />
    ///     [<see cref="InspectorCollectionItemAttributesAttribute" />(typeof(<see cref="InspectorMovementCurvesDefault" />))]
    /// </summary>
    public class InspectorMovementCurvesDefault: fiICollectionAttributeProvider {
        public IEnumerable<object> GetAttributes() {
            yield return new InspectorCurveAttribute();
        }
    }

}