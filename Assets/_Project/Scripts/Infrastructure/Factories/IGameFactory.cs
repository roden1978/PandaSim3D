using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.Services;
using PlayerScripts;
using StaticData;
using UnityEngine;

namespace Infrastructure.Factories
{
    public interface IGameFactory : IService
    {
        void CleanUp();
        Player CreatePlayer();
        Mediator CreateMediator();
        public Player Player { get;}
        public Hud Hud { get; set; }
        void CreateGameMenu();
        void StartFactory();

    }
}