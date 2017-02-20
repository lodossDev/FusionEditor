using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FusionEngine;

namespace FusionEditor {

    public class CharacterView : WpfGame {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private RenderManager renderManager;
        private Player_Ryo ryo;
        private float _scale = 0f;

        public float  Scale
        {
            get
            {
                return _scale;
            }

            set
            {
                _scale = value;
            }
        }

        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);
            
            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            FusionEngine.System.graphicsDevice = _graphicsDeviceManager.GraphicsDevice;
            FusionEngine.System.contentManager = Content;
            FusionEngine.System.spriteBatch = _spriteBatch;

            ryo = new Player_Ryo();
            renderManager = new RenderManager();
            renderManager.AddEntity(ryo);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
            
            renderManager.Update(time);
        }

        protected override void Draw(GameTime time)
        {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
                renderManager.Draw(time);
            _spriteBatch.End();
        }
    }
}
