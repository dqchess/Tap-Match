using UnityEngine;

namespace Mkey
{
    public class AnimCallBack : MonoBehaviour
    {
        private System.Action cBack;

        public void EndCallBack()
        {
            cBack?.Invoke();
        }

        public void SetEndCallBack(System.Action cBack)
        {
            this.cBack = cBack;
        }
    }
}