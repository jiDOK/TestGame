using Raylib_cs;
using System.Collections.Generic;
using System;

namespace HelloWorld;

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
        }
        player.Update();
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].Update();
            //if (!bullets[i].isAlive)
            //{
            //    bullets.Remove(bullets[i]);
            //}
        }
    }
    public void Draw()
    {
        player.Draw();
        for (int i = 0; i < bullets.Count; i++)
        {
            bullets[i].Draw();
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
        if (Raylib.IsKeyDown(KeyboardKey.KEY_W))
        {
            posY -= 3;
            if(posY < 0) posY = 0;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_S))
        {
            posY += 3;
            if(posY > 480) posY = 480;
        }
        if (Raylib.IsKeyDown(KeyboardKey.KEY_A))
        {
            posX -= 3;
            if(posX < 0) posX = 0;
        }
        else if (Raylib.IsKeyDown(KeyboardKey.KEY_D))
        {
            posX += 3;
            if(posX > 800) posX = 800;
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
    float posX;
    float posY;
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










