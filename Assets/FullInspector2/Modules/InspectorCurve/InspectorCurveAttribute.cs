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

        public float X;
        public float Y;

        public float Height;
        public float Width;


        /// <summary>
        ///     Creates a curve field constrained to the rect defined by the passed parameters.
        /// </summary>
        public InspectorCurveAttribute(float x = 0, float y = 0, float width = 1, float height = 1) {
            X = x;
            Y = y;
            Width = width;
            Height = height;
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