INCLUDE ../GlobalSettings/global.ink

#speaker:Гонец 
#portrait:ms_yellow_neutral 
#layout:right
Пан, народ измотан ночными караулами. Просят день пиршества: бочка эля, кости на костре, музыка — чтоб забыть страх перед громом осады.
#speaker:Гонец 
#portrait:ms_yellow_neutral 
#layout:left
*   Дать им пир.
-> accept
*   Не до пиров.
-> refuse

=== accept ===
Отпущу бочку и тушу, но рассвет встретим трезвыми. Коль кто перепутает флейту с мечом — хай будет бит плетью.
#speaker:Гонец 
#portrait:ms_yellow_neutral 
#layout:right
Мудро, пан. С песней и жареным мясом дух поднимется выше стены.
~ Food -= 10
~ Gold -= 10
~ PeopleSatisfaction += 2
    -> END

=== refuse ===
Праздник? Мы стоим на краю клинка. Скажи людям: кто хочет веселья — найдёт его в загробном трактире.
#speaker:Гонец 
#portrait:ms_yellow_neutral 
#layout:rightt
Как прикажете… Только клинок тяжелеет в руке, когда сердце пусто.
~ PeopleSatisfaction -= 2
-> END