INCLUDE ../GlobalSettings/global.ink
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    (разглядывая найденные улики)
    1. Отчёт цел, но явно кто-то его изучал.
    2. Этот кулон... Я где-то видел такой герб.
    3. Эти травы явно не отсюда. И среди моих подданных нет травницы.
    Кто-то недавно тут был!

    *   [Изучить кулон подробнее]
        # speaker: пан Яков
        # portrait: dr_green_neutral
        # layout: left
        -> analyzePendant

    *   [Изучить мешочек]
        # speaker: пан Яков
        # portrait: dr_green_neutral
        # layout: left
        -> followFlour

=== analyzePendant ===
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    Это же мой герб! Как он умудрился украсть его из моего кабинета?!
    -> connectClues

=== followFlour ===
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    От мешочка идут россыпи травинок вглубь подвала... Похоже, кто-то хотел оставить себе путь к выходу. Следы ведут прямо от двери.
    -> connectClues

=== connectClues ===
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    Значит, шпионом был кто-то из наших — иначе как бы он получил кулон? Я точно помню, что положил его в своём кабинете.
    -> realization

=== realization ===
    # speaker: пан Яков
    # portrait: dr_green_neutral
    # layout: left
    Хм... След из травинок ещё свежий... Предатель где-то здесь, на складе!
    -> END