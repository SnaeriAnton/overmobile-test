using UnityEngine;

namespace Game
{
    public abstract class Entity : MonoBehaviour
    {
        public EventPoint Point;
        public int Power;
        
        [SerializeField] private SpriteRenderer _indicatorSprite;

        public virtual bool CanShowIndicator => true;

        public abstract void Hit();
        public void SetActiveIndicator(bool value) => _indicatorSprite.gameObject.SetActive(value);

    }
}
