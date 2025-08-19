using UnityEngine;

namespace SonDT
{
    public class ScrollingMaterial : MonoBehaviour
    {
        public Vector2 Speed;

        Material _materialToScroll;
        private Vector2 currentscroll;

        void Awake()
        {
            _materialToScroll = GetComponent<SpriteRenderer>().material;
            float n = _materialToScroll.GetTextureScale("_MainTex").y;
            float scale = transform.localScale.z;
        }

        void Update()
        {
            currentscroll += Speed * Time.deltaTime;
            _materialToScroll.mainTextureOffset = currentscroll;
        }
    }
}