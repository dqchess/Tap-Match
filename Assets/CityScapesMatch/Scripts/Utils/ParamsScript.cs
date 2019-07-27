using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class ParamsScript : MonoBehaviour
    {
        public ParamObject pObject;
    }

    [Serializable]
    public class ParamObject
    {
        public string description;
        public System.Object param;
    }
}
