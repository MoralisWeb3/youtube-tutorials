# Supported Events

The Event System supports a number of events, and they can be customized further in user custom user written Input Modules.

The events that are supported by the Standalone Input Module and Touch Input Module are provided by interface and can be implemented on a MonoBehaviour by implementing the interface. If you have a valid Event System configured the events will be called at the correct time.

- [IPointerEnterHandler](../api/UnityEngine.EventSystems.IPointerEnterHandler.html) - OnPointerEnter - Called when a pointer enters the object
- [IPointerExitHandler](../api/UnityEngine.EventSystems.IPointerExitHandler.html) - OnPointerExit - Called when a pointer exits the object
- [IPointerDownHandler](../api/UnityEngine.EventSystems.IPointerDownHandler.html) - OnPointerDown - Called when a pointer is pressed on the object
- [IPointerUpHandler](../api/UnityEngine.EventSystems.IPointerUpHandler.html)- OnPointerUp - Called when a pointer is released (called on the GameObject that the pointer is clicking)
- [IPointerClickHandler](../api/UnityEngine.EventSystems.IPointerClickHandler.html) - OnPointerClick - Called when a pointer is pressed and released on the same object
- [IInitializePotentialDragHandler](../api/UnityEngine.EventSystems.IInitializePotentialDragHandler.html) - OnInitializePotentialDrag - Called when a drag target is found, can be used to initialize values
- [IBeginDragHandler](../api/UnityEngine.EventSystems.IBeginDragHandler.html) - OnBeginDrag - Called on the drag object when dragging is about to begin
- [IDragHandler](../api/UnityEngine.EventSystems.IDragHandler.html) - OnDrag - Called on the drag object when a drag is happening
- [IEndDragHandler](../api/UnityEngine.EventSystems.IEndDragHandler.html) - OnEndDrag - Called on the drag object when a drag finishes
- [IDropHandler](../api/UnityEngine.EventSystems.IDropHandler.html) - OnDrop - Called on the object where a drag finishes
- [IScrollHandler](../api/UnityEngine.EventSystems.IScrollHandler.html) - OnScroll - Called when a mouse wheel scrolls
- [IUpdateSelectedHandler](../api/UnityEngine.EventSystems.IUpdateSelectedHandler.html) - OnUpdateSelected - Called on the selected object each tick
- [ISelectHandler](../api/UnityEngine.EventSystems.ISelectHandler.html) - OnSelect - Called when the object becomes the selected object
- [IDeselectHandler](../api/UnityEngine.EventSystems.IDeselectHandler.html) - OnDeselect - Called on the selected object becomes deselected
- [IMoveHandler](../api/UnityEngine.EventSystems.IMoveHandler.html) - OnMove - Called when a move event occurs (left, right, up, down)
- [ISubmitHandler](../api/UnityEngine.EventSystems.ISubmitHandler.html) - OnSubmit - Called when the submit button is pressed
- [ICancelHandler](../api/UnityEngine.EventSystems.ICancelHandler.html) - OnCancel - Called when the cancel button is pressed
