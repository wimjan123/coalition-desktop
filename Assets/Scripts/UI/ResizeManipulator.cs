using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.UI
{
    public class ResizeManipulator : Manipulator
    {
        private VisualElement targetWindow;
        private ResizeDirection activeDirection = ResizeDirection.None;
        private Vector2 startSize;
        private Vector2 startPosition;
        private Vector2 startPointerPosition;
        private bool isResizing = false;

        // Minimum and maximum window sizes
        private const float MinWidth = 300f;
        private const float MinHeight = 200f;
        private const float MaxWidth = 1920f;
        private const float MaxHeight = 1200f;

        public enum ResizeDirection
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        public ResizeManipulator(VisualElement target)
        {
            targetWindow = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            // Register callbacks on all resize handles
            var resizeHandles = targetWindow.Query(className: "resize-handle").ToList();

            foreach (var handle in resizeHandles)
            {
                handle.RegisterCallback<PointerDownEvent>(OnPointerDown);
                handle.RegisterCallback<PointerMoveEvent>(OnPointerMove);
                handle.RegisterCallback<PointerUpEvent>(OnPointerUp);
                handle.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            var resizeHandles = targetWindow.Query(className: "resize-handle").ToList();

            foreach (var handle in resizeHandles)
            {
                handle.UnregisterCallback<PointerDownEvent>(OnPointerDown);
                handle.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
                handle.UnregisterCallback<PointerUpEvent>(OnPointerUp);
                handle.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
            }
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0) return; // Only left mouse button

            var handle = evt.target as VisualElement;
            activeDirection = GetResizeDirection(handle);

            if (activeDirection == ResizeDirection.None) return;

            // Check if window is maximized
            var windowElement = targetWindow as WindowElement;
            if (windowElement != null && windowElement.IsMaximized) return;

            startSize = new Vector2(targetWindow.resolvedStyle.width, targetWindow.resolvedStyle.height);
            startPosition = new Vector2(targetWindow.resolvedStyle.left, targetWindow.resolvedStyle.top);
            startPointerPosition = evt.position;
            isResizing = true;

            handle.CapturePointer(evt.pointerId);
            targetWindow.AddToClassList("resizing");

            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isResizing || activeDirection == ResizeDirection.None) return;

            Vector2 delta = evt.position - startPointerPosition;
            ApplyResize(delta);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!isResizing) return;

            isResizing = false;
            activeDirection = ResizeDirection.None;

            var handle = evt.target as VisualElement;
            handle?.ReleasePointer(evt.pointerId);
            targetWindow.RemoveFromClassList("resizing");

            evt.StopPropagation();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (isResizing)
            {
                isResizing = false;
                activeDirection = ResizeDirection.None;
                targetWindow.RemoveFromClassList("resizing");
            }
        }

        private ResizeDirection GetResizeDirection(VisualElement handle)
        {
            if (handle == null) return ResizeDirection.None;

            if (handle.ClassListContains("resize-top"))
                return ResizeDirection.Top;
            if (handle.ClassListContains("resize-bottom"))
                return ResizeDirection.Bottom;
            if (handle.ClassListContains("resize-left"))
                return ResizeDirection.Left;
            if (handle.ClassListContains("resize-right"))
                return ResizeDirection.Right;
            if (handle.ClassListContains("resize-top-left"))
                return ResizeDirection.TopLeft;
            if (handle.ClassListContains("resize-top-right"))
                return ResizeDirection.TopRight;
            if (handle.ClassListContains("resize-bottom-left"))
                return ResizeDirection.BottomLeft;
            if (handle.ClassListContains("resize-bottom-right"))
                return ResizeDirection.BottomRight;

            return ResizeDirection.None;
        }

        private void ApplyResize(Vector2 delta)
        {
            var newSize = startSize;
            var newPosition = startPosition;

            // Get desktop bounds for constraints
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;

            switch (activeDirection)
            {
                case ResizeDirection.Right:
                    newSize.x = Mathf.Clamp(startSize.x + delta.x, MinWidth,
                        Mathf.Min(MaxWidth, desktopBounds.width - startPosition.x));
                    break;

                case ResizeDirection.Bottom:
                    newSize.y = Mathf.Clamp(startSize.y + delta.y, MinHeight,
                        Mathf.Min(MaxHeight, desktopBounds.height - startPosition.y));
                    break;

                case ResizeDirection.Left:
                    var newWidth = startSize.x - delta.x;
                    if (newWidth >= MinWidth && startPosition.x + delta.x >= 0)
                    {
                        newSize.x = newWidth;
                        newPosition.x = startPosition.x + delta.x;
                    }
                    break;

                case ResizeDirection.Top:
                    var newHeight = startSize.y - delta.y;
                    if (newHeight >= MinHeight && startPosition.y + delta.y >= 0)
                    {
                        newSize.y = newHeight;
                        newPosition.y = startPosition.y + delta.y;
                    }
                    break;

                case ResizeDirection.TopLeft:
                    // Resize from top-left corner
                    var tlNewWidth = startSize.x - delta.x;
                    var tlNewHeight = startSize.y - delta.y;

                    if (tlNewWidth >= MinWidth && startPosition.x + delta.x >= 0)
                    {
                        newSize.x = tlNewWidth;
                        newPosition.x = startPosition.x + delta.x;
                    }
                    if (tlNewHeight >= MinHeight && startPosition.y + delta.y >= 0)
                    {
                        newSize.y = tlNewHeight;
                        newPosition.y = startPosition.y + delta.y;
                    }
                    break;

                case ResizeDirection.TopRight:
                    // Resize from top-right corner
                    newSize.x = Mathf.Clamp(startSize.x + delta.x, MinWidth,
                        Mathf.Min(MaxWidth, desktopBounds.width - startPosition.x));

                    var trNewHeight = startSize.y - delta.y;
                    if (trNewHeight >= MinHeight && startPosition.y + delta.y >= 0)
                    {
                        newSize.y = trNewHeight;
                        newPosition.y = startPosition.y + delta.y;
                    }
                    break;

                case ResizeDirection.BottomLeft:
                    // Resize from bottom-left corner
                    var blNewWidth = startSize.x - delta.x;
                    if (blNewWidth >= MinWidth && startPosition.x + delta.x >= 0)
                    {
                        newSize.x = blNewWidth;
                        newPosition.x = startPosition.x + delta.x;
                    }

                    newSize.y = Mathf.Clamp(startSize.y + delta.y, MinHeight,
                        Mathf.Min(MaxHeight, desktopBounds.height - startPosition.y));
                    break;

                case ResizeDirection.BottomRight:
                    // Resize from bottom-right corner
                    newSize.x = Mathf.Clamp(startSize.x + delta.x, MinWidth,
                        Mathf.Min(MaxWidth, desktopBounds.width - startPosition.x));
                    newSize.y = Mathf.Clamp(startSize.y + delta.y, MinHeight,
                        Mathf.Min(MaxHeight, desktopBounds.height - startPosition.y));
                    break;
            }

            // Apply the new size and position
            targetWindow.style.width = newSize.x;
            targetWindow.style.height = newSize.y;
            targetWindow.style.left = newPosition.x;
            targetWindow.style.top = newPosition.y;
        }
    }
}