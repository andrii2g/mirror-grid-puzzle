# mirror-grid-puzzle

Rotate five mirrors on a 6×6 board to guide a beam to its target. The game runs in the browser using standalone Blazor WebAssembly and .NET 10.

## Play locally

With the .NET 10 SDK installed, run this command from the repository root and open the printed localhost URL.

```sh
dotnet run --project src/MirrorGridPuzzle
```

Click a mirror or focus it and press Enter or Space to rotate it 90°. The beam redraws after each rotation. Select **Reset mirrors** to restore the starting board.

## How the beam works

The source fires east and the beam moves between adjacent cells. A `/` mirror exchanges north with east and south with west. A `\` mirror exchanges north with west and south with east. Each diagonal turns the beam by 90°, following the law of reflection. [OpenStax, The Law of Reflection](https://openstax.org/books/university-physics-volume-3/pages/1-2-the-law-of-reflection)

Tracing stops when the beam hits the target, exits the board, or revisits a cell with the same incoming direction. The source cell lets the beam pass through on a return trip. There are only `6 × 6 × 4 = 144` possible cell and direction states.

## Run the tests

```sh
dotnet test
```

The tests cover all eight mirror reflections and all 32 mirror configurations. The level has one win, two loops, and 29 exits.

<details>
<summary>Level fixtures and solution</summary>

Coordinates use 1-based `(row, column)`. The source is at `(2, 3)` and the target is at `(4, 1)`.

| Configuration | A (2, 5) | B (5, 5) | C (5, 2) | D (2, 2) | E (2, 1) | Outcome |
| --- | :-: | :-: | :-: | :-: | :-: | --- |
| Starting board | `\` | `/` | `/` | `/` | `/` | Exits south from `(6, 2)` |
| Rotate C | `\` | `/` | `\` | `/` | `/` | Loops back to `(2, 3, East)` |
| Then rotate D | `\` | `/` | `\` | `\` | `/` | Reaches the target through A, B, C, D, and E |

</details>
