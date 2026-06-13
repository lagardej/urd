# AGENTS.md — Urd Project

## What is Urd

A civilisation-scale planetary simulation engine. Simulates physical processes (tectonics, climate, hydrology, geology, weather, vegetation, fauna) that constrain civilisation behaviour over a 10k year period. Runs headless; Godot is the rendering layer only.

## Solution structure

| Project | Role |
|---|---|
| `Urd.Engine` | Engine framework: topology, data validation, autodoc attributes, scheduling contracts. No simulation logic. |
| `Urd.Components` | Domain components (tectonics, climate, civilisation, …). References `Urd.Engine`. |
| `Urd.Autodoc` | Console tool. Generates `README.adoc` per component at build time. References `Urd.Engine` only; loads `Urd.Components.dll` dynamically. |
| `Urd.Godot` | Godot 4 / C# rendering and third-party adapters. References `Urd.Engine` and `Urd.Components`. |

Solution file: `Urd.slnx`

## Key design decisions

- **Topology**: Spherical grid abstracted behind `IGrid` / `Resolution`. `Urd.Engine` and `Urd.Components` are backend-agnostic. The backend is bound in `Urd.Godot`.
- **Timescale**: civilisation scale is the primary runtime constraint. Longer-cadence processes are frozen at generation unless perturbed by a forcing.

## Canonical terminology

See `docs/glossary.adoc` for the full naming authority. Key terms:

- **Component** — facade for a domain model; takes parameters, exposes state, responds to forcings.
- **DynaCore** — computational core of a component; owns state and advances it over time.
- **Forcing** — external perturbation introduced into a running simulation.
- **Parameter** — typed, validated simulation input; frozen once the simulation starts.
- **State** — observable value produced by a component as it advances; read-only to consumers.

## Autodoc

Every `[Component]`-annotated class in `Urd.Components` gets a generated `README.adoc` at its folder root after build. See `Urd.Autodoc/component-template.adoc` for the document structure.

Attributes (defined in `Urd.Engine/Autodoc/`):

| Attribute | Target | Purpose |
|---|---|---|
| `[Component(summary)]` | Class | Marks a component; provides index summary. |
| `[Parameters]` / `[Parameter(unit, purpose)]` | Holder / property | Documents simulation inputs. |
| `[States]` / `[State(unit, purpose)]` | Holder / property | Documents exposed state. |
| `[Forcing]` | Record | Marks a forcing type; listed in component doc. |

## Conventions

- Oxford spelling (`-ize` not `-ise`).
- Backlog files: `.md`, kebab-case under `docs/backlog/`.
- Design documents: `.adoc` under `docs/design/`.
- No GDScript; C# only.
