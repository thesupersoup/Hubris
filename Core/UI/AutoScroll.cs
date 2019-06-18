using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hubris
{
    /// <summary>
    /// Can be used with any Rect Transform to synchronize the H Delta and Y Pos values
    /// </summary>
    public class AutoScroll : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _rt = null;
        private bool _ready = false;
        float _prevDelta = 0.0f;

        // Start is called before the first frame update
        void Start()
        {
            if (_rt == null)
            {
                _rt = GetComponent<RectTransform>();
            }

            if(_rt != null)
            {
                _ready = true;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(_ready)
            {
                if (_rt.sizeDelta.y > 0.0f && _rt.sizeDelta.y != _prevDelta)
                {
                    _prevDelta = _rt.sizeDelta.y;
                    _rt.Translate(new Vector3(0.0f, _prevDelta));   // Adjust Pos Y by H Delta amount
                }
            }
        }
    }
}
