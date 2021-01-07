using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lab5a_Mono
{
    class GameObject
    {
        public Texture2D sprite;
        public Vector2 position;
        public float rotation;
        public Vector2 center;
        public Vector2 velocity;
        public bool alive;
        public bool explode;
        public bool hit;
        public int x;
        public int y;
        public int frameIndex;
        public Rectangle spriteRect;

        public GameObject(Texture2D loadedTexture)
        {
            rotation = 0.0f;
            position = Vector2.Zero;
            sprite = loadedTexture;
            center = new Vector2(sprite.Width / 2, sprite.Height / 2);
            velocity = Vector2.Zero;
            alive = false;
            explode = false;
            hit = false;

            int x = 0;
            int y = 0;
            int frameIndex = 0;
            Rectangle spriteRect; 

        }
    }
}
