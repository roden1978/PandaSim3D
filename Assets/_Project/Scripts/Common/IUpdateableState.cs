using System;

namespace Common
{
    public interface IUpdateableState
    {
        public void Update();
        public void Exit();
    }
}