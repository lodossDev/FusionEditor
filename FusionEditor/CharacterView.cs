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
        private Entity actor;
        private Vector2 baseScale = Vector2.Zero;

        private List<Rectangle> frameBoxes = new List<Rectangle>();
        private List<Vector2> frameOffsets = new List<Vector2>();

        private Rectangle bodyRect = Rectangle.Empty;
        private Rectangle boundsRect = Rectangle.Empty;
        private Rectangle depthRect = Rectangle.Empty;

        private Vector2 bodyOffset = Vector2.Zero;
        private Vector2 boundsOffset = Vector2.Zero;
        private Vector2 depthOffset = Vector2.Zero;

        public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(string), typeof(CharacterView), new PropertyMetadata("", ActorOnChangeValue));
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register("Animation", typeof(string), typeof(CharacterView), new PropertyMetadata("", AnimationOnChangeValue));
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register("Frame", typeof(string), typeof(CharacterView), new PropertyMetadata("", FrameOnChangeValue));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(float), typeof(CharacterView), new PropertyMetadata(0f, ScaleOnChangeValue));
        public static readonly DependencyProperty ShowAttackBoxesProperty = DependencyProperty.Register("ShowAttackBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowAttackBoxesOnChangeValue));
        public static readonly DependencyProperty ShowBodyBoxesProperty = DependencyProperty.Register("ShowBodyBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowBodyBoxesOnChangeValue));
        public static readonly DependencyProperty ShowBoundsBoxesProperty = DependencyProperty.Register("ShowBoundsBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowBoundsBoxesOnChangeValue));

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

        public bool ShowAttackBoxes {
            get {
                return (bool)GetValue(ShowAttackBoxesProperty);
            }

            set {
                SetValue(ShowAttackBoxesProperty, value);
            }
        }

        public bool ShowBodyBoxes {
            get {
                return (bool)GetValue(ShowBodyBoxesProperty);
            }

            set {
                SetValue(ShowBodyBoxesProperty, value);
            }
        }

        public bool ShowBoundsBoxes {
            get {
                return (bool)GetValue(ShowBoundsBoxesProperty);
            }

            set {
                SetValue(ShowBoundsBoxesProperty, value);
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

                    if (string.IsNullOrEmpty(animation) == false) { 
                        Animation.State state;

                        bool hasState = Enum.TryParse(animation, true, out state);

                        if (hasState) {
                            instance.actor.SetAnimationState(state);
                        } else {
                            MessageBox.Show("Cannot find state: " + animation);
                        }
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
                    instance.actor.GetCurrentSprite().SetCurrentFrame(Convert.ToInt32(frame));
                }
            }
        }

        private static void ScaleOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null){
                CharacterView instance = source as CharacterView;
                float sx = instance.actor.GetScale().X;
                float sy = instance.actor.GetScale().Y;

                float oldScale = (float)Math.Round((float)e.OldValue);
                float newScale = (float)Math.Round((float)e.NewValue);

                if (float.IsNaN(newScale) == false && newScale > 0) {
                    sx += newScale - oldScale;
                    sy += newScale - oldScale;

                    if (sx <= instance.baseScale.X - 1) {
                        sx = instance.baseScale.X - 1;
                    }

                    if (sy <= instance.baseScale.Y - 1) {
                        sy = instance.baseScale.Y - 1;
                    }

                    instance.actor.SetScale(sx, sy);
                    instance.actor.MoveY(-(newScale - oldScale) * 20);
                    instance.CheckScale();
                } else {
                    instance.actor.SetScale(instance.baseScale.X - 1, instance.baseScale.Y - 1);
                    instance.actor.SetPostion(400, 0, 200);
                    instance.CheckScale();
                }
            }
        }

        private static void ShowAttackBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance.renderManager.ShowAttackBoxes();
                } else {
                    instance.renderManager.HideAttackBoxes();
                }
            }
        }

        private static void ShowBodyBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance.renderManager.ShowBodyBoxes();
                } else {
                    instance.renderManager.HideBodyBoxes();
                }
            }
        }

        private static void ShowBoundsBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance.renderManager.ShowBoundsBoxes();
                } else {
                    instance.renderManager.HideBoundsBoxes();
                }
            }
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

            actor = new Player_Ryo();
            actor.SetAnimationType(FusionEngine.Animation.Type.NONE);

            baseScale = actor.GetScale();
            StoreOldBoxesPositions();

            actor.SetScale(baseScale.X - 1, baseScale.Y - 1);
            CheckScale();

            renderManager = new RenderManager();
            renderManager.AddEntity(actor);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
        }

        private void StoreOldBoxesPositions() {
            bodyRect.Width = actor.GetBodyBox().GetWidth();
            bodyRect.Height = actor.GetBodyBox().GetHeight();

            bodyOffset.X = actor.GetBodyBox().GetOffset().X;
            bodyOffset.Y = actor.GetBodyBox().GetOffset().Y;

            boundsRect.Width = actor.GetBoundsBox().GetWidth();
            boundsRect.Height = actor.GetBoundsBox().GetHeight();

            boundsOffset.X = actor.GetBoundsBox().GetOffset().X;
            boundsOffset.Y = actor.GetBoundsBox().GetOffset().Y;

            depthRect.Width = actor.GetDepthBox().GetWidth();
            depthRect.Height = actor.GetDepthBox().GetHeight();

            depthOffset.X = actor.GetDepthBox().GetOffset().X;
            depthOffset.Y = actor.GetDepthBox().GetOffset().Y;

            for (int i = 0; i < actor.GetAllFrameBoxes().Count; i++) {
                CLNS.BoundingBox box = actor.GetAllFrameBoxes()[i];

                Rectangle frameRect = new Rectangle(0, 0, box.GetWidth(), box.GetHeight());
                frameBoxes.Add(frameRect);

                Vector2 frameOffset = new Vector2(box.GetOffset().X, box.GetOffset().Y);
                frameOffsets.Add(frameOffset);
            }
        }

        private void CheckScale() {
            Debug.WriteLine("OLD VALUE SCALE: " + baseScale);

            float diffX = ((actor.GetScale().X - baseScale.X) / baseScale.X);
            float diffY = ((actor.GetScale().Y - baseScale.Y) / baseScale.Y);

            Debug.WriteLine("diffX: " + diffX);
            Debug.WriteLine("diffY: " + diffY);

            for (int i = 0; i < actor.GetAllFrameBoxes().Count; i++) {
                CLNS.BoundingBox box = actor.GetAllFrameBoxes()[i];
                Rectangle frameRect = frameBoxes[i];
                Vector2 frameOffset = frameOffsets[i];

                box.SetRectWidth((int)(frameRect.Width + (frameRect.Width * diffX)));
                box.SetRectHeight((int)((frameRect.Height + (frameRect.Height * diffY))));
                box.SetOffSet((frameOffset.X + (frameOffset.X * diffX)), (frameOffset.Y + (frameOffset.Y * diffY)));
            }

            actor.GetBodyBox().SetRectWidth((int)(bodyRect.Width + (bodyRect.Width * diffX)));
            actor.GetBodyBox().SetRectHeight((int)(bodyRect.Height + (bodyRect.Height * diffY)));
            actor.GetBodyBox().SetOffSet((bodyOffset.X + (bodyOffset.X * diffX)), (bodyOffset.Y + (bodyOffset.Y * diffY)));

            actor.GetBoundsBox().SetRectWidth((int)(boundsRect.Width + (boundsRect.Width * diffX)));
            actor.GetBoundsBox().SetRectHeight((int)(boundsRect.Height + (boundsRect.Height * diffY)));
            actor.GetBoundsBox().SetOffSet((boundsOffset.X + (boundsOffset.X * diffX)), (boundsOffset.Y + (boundsOffset.Y * diffY)));

            actor.GetDepthBox().SetRectWidth((int)(depthRect.Width + (depthRect.Width * diffX)));
            actor.GetDepthBox().SetRectHeight((int)(depthRect.Height + (depthRect.Height * diffY)));
            actor.GetDepthBox().SetOffSet((depthOffset.X + (depthOffset.X * diffX)), (depthOffset.Y + (depthOffset.Y * diffY)));
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
