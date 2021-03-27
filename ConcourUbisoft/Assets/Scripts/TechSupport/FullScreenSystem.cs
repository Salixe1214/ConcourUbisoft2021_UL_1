using TechSupport.Surveillance;
using UnityEngine;

namespace TechSupport
{
    public class FullScreenSystem
    {
        private readonly Rect _fullSize = new Rect(Vector2.zero, Vector2.one);
        private Rect _originRect = Rect.zero;
        private static SurveillanceCamera _target;

        public static Camera Current { get => _target.GetCamera(); }

        #region Full Screen

        public void RenderFullScreen()
        {
            _target.Enable(true);
            _target.EnableController(true);
            _target.ActivateClock(true);
            _originRect = _target.GetCamera().rect;
            _target.GetCamera().rect = _fullSize;
        }

        public void EscapeFullScreen()
        {
            _target.EnableController(false);
            _target.ActivateClock(false);
            _target.GetCamera().rect = _originRect;
        }

        #endregion

        #region Getter Setter

        public void SetTarget(SurveillanceCamera camera)
        {
            _target = camera;
        }

        public SurveillanceCamera GetTarget()
        {
            return _target;
        }

        #endregion
    }
}
