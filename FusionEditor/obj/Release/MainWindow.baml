   M S B A M L   `   `   `  ���� G  CFusionEditor, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:FusionEditorFusionEditor  +clr-namespace:FusionEditorFusionEditor      FusionEditor.MainWindow   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:FusionEditor     3FusionEditorclr-namespace:FusionEditor     -     � Title$  Character Viewer��$	��941��$
��1024�� � 
ResizeMode$ NoResize=� � Icon$ ./FusionIcon.ico��.���  � ColumnDefinitions �� $	��69* ��� $
��949* �.+�    FusionEditor.CharacterView  -     Name$ characterView��$	��800��$��17,85,201,17q�$��2��   	Animation �� *SelectedValue+ �� ElementName$ 
animations��   Frame �� *SelectedValue+$ frames��   Scale �� *Value+$ scaleSlider��	   ShowAttackBoxes	 �� *	IsChecked+$ checkAttackBoxes��
   ShowBodyBoxes
 �� *	IsChecked+$ checkBodyBoxes��   ShowBoundsBoxes �� *	IsChecked+$ checkBoundsBoxes��   SelectedBoxType �� *SelectedValue+$ comboBoxType��   SelectedBoxItem �� *SelectedValue+$ comboBoxItems��   ShowBaseSprite �� *	IsChecked+$ checkBaseSprite��   Actor �� *SelectedValue+$ actors�� �� IsAsync . �� -    ��Name$ scaleSlider��$
��Left=�$��
777,10,0,0q�$	��Top=�$	��152�� �� TickFrequency$ 1Q� �� TickPlacement$ BottomRight=� �� IsSnapToTickEnabled . $��1���� -    ��Name$ actors��$
��Left=�$��	10,29,0,0q�$	��Top=�$	��120��$��2���� -   $ 
animations��$
��Left=�$��	66,29,0,0q�$	��Top=�$	��120��$��1���� -   $ frames��$
��Left=�$��
191,29,0,0q�$	��Top=�$	��120��$��1������ *Frames+$ characterView���� -    ��Name$ checkAttackBoxes��$��Show Attack Boxes��$
��Left=�$��
640,12,0,0q�$	��Top=�$��1���� -   $ checkBodyBoxes��$��Show Body Boxes��$
��Left=�$��
640,32,0,0q�$	��Top=�$��1���� -	   $ checkBoundsBoxes��$��Show Bounds Boxes��$
��Left=�$��
640,52,0,0q�$	��Top=�$��1���� -
    ��Name$ entityLabel��$��Entity��$
��Left=�$��49,2,0,0q�$	��Top=�$��41��$��2���� -   $ animationsLabel��$��
Animations��$
��Left=�$��85,2,0,0q�$	��Top=�$��1���� -   $ framesLabel��$��Frames��$
��Left=�$��	221,2,0,0q�$	��Top=�$��1���� -   $ comboBoxType��$
��Left=�$��
367,29,0,0q�$	��Top=�$	��120�� �� SelectedValuePath$ Content��$��1��.@��� .��
Attack Box�� .��
Body Box�� -   $ boxTypeLabel��$��Box Type��$
��Left=�$��	395,2,0,0q�$	��Top=�$��1���� -   $ comboBoxItems��$��1��$
��Left=�$��
498,29,0,0q�$	��Top=�$	��120������ *
BoxItems+$ characterView���� -   $ boxItemsLabel��$��	Box Items��$
��Left=�$��	518,2,0,0q�$	��Top=�$��1���� -    ��Name$ boxWidth��$
��Left=�$��23��$��818,103,0,0q� �� TextWrapping$
 Wrap=�$	��Top=�$��71��$��1������ *SelectedRectItem.Width+$ characterView���� -   $ boxWidthLabel��$��Width��$
��Left=�$��25��$��772,101,0,0q�$	��Top=�$��1��$
��Bold"�$��45���� -   $ boxHeightLabel��$��Height��$
��Left=�$��26��$��768,128,0,0q�$	��Top=�$��1��$
��Bold"�$��52���� -   $ 	boxHeight��$
��Left=�$��23��$��818,131,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *SelectedRectItem.Height+$ characterView���� -   $
 boxX��$
��Left=�$��23��$��818,160,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *SelectedRectItem.X+$ characterView���� -   $ 	boxXLabel��$��X��$
��Left=�$��25��$��798,158,0,0q�$	��Top=�$��1�� J� RenderTransformOrigin$ 
0.941,0.56&�$
��Bold"��� -   $ 	boxYLabel��$��Y��$
��Left=�$��26��$��799,186,0,0q�$	��Top=�$��1��$
��Bold"��� -   $
 boxY��$
��Left=�$��23��$��818,189,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *SelectedRectItem.Y+$ characterView���� -    ��Name$ saveBtn��$
��Save��$��1��$
��Left=�$��832,216,0,0q�$	��Top=�$��47������ *SaveBoxCommand+$ characterView�� �� Mode$ OneWay=��� -   $ checkBaseSprite��$��Show Base Sprite��$��1��$
��Left=�$��
777,52,0,0q�$	��Top=��� -   $ frameX��$
��Left=�$��23��$��818,270,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *FrameOffset.X+$ characterView���� -   $ 
labelOffX1��$��Offset X��$
��Left=�$��25��$��764,267,0,0q�$	��Top=�$��1��$ 
0.941,0.56&�$
��Bold"��� -   $ 	labelOffY��$��Offset Y��$
��Left=�$��26��$��765,298,0,0q�$	��Top=�$��1��$
��Bold"��� -   $ frameY��$
��Left=�$��23��$��818,299,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *FrameOffset.Y+$ characterView���� -   $ saveBtn_Copy��$
��Save��$��1��$
��Left=�$��832,413,0,0q�$	��Top=�$��47������ *SaveFrameCommand+$ characterView��$ OneWay=��� -    $ label��$��Box Properties��$��1��$
��Left=�$��
750,76,0,0q�$	��Top=�$	��158���� -!   $ 
label_Copy��$��Frame Properties��$��1��$
��Left=�$��750,243,0,0q�$	��Top=�$	��158���� -"   $ 
spriteBoxX��$
��Left=�$��23��$��818,357,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *SpriteOffset.X+$ characterView���� -#   $ labelSX��$��Offset X��$
��Left=�$��25��$��764,355,0,0q�$	��Top=�$��1��$ 
0.941,0.56&�$
��Bold"��� -$   $ labelSY��$��Offset Y��$
��Left=�$��26��$��765,384,0,0q�$	��Top=�$��1��$
��Bold"��� -%   $ 
spriteBoxY��$
��Left=�$��23��$��818,386,0,0q�$
 Wrap=�$	��Top=�$��71��$��1������ *SpriteOffset.Y+$ characterView���� -&   $ label_Copy1��$��Sprite Properties��$��1��$
��Left=�$��750,325,0,0q�$	��Top=�$	��158���� -'    ��Name$ 	scrollBar��$
��Left=�$��
323,20,0,0q�$	��Top=�$ 0.529,0.543&�$��39��$��22�� � SmallChange$ 1Q�  � Minimum$  	-10000000Q�! � Maximum$! 10000000Q�$��1��z�V� .~��� �� �� " �� Angle$	" 270Q�T� # T� X$# -2.471Q�$ T� Y$$ -1.485Q��� -(   $ 	btnAddBox��$	��Add��$��1��$
��Left=�$��24��$��
367,55,0,0q�$	��Top=�$��58��$ 	0.547,0.5&����� *AddBoxCommand+$ characterView��$ OneWay=��� -)   $ labelBaseProp��$��Base Properties��$��1��$
��Left=�$��750,440,0,0q�$	��Top=�$��95���� -*   $ baseX��$��Offset X��$��1��$
��Left=�$��28��$��764,468,0,0q�$	��Top=�$��58��$
��Bold"��� -+   $ baseY��$��Offset Y��$��1��$
��Left=�$��28��$��764,500,0,0q�$	��Top=�$��58��$
��Bold"��� -,   $ btnRemoveBox��$��Remove��$��1��$
��Left=�$��24��$��
429,55,0,0q�$	��Top=�$��58��$ 	0.547,0.5&����� *RemoveBoxCommand+$ characterView��$ OneWay=��� --   $ 	baseTextX��$��1��$
��Left=�$��26��$��818,468,0,0q�$
 Wrap=�$	��Top=�$��71������ *BaseOffset.X+$ characterView���� -.   $ 	baseTextY��$��1��$
��Left=�$��25��$��818,501,0,0q�$
 Wrap=�$	��Top=�$��71������ *BaseOffset.Y+$ characterView���� -/   $ labelPosition��$��Position Properties��$��1��$
��Left=�$��750,678,0,0q�$	��Top=�$	��117���� -0   $ 	posXLabel��$��X��$��1��$
��Left=�$��28��$��800,704,0,0q�$	��Top=�$��22��$
��Bold"��� -1   $ 	posyLabel��$��Y��$��1��$
��Left=�$��28��$��801,736,0,0q�$	��Top=�$��21��$
��Bold"��� -2   $ posTextX��$��1��$
��Left=�$��26��$��818,704,0,0q�$
 Wrap=�$	��Top=�$��71������ *
Position.X+$ characterView���� -3   $ posTextY��$��1��$
��Left=�$��25��$��818,737,0,0q�$
 Wrap=�$	��Top=�$��71������ *
Position.Y+$ characterView���� -4   $ baseSave��$
��Save��$��1��$
��Left=�$��832,531,0,0q�$	��Top=�$��47������ *SaveBaseCommand+$ characterView��$ OneWay=��� -5   $ posSave��$
��Save��$��1��$
��Left=�$��832,767,0,0q�$	��Top=�$��47������ *SavePositionCommand+$ characterView��$ OneWay=��� -6   $ labelShadow��$��Shadow Properties��$��1��$
��Left=�$��750,561,0,0q�$	��Top=�$	��117���� -7   $ shadowPosXLabel��$��Offset X��$��1��$
��Left=�$��28��$��764,586,0,0q�$	��Top=�$��58��$
��Bold"��� -8   $ shadowPosYLabel��$��Offset Y��$��1��$
��Left=�$��28��$��765,618,0,0q�$	��Top=�$��57��$
��Bold"��� -9   $ shadowX��$��1��$
��Left=�$��26��$��818,586,0,0q�$
 Wrap=�$	��Top=�$��71������ *ShadowOffset.X+$ characterView���� -:   $ shadowY��$��1��$
��Left=�$��25��$��818,619,0,0q�$
 Wrap=�$	��Top=�$��71������ *ShadowOffset.Y+$ characterView���� -;   $ 
shadowSave��$��1��$
��Left=�$��832,649,0,0q�$	��Top=�$��47��$
��Save������ *SaveShadowCommand+$ characterView��$ OneWay=�