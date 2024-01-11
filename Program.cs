using Raylib_cs;
using System.Collections.Generic;
using System;
using System.Numerics;


class Program
{
    public static void Main()
    {
        Raylib.InitWindow(800, 480, "Game 01");
        Raylib.SetTargetFPS(60);
        Game game = new Game();

        while (!Raylib.WindowShouldClose())
        {
            game.Update();
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);
            //Raylib.DrawText("Hello, world!", 12, 12, 20, Color.BLACK);
            game.Draw();
            Raylib.EndDrawing();
        }
        game.Unsubscribe();

        Raylib.CloseWindow();
    }
}

public class Game
{
    List<Bullet> bullets = new List<Bullet>(32);
    List<Enemy> enemies = new List<Enemy>();
    Player player;
    float enemyTimer;

    public Game()
    {
        player = new Player(bullets);
        player.BulletSpawned += OnBulletSpawned;
        Bullet.LeftScreen += OnLeftScreen;
    }

    public void Unsubscribe()
    {
        Bullet.LeftScreen -= OnLeftScreen;
        player.BulletSpawned -= OnBulletSpawned;
    }

    public void OnLeftScreen(Bullet bullet)
    {
        bullets.Remove(bullet);
    }

    public void OnBulletSpawned(float x, float y)
    {
        Bullet b = new Bullet((int)x, (int)y);
        bullets.Add(b);
    }

    public void Update()
    {
        enemyTimer += Raylib.GetFrameTime();
        if (enemyTimer >= 3f)
        {
            enemyTimer = 0f;
            int randX = Raylib.GetRandomValue(0, 800);
            Enemy enemy = new Enemy(randX, -50);
            enemies.Add(enemy);
        }
        // Collision check
        // genestete for-Schleife fuer Enemies u. Bullets
        for (int b = bullets.Count - 1; b >= 0; b--)
        {
            for (int e = enemies.Count - 1; e >= 0; e--)
            {
                // Vector2 jeweils aus posX u. posY erstellen
                Vector2 bPos = bullets[b].Pos;
                Vector2 ePos = enemies[e].Pos;
                float bRad = bullets[b].Radius;
                float eRad = enemies[e].Radius;
                // Raylib.CheckCollisionCircles()-Methode benutzen
                // wenn sie kollidieren: aus den Listen rauswerfen
                if (Raylib.CheckCollisionCircles(bPos, bRad, ePos, eRad))
                {
                    bullets.Remove(bullets[b]);
                    enemies.Remove(enemies[e]);
                    break;
                }
            }
        }
        // player Punkte geben
        player.Update();
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].Update();
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Update();
        }
    }
    public void Draw()
    {
        player.Draw();
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].Draw();
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].Draw();
        }
    }
}

public class Player
{
    List<Bullet> bullets = new List<Bullet>();
    int sizeX = 15;
    int sizeY = 30;
    float posX = 400f;
    float posY = 240f;
    float angle;
    Vector2 startDir = new Vector2(0f, -1f);
    Vector2 dir = new Vector2(0f, -1f);
    float speed;

    public event Action<float, float>? BulletSpawned;

    public Player(List<Bullet> bullets)
    {
        this.bullets = bullets;
    }

    public void Update()
    {
        dir = Raymath.Vector2Rotate(startDir, Raylib.DEG2RAD * 45f);
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_R)) Console.WriteLine(dir.X.ToString() + " " + dir.Y.ToString());
        if(posX < 0 || posX > 800 || Raylib.IsKeyPressed(KeyboardKey.KEY_LEFT_SHIFT))
        {
            speed = 0f;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W) || Raylib.IsKeyDown(KeyboardKey.KEY_UP))
        {
            //posY -= 3;
            posX += dir.X;
            posY += dir.Y;
            if (posY < 0) posY = 0;
        }
        //else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        //{
        //    posY += 3;
        //    if (posY > 480) posY = 480;
        //}
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            speed = -3f;
            //posX -= 3;
            //if (posX < 0) posX = 0;
            //angle -= 3f;
            //dir = Raymath.Vector2Rotate(startDir, Raylib.DEG2RAD * angle);
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            speed = 3f;
            //posX += 3;
            //if (posX > 800) posX = 800;
            //angle += 3f;
            //dir = Raymath.Vector2Rotate(startDir, Raylib.DEG2RAD * angle);
        }

        posX += speed;

        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            BulletSpawned?.Invoke(posX, posY);
            //Bullet b = new Bullet((int)posX, (int)posY);
            //bullets.Add(b);
        }
    }

    public void Draw()
    {
        //Raylib.DrawRectangle(posX, posY, sizeX, sizeY, Color.BLUE);
        Rectangle rect = new Rectangle(posX, posY, sizeX, sizeY);
        Vector2 origin = new Vector2(sizeX / 2, sizeY / 2);
        Raylib.DrawRectanglePro(rect, origin, angle, Color.BLUE);
        Raylib.DrawCircle((int)posX, (int)posY, 3f, Color.RED);

    }
}

public class Bullet
{
    public Vector2 Pos => new Vector2(posX, posY);
    int posX;
    int posY;

    public float Radius => radius;
    float radius = 5f;

    public bool isAlive = true;

    public static event Action<Bullet>? LeftScreen;

    public Bullet(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public void Update()
    {
        posY -= 5;
        if (posY < 50)
        {
            //isAlive = false;
            LeftScreen?.Invoke(this);
        }
    }

    public void Draw()
    {
        Raylib.DrawCircle((int)posX, (int)posY, radius, Color.RED);
    }
}

public class Enemy
{
    public float Radius => sizeX / 2;
    int sizeX = 20;
    int sizeY = 20;

    public Vector2 Pos => new Vector2(posX, posY);
    int posX;
    int posY;

    public Enemy(int x, int y)
    {
        posX = x;
        posY = y;
    }

    public void Update()
    {
        posY += 3;
    }

    public void Draw()
    {
        Raylib.DrawRectangle(posX - sizeX / 2, posY - sizeY / 2, sizeX, sizeY, Color.ORANGE);
        //Raylib.DrawCircle(posX, posY, sizeX/2, Color.BLACK);
    }
}








