using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SpaceMouse.Managers
{
    public class InputManager
    {
        private static InputManager instance;
        private KeyboardState newKeyboardState;
        private KeyboardState oldKeyboardState;

        private InputManager() { }

        public static InputManager Instance
        {
            get
            {
                //Si instance es null, crea una nueva instancia usando el constructor privado
                if (instance == null)
                {
                    //Se instancia desde el constructor
                    instance = new InputManager();
                }
                return instance;
            }
        }

        public void Update()
        {
            oldKeyboardState = newKeyboardState;
            if (!ScreenManager.Instance.isTransitioning)
                newKeyboardState = Keyboard.GetState();
        }

        public Boolean KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (oldKeyboardState.IsKeyUp(key) && newKeyboardState.IsKeyDown(key))
                    return true;
            }

            return false;
        }

        public Boolean KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (oldKeyboardState.IsKeyDown(key) && newKeyboardState.IsKeyUp(key))
                    return true;
            }

            return false;
        }

        public Boolean KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (oldKeyboardState.IsKeyDown(key))
                    return true;
            }

            return false;
        }
    }
}
