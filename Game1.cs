using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

namespace Lab5a_Mono
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D backgroundTexture; //The texture that will define the game background.
        //To animate explosion
        //bool explode; 
        Texture2D explosion1;
        Texture2D bigExplosion;

        //for spritesheet
        //float time = 0f;
        //float frameTime = .05f;
        int totalFrames = 16;
        //int frameIndex = 0; 
        //int frameIndex = 0;
        int spritesInRow = 4;
        //int x = 0;
        //int y = 0;
        int width = 60;
        int height = 60;
        //Rectangle spriteRect;

        //for bigexplosion spritesheet
        int big_width = 120;
        int big_height = 120; 

        ///<summary>
        ///The Viewport is used to draw on part of the screen. It should be set before any
        ///geometry is drawn, so that the Viewport parameters will take effect.
        ///To draw multiple views within a scene, repeat seeting Viewport and draw a geometry sequence for each view. 
        /// </summary>
        Rectangle viewportRect;

        //Game compontent variables.
        GameObject cannon; //The cannon will be controlled by human input.
        const int maxCannonBalls = 3; //Maximum number of cannonballs the cannon is able to shoot at a time.
        GameObject[] cannonBalls;

        //adding deathball to be able to drop from enemy ships
        GameObject[] deathBall; //need []???

        //Placeholder for when things explode
        GameObject transparent;

        //Sets up user input. 
        GamePadState previousGamePadState = GamePad.GetState(PlayerIndex.One);
        KeyboardState previousKeyboardState = Keyboard.GetState();

        //Enemy variables.
        const int maxEnemies = 3; //Maximum number of enemies rendered on the screen at a time. 
        const float maxEnemyHeight = 0.1f;
        const float minEnemyHeight = 0.5f;
        const float maxEnemyVelocity = 5.0f;
        const float minEnemyVelocity = 1.0f;
        Random random = new Random(); // create a source of random integers 
        GameObject[] enemies;

        //Score keeping variables. 
        int score;
        SpriteFont font;
        Vector2 scoreDrawPoint = new Vector2(0.1f, 0.1f);

        // Sound Effects
        SoundEffect explosion;
        SoundEffect missilelaunch;
        //new sound effect for deathBall
        SoundEffect bomb_drop;
        SoundEffect bomb_explosion;
        SoundEffect the_trooper_song;
        //fix looping of explosion
        SoundEffectInstance seInstance1;
        SoundEffectInstance background_music;


        bool animate_cannon = false;
        int hit_points = 100;
        int max_score = 15000;
        int start_bombs = 3000;

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================  

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }//constructor

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        //This method must be called once per frame.
        public void UpdateCannonBalls()
        {
            //"foreach" is a C# construct that we haven't used before.  Look it up.
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    ///<summary>
                    ///Here we check to see if the cannonball is alive; to do this,
                    ///we move it and do a simple check to determine if cannonball is offscreen.
                    ///If you were to add gravity, it would need to update every frame so that
                    ///the cannonball follows a realistic ballistic projectile path.
                    ///</summary>
                    ball.position += ball.velocity;
                    if (!viewportRect.Contains(new Point(
                        (int)ball.position.X,
                        (int)ball.position.Y)))
                    {
                        ball.alive = false;
                        continue; //we're done here.
                    }

                    //Creates an "invisible" rectangle that is the size of the cannonball sprite,
                    //used (later) to perform collision detection. 
                    Rectangle cannonBallRect = new Rectangle(
                        (int)ball.position.X,
                        (int)ball.position.Y,
                        ball.sprite.Width,
                        ball.sprite.Height);

                    //Check to see if the cannonball rectangle intersects with enemy rectangle.
                    //foreach (GameObject enemy in enemies)
                    for (int i = 0; i < maxEnemies; i ++)
                    {
                        //Construct rectangle at the coordinates of the enemies on screen.
                        Rectangle enemyRect = new Rectangle(
                            (int)enemies[i].position.X,
                            (int)enemies[i].position.Y,
                            enemies[i].sprite.Width,
                            enemies[i].sprite.Height);

                        //If collision between cannonball and enemy, then eliminate the alien ship.
                        if (cannonBallRect.Intersects(enemyRect))
                        {
                            ball.alive = false;

                            //enemies[i].alive = false;
                            enemies[i].explode = true;
                            //make an explosions sound
                            explosion.Play();
                            //big explosion spritesheet
                            //enemy_explode();


                            score += 1000;
                            break;
                        }//if
                    }//foreach

                    foreach (GameObject deathball in deathBall)
                    {
                        //Construct deathball rectangle
                        Rectangle dbRect = new Rectangle((int)deathball.position.X, (int)deathball.position.Y, deathball.sprite.Width, deathball.sprite.Height);

                        if (cannonBallRect.Intersects(dbRect))
                        {
                            deathball.explode = true;
                            ball.alive = false;
                            //deathball.alive = false;
                            seInstance1.Play();
                            score += 500;
                        }//if

                    }//foreach

                }
            }
        }

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        public void enemy_explode()
        {
            for (int i = 0; i < maxEnemies; i++)
            {
                if (enemies[i].explode==true)
                {
                    enemies[i].frameIndex++;
                    enemies[i].x = enemies[i].x + big_width;

                    //Reset at the end of row
                    if (enemies[i].frameIndex % spritesInRow == 0)
                    {
                        enemies[i].x = 0;
                        enemies[i].y = enemies[i].y + big_height;
                    }//if

                    //Reset if ran through spritesheet
                    if (enemies[i].frameIndex > totalFrames)
                    {
                        enemies[i].frameIndex = 0;
                        enemies[i].x = 0;
                        enemies[i].y = 0;
                        enemies[i].alive = false;
                        enemies[i].explode = false;
                    }//if

                }//if

            }//for

        }//enemy explode


        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================


        //Creates a method that render and kill enemies. 
        public void UpdateEnemies()
        {
            foreach (GameObject enemy in enemies)
            {
                if (enemy.alive)
                {

                    enemy.position += enemy.velocity;
                    //UpdateDeathball();
                    //Check to see if enemy is alive by seeing if it is still within the viewport
                    if (!viewportRect.Contains(new Point(
                        (int)enemy.position.X,
                        (int)enemy.position.Y)))
                    {
                        enemy.alive = false;
                        enemy.explode = false; //stop new spawning enemies from spawning as explosions
                    }
                }
                else
                {
                    enemy.alive = true;
                    //enemy.explode = false;
                    enemy.position = new Vector2(
                        viewportRect.Right,
                        MathHelper.Lerp(
                        (float)viewportRect.Height * minEnemyHeight,
                        (float)viewportRect.Height * maxEnemyHeight,
                        (float)random.NextDouble()));
                    enemy.velocity = new Vector2(
                        MathHelper.Lerp(
                        -minEnemyVelocity,
                        -maxEnemyVelocity,
                        (float)random.NextDouble()), 0);

                }//else

            }//foreach
        }//update enemies

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================


        public void UpdateDeathball()
        {
            bool repeat = true;
            for (int i = 0; i < maxEnemies; i++)
            {
                if ((enemies[i].alive) && (score >= start_bombs)) //get rid of macro
                {
                    if (deathBall[i].alive)
                    {
                        //Check to see if it is alive
                        deathBall[i].position += deathBall[i].velocity;

                        
                        Rectangle dbrect = new Rectangle((int)deathBall[i].position.X, (int)deathBall[i].position.Y, deathBall[i].sprite.Width, deathBall[i].sprite.Height);
                        Rectangle cannonrect = new Rectangle((int)cannon.position.X, (int)cannon.position.Y, cannon.sprite.Width, cannon.sprite.Height);


                        //rotate cannonrect based on cannon rotation???

                        //double newx = cannon.position.X + Math.Cos(cannon.rotation);
                        //double newy = cannon.position.Y + Math.Sin(cannon.rotation);
                        //Rectangle cannonrect = new Rectangle((int)newx, (int)newy, cannon.sprite.Width, cannon.sprite.Height);


                        /*
                        Vector2 upper_left = new Vector2( , );
                        Vector2 upper_right = new Vector2(,);

                        Vector2 lower_left = new Vector2(,);
                        Vector2 lower_right = new Vector2(,);
                        

                        Vector2 upper_left = new Vector2(cannon.position.X, cannon.position.Y);
                        Vector2 upper_right = new Vector2(cannon.position.X + cannon.sprite.Width, cannon.position.Y);
                        Vector2 lower_left = new Vector2(cannon.position.X, cannon.position.Y + cannon.sprite.Height);
                        Vector2 lower_right = new Vector2(cannon.position.X + cannon.sprite.Width, cannon.position.Y + cannon.sprite.Height);

                        */

                        //if mostly towards the bottom of the screen, play explosion sound
                        if (deathBall[i].position.Y >= (500) && deathBall[i].alive == true ||
                            (cannonrect.Intersects(dbrect)))
                        {
                            //bomb_explosion.Play();
                            seInstance1.Play();
                            deathBall[i].explode = true;
                            deathBall[i].frameIndex++;
                            deathBall[i].x = deathBall[i].x + width;

                            //flag to make cannon flash red on hit
                            //changed from if (cannonrect.Intersects(dbrect) &&...


                            if (cannonrect.Intersects(dbrect) && deathBall[i].frameIndex % 2 == 0 && repeat == true)
        
                         // if(Cannon_vs_db(upper_left, upper_right, lower_right, lower_left, deathBall[i])&& deathBall[i].frameIndex % 2 == 0 && repeat == true)
                            {
                                if (repeat == true)
                                {
                                    cannon.hit = true;

                                    hit_points--;

                                    repeat = false;
                                }//if


                                animate_cannon = true; 
                            }//if
                            else
                            {
                                cannon.hit = false;
                                animate_cannon = false;
                            }

                            //Reset at the end of row
                            if (deathBall[i].frameIndex % spritesInRow == 0)
                            {
                                deathBall[i].x = 0;
                                deathBall[i].y = deathBall[i].y + height;
                            }//if

                            //Reset if ran through spritesheet
                            if (deathBall[i].frameIndex > totalFrames)
                            {
                                deathBall[i].frameIndex = 0;
                                deathBall[i].x = 0;
                                deathBall[i].y = 0;
                                deathBall[i].alive = false;
                                deathBall[i].explode = false;
                            }//if


                            //seInstance1.Play();

                        }//if

                        if (!viewportRect.Contains(new Point(
                                (int)deathBall[i].position.X,
                                (int)deathBall[i].position.Y)))
                        {
                            deathBall[i].alive = false;
                            deathBall[i].explode = false;
                        }//if


                    }//if
                    else
                    {
                        if (enemies[i].position.X > graphics.PreferredBackBufferWidth/4)
                        { 
                            deathBall[i].alive = true;
                            bomb_drop.Play(.3f, 0.0f, 0.0f);  //half volume
                            deathBall[i].position = new Vector2((enemies[i].position.X + enemies[i].center.X), enemies[i].position.Y + (enemies[i].center.Y * 2));
                            deathBall[i].velocity = new Vector2(-1, 3);
                                //deathBall[i].explode = false;
                        }
                    }//else

                }//if
            }//foreach
        }//update deathball

        //========================================================================================================================================================================================================================   

            /*

        bool Cannon_vs_db(Vector2 upper_left, Vector2 upper_right, Vector2 lower_right, Vector2 lower_left, GameObject db)
        {
            //Formula to get new point for the upper left of the cannonrect
            double ul_x = (upper_left.X * Math.Cos(cannon.rotation) - (upper_left.Y * Math.Sin(cannon.rotation)));
            double ul_y = (upper_left.Y * Math.Cos(cannon.rotation)) + (upper_left.X * Math.Sin(cannon.rotation));

            upper_left = new Vector2( (float)ul_x, (float)ul_y);

            //Formula to get new point for the upper right of the cannonrect
            double ur_x = (upper_right.X * Math.Cos(cannon.rotation) - (upper_right.Y * Math.Sin(cannon.rotation)));
            double ur_y = (upper_right.Y * Math.Cos(cannon.rotation)) + (upper_right.X * Math.Sin(cannon.rotation));

            upper_right = new Vector2((float)ur_x, (float)ur_y);

            //Formulat to get new point for the lower right of the cannonrect
            double lr_x = (lower_right.X * Math.Cos(cannon.rotation) - (lower_right.Y * Math.Sin(cannon.rotation)));
            double lr_y = (lower_right.Y * Math.Cos(cannon.rotation)) + (lower_right.X * Math.Sin(cannon.rotation));

            lower_right = new Vector2((float)lr_x, (float)lr_y);

            //Formula to get a new point for the lower left of the cannorect
            double ll_x = (lower_left.X * Math.Cos(cannon.rotation) - (lower_left.Y * Math.Sin(cannon.rotation)));
            double ll_y = (lower_left.Y * Math.Cos(cannon.rotation)) + (lower_left.X * Math.Sin(cannon.rotation));

            lower_left = new Vector2((float)ll_x, (float)ll_y);


            //Construct points for the deathball[
            Vector2 db_ul = new Vector2(db.position.X, db.position.Y);
            Vector2 db_ur = new Vector2(db.position.X + db.sprite.Width, db.position.Y);
            Vector2 db_ll = new Vector2(db.position.X, db.sprite.Height + db.position.Y);
            Vector2 db_lr = new Vector2(db.position.X + db.sprite.Width, db.position.Y + db.sprite.Height);

            //Does the cannonrect intersect with the dbrect?
            
            //Not if one rectangle is on the left side of the other
            if (upper_left.X > db_lr.X || db_ul.X > lower_right.X)
            {
                return false;
            }//if

            //Not if one rectangle is above the other
            if (upper_left.Y < db_lr.Y || db_ul.Y < lower_right.Y)
            {

                return false;
            }//if

            return true;

        }//rotate_cannon function--used to d

    */
         //========================================================================================================================================================================================================================   
         //========================================================================================================================================================================================================================

            
        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        //Creates a method that will use the rotation of the cannon to fire a cannonball at the proper velocity.
        public void FireCannonBall()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (!ball.alive)
                {
                    missilelaunch.Play();
                    ball.alive = true;
                    ball.position = cannon.position - ball.center;//This allows the cannonball to be shot from the middle of the cannon.
                    ball.velocity = new Vector2(
                        (float)Math.Cos(cannon.rotation),
                        (float)Math.Sin(cannon.rotation)) * 5.0f;
                    return;
                }
            }
        }

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        public void Gravity_on_Ball()
        {
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.position.X > cannon.position.X + (cannon.center.X * 2))
                {
                    double effect_of_grav = .1;
                    float grav = (float)effect_of_grav;
                    Vector2 gravity = new Vector2(0, grav);
                    ball.velocity += gravity;
                }

            }//foreach
        }//gravity on ball function

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            graphics.ApplyChanges();
            Window.Title = "Lab5a - Alien Invaders";

            base.Initialize();
        }

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Creates a new SpriteBatch, which is used to draw textures.
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            // Load the SoundEffect resource
            explosion = Content.Load<SoundEffect>("explosion");
            missilelaunch = Content.Load<SoundEffect>("missilelaunch");
            bomb_drop = Content.Load<SoundEffect>("bomb_drop2");
            bomb_explosion = Content.Load<SoundEffect>("Explosion+7");
            the_trooper_song = Content.Load<SoundEffect>("the_trooper");
            //fix explosion looping problem
            seInstance1 = bomb_explosion.CreateInstance();
            seInstance1.IsLooped = false;
            background_music = the_trooper_song.CreateInstance();
            background_music.IsLooped = true;

            //Initialize game background.
            backgroundTexture =
                   Content.Load<Texture2D>("background");

            //Initialize the position and texture of the player's cannon. 
            cannon = new GameObject(Content.Load<Texture2D>("cannon"));
            cannon.position = new Vector2(120, graphics.GraphicsDevice.Viewport.Height - 80);

            //Transparent object to use when things explode
            transparent = new GameObject(Content.Load<Texture2D>("transparent"));


            //Initializes deathBall dropped by enemies
            deathBall = new GameObject[maxEnemies];  //can't have more deathballs than ships
            for (int i = 0; i < maxEnemies; i++)
            {
                //Load deathball
                deathBall[i] = new GameObject(Content.Load<Texture2D>("death_ball"));

            }


            //Initialize an array of new cannonball GameObjects that can be fired by the player.
            cannonBalls = new GameObject[maxCannonBalls];
            for (int i = 0; i < maxCannonBalls; i++)
            {
                //Load player's cannonball sprite.
                cannonBalls[i] = new GameObject(Content.Load<Texture2D>("cannonball"));
            }

            //Initialize enemies.
            enemies = new GameObject[maxEnemies];
            for (int i = 0; i < maxEnemies; i++)
            {
                //Load enemy sprite.
                enemies[i] = new GameObject(
                    Content.Load<Texture2D>("enemy"));
            }

            //Load the font sprite that will be used for Score keeping.
            font = Content.Load<SpriteFont>("Arial");

            //Create a Rectangle that represents the full drawable area of the game screen.
            viewportRect = new Rectangle(0, 0,
                graphics.GraphicsDevice.Viewport.Width,
                graphics.GraphicsDevice.Viewport.Height);

            //initialize explosion animation
            explosion1 = Content.Load<Texture2D>("explosion_spritesheet2");
            bigExplosion = Content.Load<Texture2D>("bigexplosion_spritesheet");
            //use []??? nah apparently
        }

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        //========================================================================================================================================================================================================================   
        //========================================================================================================================================================================================================================

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        /// 
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            background_music.Play();
            //Checks for user input from Xbox360 controller.
            //Uses controller joysticks to move the cannon. 
            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            cannon.rotation += gamePadState.ThumbSticks.Left.X * 0.1f;
            if (gamePadState.Buttons.A == ButtonState.Pressed &&
                previousGamePadState.Buttons.A == ButtonState.Released) //Pressing A fires the cannonball.
            {
                FireCannonBall();
            }

#if !XBOX   // This is how to include XBOX-specific or Windows-specific code
            // Checks for user input and computer keyboard.
            // Uses arrow keys to move the cannon.

            KeyboardState keyboardState = Keyboard.GetState();
             if (keyboardState.IsKeyDown(Keys.Left))
            {
                cannon.rotation -= 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                cannon.rotation += 0.1f;
            }
            if (keyboardState.IsKeyDown(Keys.Space) &&
                previousKeyboardState.IsKeyUp(Keys.Space)) //Pressing spacebar fires cannonball.
            {
                FireCannonBall();
            }
#endif
            //Determines how far you can rotate the cannon.
            //Clamp restricts a value to be within a specified range (in this case, 0 to 90 degrees).
            cannon.rotation = MathHelper.Clamp(cannon.rotation, -MathHelper.PiOver2, 0);

            //Update our game objects and sound
            UpdateCannonBalls();
            UpdateEnemies();
            UpdateDeathball();
            enemy_explode();
            
            Gravity_on_Ball();
            //Add future game logic here. 

            previousGamePadState = gamePadState;
#if !XBOX
            previousKeyboardState = keyboardState;
#endif

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            //Draw the backgroundTexture sized to the width
            //and height of the screen.
            spriteBatch.Draw(backgroundTexture, viewportRect,
                Color.White);

            //Draw player cannonballs only if they are alive.
            foreach (GameObject ball in cannonBalls)
            {
                if (ball.alive)
                {
                    spriteBatch.Draw(ball.sprite,
                        ball.position, Color.White);
                }
            }

            //Draw the cannon.
            //animate cannon makes it flash red if hit
            if (animate_cannon)
            {
                spriteBatch.Draw(cannon.sprite,
                cannon.position,
                null,
                Color.Red,
                cannon.rotation,
                cannon.center, 1.0f,
                SpriteEffects.None, 0);
            }
            else
            {
                    spriteBatch.Draw(cannon.sprite,
                 cannon.position,
                 null,
                 Color.White,
                 cannon.rotation,
                 cannon.center, 1.0f,
                 SpriteEffects.None, 0);
            }
         

            //Draw the alien ships.
            //foreach (GameObject enemy in enemies)
            for (int i = 0; i < maxEnemies; i++)
            {
                if (enemies[i].alive && !enemies[i].explode)
                {
                    spriteBatch.Draw(enemies[i].sprite,
                        enemies[i].position, Color.White);
                }//if

                if (enemies[i].explode)
                {
                    enemies[i].spriteRect = new Rectangle(enemies[i].x, enemies[i].y, big_width, big_height);
                    spriteBatch.Draw(bigExplosion, enemies[i].position, enemies[i].spriteRect, Color.White);


                }//if

            }//for



            //foreach (GameObject deathball in deathBall)
            for (int i = 0; i < maxEnemies; i++)
            {
                if (deathBall[i].alive && !deathBall[i].explode)
                {
                    spriteBatch.Draw(deathBall[i].sprite, deathBall[i].position, Color.White);
                }//draw deathball


                if (deathBall[i].explode)
                {
                    
                    deathBall[i].spriteRect = new Rectangle(deathBall[i].x, deathBall[i].y, width, height);
                    spriteBatch.Draw(explosion1, deathBall[i].position, deathBall[i].spriteRect, Color.White);

                }
            }//foreach
            

                    //Draws the scoreboard.
                    spriteBatch.DrawString(font,
                "Score: " + score.ToString() + "\nHP: " + hit_points.ToString(),  //note the use of the ToString function
                                               //Indicates coordinates on where to draw the score. 
                new Vector2(scoreDrawPoint.X * viewportRect.Width,
                scoreDrawPoint.Y * viewportRect.Height),
                Color.Purple);

                if (score >= max_score)
                {
                spriteBatch.DrawString(font, "You win!", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Purple);
                }

                if (hit_points <= 0)
                {
                spriteBatch.DrawString(font, "You lose!", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Purple);
                }

            spriteBatch.End();



                    base.Draw(gameTime);
                }
            }
            //========================================================================================================================================================================================================================   
            //========================================================================================================================================================================================================================

        }
    
