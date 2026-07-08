using System.Collections.Generic;
using UnityEngine;

namespace CardLinker.Grid
{
    /// <summary>
    /// Grid'in rezervasyon durumunu ve koordinat dönüşümlerini yönetir.
    /// Sahnede TEK bir GridManager olmalı. Bu sınıfın kendisi MonoBehaviour -
    /// çünkü transform.position'ı grid'in world-space origin'i olarak kullanıyoruz
    /// (GridManager objesini sahnede taşıyarak tüm grid'i konumlandırabilirsin).
    ///
    /// PERFORMANS NOTU: _owner dizisi Awake'de bir kez tahsis edilir. Drag sırasında
    /// (her frame) bu sınıfa hiç dokunulmaz - sadece MouseDown/MouseUp anlarında
    /// (nadiren tetiklenen olaylar) ReserveArea/ReleaseArea/TryFindNearestFreeAnchor
    /// çağrılır. Bu yüzden oradaki küçük List tahsisleri GC birikimine yol açmaz.
    /// </summary>
    public class GridManager : MonoBehaviour
    {
        public static GridManager Instance { get; private set; }

        [SerializeField] private float cellSize = 1f;
        [SerializeField] private int gridWidth = 400;
        [SerializeField] private int gridHeight = 500;
        [SerializeField] private int maxSearchRadius = 100;

        /// <summary>Hangi hücrede hangi GridCard var. null = boş.</summary>
        private GridCard[,] _owner;

        public float CellSize => cellSize;
        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _owner = new GridCard[gridWidth, gridHeight];
        }

        // ---- Koordinat dönüşümleri ----

        /// <summary>
        /// Bir footprint'in (genişlik x yükseklik) MERKEZİNİN bulunmasını istediğimiz
        /// world pozisyonundan, o footprint'in sol-alt "anchor" hücresini hesaplar.
        /// Çift/tek boyutlarda özel durum gerektirmeyen genel bir formül.
        /// </summary>
        public Vector2Int WorldCenterToAnchorCell(Vector3 worldCenterPos, Vector2Int footprintSize)
        {
            float localX = (worldCenterPos.x - transform.position.x) / cellSize;
            float localY = (worldCenterPos.y - transform.position.y) / cellSize;

            int anchorX = Mathf.RoundToInt(localX - footprintSize.x / 2f);
            int anchorY = Mathf.RoundToInt(localY - footprintSize.y / 2f);

            return ClampAnchor(new Vector2Int(anchorX, anchorY), footprintSize);
        }

        /// <summary>Bir anchor hücre + footprint için, footprint'in merkezinin world pozisyonu.</summary>
        public Vector3 AnchorCellToWorldCenter(Vector2Int anchorCell, Vector2Int footprintSize)
        {
            float worldX = transform.position.x + (anchorCell.x + footprintSize.x / 2f) * cellSize;
            float worldY = transform.position.y + (anchorCell.y + footprintSize.y / 2f) * cellSize;
            return new Vector3(worldX, worldY, transform.position.z);
        }

        public Vector2Int ClampAnchor(Vector2Int anchor, Vector2Int footprintSize)
        {
            int maxX = gridWidth - footprintSize.x;
            int maxY = gridHeight - footprintSize.y;
            return new Vector2Int(Mathf.Clamp(anchor.x, 0, maxX), Mathf.Clamp(anchor.y, 0, maxY));
        }

        // ---- Rezervasyon ----

        public bool IsWithinBounds(Vector2Int anchor, Vector2Int footprintSize)
        {
            return anchor.x >= 0 && anchor.y >= 0
                   && anchor.x + footprintSize.x <= gridWidth
                   && anchor.y + footprintSize.y <= gridHeight;
        }

        public bool IsAreaFree(Vector2Int anchor, Vector2Int footprintSize, GridCard ignoreCard)
        {
            if (!IsWithinBounds(anchor, footprintSize))
                return false;

            for (int x = anchor.x; x < anchor.x + footprintSize.x; x++)
            {
                for (int y = anchor.y; y < anchor.y + footprintSize.y; y++)
                {
                    var occupant = _owner[x, y];
                    if (occupant != null && occupant != ignoreCard)
                        return false;
                }
            }

            return true;
        }

        public void ReserveArea(Vector2Int anchor, Vector2Int footprintSize, GridCard owner)
        {
            for (int x = anchor.x; x < anchor.x + footprintSize.x; x++)
            {
                for (int y = anchor.y; y < anchor.y + footprintSize.y; y++)
                {
                    _owner[x, y] = owner;
                }
            }
        }

        public void ReleaseArea(Vector2Int anchor, Vector2Int footprintSize)
        {
            for (int x = anchor.x; x < anchor.x + footprintSize.x; x++)
            {
                for (int y = anchor.y; y < anchor.y + footprintSize.y; y++)
                {
                    _owner[x, y] = null;
                }
            }
        }

        /// <summary>
        /// desiredAnchor'a en yakın BOŞ alanı arar (genişleyen halka taraması).
        /// Sadece drop anında (nadiren) çağrılır - bu yüzden içeride List kullanımı
        /// GC birikimine yol açmaz.
        /// </summary>
        public bool TryFindNearestFreeAnchor(Vector2Int desiredAnchor, Vector2Int footprintSize, GridCard ignoreCard, out Vector2Int result)
        {
            desiredAnchor = ClampAnchor(desiredAnchor, footprintSize);

            if (IsAreaFree(desiredAnchor, footprintSize, ignoreCard))
            {
                result = desiredAnchor;
                return true;
            }

            var ringCandidates = new List<Vector2Int>();

            for (int radius = 1; radius <= maxSearchRadius; radius++)
            {
                ringCandidates.Clear();

                for (int dx = -radius; dx <= radius; dx++)
                {
                    for (int dy = -radius; dy <= radius; dy++)
                    {
                        // Sadece bu radius'un dış halkası (önceki radius'larda zaten denendi)
                        if (Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy)) != radius)
                            continue;

                        var candidate = new Vector2Int(desiredAnchor.x + dx, desiredAnchor.y + dy);

                        if (!IsWithinBounds(candidate, footprintSize))
                            continue;

                        if (IsAreaFree(candidate, footprintSize, ignoreCard))
                            ringCandidates.Add(candidate);
                    }
                }

                if (ringCandidates.Count > 0)
                {
                    result = GetClosest(desiredAnchor, ringCandidates);
                    return true;
                }
            }

            result = default;
            return false;
        }

        private static Vector2Int GetClosest(Vector2Int origin, List<Vector2Int> candidates)
        {
            var best = candidates[0];
            int bestDistSqr = SqrDistance(origin, best);

            for (int i = 1; i < candidates.Count; i++)
            {
                int distSqr = SqrDistance(origin, candidates[i]);
                if (distSqr < bestDistSqr)
                {
                    bestDistSqr = distSqr;
                    best = candidates[i];
                }
            }

            return best;
        }

        private static int SqrDistance(Vector2Int a, Vector2Int b)
        {
            int dx = a.x - b.x;
            int dy = a.y - b.y;
            return dx * dx + dy * dy;
        }
    }
}
