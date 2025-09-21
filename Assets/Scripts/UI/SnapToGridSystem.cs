using UnityEngine;

namespace Coalition.UI
{
    public static class SnapToGridSystem
    {
        public const float GridSize = 20f;

        public enum QuadrantPosition
        {
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        /// <summary>
        /// Snaps a position to the nearest grid point
        /// </summary>
        public static Vector2 SnapToGrid(Vector2 position)
        {
            return new Vector2(
                Mathf.Round(position.x / GridSize) * GridSize,
                Mathf.Round(position.y / GridSize) * GridSize
            );
        }

        /// <summary>
        /// Returns position for half-screen snapping
        /// </summary>
        public static Vector2 SnapToHalf(Vector2 screenSize)
        {
            return new Vector2(screenSize.x / 2f, screenSize.y);
        }

        /// <summary>
        /// Returns position for quarter-screen snapping
        /// </summary>
        public static Vector2 SnapToQuarter(Vector2 screenSize, QuadrantPosition quadrant)
        {
            float halfWidth = screenSize.x / 2f;
            float halfHeight = screenSize.y / 2f;

            return quadrant switch
            {
                QuadrantPosition.TopLeft => new Vector2(0, 0),
                QuadrantPosition.TopRight => new Vector2(halfWidth, 0),
                QuadrantPosition.BottomLeft => new Vector2(0, halfHeight),
                QuadrantPosition.BottomRight => new Vector2(halfWidth, halfHeight),
                _ => Vector2.zero
            };
        }

        /// <summary>
        /// Returns size for quarter-screen snapping
        /// </summary>
        public static Vector2 GetQuarterSize(Vector2 screenSize)
        {
            return new Vector2(screenSize.x / 2f, screenSize.y / 2f);
        }

        /// <summary>
        /// Returns size for half-screen snapping
        /// </summary>
        public static Vector2 GetHalfSize(Vector2 screenSize)
        {
            return new Vector2(screenSize.x / 2f, screenSize.y);
        }

        /// <summary>
        /// Determines if a position is close to a snap zone
        /// </summary>
        public static bool IsInSnapZone(Vector2 position, Vector2 screenSize, float threshold = 20f)
        {
            // Left edge
            if (position.x <= threshold) return true;

            // Right edge
            if (position.x >= screenSize.x - threshold) return true;

            // Top edge
            if (position.y <= threshold) return true;

            // Bottom edge (above dock area)
            if (position.y >= screenSize.y - threshold) return true;

            return false;
        }

        /// <summary>
        /// Gets the appropriate snap zone for a given position
        /// </summary>
        public static SnapZone GetSnapZone(Vector2 position, Vector2 screenSize, float threshold = 20f)
        {
            bool isLeft = position.x <= threshold;
            bool isRight = position.x >= screenSize.x - threshold;
            bool isTop = position.y <= threshold;
            bool isBottom = position.y >= screenSize.y - threshold;

            // Corner zones (higher priority)
            if (isTop && isLeft) return SnapZone.TopLeft;
            if (isTop && isRight) return SnapZone.TopRight;
            if (isBottom && isLeft) return SnapZone.BottomLeft;
            if (isBottom && isRight) return SnapZone.BottomRight;

            // Edge zones
            if (isLeft) return SnapZone.Left;
            if (isRight) return SnapZone.Right;
            if (isTop) return SnapZone.Top;
            if (isBottom) return SnapZone.Bottom;

            return SnapZone.None;
        }

        /// <summary>
        /// Applies the appropriate snap layout for a given zone
        /// </summary>
        public static (Vector2 position, Vector2 size) GetSnapLayout(SnapZone zone, Vector2 screenSize)
        {
            var halfWidth = screenSize.x / 2f;
            var halfHeight = screenSize.y / 2f;

            return zone switch
            {
                SnapZone.Left => (Vector2.zero, new Vector2(halfWidth, screenSize.y)),
                SnapZone.Right => (new Vector2(halfWidth, 0), new Vector2(halfWidth, screenSize.y)),
                SnapZone.Top => (Vector2.zero, new Vector2(screenSize.x, halfHeight)),
                SnapZone.Bottom => (new Vector2(0, halfHeight), new Vector2(screenSize.x, halfHeight)),
                SnapZone.TopLeft => (Vector2.zero, new Vector2(halfWidth, halfHeight)),
                SnapZone.TopRight => (new Vector2(halfWidth, 0), new Vector2(halfWidth, halfHeight)),
                SnapZone.BottomLeft => (new Vector2(0, halfHeight), new Vector2(halfWidth, halfHeight)),
                SnapZone.BottomRight => (new Vector2(halfWidth, halfHeight), new Vector2(halfWidth, halfHeight)),
                _ => (Vector2.zero, screenSize)
            };
        }

        public enum SnapZone
        {
            None,
            Left,
            Right,
            Top,
            Bottom,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }
    }
}