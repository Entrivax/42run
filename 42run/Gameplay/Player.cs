using OpenTK;
using System.Linq;

namespace _42run.Gameplay
{
    public class Player
    {
        public Vector3 Position { get; set; }
        private float _lastY = 0;
        public Vector3 PositionForCamera => new Vector3(Position.X, _lastY, Position.Z);
        public float Speed { get; set; }
        private const float _speedIncrease = 1f;
        private const float _maxSpeed = 22.5f;
        private static Vector3 _bbP1 = new Vector3(-0.6375f, 0, -0.65f);
        private static Vector3 _bbP2 = new Vector3(0.6375f, 1.7f, 0f);
        private static AxisAlignedBB[] _boundingBoxes = new[]
        {
            new AxisAlignedBB(_bbP1, _bbP2),
            new AxisAlignedBB(Vector3.ComponentMin(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.EAST)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.EAST))), Vector3.ComponentMax(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.EAST)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.EAST)))),
            new AxisAlignedBB(Vector3.ComponentMin(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.SOUTH)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.SOUTH))), Vector3.ComponentMax(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.SOUTH)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.SOUTH)))),
            new AxisAlignedBB(Vector3.ComponentMin(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.WEST)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.WEST))), Vector3.ComponentMax(new Vector3(new Vector4(_bbP1, 1) * DirectionHelper.GetRotationFromDirection(Direction.WEST)), new Vector3(new Vector4(_bbP2, 1) * DirectionHelper.GetRotationFromDirection(Direction.WEST)))),
        };
        public AxisAlignedBB BoundingBox { get { return _boundingBoxes[(int)CurrentDirection]; } }
        public World World { get; set; }
        private const float _maxSidewayDistance = 2f;
        private float _sidewayMove = 0f;
        private const float _sidewaySpeed = 0.2f;
        private const float _gravity = 0.025f;
        private const float _jumpPower = 0.4f;
        private float _yVelocity = 0f;
        private bool _onGround = true;
        private const float _maxStepSize = 0.4f;
        public bool Dead = false;
        public Direction CurrentDirection = Direction.NORTH;
        public float Score = 0f;
        private const float _scoreIncrementation = 15f;
        
        public void Update(double time)
        {
            if (Dead)
            {
                return;
            }
            Score += _scoreIncrementation * (float)time;
            Speed += (float)time * _speedIncrease;
            if (Speed > _maxSpeed)
                Speed = _maxSpeed;
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

            var intersection = GetIntersection();
            if (intersection != null)
            {
                if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Left))
                {
                    Position = new Vector3(intersection.Position.X, Position.Y, intersection.Position.Z) + DirectionHelper.GetVectorFromDirection(intersection.Direction) * 3f;
                    var dir = (int)CurrentDirection - 1;
                    if (dir < 0)
                        dir = (int)Direction.WEST;
                    CurrentDirection = (Direction)dir;
                    intersection.Used = true;
                }
                else if (KeyboardHelper.IsKeyPressed(OpenTK.Input.Key.Right))
                {
                    Position = new Vector3(intersection.Position.X, Position.Y, intersection.Position.Z) + DirectionHelper.GetVectorFromDirection(intersection.Direction) * 3f;
                    var dir = (int)CurrentDirection + 1;
                    if (dir > 3)
                        dir = (int)Direction.NORTH;
                    CurrentDirection = (Direction)dir;
                    intersection.Used = true;
                }
            }

            _yVelocity -= _gravity;

            Position += DirectionHelper.GetVectorFromDirection(CurrentDirection) * Speed * (float)time + (Vector3.UnitY * _yVelocity);

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

            if (CollideWithObstacle())
                Dead = true;
        }

        private Matrix3 _rotationRight = Matrix3.CreateRotationY(-MathHelper.PiOver2);

        public Vector3 GetPosition()
        {
            return _rotationRight * DirectionHelper.GetVectorFromDirection(CurrentDirection) * _sidewayMove + Position;
        }

        public Intersection GetIntersection()
        {
            var pos = GetPosition();
            var typeInter = typeof(Intersection);
            return (Intersection)World.Grounds.FirstOrDefault(g => g.GetType() == typeInter && BoundingBox.IntersectWith(pos, ((Intersection)g).ActivableBoundingBox, g.Position) && !((Intersection)g).Used);
        }

        public bool CollideWithObstacle()
        {
            var position = GetPosition();
            return World.Obstacles.Any(obstacle => CheckIfCollideWith(obstacle, position));
        }

        public bool CheckIfCollideWith(Obstacle obstacle, Vector3 position) => BoundingBox.IntersectWith(position, obstacle.BoundingBox, obstacle.Position);
    }
}
