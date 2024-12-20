Odnosnie id itemkow w Steamie:
Postanowilem sobie, ze materialy beda sie zaczynac na 1 (1 + numer), skiny jako modele na 2, i rozne generator etc na 3

Dla dodania nowego materialu do gry:
1. Stworzyc ScriptableObject pod ten material -> Ctrl+D po wybraniu jednego z poprzednich (sa w Assets/Prefabs/Materials/MatDatas) albo PPM -> Create -> MaterialSO
2. Stworzyc to jako object Steam Inventory (dodalem file steam_items.json do foldera Assets dla latwiejszego uzycia)
3. Wstawic utworzony steam id do nowego ScriptableObjectu
4. Dodac ten ScriptableObject do listy na scenie "Game" (Game -> GameObject "Manager" -> Script "Skins Manager" -> MatDatas)
W MaterialSO effect moze byc null, jezeli go nie musi byc

Dla dodanie nowego skinu (modelu) do gry:
Basically to samo, tylko dodany musi byc w liste "SkinDatas" i ten skin musi miec cene ustawiona w Steamie 