System.Collections.Generic.Missings
==================================

Beschreibung
------------
Dieses Projekt stellt generische Collection-Klassen bereit, die im .NET-Framework
nicht enthalten sind. Ziel ist eine einfache, direkte und sofort nutzbare
Erweiterung von System.Collections.Generic ohne zusätzliche Abhängigkeiten.

Die Bibliothek enthält:
- Erweiterte Stack-Implementationen
- LayerStack für segmentierte Stapelbereiche
- Byte-Cache für temporäre Daten
- Byte-Buffer mit Datei-Flush
- Hilfsfunktionen zur Byte-Konvertierung

Alle Klassen sind eigenständig, kompakt und ohne externe Pakete nutzbar.


Installation
------------
1. Projekt klonen oder als NuGet-Paket einbinden.
2. Namespace einbinden:

   using System.Collections.Generic.Missings;

3. Klassen direkt verwenden.


Beispiele
---------

Stack:
------
var stack = new Stack<int>(128);
stack.Push(10);
stack.Push(20, 1);
var result = stack.PopRange(2);

Map:
------
var map = new Map<int, string>(256);
map.Add(1, "Apfel);
map.Add(2, "Birne);
map.Add(3, "Grapefruit);

Array:
-------
var array = new Array<int>(4096);
for(int i=0; i<10; i++)
    array.Add(i);

var Find = array.FindFirst(5);
Console.WriteLine(Find);



Projektstruktur
---------------
System/
  - Collections/
     * Generic/
      + Missings/
        1. Array.cs
        2. BinQueue.cs
        3. BinQueue.cs
        4. FixedArray.cs
        5. FixedMap.cs
        6. IArray.cs
        7. IMap.cs
        8. IPair.cs
        9. ISortedMap.cs
        10. ITuple.cs
        11. Map.cs
        12. MultiMap.cs
        13. Node.cs
        14. Pair.cs
        15. Quad.cs
        16 Queue.cs
        17. SortedMap.cs
       18.  Stack.cs
       19. Trople.cs
        20. Tuple.cs
        21. TupleList.cs
LICENSE
README.txt


Lizenz
------
Dieses Projekt steht unter der European Union Public Licence (EUPL) Version 1.2.
Der vollständige Lizenztext befindet sich in der Datei LICENSE.


Hinweise für Entwickler
-----------------------
- Keine externen Abhängigkeiten.
- Alle Klassen sind bewusst einfach gehalten.
- Ziel ist Erweiterbarkeit ohne Framework-Overhead.
- Änderungen sollten klar dokumentiert und nachvollziehbar sein.


Status
------
Aktive Entwicklung. Weitere Collection-Typen werden nach Bedarf ergänzt.
