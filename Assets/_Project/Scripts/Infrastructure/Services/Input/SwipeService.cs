using UnityEngine;

namespace Services.Input
{
    public class SwipeService : IInputService
    {
        private const float MINSwipeLength = 200f;
        public Vector2 Horizontal { get; private set; }
        public Vector2 Vertical { get; private set; }
    
        private readonly bool _isMobile;
        
        private Touch _touch;
        private bool _canSwipe;
        private Vector2 _firstPressPos;
        private Vector2 _secondPressPos;
        private Vector2 _currentSwipe;
        private Vector2 _tapPosition;
        private Vector2 _swipeDelta;
        private bool _isSwiping;
    
        public SwipeService() => 
            _isMobile = Application.isMobilePlatform;

        public void Update()
        {
            Horizontal = Vector2.zero;
            Vertical = Vector2.zero;
            
            if (!_isMobile)
            {
                if (UnityEngine.Input.GetMouseButtonDown(0))
                {
                    _isSwiping = true;
                    _tapPosition = UnityEngine.Input.mousePosition;
                }
                else if (UnityEngine.Input.GetMouseButtonUp(0))
                {
                    ResetSwipe();
                }
            }
            else
            {
                if (UnityEngine.Input.touchCount > 0)
                {
                    if (UnityEngine.Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        _isSwiping = true;
                        _tapPosition = UnityEngine.Input.GetTouch(0).position;
                    }
                    else if (UnityEngine.Input.GetTouch(0).phase is TouchPhase.Canceled or TouchPhase.Ended)
                    {
                        ResetSwipe();
                    }
                }
            }
        
            CheckSwipe();
        }

        private void CheckSwipe()
        {
            _swipeDelta = Vector2.zero;
            if (_isSwiping)
            {
                if (!_isMobile && UnityEngine.Input.GetMouseButton(0))
                {
                    _swipeDelta = (Vector2) UnityEngine.Input.mousePosition - _tapPosition;
                }
                else if (UnityEngine.Input.touchCount > 0)
                {
                    _swipeDelta = UnityEngine.Input.GetTouch(0).position - _tapPosition;
                }
            }

            if (_swipeDelta.magnitude > MINSwipeLength)
            {
                if (Mathf.Abs(_swipeDelta.x) > Mathf.Abs(_swipeDelta.y))
                    Horizontal = _swipeDelta.x > 0 ? Vector2.right : Vector2.left;
                else
                    Vertical = _swipeDelta.y > 0 ? Vector2.up : Vector2.down;
            
                ResetSwipe();
            }
        }

        private void ResetSwipe()
        {
            _tapPosition = Vector2.zero;
            _swipeDelta = Vector2.zero;
            _isSwiping = false;
        }
    
    }
}
