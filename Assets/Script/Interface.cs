using UnityEngine;

public class Interface : MonoBehaviour { }


public interface ICharacter
{
 // public void Spawn();
    public void Move();
    public void ReLoadHp();
}

public interface IAttackable
{
    public void Attack();
}
