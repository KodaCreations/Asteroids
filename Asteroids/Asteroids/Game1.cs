using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Asteroids
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Texture2D ast_texture, bg_texture, gamemenu_texture, gameover_texture;
        Vector2 ast_position, ast_direction, bg_position, score_position, life_position, end_score_position;
        Rectangle ast_boundingbox, mouse_rectangle, game_field, spawn_area;
        int spawn_area_boundry, spawn_timer, score, life, difficulty;
        float ast_speed, respawn_time;
        MouseState mouse_state, old_mouse_state;
        List<Asteroid> asteroidList;
        Random rnd;
        KeyboardState key_state, old_key_state;

        enum GameState { GameMenu, InGame, GameOver }
        GameState currentGameState = GameState.GameMenu;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;
        }


        protected override void Initialize()
        { base.Initialize(); }


        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("SpriteFont1");
            ast_texture = Content.Load<Texture2D>(@"asteroid");
            bg_texture = Content.Load<Texture2D>(@"background");
            gamemenu_texture = Content.Load<Texture2D>(@"gamemenu");
            gameover_texture = Content.Load<Texture2D>(@"gameover");
            asteroidList = new List<Asteroid>();
            bg_position = new Vector2(0, 0);
            score_position = new Vector2(10, 10);
            life_position = new Vector2(10, 33);
            end_score_position = new Vector2((Window.ClientBounds.Width / 2) - 85, Window.ClientBounds.Height / 2);
            rnd = new Random();
            game_field = new Rectangle((int)-ast_texture.Width, (int)-ast_texture.Height, Window.ClientBounds.Width + ast_texture.Width, Window.ClientBounds.Height + ast_texture.Height);
            spawn_area_boundry = 200;
            spawn_area = new Rectangle((int)spawn_area_boundry, (int)spawn_area_boundry, Window.ClientBounds.Width - (spawn_area_boundry * 2), Window.ClientBounds.Height - (spawn_area_boundry * 2));

            ResetGame();

            //Creates two asteroids at the start of the game
            for (int i = 0; i < 2; i++)
            {
                CreateAsteroid();
            }
        }


        protected override void UnloadContent()
        { }

        //Method to create an asteroid and add it to the list of asteroids
        private void CreateAsteroid()
        {
            ast_position = new Vector2(rnd.Next(spawn_area.Left, spawn_area.Right), rnd.Next(spawn_area.Top, spawn_area.Bottom));
            ast_direction = new Vector2((float)Math.Cos(rnd.Next()), (float)Math.Sin(rnd.Next()));
            ast_speed = rnd.Next(2, 3);
            ast_boundingbox = new Rectangle(0, 0, ast_texture.Width, ast_texture.Height);
            Asteroid temp_asteroid = new Asteroid(ast_texture, ast_position, ast_direction, ast_speed, ast_boundingbox);
            asteroidList.Add(temp_asteroid);
        }


        //Method to reset various variables to the state they're in when the game starts
        private void ResetGame()
        {
            spawn_timer = 0;
            respawn_time = 60;
            difficulty = 0;
            score = 0;
            life = 10;
        }


        protected override void Update(GameTime gameTime)
        {
            old_key_state = key_state;
            key_state = Keyboard.GetState();

            old_mouse_state = mouse_state;
            mouse_state = Mouse.GetState();
            mouse_rectangle = new Rectangle((int)mouse_state.X, (int)mouse_state.Y, 5, 5);

            //Allows the game to shut down using the Escape key
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                this.Exit();
            }

            switch (currentGameState)
            {
                case GameState.GameMenu:

                    ResetGame();
                    if (key_state.IsKeyDown(Keys.F1) && old_key_state.IsKeyUp(Keys.F1))
                    {
                        currentGameState = GameState.InGame;
                    }

                    break;

                case GameState.InGame:

                    //Spawns in a new asteroid after a certain amount of time
                    if (spawn_timer >= respawn_time)
                    {
                        //Spawns in a single asteroid if score is lower than 20, and two asteroids of it's above 20
                        if (score <= 20)
                        {
                            CreateAsteroid();                
                        }
                        else if(score >= 21)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                CreateAsteroid();
                            }
                        }
                        spawn_timer = 0;

                        //Increases the spawnrate of asteroids after a certain amount of time
                        if (difficulty == 600)
                        {
                            respawn_time *= 0.7f;
                            difficulty = 0;
                        }
                    }
                    else
                    {
                        spawn_timer += 1;
                        difficulty += 1;
                    }

                    //Handles updates for each asteroid
                    for (int i = 0; i < asteroidList.Count; ++i)
                    {
                        asteroidList[i].Update();
                        Rectangle asteroidBox = new Rectangle((int)asteroidList[i].position.X, (int)asteroidList[i].position.Y, asteroidList[i].boundingbox.Width, asteroidList[i].boundingbox.Height);

                        //Checks if the asteroid is being clicked
                        if (asteroidBox.Intersects(mouse_rectangle) && mouse_state.LeftButton == ButtonState.Pressed && old_mouse_state.LeftButton == ButtonState.Released)
                        {
                            asteroidList.RemoveAt(i);
                            score += 1;
                        }

                        //Checks if the asteroid is within the game field
                        if (!asteroidBox.Intersects(game_field))
                        {
                            asteroidList.RemoveAt(i);
                            life -= 1;
                        }
                    }

                    if (life == 0)
                    {
                        currentGameState = GameState.GameOver;
                        asteroidList.Clear();
                    }

                    break;

                case GameState.GameOver:

                    if (key_state.IsKeyDown(Keys.F1) && old_key_state.IsKeyUp(Keys.F1))
                    {
                        currentGameState = GameState.GameMenu;
                    }

                    break;

            }



            Window.Title = "Asteroids! Current score: " + score.ToString();
            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();

            switch (currentGameState)
            {
                case GameState.GameMenu:

                    //Draws the game menu
                    spriteBatch.Draw(gamemenu_texture, bg_position, Color.White);

                    break;

                case GameState.InGame:

                    spriteBatch.Draw(bg_texture, bg_position, Color.White);

                    //Draws each asteroid in the asteroid list
                    foreach (Asteroid asteroid in asteroidList)
                    {
                        asteroid.Draw(spriteBatch);
                    }

                    //Draws out score and remaining lives
                    spriteBatch.DrawString(spriteFont, @"Score: " + score, score_position, Color.White);
                    spriteBatch.DrawString(spriteFont, @"Life " + life, life_position, Color.White);

                    break;

                case GameState.GameOver:

                    //Draws the game over screen and final score
                    spriteBatch.Draw(gameover_texture, bg_position, Color.White);
                    spriteBatch.DrawString(spriteFont, @"Final score: " + score, end_score_position, Color.White);

                    break;
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
