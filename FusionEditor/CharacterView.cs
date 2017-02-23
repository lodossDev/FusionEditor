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
using System.Windows.Input;

namespace FusionEditor {

    public class CharacterView : WpfGame {
        private bool ENABLE_ANIMATION_CHECK = false;
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;
        private KeyboardState _previousKeyboardState;
        private WpfMouse _mouse;

        private RenderManager _renderManager;
        private Entity _actor;
        private Vector2 baseScale = Vector2.Zero;

        private CLNS.BoxType _selectedBoxType;
        private CLNS.BoundingBox _selectedBoundingBox;
        private BoundingBox _selectedRectItem;
        private ICommand _saveBoxCommand;
        private ICommand _saveFrameCommand;
        private Position _frameOffset;
        private Position _spriteOffset;

        public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(string), typeof(CharacterView), new PropertyMetadata("", ActorOnChangeValue));
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register("Animation", typeof(string), typeof(CharacterView), new PropertyMetadata("", AnimationOnChangeValue));
        public static readonly DependencyProperty FrameProperty = DependencyProperty.Register("Frame", typeof(string), typeof(CharacterView), new PropertyMetadata("", FrameOnChangeValue));
        public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register("Scale", typeof(float), typeof(CharacterView), new PropertyMetadata(0f, ScaleOnChangeValue));
        public static readonly DependencyProperty ShowAttackBoxesProperty = DependencyProperty.Register("ShowAttackBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowAttackBoxesOnChangeValue));
        public static readonly DependencyProperty ShowBodyBoxesProperty = DependencyProperty.Register("ShowBodyBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowBodyBoxesOnChangeValue));
        public static readonly DependencyProperty ShowBoundsBoxesProperty = DependencyProperty.Register("ShowBoundsBoxes", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowBoundsBoxesOnChangeValue));
        public static readonly DependencyProperty BoxItemsProperty = DependencyProperty.Register("BoxItems", typeof(List<string>), typeof(CharacterView));
        public static readonly DependencyProperty SelectedBoxTypeProperty = DependencyProperty.Register("SelectedBoxType", typeof(string), typeof(CharacterView), new PropertyMetadata("", BoxTypeOnChangeValue));
        public static readonly DependencyProperty SelectedBoxItemProperty = DependencyProperty.Register("SelectedBoxItem", typeof(string), typeof(CharacterView), new PropertyMetadata("", BoxItemOnChangeValue));
        public static readonly DependencyProperty SelectedRectItemProperty = DependencyProperty.Register("SelectedRectItem", typeof(BoundingBox), typeof(CharacterView));
        public static readonly DependencyProperty ShowBaseSpriteProperty = DependencyProperty.Register("ShowBaseSprite", typeof(bool), typeof(CharacterView), new PropertyMetadata(false, ShowBaseSpriteOnChangeValue));
        public static readonly DependencyProperty CurrentFrameOffsetProperty = DependencyProperty.Register("FrameOffset", typeof(Position), typeof(CharacterView));
        public static readonly DependencyProperty CurrentSrpiteOffsetProperty = DependencyProperty.Register("SpriteOffset", typeof(Position), typeof(CharacterView));


        public ICommand SaveBoxCommand {
            get {
                if (_saveBoxCommand == null) {
                    _saveBoxCommand = new RelayCommand(param => this.UpdateSelectedBox(), param => this.CanUpdateSelectedBox());
                }

                return _saveBoxCommand;
            }
        }

        public ICommand SaveFrameCommand {
            get {
                if (_saveFrameCommand == null) {
                    _saveFrameCommand = new RelayCommand(param => this.UpdateFrameOffset(), param => this.CanUpdateFrameOffset());
                }

                return _saveFrameCommand;
            }
        }

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

        public bool ShowBaseSprite {
            get {
                return (bool)GetValue(ShowBaseSpriteProperty);
            }

            set {
                SetValue(ShowBaseSpriteProperty, value);
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

        public List<string> BoxItems {
            get {
                return (List<string>)GetValue(BoxItemsProperty);
            }

            set {
                SetValue(BoxItemsProperty, value);
            }
        }

        public string SelectedBoxType {
            get {
                return (string)GetValue(SelectedBoxTypeProperty);
            }

            set {
                SetValue(SelectedBoxTypeProperty, value);
            }
        }

        public string SelectedBoxItem {
            get {
                return (string)GetValue(SelectedBoxItemProperty);
            }

            set {
                SetValue(SelectedBoxItemProperty, value);
            }
        }

        public BoundingBox SelectedRectItem {
            get {
                return (BoundingBox)GetValue(SelectedRectItemProperty);
            }

            set {
                if (SelectedRectItem != value) { 
                    SetValue(SelectedRectItemProperty, value);
                }
            }
        }

        public Position FrameOffset {
            get {
                return (Position)GetValue(CurrentFrameOffsetProperty);
            }

            set {
                if (FrameOffset != value) {
                    SetValue(CurrentFrameOffsetProperty, value);
                }
            }
        }

        public Position SpriteOffset {
            get {
                return (Position)GetValue(CurrentSrpiteOffsetProperty);
            }

            set {
                if (SpriteOffset != value) {
                    SetValue(CurrentSrpiteOffsetProperty, value);
                }
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
                instance._frameOffset = new Position();
                instance._spriteOffset = new Position();

                if (instance.ENABLE_ANIMATION_CHECK == true) {
                    String animation = (string)e.NewValue;

                    if (string.IsNullOrEmpty(animation) == false) { 
                        Animation.State state;
                        bool hasState = Enum.TryParse(animation, true, out state);

                        if (hasState) {
                            instance._actor.SetAnimationState(state);

                            instance._frameOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().X);
                            instance._frameOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().Y);

                            instance._spriteOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().X);
                            instance._spriteOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().Y);
                        } else {
                            MessageBox.Show("Cannot find state: " + animation);
                        }
                    }
                }

                if (instance.ENABLE_ANIMATION_CHECK == false) {
                    instance.ENABLE_ANIMATION_CHECK = true;
                }

                instance.FrameOffset = instance._frameOffset;
                instance.SpriteOffset = instance._spriteOffset;
            }
        }

        private static void FrameOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String frame = (string)e.NewValue;

                instance._frameOffset = new Position();
                instance._spriteOffset = new Position();

                if (String.IsNullOrEmpty(frame) == false) {
                    instance._actor.GetCurrentSprite().SetCurrentFrame(Convert.ToInt32(frame));

                    instance._frameOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().X);
                    instance._frameOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().Y);

                    instance._spriteOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().X);
                    instance._spriteOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().Y);
                }

                List<string> boxItems = new List<string>();

                if (String.IsNullOrEmpty(instance.SelectedBoxType) == false 
                        && String.IsNullOrEmpty(instance.Animation) == false
                        && String.IsNullOrEmpty(instance.Frame) == false) {

                    for (int i = 0; i < instance._actor.GetCurrentBoxes(instance._selectedBoxType).Count; i++) {
                        boxItems.Add(Convert.ToString( i + 1));
                    }
                }

                instance.BoxItems = boxItems;
                instance.FrameOffset = instance._frameOffset;
                instance.SpriteOffset = instance._spriteOffset;

                Debug.WriteLine("{0} {1}: ", instance.SpriteOffset.X, instance.SpriteOffset.Y);
            }
        }

        private static void BoxTypeOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                List<string> boxItems = new List<string>();
                CharacterView instance = source as CharacterView;
                String boxType = (string)e.NewValue;

                if (String.IsNullOrEmpty(boxType) == false 
                        && String.IsNullOrEmpty(instance.Animation) == false
                        && String.IsNullOrEmpty(instance.Frame) == false) {

                    bool hasBoxType = Enum.TryParse(boxType.ToUpper().Replace("ATTACK", "HIT").Replace(" ", "_"), true, out instance._selectedBoxType);

                    if (hasBoxType) {
                        for (int i = 0; i < instance._actor.GetCurrentBoxes(instance._selectedBoxType).Count; i++) {
                            boxItems.Add(Convert.ToString( i + 1));
                        }
                    }
                }

                instance.BoxItems = boxItems;
            }
        }

        private static void BoxItemOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String boxItem = (string)e.NewValue;

                instance._selectedRectItem = new BoundingBox();

                if (String.IsNullOrEmpty(boxItem) == false 
                        && String.IsNullOrEmpty(instance.Animation) == false
                        && String.IsNullOrEmpty(instance.Frame) == false
                        && String.IsNullOrEmpty(instance.SelectedBoxType) == false) {

                    instance._selectedBoundingBox = instance._actor.GetCurrentBoxes(instance._selectedBoxType)[Convert.ToInt32(boxItem) - 1];
                    instance._selectedRectItem.X = (int)Math.Round(instance._selectedBoundingBox.GetBaseOffset().X);
                    instance._selectedRectItem.Y = (int)Math.Round(instance._selectedBoundingBox.GetBaseOffset().Y);

                    instance._selectedRectItem.Width = instance._selectedBoundingBox.GetBaseRect().Width;
                    instance._selectedRectItem.Height = instance._selectedBoundingBox.GetBaseRect().Height;

                    instance._selectedBoundingBox.SetVisibility(1.0f);
                    instance._actor.GetBodyBox().SetVisibility(0.4f);
                    instance._actor.GetBoundsBox().SetVisibility(0.4f);
                    instance._actor.GetDepthBox().SetVisibility(0.4f);

                    for (int i = 0; i < instance._actor.GetAllFrameBoxes().Count; i++) {
                        CLNS.BoundingBox box = instance._actor.GetAllFrameBoxes()[i];
                        
                        if (box != instance._selectedBoundingBox) {
                            box.SetVisibility(0.4f);
                        }
                    }
                } else {
                    if (instance._actor != null) {
                        instance._actor.GetBodyBox().SetVisibility(1.0f);
                        instance._actor.GetBoundsBox().SetVisibility(1.0f);
                        instance._actor.GetDepthBox().SetVisibility(1.0f);

                        for (int i = 0; i < instance._actor.GetAllFrameBoxes().Count; i++) {
                            CLNS.BoundingBox box = instance._actor.GetAllFrameBoxes()[i];
                            box.SetVisibility(1.0f);
                        }
                    }
                }

                instance.SelectedRectItem = instance._selectedRectItem;
            }
        }

        private static void ScaleOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null){
                CharacterView instance = source as CharacterView;
                float sx = instance._actor.GetScale().X;
                float sy = instance._actor.GetScale().Y;

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

                    instance._actor.SetScale(sx, sy);
                    instance._actor.MoveZ(-(newScale - oldScale) * 60);
                } else {
                    instance._actor.SetScale(instance.baseScale.X - 1, instance.baseScale.Y - 1);
                    instance._actor.SetPostion(400, 0, 200);
                }
            }
        }

        private static void ShowBaseSpriteOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e){
            if (e != null){
                CharacterView instance = source as CharacterView;
                bool showBaseSprite = (bool)e.NewValue;

                if (showBaseSprite == true) {
                    instance._renderManager.ShowStanceSprite();
                }else {
                    instance._renderManager.HideStanceSprite();
                }
            }
        }

        private static void ShowAttackBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance._renderManager.ShowAttackBoxes();
                } else {
                    instance._renderManager.HideAttackBoxes();
                }
            }
        }

        private static void ShowBodyBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance._renderManager.ShowBodyBoxes();
                } else {
                    instance._renderManager.HideBodyBoxes();
                }
            }
        }

        private static void ShowBoundsBoxesOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                bool showBoxes = (bool)e.NewValue;

                if (showBoxes == true) { 
                    instance._renderManager.ShowBoundsBoxes();
                } else {
                    instance._renderManager.HideBoundsBoxes();
                }
            }
        }

        private bool CanUpdateSelectedBox() {
            return true;
        }

        private void UpdateSelectedBox() {
            _selectedBoundingBox.SetBase(SelectedRectItem.Width, SelectedRectItem.Height, SelectedRectItem.X, SelectedRectItem.Y);
        }

        private bool CanUpdateFrameOffset() {
            return true;
        }

        private void UpdateFrameOffset() {
            _actor.SetFrameOffset(_actor.GetCurrentAnimationState(), _actor.GetCurrentFrame() + 1, FrameOffset.X, FrameOffset.Y);
            _actor.SetSpriteOffSet(_actor.GetCurrentAnimationState(), FrameOffset.X, FrameOffset.Y);
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

            _actor = new Player_Ryo();
            _actor.SetAnimationType(FusionEngine.Animation.Type.NONE);
            _actor.SetFrameOffset(FusionEngine.Animation.State.ATTACK2, 2, 20, 40);

            baseScale = _actor.GetScale();
            _actor.SetScale(baseScale.X - 1, baseScale.Y - 1);

            _renderManager = new RenderManager();
            _renderManager.AddEntity(_actor);

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
        }

        protected override void Update(GameTime time) {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();

            if (KeyPressed(Keys.Q)) {
                _renderManager.RenderBoxes();
            }

            _renderManager.Update(time);
            _previousKeyboardState = _keyboardState;
        }

        protected override void Draw(GameTime time) {
            _graphicsDeviceManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp);
                _renderManager.Draw(time);
            _spriteBatch.End();
        }

        private bool KeyPressed(Keys k) {
            return _previousKeyboardState.GetPressedKeys().Contains(k) && !_keyboardState.GetPressedKeys().Contains(k);
        }
    }
}
