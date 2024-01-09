using Infrastructure.Services;
using UnityEngine;

namespace Services.Input
{
    public interface IInputService : IService
    {
       Vector2 Horizontal { get;}
       Vector2 Vertical { get;}
       void Update();
    }
}