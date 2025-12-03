using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Self-contained orbit + sprite trail system — creates everything in code (no inspector refs).
/// Attach to an empty GameObject and press Play.
/// </summary>
[DisallowMultipleComponent]
public class AutoOrbitSpriteTrail : MonoBehaviour
{
    // ------ Tunables (edit in code if you want) ------
    float radius = 2f;
    Vector3 pivotRotationPerSecond = new Vector3(0f, 180f, 0f); // degrees/sec
    bool clockwise = true;

    Color dotColor = new Color(1f, 0.95f, 0f, 1f);
    Color trailColor = new Color(1f, 0.95f, 0f, 1f);

    float spawnInterval = 0.018f;
    float pieceLifetime = 0.45f;
    float pieceLength = 0.45f;
    float pieceWidth = 0.12f;
    int poolCapacity = 80;

    // ---------- internals ----------
    Transform pivot;
    Transform dot;
    Sprite dotSprite;
    Sprite trailSprite;
    Material spriteMat;

    float spawnTimer;
    Vector3 lastDotPos;

    Queue<GameObject> pool = new Queue<GameObject>();
    List<ActivePiece> active = new List<ActivePiece>();

    class ActivePiece
    {
        public GameObject go;
        public SpriteRenderer sr;
        public float t;
        public float life;
    }

    void Start()
    {
        // create material (uses Sprites/Default so it appears correctly)
        Shader s = Shader.Find("Sprites/Default");
        spriteMat = new Material(s);

        // generate small soft circle texture for dot & trail (alpha falloff)
        dotSprite = Sprite.Create(CreateSoftCircleTexture(64, 64), new Rect(0, 0, 64, 64), new Vector2(0.5f, 0.5f), 100f);
        // reuse same sprite for pieces (we will stretch it)
        trailSprite = dotSprite;

        // create pivot (this GameObject acts as pivot)
        pivot = this.transform;

        // create dot object
        GameObject dotGO = new GameObject("OrbitDot");
        dotGO.transform.SetParent(pivot, false);
        dot = dotGO.transform;
        var srDot = dotGO.AddComponent<SpriteRenderer>();
        srDot.sprite = dotSprite;
        srDot.material = spriteMat;
        srDot.color = dotColor;
        srDot.sortingOrder = 1000;
        // set initial local position at radius on +X
        dot.localPosition = Vector3.right * radius;
        dot.localScale = Vector3.one * 0.25f; // dot size
        lastDotPos = dot.position;

        // warm pool
        for (int i = 0; i < poolCapacity; i++)
            pool.Enqueue(CreatePieceObject(false));
    }

    void Update()
    {
        if (dot == null) return;

        float dir = clockwise ? -1f : 1f;

        // rotate pivot (this GameObject) by per-axis degrees/sec in local space
        Vector3 delta = pivotRotationPerSecond * dir * Time.deltaTime;
        pivot.Rotate(delta, Space.Self);

        // set dot position at radius (works if pivot moved/rotated)
        dot.position = pivot.TransformPoint(Vector3.right * radius);

        // spawn trail pieces on interval
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnPiece();
        }

        // update active pieces (fade/scale/rotate)
        UpdateActivePieces(Time.deltaTime);

        lastDotPos = dot.position;
    }

    // Spawn one piece aligned to motion tangent, placed slightly behind the dot
    void SpawnPiece()
    {
        if (trailSprite == null || dot == null) return;

        Vector3 motion = (dot.position - lastDotPos);
        if (motion.sqrMagnitude < 0.00001f)
        {
            // fallback to pivot's right direction
            motion = pivot.TransformDirection(Vector3.right);
        }
        motion.Normalize();

        // spawn position slightly behind the dot
        Vector3 spawnPos = dot.position - motion * (pieceLength * 0.5f);

        GameObject go = GetFromPool();
        go.SetActive(true);
        go.transform.position = spawnPos;

        // rotation: align sprite +X to motion in world 2D (assume XY plane or XZ depending on your scene)
        // We'll assume Y-up is vertical; if your game uses XZ plane, set rotation accordingly.
        // Here we align in XZ plane (since earlier code used XZ for orbit).
        Vector3 dir = motion;
        // convert tangent (world) to an angle around Y so sprite faces along X in world-space's XZ plane
        float angleY = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
        go.transform.rotation = Quaternion.Euler(90f, -angleY, 0f); // orient quad to face camera from above

        // But if your camera is top-down orthographic facing downwards (common for earlier screenshots),
        // we prefer a rotation that makes the sprite lie flat facing the camera:
        // We'll orient so sprite's plane faces the camera:
        // (The above Euler is a pragmatic choice that works in many top-down setups.)

        // scale so sprite is stretched along X (length) and Y (width) in local space
        go.transform.localScale = new Vector3(pieceLength, pieceWidth, 1f);

        // set sprite renderer and color
        var sr = go.GetComponent<SpriteRenderer>();
        sr.sprite = trailSprite;
        sr.material = spriteMat;
        sr.color = trailColor;
        sr.sortingOrder = 900;

        // add to active list for life handling
        var pd = go.GetComponent<PieceData>();
        pd.ResetLife(pieceLifetime);
        ActivePiece ap = new ActivePiece { go = go, sr = sr, t = 0f, life = pieceLifetime };
        active.Add(ap);
    }

    void UpdateActivePieces(float dt)
    {
        for (int i = active.Count - 1; i >= 0; i--)
        {
            ActivePiece ap = active[i];
            ap.t += dt;
            float r = Mathf.Clamp01(ap.t / ap.life);

            // alpha fade (ease out)
            float alpha = Mathf.Lerp(1f, 0f, r * 1.1f);
            Color c = trailColor;
            c.a = alpha;
            ap.sr.color = c;

            // taper length slightly over life
            float lenMul = Mathf.Lerp(1f, 0.5f, r);
            // reduce width too
            float widMul = Mathf.Lerp(1f, 0.4f, r);
            ap.go.transform.localScale = new Vector3(pieceLength * lenMul, pieceWidth * widMul, 1f);

            if (ap.t >= ap.life)
            {
                // recycle
                ap.go.SetActive(false);
                pool.Enqueue(ap.go);
                active.RemoveAt(i);
            }
        }
    }

    // Create pooled GameObject for piece
    GameObject CreatePieceObject(bool active)
    {
        GameObject go = new GameObject("TrailPiece");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.material = spriteMat;
        sr.sprite = trailSprite;
        sr.color = trailColor;
        sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sr.receiveShadows = false;
        go.SetActive(active);
        go.transform.localScale = Vector3.one;
        go.AddComponent<PieceData>();
        return go;
    }

    GameObject GetFromPool()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return CreatePieceObject(true);
    }

    // simple soft circular texture generator (alpha falloff)
    Texture2D CreateSoftCircleTexture(int w, int h)
    {
        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        Color32[] colors = new Color32[w * h];

        float cx = (w - 1) / 2f;
        float cy = (h - 1) / 2f;
        float maxR = Mathf.Min(cx, cy);

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                float dist = Mathf.Sqrt(dx * dx + dy * dy);
                float t = Mathf.Clamp01(dist / maxR);
                // alpha falloff (soft)
                float a = Mathf.Clamp01(1f - t * t);
                // slightly bias center bright
                Color col = Color.white * 1f;
                col.a = a;
                colors[y * w + x] = col;
            }
        }

        tex.SetPixels32(colors);
        tex.Apply();
        return tex;
    }

    // small helper component to carry life info if needed
    class PieceData : MonoBehaviour
    {
        public float life;
        public void ResetLife(float l) { life = l; }
    }
}
