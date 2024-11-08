using System;
using System.Collections.Generic;
using System.Media;
using Tao.Sdl;

public struct GameObject
{
    public float positionX;
    public float positionY;
    public float speedX;
    public float speedY;
    public int width;
    public int height;
    public Image image;
    public bool isSolid;
    public bool death;
    public bool delete;
}

namespace MyGame
{
    class Program
    {
        // Estados del juego
        enum GameState
        {
            StartScreen,
            Stage1,
            Stage2,
            GameOver,
            Win
        }

        static GameState currentState = GameState.StartScreen;

        //Main menu
        static Image mainMenu = Engine.LoadImage("assets/mainmenu.png");

        //Game over
        static float gameOverTimer = 2f;
        static Image gameOver = Engine.LoadImage("assets/game-over.png");

        // Delta Time
        static DateTime startTime;
        static float deltaTime;
        static float lastFrame = 0;

        // Player
        static GameObject player;

        //Enemy
        static GameObject enemy;
        static GameObject enemy1;
        static float enemyDelete = 2f;
        static float enemy1Delete = 2f;

        //Chest
        static GameObject chest;

        // Animacion del player
        static Image idleImage;
        static List<Image> walkRight;
        static List<Image> walkLeft;
        static int currentFrame = 0;
        static float animationTime = 0f;
        static float animationSpeed = 0.1f;

        static List<Image> walkEnemyRight;
        static List<Image> walkEnemyLeft;
        static int currentEnemyFrame = 0;
        static float animationEnemyTime = 0f;

        static List<Image> walkEnemy1;
        static int currentEnemy1Frame = 0;
        static float animationEnemy1Time = 0f;

        static List<GameObject> platforms;
        static List<GameObject> spikes;


        static bool onGround = false;
        static float gravity = 2000f;
        static bool canDash = true;
        static bool dashing = false;
        static float dashTimeLeft = 0f;
        static float dashCooldownTimeLeft = 0f;
        static bool death = false;

        static Image dashCooldownImage = Engine.LoadImage("assets/aviable-dash.png");

        //Jumpad
        static GameObject jumpad;

        //Spikes
        static GameObject spike1;
        static GameObject spike2;
        static GameObject spike3;

        // Funciones
        static void StartDash()
        {
            dashing = true;
            canDash = false;
            dashTimeLeft = 0.2f;
            dashCooldownTimeLeft = 1.0f;
            dashCooldownImage = Engine.LoadImage("assets/cooldown-dash.png");
        }

        static void EndDash()
        {
            dashing = false;
            player.speedX /= 5; 
        }


        static bool IsColliding(GameObject a, GameObject b)
        {
            return (a.positionX < b.positionX + b.width &&
                    a.positionX + a.width > b.positionX &&
                    a.positionY < b.positionY + b.height &&
                    a.positionY + a.height > b.positionY);
        }


        static void InitializeGameObjects()
        {
            player.positionX = 100;
            player.positionY = 600;
            player.speedX = 0;
            player.speedY = 0;
            player.width = 52;
            player.height = 52;
            idleImage = Engine.LoadImage("assets/player/idle.png");
            player.image = idleImage;

            enemy.positionX = 510;
            enemy.positionY = 692;
            enemy.speedX = 250;
            enemy.speedY = 0;
            enemy.width = 52;
            enemy.height = 56;
            enemy.image = idleImage;
            enemy.death = false;
            enemy.delete = false;

            enemy1.positionX = 655;
            enemy1.positionY = 140;
            enemy1.speedX = 150;
            enemy1.speedY = 20;
            enemy1.width = 40;
            enemy1.height = 44;
            enemy1.image = idleImage;
            enemy1.delete = false;

            chest.positionX = 1490;
            chest.positionY = 105;
            chest.width = 64;
            chest.height = 48;
            chest.image = Engine.LoadImage("assets/chest.png");

            //Animaciones
            walkRight = new List<Image>();
            walkRight.Add(Engine.LoadImage("assets/player/right1.png"));
            walkRight.Add(Engine.LoadImage("assets/player/right2.png"));
            walkRight.Add(Engine.LoadImage("assets/player/right3.png"));

            walkLeft = new List<Image>();
            walkLeft.Add(Engine.LoadImage("assets/player/left1.png"));
            walkLeft.Add(Engine.LoadImage("assets/player/left2.png"));
            walkLeft.Add(Engine.LoadImage("assets/player/left3.png"));

            walkEnemyRight = new List<Image>();
            walkEnemyRight.Add(Engine.LoadImage("assets/enemy/enemy-right1.png"));
            walkEnemyRight.Add(Engine.LoadImage("assets/enemy/enemy-right2.png"));
            walkEnemyRight.Add(Engine.LoadImage("assets/enemy/enemy-right3.png"));

            walkEnemyLeft = new List<Image>();
            walkEnemyLeft.Add(Engine.LoadImage("assets/enemy/enemy-left1.png"));
            walkEnemyLeft.Add(Engine.LoadImage("assets/enemy/enemy-left2.png"));
            walkEnemyLeft.Add(Engine.LoadImage("assets/enemy/enemy-left3.png"));


            walkEnemy1 = new List<Image>();
            walkEnemy1.Add(Engine.LoadImage("assets/enemy/enemy-fly1.png"));
            walkEnemy1.Add(Engine.LoadImage("assets/enemy/enemy-fly2.png"));
            walkEnemy1.Add(Engine.LoadImage("assets/enemy/enemy-fly3.png"));


            //Stage 1
            platforms = new List<GameObject>();
            for (float x = 65; x < 1520; x += 60)
                {
                    GameObject platform = new GameObject
                    {
                        positionX = x,
                        positionY = 750,
                        width = 60,
                        height = 60,
                        image = Engine.LoadImage("assets/platform.png"),
                        isSolid = true
                    };
                    platforms.Add(platform);
                }

            for (float x = 0; x < 1600; x += 60)
            {
                GameObject block = new GameObject
                {
                    positionX = x,
                    positionY = 0,
                    width = 60,
                    height = 60,
                    image = Engine.LoadImage("assets/block.png"),
                    isSolid = true
                };
                platforms.Add(block);
            }

            for (float y = 0; y < 900; y += 60)
            {
                GameObject block1 = new GameObject
                {
                    positionX = 0,
                    positionY = y,
                    width = 60,
                    height = 60,
                    image = Engine.LoadImage("assets/block.png"),
                    isSolid = true
                };
                platforms.Add(block1);
            }

            for (float y = 0; y < 900; y += 60)
            {
                GameObject block2 = new GameObject
                {
                    positionX = 1560,
                    positionY = y,
                    width = 60,
                    height = 60,
                    image = Engine.LoadImage("assets/block.png"),
                    isSolid = true
                };
                platforms.Add(block2);
            }

            GameObject block3 = new GameObject
            {
                positionX = 420,
                positionY = 685,
                width = 60,
                height = 60,
                image = Engine.LoadImage("assets/block.png"),
                isSolid = true
            };
            platforms.Add(block3);

            GameObject block4 = new GameObject
            {
                positionX = 840,
                positionY = 685,
                width = 60,
                height = 60,
                image = Engine.LoadImage("assets/block.png"),
                isSolid = true
            };
            platforms.Add(block4);

            GameObject platformFloat = new GameObject
            {
                positionX = 100,
                positionY = 500,
                width = 184,
                height = 28,
                image = Engine.LoadImage("assets/platform-float.png"),
                isSolid = true
            };
            platforms.Add(platformFloat);

            GameObject platformMini = new GameObject
            {
                positionX = 1400,
                positionY = 650,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini);

            GameObject platformMini1 = new GameObject
            {
                positionX = 1250,
                positionY = 550,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini1);

            GameObject platformMini2 = new GameObject
            {
                positionX = 1400,
                positionY = 450,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini2);

            GameObject platformMini3 = new GameObject
            {
                positionX = 1250,
                positionY = 400,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini3);

            GameObject platformMini4 = new GameObject
            {
                positionX = 1000,
                positionY = 400,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini4);

            GameObject platformMini5 = new GameObject
            {
                positionX = 700,
                positionY = 400,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini5);

            GameObject platformMini6 = new GameObject
            {
                positionX = 260,
                positionY = 250,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini6);

            GameObject platformMini7 = new GameObject
            {
                positionX = 540,
                positionY = 220,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini7);

           

            GameObject platformFloat2 = new GameObject
            {
                positionX = 900,
                positionY = 220,
                width = 184,
                height = 28,
                image = Engine.LoadImage("assets/platform-float.png"),
                isSolid = true
            };
            platforms.Add(platformFloat2);

            GameObject platformMini8 = new GameObject
            {
                positionX = 1200,
                positionY = 220,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini8);

            GameObject platformMini9 = new GameObject
            {
                positionX = 1270,
                positionY = 220,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini9);

            GameObject platformMini10 = new GameObject
            {
                positionX = 1340,
                positionY = 220,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini10);

            GameObject platformMini11 = new GameObject
            {
                positionX = 1410,
                positionY = 220,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini.png"),
                isSolid = true
            };
            platforms.Add(platformMini11);

            GameObject platformFinale = new GameObject
            {
                positionX = 1490,
                positionY = 160,
                width = 56,
                height = 28,
                image = Engine.LoadImage("assets/platform-mini-finale.png"),
                isSolid = true
            };
            platforms.Add(platformFinale);

            jumpad.positionX = 160;
            jumpad.positionY = 450;
            jumpad.width = 58;
            jumpad.height = 48;
            jumpad.image = Engine.LoadImage("assets/jumpad.png");


            spikes = new List<GameObject>();

            GameObject spike1 = new GameObject
            {
                positionX = 160,
                positionY = 60,
                width = 64,
                height = 48,
                image = Engine.LoadImage("assets/ceiling-spikes.png"),
            };
            spikes.Add(spike1);

            GameObject spike2 = new GameObject
            {
               positionX = 1250,
               positionY = 700,
                width = 60,
                height = 48,
               image = Engine.LoadImage("assets/spikes.png"),

            };
            spikes.Add(spike2);

            GameObject spike3 = new GameObject {
                positionX = 1515,
                positionY = 600,
                width = 48,
                height = 64,
                image = Engine.LoadImage("assets/spikes-right.png"),
            };
            spikes.Add(spike3);

            GameObject spike4 = new GameObject
            {
                positionX = 840,
                positionY = 640,
                width = 64,
                height = 48,
                image = Engine.LoadImage("assets/spikes.png"),
            };
            spikes.Add(spike4);

            GameObject spike5 = new GameObject
            {
                positionX = 1200,
                positionY = 171,
                width = 64,
                height = 48,
                image = Engine.LoadImage("assets/spikes.png"),
            };
            spikes.Add(spike5);

            GameObject spike6 = new GameObject
            {
                positionX = 1410,
                positionY = 171,
                width = 64,
                height = 48,
                image = Engine.LoadImage("assets/spikes.png"),
            };
            spikes.Add(spike6);

        }


            static void Main(string[] args)
        {
            Engine.Initialize();
            startTime = DateTime.Now;
            InitializeGameObjects();

            while (true)
            {
                CheckInputs();
                Update();
                Render();
                Sdl.SDL_Delay(20);
            }
        }

        static void CheckInputs()
        {
            // Menu de inicio
            if (currentState == GameState.StartScreen) 
            {
                if (Engine.KeyPress(Engine.KEY_ESP))
                {
                    currentState = GameState.Stage1;
                }
            }
            // Game Over
            else if (currentState == GameState.GameOver) 
            {
                if (Engine.KeyPress(Engine.KEY_ESP))
                {
                    death = false;
                    enemy.death = false;
                    enemy1.death = false;
                    enemy.delete = false;
                    enemy1.delete = false; 
                    enemy.speedX = 250;
                    enemy1.speedX = 150;
                    enemy1.speedY = 20;
                    enemyDelete = 2f;
                    enemy1Delete = 2f;
                    gameOverTimer = 2f;
                    player.positionX = 200;
                    player.positionY = 200;
                    currentState = GameState.Stage1;
                }
            }
            // Jugando
            else if (currentState == GameState.Stage1 || currentState == GameState.Stage2) 
            {
                if (!death) {
                    if (Engine.KeyPress(Engine.KEY_A))
                    {
                        player.speedX = -300;
                    }
                    else if (Engine.KeyPress(Engine.KEY_D))
                    {
                        player.speedX = 300;
                    }
                    else
                    {
                        player.speedX = 0;
                    }

                    if (Engine.KeyPress(Engine.KEY_ESP))
                    {
                        if (onGround)
                        {
                            onGround = false;
                            player.speedY = -700f;
                        }
                    }

                    if (Engine.KeyPress(Engine.KEY_SHIFT))
                    {
                        if (canDash && !dashing)
                        {   
                            StartDash();
                        }
                    }
                }
                
            }
            if (Engine.KeyPress(Engine.KEY_ESC))
            {
                Environment.Exit(0);
            }
        }


        static void Update()
        {
            // Delta time
            float currentTime = (float)(DateTime.Now - startTime).TotalSeconds;
            deltaTime = currentTime - lastFrame;
            lastFrame = currentTime;

            if (currentState == GameState.StartScreen)
            {
                return;
            } else if (currentState == GameState.Stage1)
            {
                onGround = false;
                // Dash
                if (dashing)
                {

                    player.speedX *= 5;
                    dashTimeLeft -= deltaTime;
                    if (dashTimeLeft <= 0)
                    {
                        EndDash();
                    }
                }
                else
                {
                    dashCooldownTimeLeft -= deltaTime;
                    if (dashCooldownTimeLeft <= 0)
                    {
                        canDash = true;
                        dashCooldownImage = Engine.LoadImage("assets/aviable-dash.png");
                    }
                }


                player.positionX += player.speedX * deltaTime;

                enemy.positionX += enemy.speedX * deltaTime;
                enemy1.positionX += enemy1.speedX * deltaTime;
                enemy1.positionY += enemy1.speedY * deltaTime;

                //Colision horizontal del player
                foreach (GameObject platform in platforms)
                {
                    if (IsColliding(player, platform))
                    {
                        // Colision desde la derecha
                        if (player.speedX > 0 && player.positionX + player.width > platform.positionX &&
                            player.positionY + player.height > platform.positionY && player.positionY < platform.positionY + platform.height)
                        {
                            player.positionX = platform.positionX - player.width;
                            player.speedX = 0;
                        }
                        // Colision desde la izquierda
                        else if (player.speedX < 0 && player.positionX < platform.positionX + platform.width &&
                                 player.positionY + player.height > platform.positionY && player.positionY < platform.positionY + platform.height)
                        {
                            player.positionX = platform.positionX + platform.width;
                            player.speedX = 0;
                        }
                    }
                }

                //Colision del enemigo tierra
                foreach (GameObject platform in platforms)
                {
                    if (IsColliding(enemy, platform))
                    {
                        // Colision desde la derecha
                        if (enemy.speedX > 0 && enemy.positionX + enemy.width > platform.positionX &&
                            enemy.positionY + enemy.height > platform.positionY && enemy.positionY < platform.positionY + platform.height)
                        {
                            enemy.positionX = platform.positionX - enemy.width;
                            enemy.speedX *= -1;
                        }
                        // Colision desde la izquierda
                        else if (enemy.speedX < 0 && enemy.positionX < platform.positionX + platform.width &&
                                 enemy.positionY + enemy.height > platform.positionY && enemy.positionY < platform.positionY + platform.height)
                        {
                            enemy.positionX = platform.positionX + platform.width;
                            enemy.speedX *= -1;
                        }
                    }
                }

                //Logica enemigo aire
                if (enemy1.positionX >= 900)
                {
                    enemy1.speedX *= -1;
                } else if (enemy1.positionX < 655) { 
                    enemy1.speedX *= -1;
                }

                if (enemy1.positionY >= 145)
                {
                    enemy1.speedY *= -1;
                } else if (enemy1.positionY < 140) {  
                    enemy1.speedY *= -1;
                }

                //Gravedad
                if (!onGround)
                {
                    player.speedY += gravity * deltaTime;
                }
                else
                {
                    player.speedY = 0;
                }

                player.positionY += player.speedY * deltaTime;

                //Colision vertical del player 

                foreach (GameObject platform in platforms)
                {
                    if (IsColliding(player, platform))
                    {
                        if (player.speedY > 0 && player.positionY < platform.positionY)
                        {
                            player.positionY = platform.positionY - player.height;
                            player.speedY = 0;
                            onGround = true;
                        }
                        else if (player.speedY < 0)
                        {
                            player.positionY = platform.positionY + platform.height;
                            player.speedY = 0;
                        }
                    }
                }


                //Jumpad 
                if (IsColliding(player, jumpad))
                {
                    if (!death) { 
                        player.speedY = -1400f;
                    }
                }

                //Spikes 
                foreach (GameObject spike in spikes)
                {
                    if (IsColliding(player, spike))
                    {
                        player.speedX = 0;
                        if (death == false) {
                            death = true;
                            return;
                        }
                    }
                }

                //Enemigos
                if (enemy.death)
                {
                    enemy.speedX = 0;
                    enemy.image = Engine.LoadImage("assets/enemy/enemy-death.png");
                    enemyDelete -= deltaTime;
                    if (enemyDelete<= 0)
                    {
                        enemy.delete = true;
                    }
                }

                if (IsColliding(player, enemy))
                {
                    if (!dashing && !enemy.death)
                    {
                        player.speedX = 0;
                        death = true;
                        return;
                    } else if (dashing)
                    {
                        enemy.death = true;
                        return;
                    }
                }

                if (enemy1.death)
                {
                    enemy1.speedX = 0;
                    enemy1.speedY = 0;
                    enemy1.image = Engine.LoadImage("assets/enemy/enemy-fly-death.png");
                    enemy1Delete -= deltaTime;
                    if (enemy1Delete <= 0)
                    {
                        enemy1.delete = true;
                    }
                }

                if (IsColliding(player, enemy1))
                {
                    if (!dashing && !enemy1.death)
                    {
                        player.speedX = 0;
                        death = true;
                        return;
                    }
                    else if (dashing)
                    {
                        enemy1.death = true;
                        return;
                    }
                }

                //Chest 

                if (IsColliding(player, chest))
                {
                    if (!death)
                    {
                        currentState = GameState.Win;
                    }
                }

                //Death
                if (death)
                {
                   player.speedX = 0;
                   player.image = Engine.LoadImage("assets/player/death.png");
                    gameOverTimer -= deltaTime;
                        if (gameOverTimer <= 0)
                        {
                            currentState = GameState.GameOver;
                            return;
                        }
                }

                // Animaciones
                if (!death) {
                    if (player.speedY < 0) //Saltar
                    {
                        if (player.speedX > 0) {
                            player.image = Engine.LoadImage("assets/player/jump.png");
                        } else
                        {
                            player.image = Engine.LoadImage("assets/player/jump-left.png");
                        }
                    } else if (player.speedY > 0)
                    {
                        player.image = Engine.LoadImage("assets/player/fall.png");
                    }
                    else if (player.speedX > 0) // Derecha
                    {
                        animationTime += deltaTime;
                        if (animationTime >= animationSpeed)
                        {
                            currentFrame = (currentFrame + 1) % walkRight.Count;
                            player.image = walkRight[currentFrame];
                            animationTime = 0f;
                        }
                    }
                    else if (player.speedX < 0) // Izquierda
                    {
                        animationTime += deltaTime;
                        if (animationTime >= animationSpeed)
                        {
                            currentFrame = (currentFrame + 1) % walkLeft.Count;
                            player.image = walkLeft[currentFrame];
                            animationTime = 0f;
                        }
                    }
                    else // Moviendon't
                    {
                        if (player.image != idleImage)
                        {
                            player.image = idleImage;
                        }
                    }
                }
                //Enemigo
                if (enemy.speedX > 0) // Derecha
                {
                    animationEnemyTime += deltaTime;
                    if (animationEnemyTime >= animationSpeed)
                    {
                        currentEnemyFrame = (currentEnemyFrame + 1) % walkEnemyRight.Count;
                        enemy.image = walkEnemyRight[currentEnemyFrame];
                        animationEnemyTime = 0f;
                    }
                }
                else if (enemy.speedX < 0) // Izquierda
                {
                    animationEnemyTime += deltaTime;
                    if (animationEnemyTime >= animationSpeed)
                    {
                        currentEnemyFrame = (currentEnemyFrame + 1) % walkEnemyLeft.Count;
                        enemy.image = walkEnemyLeft[currentEnemyFrame];
                        animationEnemyTime = 0f;
                    }
                }

                if (enemy1.speedX != 0) { 
                    animationEnemy1Time += deltaTime;
                    if (animationEnemy1Time >= animationSpeed)
                    {
                        currentEnemy1Frame = (currentEnemy1Frame + 1) % walkEnemy1.Count;
                        enemy1.image = walkEnemy1[currentEnemy1Frame];
                        animationEnemy1Time = 0f;
                    }
                }
            }
        }

        static void Render()
        {
            if (currentState == GameState.StartScreen)
            {
                Engine.Clear();
                Engine.Draw(mainMenu, 0, 0);
                Engine.Show();
            }
            else if (currentState == GameState.GameOver) {

                Engine.Clear();
                Engine.Draw(gameOver, 0, 0);
                Engine.Show();

            }
            else if (currentState == GameState.Stage1)
            {
                Engine.Clear();
                Engine.Draw(player.image, player.positionX, player.positionY);
                if (!enemy.delete) { 
                    Engine.Draw(enemy.image, enemy.positionX, enemy.positionY);
                }
                if (!enemy1.delete)
                {
                    Engine.Draw(enemy1.image, enemy1.positionX, enemy1.positionY);
                }
                foreach (GameObject platform in platforms)
                {
                    Engine.Draw(platform.image, platform.positionX, platform.positionY);
                }
                Engine.Draw(dashCooldownImage, 800, 820);
                Engine.Draw(jumpad.image, jumpad.positionX, jumpad.positionY);
                foreach (GameObject spike in spikes)
                {
                    Engine.Draw(spike.image, spike.positionX, spike.positionY);
                }
                Engine.Draw(chest.image, chest.positionX, chest.positionY);
                Engine.Show();
            }
        }
    }
}
