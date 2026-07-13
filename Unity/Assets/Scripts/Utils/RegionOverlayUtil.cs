using UnityEngine;

public static class RegionOverlayUtil
{
    public static void SnapOverlayToCollider(PolygonCollider2D col, SpriteRenderer overlay)
    {
        if (col == null || overlay == null || overlay.sprite == null) return;

        Bounds colBounds = GetLocalBounds(col);      // collider local space
        Bounds spriteBounds = overlay.sprite.bounds; // sprite local space

        // Put the overlay in the same parent space as the collider.
        overlay.transform.SetParent(col.transform.parent, worldPositionStays: false);

        // Match the region center. If pivot/origin differs, this corrects it.
        overlay.transform.localPosition = colBounds.center - (Vector3)spriteBounds.center;

        // Match rotation if needed.
        overlay.transform.localRotation = Quaternion.identity;

        // Match size.
        overlay.transform.localScale = new Vector3(
            colBounds.size.x / spriteBounds.size.x,
            colBounds.size.y / spriteBounds.size.y,
            1f
        );
    }

    private static Bounds GetLocalBounds(PolygonCollider2D col)
    {
        var pts = col.GetPath(0);
        if (pts == null || pts.Length == 0)
            return new Bounds(col.offset, Vector3.zero);

        Vector3 first = pts[0] + col.offset;
        Bounds b = new Bounds(first, Vector3.zero);

        for (int i = 1; i < pts.Length; i++)
            b.Encapsulate((Vector3)(pts[i] + col.offset));

        return b;
    }
}