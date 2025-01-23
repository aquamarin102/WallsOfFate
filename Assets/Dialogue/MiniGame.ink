INCLUDE global.ink

Сыграем в игру?
+[Да]
   -> chosen("Charmander")
   
+[Нет,я пожалуй откажусь]
    -> chosen1("Bulbasaur")
    
+[Давай потом]
-> chosen2("Squirtle")

===chosen(pokemon)===
~ pokemon_name = pokemon
Отлично
-> DONE

===chosen1(pokemon)===
~ pokemon_name = pokemon
Как жаль
-> DONE

===chosen2(pokemon)===
~ pokemon_name = pokemon
Ну ладно
-> DONE

->END