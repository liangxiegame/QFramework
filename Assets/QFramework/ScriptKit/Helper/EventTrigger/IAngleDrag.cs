using System.Collections;
using UnityEngine.EventSystems;

public interface IAngleDrag  {
    bool CheckAngleToDrag(PointerEventData eventData);
}
