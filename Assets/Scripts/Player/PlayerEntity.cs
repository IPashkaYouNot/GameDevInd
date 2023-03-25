using Core.Enums;
using Player.PlayerAnimation;
using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerEntity : MonoBehaviour
    {
        [SerializeField] private AnimatorController _animator;

        [Header("HorizontalMovement")]
        [SerializeField] private float _horizontalSpeed;
        [SerializeField] private Direction _direction;

        [Header("VerticalMovement")]
        [SerializeField] private float _verticalSpeed;
        [SerializeField] private float _maxSize;
        [SerializeField] private float _minSize;
        [SerializeField] private float _maxVerticalPosition;
        [SerializeField] private float _minVerticalPosition;

        [Header("Jump")]
        [SerializeField] private float _jumpForce;
        [SerializeField] private float _gravityScale;
        [SerializeField] private SpriteRenderer _shadow;
        [SerializeField][Range(0, 1)] private float _shadowSizeModificator;
        [SerializeField][Range(0, 1)] private float _shadowAlphaModificator;

        [SerializeField] private DirectionalCameraPair _cameras;

        private Rigidbody2D _rb2d;

        private float _sizeModificator;
        private bool _isJumping;
        private float _startJumpVerticalPosition;
        private Vector2 _startShadowLocalScale;
        private float _startShadowTransparency;
        private Vector2 _shadowLocalPosition;
        private float _shadowVerticalPosition;

        private Vector2 _movement;

        private void Start()
        {
            _rb2d = GetComponent<Rigidbody2D>();

            _shadowLocalPosition = _shadow.transform.localPosition;
            _startShadowLocalScale = _shadow.transform.localScale;
            _startShadowTransparency = _shadow.color.a;
            var positionDifference = _maxVerticalPosition - _minVerticalPosition;
            var sizeDifference = _maxSize - _minSize;
            _sizeModificator = sizeDifference / positionDifference;
            UpdateSize();
        }

        private void Update()
        {
            if (_isJumping)
                UpdateJump();

            UpdateAnimations();
        }

        private void UpdateAnimations()
        {
            _animator.PlayAnimation(AnimationType.Idle, true);
            _animator.PlayAnimation(AnimationType.Walk, _movement.magnitude > 0);
            _animator.PlayAnimation(AnimationType.Jump, _isJumping);
        }

        public void MoveHorizontally(float direction)
        {
            _movement.x = direction;
            SetDirection(direction);
            var velocity = _rb2d.velocity;
            velocity.x = _horizontalSpeed * direction;
            _rb2d.velocity = velocity;
        }

        public void MoveVertically(float direction)
        {
            if (_isJumping)
                return;

            _movement.y = direction;
            var velocity = _rb2d.velocity;
            velocity.y = _verticalSpeed * direction;
            _rb2d.velocity = velocity;

            if (direction == 0)
                return;

            var verticalPosition = Mathf.Clamp(transform.position.y, _minVerticalPosition, _maxVerticalPosition);
            _rb2d.position = new(_rb2d.position.x, verticalPosition);
            UpdateSize();
        }

        public void Jump()
        {
            if (_isJumping) return;

            _isJumping = true;
            var jumpModificator = transform.localScale.y / _maxSize;
            _rb2d.AddForce(_jumpForce * jumpModificator * Vector2.up);
            _rb2d.gravityScale = _gravityScale * jumpModificator;
            _startJumpVerticalPosition = transform.position.y;
            _shadowVerticalPosition = _shadow.transform.position.y;
        }

        private void UpdateSize ()
        {
            var verticalDelta = _maxVerticalPosition - transform.position.y;
            var currentSizeModificator = _minSize + _sizeModificator * verticalDelta;
            transform.localScale = Vector2.one * currentSizeModificator;
        }

        private void SetDirection(float direction)
        {
            if(_direction == Direction.Right && direction < 0 || 
                _direction == Direction.Left && direction > 0)
                Flip();
        }

        private void Flip()
        {
            transform.Rotate(0, 180, 0);
            _direction = _direction == Direction.Right ? Direction.Left : Direction.Right;
            foreach (var cameraPair in _cameras.DirectionalCameras)
                cameraPair.Value.enabled = cameraPair.Key == _direction;
        }

        private void UpdateJump()
        {
            if (_rb2d.velocity.y < 0 && _rb2d.position.y <= _startJumpVerticalPosition)
            {
                ResetJump();
                return;
            }

            _shadow.transform.position = new (_shadow.transform.position.x, _shadowVerticalPosition);
            var distance = transform.position.y - _startJumpVerticalPosition;
            _shadow.color = new(0, 0, 0, _startShadowTransparency - distance * _shadowAlphaModificator);
            _shadow.transform.localScale = _startShadowLocalScale * (1 + (_shadowSizeModificator * distance)); 
        }

        private void ResetJump()
        {
            _isJumping = false;
            _shadow.transform.localPosition = _shadowLocalPosition;
            _shadow.color = new(0, 0, 0, _startShadowTransparency);
            _rb2d.position = new(_rb2d.position.x, _startJumpVerticalPosition);
            _rb2d.gravityScale = 0;
        }

        public void StartAttack()
        {
            if (!_animator.PlayAnimation(AnimationType.Attack, true))
                return;

            _animator.AnimationEnded += FinishAttack;
        }

        private void FinishAttack()
        {
            _animator.AnimationEnded -= FinishAttack;
            _animator.PlayAnimation(AnimationType.Attack, false);
        }
    }
}
