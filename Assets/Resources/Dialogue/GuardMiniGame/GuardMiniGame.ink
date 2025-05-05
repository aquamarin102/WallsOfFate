INCLUDE ../GlobalSettings/global.ink
# speaker: Стражник
# portrait: black_guard
# layout: right
Пан, хотите провети тренировочный бой?


*   Ну, ты нарвался...
    # speaker: Стражник
    # portrait: black_guard
    # layout: left
    -> gate1Guard

*   У меня нет время на игры
    # speaker: Стражник
    # portrait: black_guard
    # layout: left
    -> gate2Guard

=== gate1Guard ===
    Но это же тренировочный бой!!
    # speaker: Стражник
    # portrait: black_guard
    # layout: left
     ~ PowerCheckStart = true

    -> END

=== gate2Guard ===
    Ладно.
    # speaker: Стражник
    # portrait: black_guard
    # layout: left
    -> END