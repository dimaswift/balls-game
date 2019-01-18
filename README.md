Model is now completely separated. Got rid of States, and used separate controllers for Simulation and Unit.
Made much more robust MVC system:

Unit -> UnitController -> UnitView;
Simulation -> SimulationController -> SimulationView

Added entry point - App.cs that is responsible for creating controllers, delegating events, loading configs.
Changed HUD logic - removed singletons and added HudController that needs an App object to delegate events received by HudElements.
