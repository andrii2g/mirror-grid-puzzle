# mirror-grid-puzzle

Guide beams to their targets across four boards. The game runs locally in the browser using standalone Blazor WebAssembly and .NET 10.

## Play locally

With the .NET 10 SDK installed, run this command from the repository root and open the printed localhost URL.

```sh
dotnet run --project src/MirrorGridPuzzle
```

Click a mirror or focus it and press Enter or Space to rotate it. Each rotation redraws the full beam immediately. **Reset mirrors** restores the current board. Use the level selector to try any board, or advance after reaching all its targets.

| Level | Board | Goal |
| --- | --- | --- |
| First Light | 4×4 | Learn to turn a beam with one mirror |
| Crossed Beams | 5×5 | Route two beams through shared mirrors to their matching targets |
| Closed Circuit | 6×6 | Guide a beam through five mirrors and watch for loops |
| Off Axis | 7×7 | Reflect a beam at angles between the grid axes |

## Step through the beam

Select **Step beam** to advance through the trace and inspect the current row, column, and incoming direction. On Crossed Beams, use **Beam to step** to choose a beam. When the beam loops, the replay highlights the repeated state. Rotating a mirror or resetting the board ends the replay and redraws the full path.

In the first three levels, beams move between adjacent cells. A `/` mirror exchanges north with east and south with west. A `\` mirror exchanges north with west and south with east. The tracer stops at the beam’s target, the board edge, or a repeat of the same cell and incoming direction. The source lets the beam pass through on a return trip. For a board of side length `n`, there are at most `4 × n²` cardinal states.

In Off Axis, beams reflect from finite mirror segments and can travel between the grid axes. Each activation advances to the next angle in the sequence `15°`, `30°`, `45°`, `60°`, `90°`, `105°`, `120°`, `135°`, `150°`, `165°`. Hold Shift while activating a mirror to cycle backward. Angles increase clockwise from east. A geometric loop repeats a mirror hit at the same point with the same incoming angle, within numerical tolerance. The tracer also has a separate limit of 256 reflections. [OpenStax, The Law of Reflection](https://openstax.org/books/university-physics-volume-3/pages/1-2-the-law-of-reflection)

## Run the tests

```sh
dotnet test
```

<details>
<summary>Level fixtures and solutions</summary>

Coordinates use 1-based `(row, column)`.

### First Light

The source at `(2, 1)` fires east toward the mirror at `(2, 3)`. The starting `/` sends the beam out the top. Rotate it to `\` to reach the target at `(4, 3)`.

### Crossed Beams

Beam 1 fires east from `(3, 1)` toward its target at `(4, 1)`. Beam 2 fires south from `(1, 3)` toward its target at `(1, 4)`.

| Configuration | A (3, 3) | B (4, 3) | C (3, 4) | Outcome |
| --- | :-: | :-: | :-: | --- |
| Starting board | `/` | `\` | `\` | Both beams exit |
| Rotate A and B | `\` | `/` | `\` | Beam 1 reaches its target |
| Then rotate C | `\` | `/` | `/` | Both beams reach their targets |

### Closed Circuit

The source at `(2, 3)` fires east. The target is at `(4, 1)`. The original 32 mirror configurations yield one win, two loops, and 29 exits.

| Configuration | A (2, 5) | B (5, 5) | C (5, 2) | D (2, 2) | E (2, 1) | Outcome |
| --- | :-: | :-: | :-: | :-: | :-: | --- |
| Starting board | `\` | `/` | `/` | `/` | `/` | Exits south from `(6, 2)` |
| Rotate C | `\` | `/` | `\` | `/` | `/` | Loops back to `(2, 3, East)` |
| Then rotate D | `\` | `/` | `\` | `\` | `/` | Reaches the target through all five mirrors |

### Off Axis

The source at `(4, 2)` fires east. The target is at `(1, 5)`.

| Configuration | A (4, 4) | B (6, 7) | C (7, 5) | Outcome |
| --- | :-: | :-: | :-: | --- |
| Starting board | 15° | 105° | 45° | Beam exits |
| Winning angles | 15° | 90° | 30° | Beam reaches the target |

</details>
