INCLUDE ../GlobalSettings/global.ink
	# speaker: Гонец
	# portrait: ms_yellow_neutral
	# layout: right
	Пан, перелистывал я старые архивы и нашел любопытную запись - ваш дед спрятал в замке семейное сокровище!

	*   # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: left
	    Где именно искать?
	    -> askDetails

	*   # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: left
	    Старые сказки...
	    -> dismissClaim

	=== askDetails ===
	    # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: left
	    В записи сказано: "Под защитой каменного стража". Думаю, это статуя в главном зале.
	    ~ TalkedWithMagnate = true
	    -> END

	=== dismissClaim ===
	    # speaker: Гонец
	    # portrait: ms_yellow_neutral
	    # layout: left
	    Как знаете, пан. Но я бы проверил...
	    ~ QuestComplite = true
	    -> END