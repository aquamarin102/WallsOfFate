INCLUDE ../GlobalSettings/global.ink
# speaker: Кузнец
# portrait: SmithyPortrait
# layout: right
Пан, беда! Из кузни пропадают гвозди! Этот бродяга у ворот точно что-то знает...

*   Где гвозди?
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    -> accuseBlacksmith

*   Бродяга выглядит подозрительно...
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    -> accuseBeggar

=== accuseBlacksmith ===
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    Ты сам нечист на руку! Где гвозди!?
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: right
    Да как вы смеете! Я служу верой и правдой!
    ~ PowerCheckStart = true
    -> END

=== accuseBeggar ===
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: left
    Бродяга выглядит подозрительно...
    # speaker: Кузнец
    # portrait: SmithyPortrait
    # layout: right
    Так и знал! Пусть стража разберётся!
    ~ TalkedWithMagnate = true
    -> END
