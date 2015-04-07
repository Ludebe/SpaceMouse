using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using SpaceMouse.Managers;

namespace SpaceMouse
{
    public class Player
    {
        /* Atributos */
        public Image Image;                        //Imagen que contiene el sprite del jugador
        public Vector2 Velocity;                    //Velocity, eso(?)

        private Vector2 InitialPos;
        private Vector2 TextureSize;

        private string texturePath;                 //Dirección de la textura del jugador
        private const float JUMP_HEIGHT = 100;      //Cuanto de Y subir
        private float initialY, finalY;            //Posiciones X,Y que cambian al saltar.
        private bool lookLeft;                    //Variable para ver si el mirando a a izquierda o derecha. Mediochotoesto.
        private bool startedRuning;
        private static Player instance;

        public bool Jumping;                        //Para saber si está saltando o no,así lo afecta o no la gravedad.

        /*
         * ♥ ♥ ♥ Singleton ♥ ♥ ♥
         * 
         */
        public static Player Instance
        {
            get
            {
                if (instance == null)
                    instance = new Player();

                return instance;
            }
        }
        /*
         * Constructor
         * 
         */
        private Player()
        {
            //Vectores para l imagen (velocidad(?), posición inicial y tamaño de la imagen)
            Velocity = new Vector2(100, 400);
            InitialPos = new Vector2(0, 0);
            TextureSize = new Vector2(1, 1);
            startedRuning = false;

            //Texura del sprite
            texturePath = "Gameplay/player";

            //Cargo la imagen con el sprite
            Image = new Image(texturePath, InitialPos, TextureSize, 100);

            //Valores por defecto para el salto y visión
            Jumping = false;
            lookLeft = false;

            //Le agrego un efecto para que funcione como sprite
            Image.effects.Add("SpriteSheetEffect");

        }

        public void LoadContent()
        {
            //Cargo la imagen
            Image.Loadcontent();
            Image.spriteSheetEffect.amountOfFrames = new Vector2(4, 2);
            Image.spriteSheetEffect.currentFrame = new Vector2(0);
        }

        public void Update(GameTime gameTime)
        {
            Image.isActive = true;
            /* En esta parte controlo el movimiento del personaje según la tecla
             * que se presiona
             * 
             */
            if (InputManager.Instance.KeyPressed(Keys.Space) && !Jumping)
            {
                /*
                 * Si aprieto espaciadora y no está saltando, empieza a
                 * saltar y le asigna valores a initialY y finalY.
                 * 
                 */
                Jumping = true;
                initialY = Image.position.Y;
                finalY = initialY - JUMP_HEIGHT;
            }

            //Saltar!
            saltar(gameTime);

            if (InputManager.Instance.KeyDown(Keys.Left))
            {
                //Está mirando a la izquierda
                lookLeft = true;

                if (!startedRuning)
                    Image.spriteSheetEffect.currentFrame.X = 1;

                startedRuning = true;
                //Cambia la posición
                Image.position.X -= Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //En base a lookLeft, elige un sprite u otro
                Image.spriteSheetEffect.currentFrame.Y = (lookLeft) ? 1 : 0;
            }

            else if (InputManager.Instance.KeyDown(Keys.Right))
            {
                //Está mirando a la derecha, entonces lookLeft es false
                lookLeft = false;

                if (!startedRuning)
                    Image.spriteSheetEffect.currentFrame.X = 1;

                startedRuning = true;

                //De nuevo, cambia la posición
                Image.position.X += Velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //En base al lookLeft, elige un sprite u otro (obviamente es 0, por lo que va a mira a la derecha)
                Image.spriteSheetEffect.currentFrame.Y = (lookLeft) ? 1 : 0;
            }
            else
            {
                //El jugador está quieto
                Image.isActive = false;
                startedRuning = false;

                //Cambia al frame en donde está quieto
                Image.spriteSheetEffect.currentFrame.X = 0;

                //En base al lookLeft decide hacia que lado mirar
                Image.spriteSheetEffect.currentFrame.Y = (lookLeft) ? 1 : 0;
            }

            Image.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //Image.scale = new Vector2(2, 2);
            Image.Draw(spriteBatch);
        }

        private void saltar(GameTime gameTime)
        {
            //TODO: Animacion

            //Si está saltando...
            if (Jumping)
            {
                //Animación
                Image.isActive = false;
                Image.spriteSheetEffect.currentFrame.X = 3;
                Image.spriteSheetEffect.currentFrame.Y = (lookLeft) ? 1 : 0;

                //Posición
                Image.position.Y -= Velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

                //Si La posición en Y es mayor o igual a la Y inicial + lo que salta...
                if (Image.position.Y <= finalY)
                    Jumping = false;
            }
        }
    }
}
