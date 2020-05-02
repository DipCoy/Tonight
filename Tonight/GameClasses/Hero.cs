﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using static SFML.Window.Keyboard;

namespace Tonight
{
    public class Hero : Sprite, IUpdatable
    {
        public Image HeroImage;
        public Map map;
        public Sight sight;
        private float speed = 500;
        private Window2D window;
        public List<Bullet> Bullets { get; set; }
        public Hero(Window2D window2D, Map map)
        {
            this.map = map;
            HeroImage = new Image("images/hero.png");
            Texture = new Texture(HeroImage);
            TextureRect = new IntRect(6, 232, 84, 53);
            Scale = new Vector2f(1f, 1f);
            Origin = new Vector2f(GetLocalBounds().Width / 2, GetLocalBounds().Height / 2);
            Position = new Vector2f(250, 150);
            sight = new Sight(window2D);
            Bullets = new List<Bullet>();
            window = window2D;
            window.KeyPressed += Shoot;
        }

        private void Shoot(object sender, KeyEventArgs e)
        {
            if (e.Code==Key.Space)
                Bullets.Add(new Bullet(Position, sight.Position,  map));
        }

        public static float GetVectorLength(Vector2f vector)
        {
            return (float) Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
        }
        public void Update(GameTime gameTime)
        {
            sight.Move();
            var cameraCenter = window.GetView().Center;
            sight.Move(cameraCenter - window.GetView().Size / 2);
            Move(gameTime);
            //sight
            RotateToCursor();

            Bullets = Bullets
                .Where(b =>
            {
                b.Update(gameTime);
                return b.IsAlive;
            })
                .ToList();
        }

        
        public void RotateToCursor()
        {
            var mouseVec = (Vector2f) Mouse.GetPosition(window);
            var sightLine = sight.Position - Position;
            var rotation = ((float) Math.Atan2(sightLine.Y, sightLine.X)) * 180 / (float) Math.PI;
            Rotation = rotation;
        }

        public bool CheckCollisions(Vector2f delta)
        {
            var objects = map.mapObjects["collision"];

            var collisionRect = GetSpriteRectangleWithoutRotation();
            collisionRect.Left += delta.X;
            collisionRect.Top += delta.Y;
            foreach (var obj in objects)
            {
                if (obj.Rect.Intersects(collisionRect))
                    return true;
            }

            return false;
        }

        private FloatRect GetSpriteRectangleWithoutRotation()
        {
            var tempRotation = Rotation;
            Rotation = 0;
            var rect = GetGlobalBounds();
            Rotation = tempRotation;
            return rect;
        }

    private void Move(GameTime gameTime)
        {
            if (IsKeyPressed(Key.W))
            {
                if (IsKeyPressed(Key.A))
                {
                    if (TryMove(Directions.UpLeft, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Up, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Left, gameTime.DeltaTime))
                        return;
                }

                if (IsKeyPressed(Key.D))
                {
                    if(TryMove(Directions.UpRight, gameTime.DeltaTime))
                        return;
                    if(TryMove(Directions.Up, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Right, gameTime.DeltaTime))
                        return;
                }

                TryMove(Directions.Up, gameTime.DeltaTime);
                return;
            }
            
            if (IsKeyPressed(Key.S))
            {
                if (IsKeyPressed(Key.A))
                {
                    if (TryMove(Directions.DownLeft, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Down, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Left, gameTime.DeltaTime))
                        return;
                }
                if (IsKeyPressed(Key.D))
                {
                    if(TryMove(Directions.DownRight, gameTime.DeltaTime))
                        return;
                    if(TryMove(Directions.Down, gameTime.DeltaTime))
                        return;
                    if (TryMove(Directions.Right, gameTime.DeltaTime))
                        return;
                }

                TryMove(Directions.Down, gameTime.DeltaTime);
                return;
            }

            if (IsKeyPressed(Key.A) && TryMove(Directions.Left, gameTime.DeltaTime))
                return;
            if (IsKeyPressed(Key.D))
                TryMove(Directions.Right, gameTime.DeltaTime);
        }
        
        private bool TryMove(Vector2f direction, float deltaTime)
        {
            var moveVector = speed * direction * deltaTime;
            if (!CheckCollisions(moveVector))
            {
                Position += moveVector;
                return true;
            }

            return false;
        }
    }
}