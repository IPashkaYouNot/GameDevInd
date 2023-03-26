using Core.Movement.Data;
using UnityEngine;
using UnityEngine.UI;

namespace ACore.Movement.Controller
{
    public class Jumper
    {
        private readonly JumpData _jumpData;
        private readonly Rigidbody2D _rigidbody;
        private readonly float _maxVerticalSize;
        private readonly Transform _transform;
        private readonly Transform _shadowTransform;
        private readonly Vector2 _shadowLocalPosition;
        private readonly Vector2 _shadowLocalScale;
        private readonly float _shadowOpacity;

        private float _startJumpVerticalPosition;
        private float _shadowVerticalPosition;
        public bool IsJumping { get; private set; }

        public Jumper(JumpData jumpData, Rigidbody2D rigidbody, float maxVerticalSize)
        {
            _jumpData = jumpData;
            _rigidbody = rigidbody;
            _maxVerticalSize = maxVerticalSize;
            _shadowTransform = _jumpData.Shadow.transform;
            _shadowLocalPosition = _shadowTransform.localPosition;
            _shadowLocalScale = _shadowTransform.localScale;
            _shadowOpacity = _jumpData.Shadow.color.a;
            _transform = _rigidbody.transform;
        }

        public void Jump()
        {
            if (IsJumping) return;

            IsJumping = true;
            _startJumpVerticalPosition = _rigidbody.position.y;
            var jumpModificator = _transform.localScale.y / _maxVerticalSize;
            var currentJumpForce = _jumpData.JumpForce * jumpModificator;
            _rigidbody.gravityScale = _jumpData.GravityScale * jumpModificator;
            _rigidbody.AddForce(Vector2.up * currentJumpForce);
            // _startjumpverticalposition = transform.position.y;
            _shadowVerticalPosition = _shadowTransform.position.y;
        }
        public void UpdateJump()
        {
            if (_rigidbody.velocity.y < 0 && _transform.position.y < _startJumpVerticalPosition)
            {
                ResetJump();
                return;
            }

            var distance = _rigidbody.transform.position.y - _startJumpVerticalPosition;
            _shadowTransform.position = new(_shadowTransform.position.x, _shadowVerticalPosition);
            _shadowTransform.localScale = _shadowLocalScale * (1 + (_jumpData.ShadowSizeModificator * distance));
            _jumpData.Shadow.color = new(0, 0, 0, _shadowOpacity - distance * _jumpData.ShadowAlphaModificator);
        }

        private void ResetJump()
        {
            _rigidbody.gravityScale = 0;
            _transform.position = new(_transform.position.x, _startJumpVerticalPosition);

            _shadowTransform.localScale = _shadowLocalScale;
            _shadowTransform.localPosition = _shadowLocalPosition;
            _jumpData.Shadow.color = new(0, 0, 0, _shadowOpacity);

            IsJumping = false;
        }
    }
}
