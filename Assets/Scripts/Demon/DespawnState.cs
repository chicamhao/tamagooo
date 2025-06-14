using UnityEngine;

namespace Demon
{
    public sealed class DespawnState : IState
    {
        StateControl _context;

        float _disappearTime = 0f;
        float _waitTime = 0;
        DespawnSettings _settings;

        public void Enter(StateControl control)
        {
            _context = control;
            _settings = _context.Settings.Despawn;
        }

        public void Update()
        {
            _disappearTime += Time.deltaTime;
            if (_disappearTime < _settings.DisappearDuration)
            {
                var t = _disappearTime / _settings.DisappearDuration;
                _context.transform.localScale = Vector3.Lerp(Vector3.one,Vector3.zero, t);
            }
            else
            {
                _waitTime += Time.deltaTime;
                if (_waitTime > _settings.WaitTime)
                {
                    _context.ChangeState(new SpawnState());
                }
            }
        }

        public void Exit()
        {
            _disappearTime = 0;
            _waitTime = 0;
            _context.transform.localScale = Vector3.one;
        }
    }
}