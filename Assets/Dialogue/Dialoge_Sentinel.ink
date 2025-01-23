Эй, стой! Кто таков и куда направляешься? #speaker:Караульный #portrait:dr_green_neutral #layout:right
-> main

=== main ===
Я ГГ, сын крестьянина из деревни на склоне. Мне нужно увидеть вашего господина, сэра Габриэля.#speaker:ГГ #portrait:ms_yellow_neutral #layout:left
Да да, конечно, паренек. Что тебе нужно от его милости, и с какой радости он должен тебя принимать?#speaker:Караульный #portrait:dr_green_neutral #layout:right
Я помог его советнику, Хансу, на дороге. Напали дикие звери, и я выручил его. Теперь хочу попросить работу и шанс на новую жизнь.#speaker:ГГ #portrait:ms_yellow_neutral #layout:left
Такой как ты и помог сэру Хансу ? Честное слово, не смеши меня!#speaker:Караульный #portrait:dr_green_neutral #layout:right
+[Мне плевать, что ты там думаешь. Просто пропусти меня!]
-> gate1
+[Ты надеешься подзаработать на мне, а?]
-> gate2
+[Может, вот это убедит тебя? *Доставая медальон.]
->gate3

===gate1===
О, у щенка есть зубы? Проваливай.#speaker:Караульный #portrait:dr_green_neutral #layout:right
(бросает на караульного презрительный взгляд)#speaker:ГГ #portrait:ms_yellow_neutral #layout:left
->DONE

===gate2===
Убирайся отсюда, пока я не надел на тебя кандалы!#speaker:Караульный #portrait:dr_green_neutral #layout:right
->DONE

===gate3===
Ну-ка, дай сюда. Хм... Это действительно медальон господского двора. Проклятье! Почему ты сразу не сказал?!#speaker:Караульный #portrait:dr_green_neutral #layout:right
Так ты и вправду помог советнику...#speaker:Караульный #portrait:dr_green_neutral #layout:right

Многое произошло... . Я должен сообщить его милости, что произошло, а потом я буду просить о работе. У меня нет другого выбора.#speaker:ГГ #portrait:ms_yellow_neutral #layout:left
(Караульный опускает оружие и открывает ворота)#speaker:Караульный #portrait:dr_green_neutral #layout:right
(С легкой ухмылкой) Ладно, проходи. Только не вздумай чудить, парень. Слышал, Меррик сейчас с сэром Габриэлем в рыцарском зале, вроде о делах замка толкуют. Надеюсь, тебе это пригодится.#speaker:Караульный #portrait:dr_green_neutral #layout:right
Спасибо.#speaker:ГГ #portrait:ms_yellow_neutral #layout:left
->DONE

->END