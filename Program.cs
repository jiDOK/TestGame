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
        Bullet.LeftScreen += OnLeftScreen;
    }

    public void OnLeftScreen(Bullet bullet)
    {
        bullets.Remove(bullet);
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
    int sizeX = 5;
    int sizeY = 10;
    int posX = 400;
    int posY = 240;

    public Player(List<Bullet> bullets)
    {
        this.bullets = bullets;
    }

    public void Update()
    {
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W) || Raylib.IsKeyDown(KeyboardKey.KEY_UP))
        {
            posY -= 3;
            if (posY < 0) posY = 0;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            posY += 3;
            if (posY > 480) posY = 480;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            posX -= 3;
            if (posX < 0) posX = 0;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            posX += 3;
            if (posX > 800) posX = 800;
        }
        if (Raylib.IsKeyPressed(KeyboardKey.KEY_SPACE))
        {
            Bullet b = new Bullet(posX, posY);
            bullets.Add(b);
        }
    }

    public void Draw()
    {
        Raylib.DrawRectangle(posX, posY, sizeX, sizeY, Color.BLUE);
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

    public static event Action<Bullet> LeftScreen;

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
        Raylib.DrawRectangle(posX - sizeX/2, posY - sizeY/2, sizeX, sizeY, Color.ORANGE);
        //Raylib.DrawCircle(posX, posY, sizeX/2, Color.BLACK);
    }
}








