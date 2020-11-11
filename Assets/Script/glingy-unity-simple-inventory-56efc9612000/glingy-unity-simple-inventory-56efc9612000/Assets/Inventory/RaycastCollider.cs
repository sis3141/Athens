using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RaycastCollider : Graphic {

    private RectTransform rect;

    public override void SetMaterialDirty () { return; }
    public override void SetVerticesDirty () { return; }

    protected override void OnPopulateMesh (VertexHelper vh) {
        vh.Clear ();
        return;
    }

    private new void Awake () {
        base.Awake ();
        rect = GetComponent<RectTransform> ();
    }

    private void OnDrawGizmosSelected () {
        if (rect != null) {
            Vector2 pos = rect.transform.position;

            float xDist = rect.sizeDelta.x * 0.5f;
            float yDist = rect.sizeDelta.y * 0.5f;

            Vector2 leftUp = new Vector2 (pos.x - xDist, pos.y + yDist);
            Vector2 rightUp = new Vector2 (pos.x + xDist, pos.y + yDist);
            Vector2 leftDown = new Vector2 (pos.x - xDist, pos.y - yDist);
            Vector2 rightDown = new Vector2 (pos.x + xDist, pos.y - yDist);

            Gizmos.color = Color.green;
            Gizmos.DrawLine (leftUp, rightUp);
            Gizmos.DrawLine (rightUp, rightDown);
            Gizmos.DrawLine (rightDown, leftDown);
            Gizmos.DrawLine (leftDown, leftUp);
        }
    }
}
