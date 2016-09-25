using FullInspector.Internal;
using UnityEngine;

namespace FullInspector {
    public partial class tk<T, TContext> {
        public class Comment : tkControl<T, TContext> {
            private readonly Value<string> _comment;
            private readonly CommentType _commentType;

            public Comment(Value<string> comment, CommentType commentType) {
                _comment = comment;
                _commentType = commentType;
            }

            protected override T DoEdit(Rect rect, T obj, TContext context, fiGraphMetadata metadata) {
                var comment = _comment.GetCurrentValue(obj, context);
                fiLateBindings.EditorGUI.HelpBox(rect, comment, _commentType);
                return obj;
            }

            protected override float DoGetHeight(T obj, TContext context, fiGraphMetadata metadata) {
                var comment = _comment.GetCurrentValue(obj, context);
                return fiCommentUtility.GetCommentHeight(comment, _commentType);
            }
        }
    }
}