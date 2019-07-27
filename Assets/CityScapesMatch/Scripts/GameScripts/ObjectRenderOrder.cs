using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ObjectRenderOrder : MonoBehaviour
    {
        // Start is called before the first frame update
        public void SetOrder(int order)
        {
            SpriteRenderer sR = GetComponent<SpriteRenderer>();
            ParticleSystemRenderer pS = GetComponent<ParticleSystemRenderer>();
            if (sR) sR.sortingOrder = order;
            if (pS) pS.sortingOrder = order;
        }
    }
}