INCLUDE ../GlobalSettings/global.ink
# speaker: Хан Аташ
# portrait: black_man
# layout: right
Ваше правление длится слишком долго, пан. Честь требует одного из двух: схлестнуться в поединке — закипела кровь наших предков, или выведем войска на равнину и решим судьбу клинками и копьями.

# speaker: Хан Аташ 
# portrait: black_man 
# layout: left
*   Принять поединок чести
-> askLocation
*   Встреться армиями
-> refuseHelp

=== askLocation ===
    Поединок чести? Пусть будет так:прямо здесь и сейчас.
    # speaker: Хан Аташ
    # portrait: black_man
    # layout: right
    Подготовь сталь острее слов, пан. Пусть судьбу вершит лишь рука одного.
     ~ PowerCheckStart = true 
    -> END

=== refuseHelp ===
    Клинки немы́рны, когда судьбу вершит полк. Пусть выстроятся наши легионы на равнине у Болотной балки.
    # speaker: Хан Аташ
    # portrait: black_man
    # layout: right
Тогда на рассвете трубы возвестят бой. Пусть земля дрожит под копытами и стали звоном.
    ~ CastleStrength -= 300
    -> END