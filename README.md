# mirror-grid-puzzle

Guide beams to their targets across five boards. The game runs locally in the browser using standalone Blazor WebAssembly and .NET 10.

## Play locally

With the .NET 10 SDK installed, run this command from the repository root and open the printed localhost URL.

```sh
dotnet run --project src/MirrorGridPuzzle
```

Choose **Beginner**, **Middle**, or **Senior** in the **Experience** selector to start with one, two, or three beams. Use **Level** to open any board or advance after reaching all its targets.

Click a mirror or focus it and press Enter or Space to rotate it. Each rotation redraws every beam immediately. **Reset mirrors** restores the current board. On boards with several beams, guide each beam to the target with the matching number.

| Experience | Level | Board | Beams | Goal |
| --- | --- | --- | ---: | --- |
| Beginner | First Light | 4×4 | 1 | Learn to turn a beam with one mirror |
| Beginner | Crossed Beams | 5×5 | 2 | Route two beams through shared diagonal mirrors |
| Beginner | Closed Circuit | 6×6 | 1 | Guide a beam through five mirrors and watch for loops |
| Middle | Off Axis | 7×7 | 2 | Route two angled beams through shared mirrors |
| Senior | Shared Focus | 10×10 | 3 | Adjust shared mirrors in 5° steps across three possible layouts |

Shared Focus randomly selects a named preset when you enter the level. Each preset has three beams at 5°, 30°, and 65°. **Reset mirrors** restores the starting angles for the preset you are playing. Leaving and re-entering the level makes a new draw.

## Step through the beam

Select **Step beam** to advance through the trace and inspect the current row, column, and incoming direction. On boards with several beams, use **Beam to step** to choose one. When the beam loops, the replay highlights the repeated state. Rotating a mirror or resetting the board ends the replay and redraws the full paths.

In the first three levels, beams move between adjacent cells. A `/` mirror exchanges north with east and south with west. A `\` mirror exchanges north with west and south with east. The tracer stops at the beam’s target, the board edge, or a repeat of the same cell and incoming direction. The source lets the beam pass through on a return trip. For a board of side length `n`, there are at most `4 × n²` cardinal states.

In the Middle and Senior tracks, beams reflect from finite mirror segments and can travel between the grid axes. Each activation advances to the next available angle for the level. Hold Shift while activating a mirror to cycle backward. Angles increase clockwise from east. A geometric loop repeats a mirror hit at the same point with the same incoming angle, within numerical tolerance. The tracer also has a separate limit of 256 reflections. [OpenStax, The Law of Reflection](https://openstax.org/books/university-physics-volume-3/pages/1-2-the-law-of-reflection)

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

Beam 1 fires east from `(4, 2)` toward `(1, 5)`. Beam 2 fires at 45° from `(1, 2)` toward `(7, 6)`. Both beams use mirror B in the solution.

| Configuration | A (4, 4) | B (6, 7) | C (7, 5) | Outcome |
| --- | :-: | :-: | :-: | --- |
| Starting board | 15° | 105° | 45° | Both beams exit |
| Set B to 90° | 15° | 90° | 45° | Beam 2 reaches its target |
| Then set C to 30° | 15° | 90° | 30° | Both beams reach their targets |

### Shared Focus

The mirrors in all three presets rotate in 5° steps from 25° through 65°.

#### Central Crossing

Beam 1 starts at `(6, 4)` at 5° and aims for `(7, 6)`. Beam 2 starts at `(4, 3)` at 30° and aims for `(4, 9)`. Beam 3 starts at `(4, 4)` at 65° and aims for `(10, 8)`.

| Configuration | A (6, 7) | B (6, 5) | C (5, 5) | Outcome |
| --- | :-: | :-: | :-: | --- |
| Starting board | 30° | 35° | 55° | All three beams exit |
| Set B to 30° | 30° | 30° | 55° | Beam 1 reaches its target |
| Then set A to 35° | 35° | 30° | 55° | Beams 1 and 3 reach their targets |
| Then set C to 60° | 35° | 30° | 60° | All three beams reach their targets |

#### Northern Relay and Far Corner

Angles are listed in mirror order A, B, C. Each winning configuration reaches all three targets.

| Preset | A | B | C | Starting angles | Winning angles |
| --- | --- | --- | --- | --- | --- |
| Northern Relay | (2, 6) | (2, 8) | (9, 6) | 65°, 60°, 30° | 60°, 55°, 35° |
| Far Corner | (8, 8) | (4, 10) | (7, 4) | 35°, 55°, 35° | 40°, 60°, 40° |

</details>
