# Sudoku Solver
Ce projet de programmation en binôme en C# a pour objectif de réaliser un solveur de sudoku dans une console. Il peut être compilé avec mono et contient un menu interne pour choisir le fichier, avec un format txt ou json.

## Prérequis
* Mono (si vous souhaitez compiler le code vous-même)
* Un fichier de sudoku au format txt ou json (voir ci-dessous pour les détails sur le format)

## Utilisation
* Compilez le .cs avec la commande `mcs V2.cs`
* Exécutez le fichier compilé avec la commande `mono V2.exe`
* Suivez les instructions affichées à l'écran pour choisir le fichier de sudoku à résoudre (le dossier de recherche de grilles est le ./grids).
* Suivez ensuite les instructions afin de choisir avec quel algorithme vous souhaitez résoudre le sudoku.
* Le résultat sera affiché à l'écran.

## Format du fichier de sudoku
Le fichier de sudoku doit être au format txt ou json et doit contenir les chiffres de la grille de sudoku, remplaçant les cases vides par des '.' ou des '0'.

Voici un exemple de fichier txt valide :

```
1 0 0 4 0 0 0 0 2 0 0 1 0 0 0 3
```

Et voici un exemple de fichier json valide :

```json
[
    [
        1,
        0,
        0,
        4,
        0,
        0,
        0,
        0,
        2,
        0,
        0,
        1,
        0,
        0,
        0,
        3
    ]
]
```