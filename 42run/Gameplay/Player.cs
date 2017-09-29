using OpenTK;
using System.Linq;

namespace _42run.Gameplay
{
    public class Player
    {
        private Vector3 _direction;
        public Vector3 Direction { get { return _direction; } set { _direction = value.Normalized(); } }
        public Vector3 Position { get; set; }
        private float _lastY = 0;
        public Vector3 PositionForCamera => new Vector3(Position.X, _lastY, Position.Z);
        public float Speed { get; set; }
        public static AxisAlignedBB BoundingBox { get; } = new AxisAlignedBB(new Vector3(-1.2f, 0, -1.2f), new Vector3(1.2f, 1.7f, 1.2f));
        public World World { get; set; }
        private Vector2 _lastTurn;
        private const float _maxSidewayDistance = 2f;
        private float _sidewayMove = 0f;
        private const float _sidewaySpeed = 0.2f;
        private const float _gravity = 0.025f;
        private const float _jumpPower = 0.4f;
        private float _yVelocity = 0f;
        private bool _onGround = true;
        private const float _maxStepSize = 0.4f;
        public bool Dead = false;
        
        public void Update(double time)
        {
            if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Right))
            {
                _sidewayMove -= _sidewaySpeed;
                if (_sidewayMove < -_maxSidewayDistance)
                    _sidewayMove = -_maxSidewayDistance;
            }
            else if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Left))
            {
                _sidewayMove += _sidewaySpeed;
                if (_sidewayMove > _maxSidewayDistance)
                    _sidewayMove = _maxSidewayDistance;
            }
            else
            {
                if (_sidewayMove > 0.01)
                    _sidewayMove -= _sidewaySpeed;
                else if (_sidewayMove < -0.01)
                    _sidewayMove += _sidewaySpeed;
                else
                    _sidewayMove = 0;
            }

            if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Up) && _onGround)
            {
                _onGround = false;
                _yVelocity = _jumpPower;
            }

            _yVelocity -= _gravity;

            Position += Direction * Speed * (float)time + (Vector3.UnitY * _yVelocity);

            var pos = GetPosition();
            var intersectionWithGrounds = World.Grounds.Where(g => g.BoundingBox.IntersectWith(g.Position, BoundingBox, pos)).ToList();
            if (intersectionWithGrounds.Count != 0)
                _yVelocity = 0;
            float? maxY = null;
            for (int i = 0; i < intersectionWithGrounds.Count; i++)
            {
                var intersectionY = intersectionWithGrounds[i].BoundingBox.Max.Y + intersectionWithGrounds[i].Position.Y;
                if (maxY == null || maxY < intersectionY)
                {
                    maxY = intersectionY;
                }
            }

            if (intersectionWithGrounds.Count == 0)
                _onGround = false;

            if (maxY != null)
            {
                if (maxY > pos.Y + _maxStepSize)
                    Dead = true;
                else if (maxY > pos.Y)
                    Position = new Vector3(Position.X, (float)maxY, Position.Z);
                _onGround = true;
            }

            if (_onGround)
                _lastY = Position.Y;
        }

        private Matrix3 _rotationRight = Matrix3.CreateRotationY(-MathHelper.PiOver2);

        public Vector3 GetPosition()
        {
            return _rotationRight * Direction * _sidewayMove + Position;
        }

        public Intersection GetIntersection()
        {
            var pos = GetPosition();
            return (Intersection)World.Grounds.FirstOrDefault(g => g.GetType() == typeof(Intersection) && ((Intersection)g).ActivableBoundingBox.IntersectWith(g.Position, BoundingBox, pos));
        }

        public bool CheckIfCollideWith(Obstacle obstacle) => BoundingBox.IntersectWith(obstacle.BoundingBox);
    }
}
