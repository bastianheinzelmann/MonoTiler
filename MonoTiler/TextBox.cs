
// Copied the code from here http://stackoverflow.com/questions/10216757/adding-inputbox-like-control-to-xna-game

using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Text;
using System.Windows.Input;
using System.Threading;
using System.Windows;

namespace EventInput
{

    public class KeyboardLayout
    {
        const uint KLF_ACTIVATE = 1; //activate the layout
        const int KL_NAMELENGTH = 9; // length of the keyboard buffer
        const string LANG_EN_US = "00000409";
        const string LANG_HE_IL = "0001101A";

        [DllImport("user32.dll")]
        private static extern long LoadKeyboardLayout(
              string pwszKLID,  // input locale identifier
              uint Flags       // input locale identifier options
              );

        [DllImport("user32.dll")]
        private static extern long GetKeyboardLayoutName(
              System.Text.StringBuilder pwszKLID  //[out] string that receives the name of the locale identifier
              );

        public static string getName()
        {
            System.Text.StringBuilder name = new System.Text.StringBuilder(KL_NAMELENGTH);
            GetKeyboardLayoutName(name);
            return name.ToString();
        }
    }

    public class CharacterEventArgs : EventArgs
    {
        private readonly char character;
        private readonly int lParam;

        public CharacterEventArgs(char character, int lParam)
        {
            this.character = character;
            this.lParam = lParam;
        }

        public char Character
        {
            get { return character; }
        }

        public int Param
        {
            get { return lParam; }
        }

        public int RepeatCount
        {
            get { return lParam & 0xffff; }
        }

        public bool ExtendedKey
        {
            get { return (lParam & (1 << 24)) > 0; }
        }

        public bool AltPressed
        {
            get { return (lParam & (1 << 29)) > 0; }
        }

        public bool PreviousState
        {
            get { return (lParam & (1 << 30)) > 0; }
        }

        public bool TransitionState
        {
            get { return (lParam & (1 << 31)) > 0; }
        }
    }

    public class KeyEventArgs : EventArgs
    {
        private Keys keyCode;

        public KeyEventArgs(Keys keyCode)
        {
            this.keyCode = keyCode;
        }

        public Keys KeyCode
        {
            get { return keyCode; }
        }
    }

    public delegate void CharEnteredHandler(object sender, CharacterEventArgs e);
    public delegate void KeyEventHandler(object sender, KeyEventArgs e);

    public static class EventInput
    {
        /// <summary>
        /// Event raised when a character has been entered.
        /// </summary>
        public static event CharEnteredHandler CharEntered;

        /// <summary>
        /// Event raised when a key has been pressed down. May fire multiple times due to keyboard repeat.
        /// </summary>
        public static event KeyEventHandler KeyDown;

        /// <summary>
        /// Event raised when a key has been released.
        /// </summary>
        public static event KeyEventHandler KeyUp;

        delegate IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        static bool initialized;
        static IntPtr prevWndProc;
        static WndProc hookProcDelegate;
        static IntPtr hIMC;

        //various Win32 constants that we need
        const int GWL_WNDPROC = -4;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_CHAR = 0x102;
        const int WM_IME_SETCONTEXT = 0x0281;
        const int WM_INPUTLANGCHANGE = 0x51;
        const int WM_GETDLGCODE = 0x87;
        const int WM_IME_COMPOSITION = 0x10f;
        const int DLGC_WANTALLKEYS = 4;

        //Win32 functions that we're using
        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmGetContext(IntPtr hWnd);

        [DllImport("Imm32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr ImmAssociateContext(IntPtr hWnd, IntPtr hIMC);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        /// <summary>
        /// Initialize the TextInput with the given GameWindow.
        /// </summary>
        /// <param name="window">The XNA window to which text input should be linked.</param>
        public static void Initialize(GameWindow window)
        {
            if (initialized)
                throw new InvalidOperationException("TextInput.Initialize can only be called once!");

            hookProcDelegate = new WndProc(HookProc);
            prevWndProc = (IntPtr)SetWindowLong(window.Handle, GWL_WNDPROC,
                (int)Marshal.GetFunctionPointerForDelegate(hookProcDelegate));

            hIMC = ImmGetContext(window.Handle);
            initialized = true;
            Console.WriteLine("Initialized!");
        }

        static IntPtr HookProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            IntPtr returnCode = CallWindowProc(prevWndProc, hWnd, msg, wParam, lParam);

            switch (msg)
            {
                case WM_GETDLGCODE:
                    returnCode = (IntPtr)(returnCode.ToInt32() | DLGC_WANTALLKEYS);
                    break;

                case WM_KEYDOWN:
                    if (KeyDown != null)
                        KeyDown(null, new KeyEventArgs((Keys)wParam));
                    break;

                case WM_KEYUP:
                    if (KeyUp != null)
                        KeyUp(null, new KeyEventArgs((Keys)wParam));
                    break;

                case WM_CHAR:
                    if (CharEntered != null)
                        CharEntered(null, new CharacterEventArgs((char)wParam, lParam.ToInt32()));
                    break;

                case WM_IME_SETCONTEXT:
                    if (wParam.ToInt32() == 1)
                        ImmAssociateContext(hWnd, hIMC);
                    break;

                case WM_INPUTLANGCHANGE:
                    ImmAssociateContext(hWnd, hIMC);
                    returnCode = (IntPtr)1;
                    break;
            }

            return returnCode;
        }
    }
}

public interface IKeyboardSubscriber
{
    void RecieveTextInput(char inputChar);
    void RecieveTextInput(string text);
    void RecieveCommandInput(char command);
    void RecieveSpecialInput(Keys key);

    bool Selected { get; set; } //or Focused
}

public class KeyboardDispatcher
{
    public KeyboardDispatcher(GameWindow window)
    {
        EventInput.EventInput.Initialize(window);
        EventInput.EventInput.CharEntered += new EventInput.CharEnteredHandler(EventInput_CharEntered);
        EventInput.EventInput.KeyDown += new EventInput.KeyEventHandler(EventInput_KeyDown);
    }

    void EventInput_KeyDown(object sender, EventInput.KeyEventArgs e)
    {
        if (_subscriber == null)
            return;

        _subscriber.RecieveSpecialInput(e.KeyCode);
    }

    void EventInput_CharEntered(object sender, EventInput.CharacterEventArgs e)
    {
        if (_subscriber == null)
            return;
        if (char.IsControl(e.Character))
        {
            //ctrl-v
            if (e.Character == 0x16)
            {
                //XNA runs in Multiple Thread Apartment state, which cannot recieve clipboard
                Thread thread = new Thread(PasteThread);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
                _subscriber.RecieveTextInput(_pasteResult);
            }
            else
            {
                _subscriber.RecieveCommandInput(e.Character);
            }
        }
        else
        {
            _subscriber.RecieveTextInput(e.Character);
        }
    }

    IKeyboardSubscriber _subscriber;
    public IKeyboardSubscriber Subscriber
    {
        get { return _subscriber; }
        set
        {
            if (_subscriber != null)
                _subscriber.Selected = false;
            _subscriber = value;
            if (value != null)
                value.Selected = true;
        }
    }

    //Thread has to be in Single Thread Apartment state in order to receive clipboard
    string _pasteResult = "";
    [STAThread]
    void PasteThread()
    {
        if (System.Windows.Clipboard.ContainsText())
        {
            _pasteResult = Clipboard.GetText();
        }
        else
        {
            _pasteResult = "";
        }
    }
}

public delegate void TextBoxEvent(TextBox sender);

public class TextBox : IKeyboardSubscriber
{
    Texture2D _textBoxTexture;
    Texture2D _caretTexture;

    SpriteFont _font;

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; private set; }

    public bool Highlighted { get; set; }

    public bool PasswordBox { get; set; }

    public event TextBoxEvent Clicked;

    string _text = "";
    public String Text
    {
        get
        {
            return _text;
        }
        set
        {
            _text = value;
            if (_text == null)
                _text = "";

            if (_text != "")
            {
                //if you attempt to display a character that is not in your font
                //you will get an exception, so we filter the characters
                //remove the filtering if you're using a default character in your spritefont
                String filtered = "";
                foreach (char c in value)
                {
                    if (_font.Characters.Contains(c))
                        filtered += c;
                }

                _text = filtered;

                while (_font.MeasureString(_text).X > Width)
                {
                    //to ensure that text cannot be larger than the box
                    _text = _text.Substring(0, _text.Length - 1);
                }
            }
        }
    }

    public TextBox(Texture2D textBoxTexture, Texture2D caretTexture, SpriteFont font)
    {
        _textBoxTexture = textBoxTexture;
        _caretTexture = caretTexture;
        _font = font;

        Height = (int)_font.MeasureString("a").Y;
        _previousMouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
    }

    MouseState _previousMouse;
    public void Update(GameTime gameTime)
    {
        MouseState mouse = Microsoft.Xna.Framework.Input.Mouse.GetState();
        Point mousePoint = new Point(mouse.X, mouse.Y);

        Rectangle position = new Rectangle(X, Y, Width, Height);
        if (position.Contains(mousePoint))
        {
            Highlighted = true;
            if (_previousMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
            {
                if (Clicked != null)
                    Clicked(this);
            }
        }
        else
        {
            Highlighted = false;
        }
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        bool caretVisible = true;

        if ((gameTime.TotalGameTime.TotalMilliseconds % 1000) < 500)
            caretVisible = false;
        else
            caretVisible = true;

        String toDraw = Text;

        if (PasswordBox)
        {
            toDraw = "";
            for (int i = 0; i < Text.Length; i++)
                toDraw += (char)0x2022; //bullet character (make sure you include it in the font!!!!)
        }

        //my texture was split vertically in 2 parts, upper was unhighlighted, lower was highlighted version of the box
        spriteBatch.Draw(_textBoxTexture, new Rectangle(X, Y, Width, Height), new Rectangle(0, Highlighted ? (_textBoxTexture.Height / 2) : 0, _textBoxTexture.Width, _textBoxTexture.Height / 2), Color.White);
        spriteBatch.Draw(_textBoxTexture, new Rectangle(X, Y, Width, Height), Color.White);


        Vector2 size = _font.MeasureString(toDraw);

        if (caretVisible && Selected)
            spriteBatch.Draw(_caretTexture, new Rectangle(X + (int)size.X + 2, Y, Width / 20, Height), Color.White); //my caret texture was a simple vertical line, 4 pixels smaller than font size.Y

        //shadow first, then the actual text
        spriteBatch.DrawString(_font, toDraw, new Vector2(X, Y) + Vector2.One, Color.Black);
        spriteBatch.DrawString(_font, toDraw, new Vector2(X, Y), Color.White);
    }


    public void RecieveTextInput(char inputChar)
    {
        Text = Text + inputChar;
    }
    public void RecieveTextInput(string text)
    {
        Text = Text + text;
    }
    public void RecieveCommandInput(char command)
    {
        switch (command)
        {
            case '\b': //backspace
                if (Text.Length > 0)
                    Text = Text.Substring(0, Text.Length - 1);
                break;
            case '\r': //return
                if (OnEnterPressed != null)
                    OnEnterPressed(this);
                Console.WriteLine(Text); //So kann man also den Text weiterverarbeiten \o/
                break;
            case '\t': //tab
                if (OnTabPressed != null)
                    OnTabPressed(this);
                break;
            default:
                break;
        }
    }
    public void RecieveSpecialInput(Keys key)
    {

    }

    public event TextBoxEvent OnEnterPressed;
    public event TextBoxEvent OnTabPressed;

    public bool Selected
    {
        get;
        set;
    }
}
