INCLUDE ../GlobalSettings/global.ink
# speaker: Гонец
# portrait: ms_yellow_neutral
# layout: right
Чёрт возьми, ваша милость! Крысы завелись в наших стенах - кто-то стащил все замковые отчёты! Последние следы ведут на склад...

*   Ладно.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
    -> investigateWarehouse

*   Хватит ныть!
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
    -> dismissTheft

=== investigateWarehouse ===
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
Я, сам разберусь с этой проблемой.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: right
Слава Богу! В тех бумагах - все наши слабые места... 
И не только наши...
    -> END

=== dismissTheft ===
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: left
Пусть стража отрабатывает свой хлеб.
    # speaker: Гонец
    # portrait: ms_yellow_neutral
    # layout: right
 Как скажете, пан... 
 Вот только вся стража занята обеспечением порядка и подготовкой к нападению.
    -> investigateWarehouse