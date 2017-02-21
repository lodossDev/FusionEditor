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
using System.Windows;
using System.Diagnostics;
using Microsoft.Xna.Framework.Input;

namespace FusionEditor {

    public class CharacterView : WpfGame {
        private bool ENABLE_ANIMATION_CHECK = false;
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;
        private KeyboardState _previousKeyboardState;
        private WpfMouse _mouse;
        private RenderManager renderManager;
        private Player_Ryo ryo;
        private Vector2 baseScale = Vector2.Zero;

        private List<Rectangle> frameBoxes = new List<Rectangle>();
        private List<Vector2> frameOffsets = new List<Vector2>();

        private Rectangle boundsRect = Rectangle.Empty;
        private Rectangle depthRect = Rectangle.Empty;
        private Vector2 boundsOffset = Vector2.Zero;
        private Vector2 depthOffset = Vector2.Zero;

        public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(string), typeof(CharacterView), new PropertyMetadata("", ActorOnChangeValue));
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register("Animation", typeof(string), typeof(CharacterView), new PropertyMetadata("", AnimationOnChangeValue));
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register("Frame", typeof(string), typeof(CharacterView), new PropertyMetadata("", FrameOnChangeValue));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(float), typeof(CharacterView), new PropertyMetadata(0f, ScaleOnChangeValue));

        public string Actor {
            get {
                return (string)GetValue(ActorProperty);
            }

            set {
                SetValue(ActorProperty, value);
            }
        }

        public string Animation {
            get {
                return (string)GetValue(AnimationProperty);
            }

            set {
                SetValue(AnimationProperty, value);
            }
        }

        public string Frame {
            get {
                return (string)GetValue(FrameProperty);
            }

            set {
                SetValue(FrameProperty, value);
            }
        }

        public float Scale {
            get {
                return (float)GetValue(ScaleProperty);
            }

            set {
                SetValue(ScaleProperty, value);
            }
        }

        private static void ActorOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String actor = (string)e.NewValue;
            }
        }

        private static void AnimationOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;

                if (instance.ENABLE_ANIMATION_CHECK == true) {
                    String animation = (string)e.NewValue;
                    Animation.State state;

                    bool hasState = Enum.TryParse(animation, true, out state);

                    if (hasState) {
                        instance.ryo.SetAnimationState(state);
                    } else {
                        MessageBox.Show("Cannot find state: " + animation);
                    }
                }

                if (instance.ENABLE_ANIMATION_CHECK == false) {
                    instance.ENABLE_ANIMATION_CHECK = true;
                }
            }
        }

        private static void FrameOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String frame = (string)e.NewValue;

                if (String.IsNullOrEmpty(frame) == false) {
                    instance.ryo.GetCurrentSprite().SetCurrentFrame(Convert.ToInt32(frame));
                }
            }
        }

        private static void ScaleOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null){
                CharacterView instance = source as CharacterView;
                float currentScale = instance.ryo.GetScale().X;
                float oldScale = (float)e.OldValue;
                float newScale = (float)e.NewValue;

                if (float.IsNaN(newScale) == false) {
                    currentScale += (newScale > oldScale ? newScale : -(oldScale));

                    if (currentScale <= 1) {
                        currentScale = 1;
                    }

                    instance.ryo.SetScale(currentScale, currentScale);
                    instance.CheckScale();
                }
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            //ENABLE_CHECK = true;
        }

        protected override void Initialize() {
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
            ryo.SetAnimationType(FusionEngine.Animation.Type.NONE);

            baseScale = ryo.GetScale();
            StoreOldBoxesPositions();

            ryo.SetScale(1, 1);
            CheckScale();

            renderManager = new RenderManager();
            renderManager.AddEntity(ryo);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
        }

        private void StoreOldBoxesPositions() {
            boundsRect.Width = ryo.GetBoundsBox().GetWidth();
            boundsRect.Height = ryo.GetBoundsBox().GetHeight();

            boundsOffset.X = ryo.GetBoundsBox().GetOffset().X;
            boundsOffset.Y = ryo.GetBoundsBox().GetOffset().Y;

            depthRect.Width = ryo.GetDepthBox().GetWidth();
            depthRect.Height = ryo.GetDepthBox().GetHeight();

            depthOffset.X = ryo.GetDepthBox().GetOffset().X;
            depthOffset.Y = ryo.GetDepthBox().GetOffset().Y;

            for (int i = 0; i < ryo.GetAllFrameBoxes().Count; i++) {
                CLNS.BoundingBox box = ryo.GetAllFrameBoxes()[i];

                Rectangle frameRect = new Rectangle(0, 0, box.GetWidth(), box.GetHeight());
                frameBoxes.Add(frameRect);

                Vector2 frameOffset = new Vector2(box.GetOffset().X, box.GetOffset().Y);
                frameOffsets.Add(frameOffset);
            }
        }

        private void CheckScale() {
            Debug.WriteLine("OLD VALUE SCALE: " + baseScale);

            float diffX = ((ryo.GetScale().X - baseScale.X) / baseScale.X);
            float diffY = ((ryo.GetScale().Y - baseScale.Y) / baseScale.Y);

            Debug.WriteLine("diffX: " + diffX);
            Debug.WriteLine("diffY: " + diffY);

            for (int i = 0; i < ryo.GetAllFrameBoxes().Count; i++) {
                CLNS.BoundingBox box = ryo.GetAllFrameBoxes()[i];
                Rectangle frameRect = frameBoxes[i];
                Vector2 frameOffset = frameOffsets[i];

                box.SetRectWidth((int)(frameRect.Width + (frameRect.Width * diffX)));
                box.SetRectHeight((int)((frameRect.Height + (frameRect.Height * diffY))));
                box.SetOffSet((frameOffset.X + (frameOffset.X * diffX)), (frameOffset.Y + (frameOffset.Y * diffY)));
            }

            ryo.GetBoundsBox().SetRectWidth((int)(boundsRect.Width + (boundsRect.Width * diffX)));
            ryo.GetBoundsBox().SetRectHeight((int)(boundsRect.Height + (boundsRect.Height * diffY)));
            ryo.GetBoundsBox().SetOffSet((boundsOffset.X + (boundsOffset.X * diffX)), (boundsOffset.Y + (boundsOffset.Y * diffY)));

            ryo.GetDepthBox().SetRectWidth((int)(depthRect.Width + (depthRect.Width * diffX)));
            ryo.GetDepthBox().SetRectHeight((int)(depthRect.Height + (depthRect.Height * diffY)));
            ryo.GetDepthBox().SetOffSet((depthOffset.X + (depthOffset.X * diffX)), (depthOffset.Y + (depthOffset.Y * diffY)));
        }

        protected override void Update(GameTime time) {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();

            if (KeyPressed(Keys.Q)) {
                renderManager.RenderBoxes();
            }

            renderManager.Update(time);
            _previousKeyboardState = _keyboardState;
        }

        protected override void Draw(GameTime time) {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                renderManager.Draw(time);
            _spriteBatch.End();
        }

        private bool KeyPressed(Keys k) {
            return _previousKeyboardState.GetPressedKeys().Contains(k) && !_keyboardState.GetPressedKeys().Contains(k);
        }
    }
}
