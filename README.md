# Civ5

A self-learning project with the goal of recreating a high-performance version of the video game Sid Meier's Civilization V through optimisation.

## Roadmap

### Hex Grid

The hex grid inherits from the abstract class Grid, which provides a basic interface to any arbitrary 2d array. The hex grid class itself then implements how the mouse world position is converted into the grid cell position.

Where a regular square grid would simply divide the relative world position by the grid cell size, the hex grid works by dividing into square chunks, each containing a maximum of 3 hex cells. The hex cell centre with the lowest distance to the mouse position is then determined to be the cell selected.

### Hex Grid Visual

The visual draws one mesh representing the entire map to improve graphical performance. Every hex cell contains 4 triangles, connecting the 6 vertex points of each hexagon. The points are found using the relative position of the hex cell in combination with the geometric properties of a hexagon, with the edges being connected in a predetermined way.

A texture atlas is then provided, where each triangle is mapped onto the correct set of pixels, depending on the underlying features of the tile.

### Map Generation

We use a slightly modified perlin noise algorithm, using the additional properties scale, octaves, persistence, and lacunarity. After which, 4 noise maps are generated for sea-level, elevation, temperature, and rainfall.

Sea level determines what land masses form, e.g. many small islands or a few large continents. Elevation then determines where flat land, hills, and mountains form on the land masses. Temperature and rainfall in combination determines a tile's terrain. E.g. a hot temperature combined with wet rainfall results in a jungle, whereas a combined with dry rainfall results in a desert.

#### Planned

- Overlay a temperature gradient to simulate North and South poles.
- Create a wraparound map by generating a 3D noise map.

### Units [In Progress]

Each unit will have a grid cell position. A player's turn will then consist of cycling through a list of units and performing actions.

### General Debug Visual [In Progress]

Because graphics are currently a bottleneck, I'd like to create a generic debug visual which can implement all aspects of the game, along with an overall debug visual which can toggle various visuals on and off.
