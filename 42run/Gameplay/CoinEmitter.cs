using OpenTK;
using System;
using System.Collections.Generic;

namespace _42run.Gameplay
{
    public class CoinEmitter
    {
        private List<CloneableCoin> _coins;

        private int _toEmit;
        private int _emitted;
        private float _emittionRate;
        private float _lastEmitted;
        private float _lifeTime;
        private float _spawnAngle;
        public float MinCoinSpeed { get; set; }
        public float CoinSpeedVariation { get; set; }
        private CloneableCoin _coinToClone;
        private Random _rand;

        public Vector3 Position;

        public CoinEmitter(ref List<CloneableCoin> coins, CloneableCoin coinToClone, Vector3 position, float emittionRate, int toEmit, float spawnAngle, float minCoinSpeed, float maxCoinSpeed)
        {
            _coinToClone = coinToClone;
            _coins = coins;
            _toEmit = toEmit;
            _emittionRate = emittionRate;
            _spawnAngle = spawnAngle;
            MinCoinSpeed = minCoinSpeed;
            CoinSpeedVariation = maxCoinSpeed - minCoinSpeed;
            _rand = new Random();
        }

        public void Update(double time)
        {
            if(_emitted >= _toEmit)
                return;
            _lifeTime += (float)time;
            if (_lifeTime - _lastEmitted > _emittionRate)
            {
                var numberToEmit = (int)((_lifeTime - _lastEmitted) / _emittionRate);
                _emitted += numberToEmit;

                for(int i = 0; i < numberToEmit; i++)
                {
                    var coin = (CloneableCoin)_coinToClone.Clone();
                    coin.Velocity = Matrix3.CreateRotationZ(((float)_rand.NextDouble() - 0.5f) * _spawnAngle) * new Vector3(0, 1, 0) * (MinCoinSpeed + (float)_rand.NextDouble() * CoinSpeedVariation);
                    coin.Position = Position;
                    coin.RotationVelocity = new Vector3(((float)_rand.NextDouble() - 0.5f) * 2f, ((float)_rand.NextDouble() - 0.5f) * 2f, ((float)_rand.NextDouble() - 0.5f) * 2f);
                    coin.SetRotations(new Vector3((float)_rand.NextDouble() - 0.5f, (float)_rand.NextDouble() - 0.5f, (float)_rand.NextDouble() - 0.5f));
                    _coins.Add(coin);
                }

                _lastEmitted += numberToEmit * _emittionRate;
            }
        }
    }
}
