   M S B A M L   `   `   `  ����F  BMedicalInfo, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:MedicalInfoMedicalInfo  5   	       MedicalInfo.DoctorsWindow   5   	   W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Y UOxyPlot.Wpf.Shared, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0R NOxyPlot.Wpf, Version=2.2.0.0, Culture=neutral, PublicKeyToken=75e952ba404cdbb0"oxyhttp://oxyplot.org/wpf   5   	   'localclr-namespace:MedicalInfo   5   	     � Title$  DoctorsWindow��5	   	   $	��550��6   $
��1269��6,    � 
ResizeMode$ 	CanResize=�69    � WindowStartupLocation$ CenterScreen=�6P   ���#F1F8FB � 	Resources 5       �� 
TargetType   
HeaderText �� Property �� Value  ContentText  
MenuButton �� 
TargetType �� CornerRadius	 �� Padding
 �� Triggers P� Property P� Value  ContentBorder��%�  &	        &	 �     &	 [    &	 �    �� 5   
   " ��6   .�5      ��  � ��$ 16(�6)   �� 5       � ��$
 Bold"�6+   �� 5       � �� �#2D5F7A6+   �� 5       � ��$ 	10,5,0,15q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 13(�6)   �� 5       � �� �#4A4A4A6+   �� 5       � ��$
 10,5q�6'   5      �� 5   
   " ��6   .�5      ��  � ��$ 20(�6)   �� 5       � �� �#E3FDFD6+   �� 5       � ��
 ���� 6+   �� 5        � ��$ 0,5,0,5q�6'   �� 5!       � ��$ 10q�6(   �� 5"       � ��$
 Left=�6;   �� 5#       �@�Cursor$
 Hand��6'   �� 5$       � ��$ 0q�60   �� 5%       � �� 5&      �� 5'      " ��6&   .R�5(      �� #��z��6!   $ 10��6K   #	 z��6]   ._�5)      �� $
��Left=�6/   $��Center=�6J   5*      5+      5-      
 5.      P� 5/       �@J�IsMouseOver . . �50      ��  � �� �#16317263   51      53      �� 56   
   " ��6   .�57      ��  � ��
 �����6+   �� 58       �@��CornerRadius$ 20��6-   �� 59       �@��Padding$ 15q�6(   �� 5:       � ��$ 0q�6'   5;      5<      .��5>      �  � ColumnDefinitions 5@   
   �� 5A      $	��200 �6   �� 5B      �� 5C      $	��380 �6   .+�5H   
   �� ���#2D5F7A$��0��6*   .+�5I      �� $��logo.png��6   $	��120��6&   $	��120��62   $��Center=�6?   $��	0,20,0,20q�6\   �� 5J      -   5        $��Главная��5J      #��[ 6'   6K   �� 5K      -   5        $��
Врачи��5K      #��[ 6%   6I   �� 5L      -   5        $��Препараты��5L      #��[ 6)   6M   �� 5M      $��
Выйти��6   #��[ 6%   ���#F57170$��Center=�6^   $��0,20,0,0q�6{   5N      � 5P   
   $��1��6   $��20,10,20,10q�6   .+�5R      ]� $��Stretch=�6   ��. $��10q�6K   .+�5S      �� $��Список врачей��6   $��24(�61   $
��Bold"�6?   ���#2D5F7A$	��Top=�6f   $��Right=�6~   5T      �� 5W       �� Orientation$ 
Horizontal=�6   $��	0,10,0,20q�62   $��Center=�6E   $	��780��6b   .+�5X      �� -   5        $��FilterTextBox��5X      $	��250��6/   $��30��6;   $��14(�6G   $��	80,0,20,0q�6U   $	��Top=�6h   �� 5Y      -   5        $��
Найти��5Y      $	��100��6)   $��30��65   $��14(�6A   
�������$	��Top=�6b   $��0,0,0,10q�6z   ��5Z      �� 5[      " ��6*   .R�5\      �� #��z��6%   #��z��6O   $��1q�6{   $ 15��6�   ._�5]   "   �� $��Center=�63   $��Center=�6P   5^      5_      ��5a      �� 5b       �� 
StartPoint$	 0,0&�6.    �� EndPoint$	 1,1&�6?   .��5c      �  � Color$ #4A90E2��6+    � Offset$	 0.0Q�6;   � 5d      $ #357ABD��6+   $	 1.0Q�6;   5e      5g      5h      �� 5k      $
��Auto=�6   $��Center=�6>   $��Center=�6[   $	��600��6v   $	��450��6�   $��0,40,0,0q�6�   .��5l      �� -   5        $��DoctorsListView��5l      ��5m      �� 5n      .R�5o      �  � Orientation$ Vertical=�6(   $��Center=�6?   5p      ��5r      �� 5s      .R�5t      �� -   5        $��10q�5t   %   $	 15q�61   ���#E1E8ED$��1q�6T   $ 10��6h   
�������$	��500��6�    J� Effect 5u   "   3  -System.Windows.Media.Effects.DropShadowEffect  5v   &      Color$ #A0A0A0��67      
BlurRadius$ 8Q�6G      ShadowDepth$ 2Q�6V   ._�5x   "   �� .+�5y   &   �� $
��Bold"�6J   $��18(�6\   ���#2D5F7A$��0,0,0,8q�6   ��60   �� *
FullName+6   �� 5z   &   $ 
Horizontal=�61   $��0,0,0,5q�6J   .+�5{   *   �� $
��🏥��64   $��14(�6>   $��Center=�6L   $��0,0,5,0q�6g   �� 5|   *   $��14(�6X   ���#4A4A4A��64   �� *SpecializationName+6f   5}   '   �� 5~   &   $ 
Horizontal=�61   $��0,0,0,5q�6J   .+�5   *   �� $��Опыт работы: ��64   $
��Bold"�6I   $��14(�6[   ���#2D5F7A6i   �� 5�   *   $��14(�6P   ���#4A4A4A��64   �� *
Experience+6^   5�   '   �� 5�   &   $ 
Horizontal=�61   $��0,0,0,5q�6J   .+�5�   *   �� $
��📞��64   $��14(�6>   $��Center=�6L   $��0,0,5,0q�6g   �� 5�   *   $��14(�6Q   ���#4A4A4A��64   �� *ContactInfo+6_   5�   '   �� 5�   &   $ 
Horizontal=�61   $��0,0,0,5q�6J   .+�5�   *   �� $
��🕒��64   $��14(�6>   $��Center=�6L   $��0,0,5,0q�6g   �� 5�   *   $��14(�6N   ���#4A4A4A��64   �� *
Schedule+6\   5�   '   �� 5�   &   $��1��60   ���#E1E8ED$��	0,10,0,10q�6J   �� 5�   &   -   5        $��Удалить��5�   -   ���#F57170
�������$��80��6g   $��Right=�6r   6�   5�   #   5�      5�      5�      5�      5�      �� 5�   
   $��2��6   
�������$��
20,20,20,0q�68   $	��Top=�6L   $	��474��6d   .+�5�      �� $ Vertical=�6   $��Center=�60   $	��Top=�6M   $��20q�6e   .+�5�      �� $��ФИО врача��6   $��14(�6-   $
��Bold"�6;   ���#4A4A4A$	��0,5q�6b   �� 5�      -	   5        $��DoctorNameTextBox��5�      $	��300��63   $��30��6?   $��14(�6K   $	��0,5q�6Y   �� 5�      $ ��Специализация��6   $��14(�61   $
��Bold"�6?   ���#4A4A4A$	��0,5q�6f   �� 5�      -
   5        $��SpecializationTextBox��5�      $	��300��67   $��30��6C   $��14(�6O   $	��0,5q�6]   �� 5�      $��Опыт работы��6   $��14(�6/   $
��Bold"�6=   ���#4A4A4A$	��0,5q�6d   �� 5�      -   5        $��ExperienceTextBox��5�      $	��300��63   $��30��6?   $��14(�6K   $	��0,5q�6Y   �� 5�      $/��)Контактная информация��6   $��14(�69   $
��Bold"�6G   ���#4A4A4A$	��0,5q�6n   �� 5�      -   5        $��ContactInfoTextBox��5�      $	��300��64   $��30��6@   $��14(�6L   $	��0,5q�6Z   �� 5�      $��Расписание��6   $��14(�6.   $
��Bold"�6<   ���#4A4A4A$	��0,5q�6c   �� 5�      -   5        $��ScheduleTextBox��5�      $	��300��61   $��30��6=   $��14(�6I   $	��0,5q�6W   �� 5�      -   5        $��Добавить��5�      $	��100��6,   $��30��68   $��14(�6D   
�������$	��Top=�6e   $��0,10,0,0q�6}   ��5�      �� 5�      " ��6*   .R�5�      �� #��z��6%   #��z��6O   $��1q�6{   $ 15��6�   ._�5�   "   �� $��Center=�63   $��Center=�6P   5�      5�      ��5�      �� 5�      $	 0,0&�6.   $	 1,1&�6?   .��5�      � $ #4A90E2��6+   $	 0.0Q�6;   � 5�      $ #357ABD��6+   $	 1.0Q�6;   5�      5�      �� 5�      -   5        $+��%Сохранить изменения��5�      $	��160��67   $��30��6C   $��14(�6O   
�������$	��Top=�6p   $��0,10,0,0q�6�   ��5�      �� 5�      " ��6*   .R�5�      �� #��z��6%   #��z��6O   $��1q�6{   $ 15��6�   ._�5�   "   �� $��Center=�63   $��Center=�6P   5�      5�      ��5�      �� 5�      $	 0,0&�6.   $	 1,1&�6?   .��5�      � $ #4A90E2��6+   $	 0.0Q�6;   � 5�      $ #357ABD��6+   $	 1.0Q�6;   5�      5�      5�      5�      5�      5�      