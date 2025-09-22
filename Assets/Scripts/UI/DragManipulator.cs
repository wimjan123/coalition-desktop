using UnityEngine;
using UnityEngine.UIElements;

namespace Coalition.UI
{
    public class DragManipulator : PointerManipulator
    {
        private Vector2 startPosition;
        private Vector2 startPointerPosition;
        private VisualElement targetWindow;
        private bool isDragging = false;

        public DragManipulator(VisualElement target)
        {
            targetWindow = target;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<PointerDownEvent>(OnPointerDown);
            target.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            target.RegisterCallback<PointerUpEvent>(OnPointerUp);
            target.RegisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
            target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);
            target.UnregisterCallback<PointerUpEvent>(OnPointerUp);
            target.UnregisterCallback<PointerCaptureOutEvent>(OnPointerCaptureOut);
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            // Only handle left mouse button
            if (evt.button != 0) return;

            // Don't drag if clicking on window controls
            if (IsClickOnControls(evt.target as VisualElement)) return;

            // Check if window is maximized
            var windowElement = targetWindow as WindowElement;
            if (windowElement != null && windowElement.IsMaximized) return;

            startPosition = new Vector2(targetWindow.resolvedStyle.left, targetWindow.resolvedStyle.top);
            startPointerPosition = evt.localPosition;
            isDragging = true;

            target.CapturePointer(evt.pointerId);
            target.AddToClassList("dragging");

            evt.StopPropagation();
        }

        private void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging || !target.HasPointerCapture(evt.pointerId)) return;

            Vector2 delta = (Vector2)evt.localPosition - startPointerPosition;
            Vector2 newPosition = startPosition + delta;

            // Constrain to desktop bounds
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;
            var windowWidth = targetWindow.resolvedStyle.width;
            var windowHeight = targetWindow.resolvedStyle.height;

            newPosition.x = Mathf.Clamp(newPosition.x, 0, desktopBounds.width - windowWidth);
            newPosition.y = Mathf.Clamp(newPosition.y, 0, desktopBounds.height - windowHeight);

            // Apply position
            targetWindow.style.left = newPosition.x;
            targetWindow.style.top = newPosition.y;

            // Check for snap zones
            CheckSnapZones(newPosition, evt.position);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging) return;

            isDragging = false;
            target.ReleasePointer(evt.pointerId);
            target.RemoveFromClassList("dragging");

            // Apply snap-to-grid
            var windowElement = targetWindow as WindowElement;
            windowElement?.SnapToGrid();

            evt.StopPropagation();
        }

        private void OnPointerCaptureOut(PointerCaptureOutEvent evt)
        {
            if (isDragging)
            {
                isDragging = false;
                target.RemoveFromClassList("dragging");
            }
        }

        private bool IsClickOnControls(VisualElement clickTarget)
        {
            // Check if click is on window control buttons
            while (clickTarget != null)
            {
                if (clickTarget.ClassListContains("window-button") ||
                    clickTarget.ClassListContains("window-controls"))
                {
                    return true;
                }
                clickTarget = clickTarget.parent;
            }
            return false;
        }

        private void CheckSnapZones(Vector2 windowPosition, Vector2 screenPosition)
        {
            var desktopBounds = Coalition.Core.DesktopManager.DesktopBounds;
            const float snapThreshold = 20f;

            var windowElement = targetWindow as WindowElement;
            if (windowElement == null) return;

            // Left edge snap
            if (windowPosition.x <= snapThreshold)
            {
                ShowSnapPreview(true, false);
            }
            // Right edge snap
            else if (windowPosition.x >= desktopBounds.width - targetWindow.resolvedStyle.width - snapThreshold)
            {
                ShowSnapPreview(false, false);
            }
            // Top corners for quarter snapping
            else if (windowPosition.y <= snapThreshold)
            {
                if (windowPosition.x <= desktopBounds.width / 2f)
                {
                    ShowSnapPreview(true, true); // Top-left quarter
                }
                else
                {
                    ShowSnapPreview(false, true); // Top-right quarter
                }
            }
            else
            {
                HideSnapPreview();
            }
        }

        private void ShowSnapPreview(bool isLeft, bool isQuarter)
        {
            // Visual feedback for snap zones
            // This would typically highlight the snap area
            // For now, we'll just change the cursor or add a visual indicator
            target.AddToClassList("snap-preview");
        }

        private void HideSnapPreview()
        {
            target.RemoveFromClassList("snap-preview");
        }
    }
}