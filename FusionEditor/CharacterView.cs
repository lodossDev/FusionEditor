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
using System.IO;
using System.Reflection;
using System.Collections.Concurrent;

namespace FusionEditor {

    public class CharacterView : WpfGame {
        private bool ENABLE_ANIMATION_CHECK = false;
        private IGraphicsDeviceService _graphicsDeviceManager;
        private SpriteBatch _spriteBatch;
        private WpfKeyboard _keyboard;
        private KeyboardState _keyboardState;
        private KeyboardState _previousKeyboardState;
        private WpfMouse _mouse;
        private MouseState _mouseState;
        private MouseState _previousMouseState;

        private RenderManager _renderManager;
        private ConcurrentDictionary<string, Entity> _entities;
        private Entity _actor;

        private CLNS.BoxType _selectedBoxType;
        private CLNS.BoundingBox _selectedBoundingBox;
        private BoundingBox _selectedRectItem;

        private ICommand _addBoxCommand;
        private ICommand _removeBoxCommand;
        private ICommand _saveBoxCommand;
        private ICommand _saveFrameCommand;
        private ICommand _saveBaseCommand;
        private ICommand _savePositionCommand;
        private ICommand _saveShadowCommand;

        private Position _position;
        private Position _baseOffset;
        private Position _frameOffset;
        private Position _spriteOffset;
        private Position _shadowOffset;
        private List<string> _frames;
        private Assembly _fusionEngineASM;
        private bool _hasSelected;

        public static readonly DependencyProperty ActorProperty = DependencyProperty.Register("Actor", typeof(string), typeof(CharacterView), new PropertyMetadata("", ActorOnChangeValue));
        public static readonly DependencyProperty AnimationProperty = DependencyProperty.Register("Animation", typeof(string), typeof(CharacterView), new PropertyMetadata("", AnimationOnChangeValue));
        public static readonly DependencyProperty FramesProperty = DependencyProperty.Register("Frames", typeof(List<string>), typeof(CharacterView));
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
        public static readonly DependencyProperty BaseOffsetProperty = DependencyProperty.Register("BaseOffset", typeof(Position), typeof(CharacterView));
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position", typeof(Position), typeof(CharacterView));
        public static readonly DependencyProperty ShadowOffsetProperty = DependencyProperty.Register("ShadowOffset", typeof(Position), typeof(CharacterView));


        public ICommand SaveBoxCommand {
            get {
                if (_saveBoxCommand == null) {
                    _saveBoxCommand = new RelayCommand(param => this.UpdateSelectedBox(), param => { return true; });
                }

                return _saveBoxCommand;
            }
        }

        public ICommand SaveFrameCommand {
            get {
                if (_saveFrameCommand == null) {
                    _saveFrameCommand = new RelayCommand(param => this.UpdateFrameOffset(), param => { return true; });
                }

                return _saveFrameCommand;
            }
        }

        public ICommand AddBoxCommand {
            get {
                if (_addBoxCommand == null) {
                    _addBoxCommand = new RelayCommand(param => this.AddBox(), param => { return true; });
                }

                return _addBoxCommand;
            }
        }

        public ICommand RemoveBoxCommand {
            get {
                if (_removeBoxCommand == null) {
                    _removeBoxCommand = new RelayCommand(param => this.RemoveBox(), param => { return true; });
                }

                return _removeBoxCommand;
            }
        }

        public ICommand SaveBaseCommand {
            get {
                if (_saveBaseCommand == null) {
                    _saveBaseCommand = new RelayCommand(param => this.SaveBaseOffset(), param => { return true; });
                }

                return _saveBaseCommand;
            }
        }

        public ICommand SavePositionCommand {
            get {
                if (_savePositionCommand == null) {
                    _savePositionCommand = new RelayCommand(param => this.SavePosition(), param => { return true; });
                }

                return _savePositionCommand;
            }
        }

        public ICommand SaveShadowCommand {
            get {
                if (_saveShadowCommand == null) {
                    _saveShadowCommand = new RelayCommand(param => this.SaveShadowOffset(), param => { return true; });
                }

                return _saveShadowCommand;
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

        public List<string> Frames {
            get {
                return (List<string>)GetValue(FramesProperty);
            }

            set {
                SetValue(FramesProperty, value);
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

        public Position Position {
            get {
                return (Position)GetValue(PositionProperty);
            }

            set {
                if (Position != value) {
                    SetValue(PositionProperty, value);
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

        public Position ShadowOffset {
            get {
                return (Position)GetValue(ShadowOffsetProperty);
            }

            set {
                if (ShadowOffset != value) {
                    SetValue(ShadowOffsetProperty, value);
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

        public Position BaseOffset {
            get {
                return (Position)GetValue(BaseOffsetProperty);
            }

            set {
                if (BaseOffset != value) {
                    SetValue(BaseOffsetProperty, value);
                }
            }
        }

        private static void ActorOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String actor = (string)e.NewValue;
                instance._frames = new List<string>();
                instance._baseOffset = new Position();
                instance._shadowOffset = new Position();
                instance._position = new Position();

                if (string.IsNullOrEmpty(actor) == false) { 
                    if (instance._actor != null) {
                        instance._renderManager.RemoveEntity(instance._actor);
                    }

                    instance._position.X = 400;
                    instance._position.Y = 200;

                    if (instance._entities.ContainsKey(actor) == false) {
                        if (instance._fusionEngineASM != null) { 
                            var type = instance._fusionEngineASM.GetType("FusionEngine." + actor);
                       
                            Entity entity = (Entity)Activator.CreateInstance(type);
                            entity.SetAnimationType(FusionEngine.Animation.Type.NONE);
                            entity.SetAnimationState(FusionEngine.Animation.State.STANCE);
                           
                            instance._entities.TryAdd(actor, entity);
                            instance._renderManager.AddEntity(entity);
                            instance._actor = entity;
                        }
                    } else {
                        instance._actor = instance._entities[actor];
                        instance._actor.SetAnimationState(FusionEngine.Animation.State.STANCE);
                        instance._renderManager.AddEntity(instance._actor);
                    }

                    instance._baseOffset.X = (int)instance._actor.GetBaseOffsetX();
                    instance._baseOffset.Y = (int)instance._actor.GetBaseOffsetY();
                    instance._shadowOffset.X = (int)instance._actor.GetCurrentSprite().GetShadowOffsetX();
                    instance._shadowOffset.Y = (int)instance._actor.GetCurrentSprite().GetShadowOffsetY();
                }

                instance.Frames = instance._frames;
                instance.BaseOffset = instance._baseOffset;
                instance.Position = instance._position;
                instance.ShadowOffset = instance._shadowOffset;

                if (instance._actor != null) { 
                    instance._actor.SetPostion(instance.Position.X, 0, instance.Position.Y);
                    instance._actor.SetScale(instance._actor.GetBaseScale().X - 1, instance._actor.GetBaseScale().Y - 1);
                    instance.CheckScale(instance.Scale, 0);

                    DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(AnimationProperty, null, "STANCE");
                    AnimationOnChangeValue(source, args);
                }
            }
        }

        private static void AnimationOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                instance._frameOffset = new Position();
                instance._spriteOffset = new Position();
                instance._frames = new List<string>();
                instance._shadowOffset = new Position();

                if (instance._actor != null 
                        && instance.ENABLE_ANIMATION_CHECK == true) {

                    String animation = (string)e.NewValue;

                    if (string.IsNullOrEmpty(animation) == false) { 
                        Animation.State state;
                        bool hasState = Enum.TryParse(animation, true, out state);

                        if (instance._actor != null) {

                            if (hasState && instance._actor.HasSprite(state)) {
                                instance._actor.SetAnimationState(state);

                                for (int i = 0; i < instance._actor.GetCurrentSprite().GetFrames(); i++) {
                                    instance._frames.Add(Convert.ToString(i + 1));
                                }

                                instance._frameOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().X);
                                instance._frameOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().Y);

                                instance._spriteOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().X);
                                instance._spriteOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().Y);

                                instance._shadowOffset.X = (int)instance._actor.GetCurrentSprite().GetShadowOffsetX();
                                instance._shadowOffset.Y = (int)instance._actor.GetCurrentSprite().GetShadowOffsetY();

                            } else {
                                instance._frames.Clear();
                                MessageBox.Show("Cannot find state: " + animation);
                            }
                        }
                    }
                }

                if (instance.ENABLE_ANIMATION_CHECK == false) {
                    instance.ENABLE_ANIMATION_CHECK = true;
                }

                instance.Frames = instance._frames;
                instance.FrameOffset = instance._frameOffset;
                instance.SpriteOffset = instance._spriteOffset;
                instance.ShadowOffset = instance._shadowOffset;
            }
        }

        private static void FrameOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                CharacterView instance = source as CharacterView;
                String frame = (string)e.NewValue;

                instance._frameOffset = new Position();
                instance._spriteOffset = new Position();
                instance._shadowOffset = new Position();

                if (instance._actor != null 
                        && String.IsNullOrEmpty(frame) == false) {

                    instance._actor.GetCurrentSprite().SetCurrentFrame(Convert.ToInt32(frame));

                    instance._frameOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().X);
                    instance._frameOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetCurrentFrameOffSet().Y);

                    instance._spriteOffset.X = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().X);
                    instance._spriteOffset.Y = (int)Math.Round(instance._actor.GetCurrentSprite().GetSpriteOffSet().Y);

                    instance._shadowOffset.X = (int)instance._actor.GetCurrentSprite().GetShadowOffsetX();
                    instance._shadowOffset.Y = (int)instance._actor.GetCurrentSprite().GetShadowOffsetY();
                }

                List<string> boxItems = new List<string>();

                if (instance._actor != null
                        && String.IsNullOrEmpty(instance.SelectedBoxType) == false 
                        && String.IsNullOrEmpty(instance.Animation) == false
                        && String.IsNullOrEmpty(instance.Frame) == false) {

                    for (int i = 0; i < instance._actor.GetCurrentBoxes(instance._selectedBoxType).Count; i++) {
                        boxItems.Add(Convert.ToString( i + 1));
                    }
                }

                instance.BoxItems = boxItems;
                instance.FrameOffset = instance._frameOffset;
                instance.SpriteOffset = instance._spriteOffset;
                instance.ShadowOffset = instance._shadowOffset;
            }
        }

        private static void BoxTypeOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null) {
                List<string> boxItems = new List<string>();
                CharacterView instance = source as CharacterView;
                String boxType = (string)e.NewValue;

                if (instance._actor != null
                        && String.IsNullOrEmpty(boxType) == false 
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

                if (instance._actor != null
                        && String.IsNullOrEmpty(boxItem) == false 
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

        private void CheckScale(float newScale, float oldScale) {
            if (_actor != null) { 
                float sx = _actor.GetScale().X;
                float sy = _actor.GetScale().Y;

                if (float.IsNaN(newScale) == false && newScale > 0) {
                    sx += newScale - oldScale;
                    sy += newScale - oldScale;

                    if (sx <= _actor.GetBaseScale().X - 1) {
                        sx = _actor.GetBaseScale().X - 1;
                    }

                    if (sy <= _actor.GetBaseScale().Y - 1) {
                        sy = _actor.GetBaseScale().Y - 1;
                    }

                    _actor.SetScale(sx, sy);
                    _actor.MoveZ(-(newScale - oldScale) * 60);

                } else {
                    _actor.SetScale(_actor.GetBaseScale().X - 1, _actor.GetBaseScale().Y - 1);
                    _actor.SetPostion(Position.X, 0, Position.Y);
                }
            }
        }

        private static void ScaleOnChangeValue(DependencyObject source, DependencyPropertyChangedEventArgs e) {
            if (e != null){
                CharacterView instance = source as CharacterView;

                if (instance._actor != null) { 
                    float sx = instance._actor.GetScale().X;
                    float sy = instance._actor.GetScale().Y;

                    float oldScale = (float)Math.Round((float)e.OldValue);
                    float newScale = (float)Math.Round((float)e.NewValue);

                    instance.CheckScale(newScale, oldScale);
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

        private void SaveShadowOffset() { 
            _actor.GetCurrentSprite().SetShadowOffsetX(ShadowOffset.X);
            _actor.GetCurrentSprite().SetShadowOffsetY(ShadowOffset.Y);
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

        private void UpdateSelectedBox() {
            if(_selectedBoundingBox != null) { 
                _selectedBoundingBox.SetBase(SelectedRectItem.Width, SelectedRectItem.Height, SelectedRectItem.X, SelectedRectItem.Y);
            }
        }

        private void UpdateFrameOffset() {
            if (_actor != null) { 
                _actor.SetFrameOffset(_actor.GetCurrentAnimationState(), _actor.GetCurrentFrame() + 1, FrameOffset.X, FrameOffset.Y);
                _actor.SetSpriteOffSet(_actor.GetCurrentAnimationState(), SpriteOffset.X, SpriteOffset.Y);
            }
        }

        private void RemoveBox() {
            if (_actor != null && string.IsNullOrEmpty(SelectedBoxType) == false 
                    && string.IsNullOrEmpty(SelectedBoxItem) == false
                    && _selectedBoundingBox != null) { 
                
                List<CLNS.BoundingBox> boxes =  _actor.GetCurrentSprite().GetBaseBoxes()[_actor.GetCurrentFrame()];
                boxes.Remove(_selectedBoundingBox);

                _selectedBoundingBox = null;

               DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(BoxItemsProperty, null, SelectedBoxType);
               BoxTypeOnChangeValue(this, args);
            }
        }

        private void AddBox() {
            if (_actor != null && string.IsNullOrEmpty(SelectedBoxType) == false) {
                _actor.AddBox(_actor.GetCurrentAnimationState(), _actor.GetCurrentSpriteFrame() + 1, new CLNS.BoundingBox(_selectedBoxType, 120, 120, 0, 0, _actor.GetCurrentSpriteFrame() + 1));

               DependencyPropertyChangedEventArgs args = new DependencyPropertyChangedEventArgs(BoxItemsProperty, null, SelectedBoxType);
               BoxTypeOnChangeValue(this, args);
            }
        }

        private void SaveBaseOffset() {
            if (_actor != null) {
                _actor.SetBaseOffset(_baseOffset.X, _baseOffset.Y);
            }
        }

        private void SavePosition() {
            if (_actor != null) {
                _actor.SetPostion(_position.X, 0, _position.Y);
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

            _fusionEngineASM = Assembly.LoadFrom("./FusionEngine.dll");
            _entities = new ConcurrentDictionary<string, Entity>();
            _renderManager = new RenderManager();
            _hasSelected = false;

            Position.X = 400;
            Position.Y = 200;
            
            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();
        }

        protected override void Update(GameTime time) {
            // every update we can now query the keyboard & mouse for our WpfGame
            _mouseState = _mouse.GetState();
            _keyboardState = _keyboard.GetState();

            if (KeyPressed(Keys.Q)) {
                _renderManager.RenderBoxes();
            }
            
            _renderManager.Update(time);
            _previousKeyboardState = _keyboardState;
            _previousMouseState = _mouseState;
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
