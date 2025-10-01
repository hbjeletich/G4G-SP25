using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Swimming
{
    [RequireComponent(typeof(Collider2D))]
    
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private AK.Wwise.Event PickupSFX = null;
        private Collider2D col;
        public int shellNumber;
        private SpriteRenderer spriteRenderer;

        void Start()
        {
            col = GetComponent<Collider2D>();
            col.isTrigger = true;

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnPickup()
        {
            spriteRenderer.enabled = false;
            col.enabled = false;
            PickupSFX.Post(gameObject);
            CollectablesManager.Instance.CollectShell(shellNumber);
        }
    }
}
