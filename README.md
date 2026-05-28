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
  Collections/
    Generic/
      Missings/
        Array.cs
        BinQueue.cs
        BinQueue.cs
        FixedArray.cs
        FixedMap.cs
        IArray.cs
        IMap.cs
        IPair.cs
        ISortedMap.cs
        ITuple.cs
        Map.cs
        MultiMap.cs
        Node.cs
        Pair.cs
        Quad.cs
        Queue.cs
        SortedMap.cs
        Stack.cs
        Trople.cs
        Tuple.cs
        TupleList.cs
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
