﻿using System;
using System.Collections.Generic;
using System.Data;
using SFML.System;
using SFML.Window;
using SFML.Audio;
using SFML.Graphics;
using TiledSharp;

namespace Tonight
{
    public class GameProcess
    {
        private readonly Window2D window2D = new Window2D();
        private GameTime gameTime = new GameTime();
        private const float TIME_BEFORE_UPDATE = 1f / 40;
        private void ShowTime(GameTime gameTime)
        {
            var fps = (1f / gameTime.DeltaTime);
            var str = gameTime.TotalTimeElapsed + " " + gameTime.DeltaTime + " " + fps;
                      Console.WriteLine(str);
        }
        
        public void Run()
        {
            var camera = new Camera(800, 600);
            var map = new Map("maps/RectMapOnlyWithWallsAndGround.tmx", camera);
            var hero = new Hero(window2D, map);
            window2D.SetMouseCursorVisible(true);
            
            var enemy = new SecurityGuy(window2D, new Vector2f(300, 150));

            var totalTimeBeforeUpdate = 0f;
            var previousTimeElapsed = 0f;

            var clock = new Clock();
            
            while (window2D.IsOpen)
            {
                window2D.DispatchEvents();
                var totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                var deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;

                totalTimeBeforeUpdate += deltaTime;

                
                if (totalTimeBeforeUpdate >= TIME_BEFORE_UPDATE)
                {
                    gameTime.Update(totalTimeBeforeUpdate, totalTimeElapsed);
                    camera.Move(hero, map);
                    hero.Update(gameTime);
                    
                    //ShowTime(gameTime);
                    totalTimeBeforeUpdate = 0f;
                    window2D.SetView(camera);
                    window2D.Clear();
                    window2D.DrawBG();
                    window2D.Draw(map);

                                                        ////It should be better
                    window2D.Draw(hero);                //
                    window2D.Draw(hero.sight);          //

                    foreach (var bullet in hero.Bullets)
                    {
                        window2D.Draw(bullet);
                    }
                    
                    
                    
                    window2D.Draw(enemy);

                    window2D.Display();
                    
                }
            }
        }
    }
}