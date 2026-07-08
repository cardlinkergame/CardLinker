using UnityEngine;

namespace CardLinker.Grid
{
    /// <summary>
    /// Grid'e snaplenen, sürüklenebilir kart. SpriteRenderer + Collider2D gerektirir
    /// (OnMouseDown/OnMouseDrag/OnMouseUp'ın çalışması için).
    ///
    /// PERFORMANS NOTU: OnMouseDrag() her frame çalışır ve BİLİNÇLİ olarak hiçbir
    /// allocation yapmaz - sadece Vector3 struct aritmetiği ve Camera.ScreenToWorldPoint
    /// (struct döner, heap alloc yok). GridManager'a sadece MouseDown/MouseUp anında
    /// (drag başı/sonu) dokunulur, drag sırasında hiç değil.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class GridCard : MonoBehaviour
    {
        [Tooltip("Kartın kapladığı hücre sayısı (genişlik x yükseklik)")]
        [SerializeField] private Vector2Int footprintSize = new Vector2Int(4, 5);

        private Camera _mainCamera;
        private Vector2Int _anchorCell;
        private Vector3 _dragOffset;
        private Vector2Int _anchorBeforeDrag;
        private Vector3 _worldPosBeforeDrag;
        private bool _isDragging;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        /// <summary>
        /// Kartı ilk kez belirli bir anchor hücresine yerleştirir ve rezerve eder.
        /// Spawn/bootstrap kodu bunu çağırmalı.
        /// </summary>
        public void PlaceAt(Vector2Int anchorCell)
        {
            _anchorCell = anchorCell;
            transform.position = GridManager.Instance.AnchorCellToWorldCenter(anchorCell, footprintSize);
            GridManager.Instance.ReserveArea(anchorCell, footprintSize, this);
        }

        private void OnMouseDown()
        {
            _isDragging = true;
            _anchorBeforeDrag = _anchorCell;
            _worldPosBeforeDrag = transform.position;

            _dragOffset = transform.position - GetMouseWorldPosition();

            // Sürükleme sırasında bu kartın kapladığı alan boşa çıkar.
            GridManager.Instance.ReleaseArea(_anchorCell, footprintSize);
        }

        private void OnMouseDrag()
        {
            if (!_isDragging)
                return;

            transform.position = GetMouseWorldPosition() + _dragOffset;
        }

        private void OnMouseUp()
        {
            if (!_isDragging)
                return;

            _isDragging = false;

            var desiredAnchor = GridManager.Instance.WorldCenterToAnchorCell(transform.position, footprintSize);

            if (GridManager.Instance.TryFindNearestFreeAnchor(desiredAnchor, footprintSize, this, out var foundAnchor))
            {
                _anchorCell = foundAnchor;
                transform.position = GridManager.Instance.AnchorCellToWorldCenter(foundAnchor, footprintSize);
                GridManager.Instance.ReserveArea(foundAnchor, footprintSize, this);
            }
            else
            {
                Debug.LogWarning($"[GridCard:{name}] Boş yer bulunamadı, kart eski konumuna dönüyor.");
                _anchorCell = _anchorBeforeDrag;
                transform.position = _worldPosBeforeDrag;
                GridManager.Instance.ReserveArea(_anchorCell, footprintSize, this);
            }
        }

        private Vector3 GetMouseWorldPosition()
        {
            var screenPos = Input.mousePosition;
            screenPos.z = _mainCamera.WorldToScreenPoint(transform.position).z;
            return _mainCamera.ScreenToWorldPoint(screenPos);
        }
    }
}
