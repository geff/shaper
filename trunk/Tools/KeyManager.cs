using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

public class KeyManager
{
    private KeyState keyState;
    private Keys key;

    public delegate void KeyPressedHandler(Keys key);
    public event KeyPressedHandler KeyPressed;

    public KeyManager(Keys key)
    {
        this.key = key;
        this.keyState = KeyState.Up;
    }

    public void Update(KeyboardState keyBoardState)
    {
        if (keyState == KeyState.Down && keyBoardState.IsKeyUp(key))
        {
            KeyPressed(key);
        }

        if (keyBoardState.IsKeyUp(key))
        {
            keyState = KeyState.Up;
        }
        else
        {
            keyState = KeyState.Down;
        }
    }
}
