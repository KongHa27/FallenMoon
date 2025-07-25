using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 상호작용 가능한 오브젝트의 기본 인터페이스
/// </summary>
public interface IInteractable
{
    bool CanInteract { get; }
    void Interact(GameObject interactor);
    string GetInteractionText();
}
