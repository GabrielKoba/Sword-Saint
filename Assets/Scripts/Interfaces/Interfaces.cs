using UnityEngine;

public interface IInteractable {
    void Interact();
}
public interface IDamageable {
    void TakeDamage(int damage);
}