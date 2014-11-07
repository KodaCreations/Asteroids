using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroids
{
    class Asteroid
    {
        public Vector2 position;
        public Vector2 direction;
        public float speed;
        public Rectangle boundingbox;
        public Texture2D texture;
        

        public Asteroid(Texture2D texture, Vector2 position, Vector2 direction, float speed, Rectangle boundingbox)
        {
            this.position = position;
            this.direction = direction;
            this.speed = speed;
            this.boundingbox = boundingbox;
            this.texture = texture;
        }


        public void Update()
        {
            position += direction * speed;
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }
}