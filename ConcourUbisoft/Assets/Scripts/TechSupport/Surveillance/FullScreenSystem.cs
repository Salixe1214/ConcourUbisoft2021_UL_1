using UnityEngine;

namespace TechSupport.Surveillance
{
    public class FullScreenSystem
    {
        private readonly Rect _fullSize = new Rect(Vector2.zero, Vector2.one);
        private Rect _originRect = Rect.zero;
        private Camera _target;

        #region Full Screen

        public void RenderFullScreen()
        {
            _target.enabled = true;
            _originRect = _target.rect;
            _target.rect = _fullSize;
        }

        public void EscapeFullScreen()
        {
            _target.rect = _originRect;
        }

        #endregion

        #region Getter Setter

        public void SetTarget(Camera camera)
        {
            _target = camera;
        }

        public Camera GetTarget()
        {
            return _target;
        }

        #endregion
    }
}
