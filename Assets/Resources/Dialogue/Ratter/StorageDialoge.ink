INCLUDE ../GlobalSettings/global.ink
	# speaker: Плотник
	# portrait: black_man
	# layout: right
	Н-не убивайте... Мне всего лишь 30 серебреников предложили...

	*   # speaker: Плотник
	    # portrait: drunk_carpenter
	    # layout: left   
	    Перед рыцарем не завинился?! УБИВАААЮЮЮ!!!
	    -> executeTraitor

	*	# speaker: Плотник
	    # portrait: black_man
	    # layout: left   
	    Я даю тебе последний шанс...
	    -> exileTraitor

	=== executeTraitor ===
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: left
	    Я хочу к маме... 
	    ~ PeopleSatisfaction -= 1
	    ~ CastleStrength += 2
	    -> END

	=== exileTraitor ===
	    # speaker: Плотник
	    # portrait: black_man
	    # layout: left
	    Я устал... я ухожу...
	    ~ Food -= 10
	    ~ CastleStrength += 1
	    -> END