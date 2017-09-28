using OpenTK;
using System.Linq;

namespace _42run.Gameplay
{
    public class Player
    {
        private Vector3 _direction;
        public Vector3 Direction { get { return _direction; } set { _direction = value.Normalized(); } }
        public Vector3 Position { get; set; }
        public float Speed { get; set; }
        public static AxisAlignedBB BoundingBox { get; } = new AxisAlignedBB(new Vector3(-0.2f, 0, -0.2f), new Vector3(0.2f, 1.7f, 0.2f));
        public World World { get; set; }
        private Vector2 _lastTurn;
        private const float _maxSidewayDistance = 2f;
        private float _sidewayMove = 0f;
        private const float _sidewaySpeed = 0.1f;
        private const float _gravity = 0.1f;
        
        public void Update(double time)
        {
            if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Left))
            {
                _sidewayMove -= _sidewaySpeed;
                if (_sidewayMove < -_maxSidewayDistance)
                    _sidewayMove = -_maxSidewayDistance;
            }
            else if (KeyboardHelper.GetKeyboardState().IsKeyDown(OpenTK.Input.Key.Right))
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

            

            Position += Direction * Speed * (float)time;
        }

        private Matrix3 _rotationRight = Matrix3.CreateRotationY(-MathHelper.PiOver2);

        public Vector3 GetPosition()
        {
            return _rotationRight * Direction * _sidewayMove + Position;
        }

        public Intersection GetIntersection()
        {
            return (Intersection)World.Grounds.FirstOrDefault(g => g.GetType() == typeof(Intersection) && ((Intersection)g).BoundingBox.IntersectWith(BoundingBox));
        }

        public bool CheckIfCollideWith(Obstacle obstacle) => BoundingBox.IntersectWith(obstacle.BoundingBox);
    }
}
