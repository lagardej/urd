# The Three Norns

Architectural metaphor for the separation of concerns in Urtharbrunnr (*Urðarbrunnr* — the Well of Urðr).

The well beneath Yggdrasil is where the three Norns assemble to weave the web of wyrd (*vefr*).
The loom is the engine. The threads are the fates of agents — the simulation data.
The web is the product: the emergent tapestry of a civilisation over time.

The project is currently named **Urd**, which is a misnomer — *Urðr* is one of the three Norns, not the assembly of all three.
The correct umbrella name is **Urtharbrunnr**.

Romanisation convention: ð → th (e.g. *Urðr* → Urth, *Verðandi* → Verthandi).

## Urth — *Urðr* — what has been

The well. The source and the record.

- Immutable substrate: topology, grid
- Frozen inputs: validated parameters
- Persistence: saved state, history, replays
- Documentation: the record of what exists (`Autodoc`)

## Verthandi — *Verðandi* — what is becoming

The active weaving. The present tick.

- Clock: the beat of the loom
- Messaging: threads in motion
- Components: agents being woven
- Inspection, Profiling: watching the weave as it happens

## Skuld — *Skuld* — what shall be

The futures enqueued.

- Scheduling: jobs due, work not yet done
- Forcings: perturbations not yet applied
- Possibly: the engine that animates agents going forward

## Current state

`Urd.Engine` conflates all three. The name is a misnomer — it is mostly Verthandi.
Separating concerns along these lines is a future refactor, not immediate work.
