# Zombie Shooter

The following code primarily focuses on the gameplay mechanics of a first-person zombie shooter game. It encompasses zombie behavior, player movement, camera control, and health management. Here's a detailed breakdown of the core components:

## 1. ZombieController
- **Purpose**: Manages the behavior of zombies within the game.
- **Key Features**:
  - **Chasing the Player**: Zombies chase the player when within a specific trigger range.
  - **Aggro System**: Upon detecting the player, zombies enter an "aggro" state, changing their appearance to signal hostility.
  - **Attack**: Zombies can attack the player when within close range, with an attack cooldown to prevent spam damage.
  - **Return to Spawner**: Zombies can be commanded to return to their spawn point if necessary.
  - **Health Management**: Zombies have health that decreases when they are hit. Upon reaching zero health, the zombie is destroyed.

### Key Methods:
- `ChasePlayer()`: Moves the zombie toward the player, except when it is attacking.
- `AttackPlayer()`: Triggers an attack animation and applies damage to the player.
- `ReturnToSpawner()`: Sends the zombie back to its spawn point.

## 2. ZombieSpawner
- **Purpose**: Manages the spawning and handling of multiple zombies.
- **Key Features**:
  - **Spawning Zombies**: Zombies are instantiated at regular intervals at specific spawn points.
  - **Zombie Limit**: The number of active zombies is constrained by `maxZombies`. When a zombie is killed, it is removed from the active list.
  - **Returning Zombies**: Zombies can be sent back to their spawn point, which helps reset their positions if needed.

### Key Methods:
- `SpawnZombie()`: Spawns new zombies if the count is below the max limit.
- `ReturnZombiesToSpawner()`: Returns all active zombies to the spawn point.
- `DestroyAllZombies()`: Destroys all zombies, typically for game resets or clean-ups.

## 3. MoveCamera
- **Purpose**: Smoothly follows the player's camera movements.
- **Key Features**: The camera position updates continuously to ensure it stays in sync with the player's camera, providing a seamless experience.

## 4. ObjectData
- **Purpose**: Manages health and stamina for the player and other objects.
- **Key Features**:
  - **Health and Stamina**: Both health and stamina are tracked for the player, with regeneration rates for each.
  - **Damage Handling**: When the player takes damage, health is updated accordingly. If health reaches zero, the player dies.
  - **Stamina Management**: Stamina regenerates over time and is consumed during dashes.

### Key Methods:
- `DealDamage()`: Applies damage to the player or other objects, with special handling for explosion damage.
- `RegenerateStamina()`: Gradually regenerates stamina over time.
- `RegenerateHealth()`: Heals the player over time if their health isn't full.

## 5. PlayerCam
- **Purpose**: Handles camera movement based on mouse input.
- **Key Features**: Allows the player to rotate the camera vertically and horizontally with mouse movement, while keeping the cursor confined within the game window.

## 6. PlayerMovement
- **Purpose**: Controls the player’s movement and actions.
- **Key Features**:
  - **Movement**: The player moves in the 3D world using input keys (W, A, S, D) and Rigidbody physics.
  - **Jumping**: The player can jump, with a cooldown limiting how often they can jump.
  - **Dashing**: The player can dash in any direction using stamina, providing a burst of speed.
  - **Speed Control**: The player’s movement is limited to a maximum speed to prevent excessive velocity.

### Key Methods:
- `MovePlayer()`: Handles player movement based on input and whether the player is grounded.
- `Dash()`: Allows the player to dash, consuming stamina in the process.
- `Jump()`: Handles the jumping mechanics, including air control and cooldowns.

## 7. GameController
- **Purpose**: Manages the game-over state.
- **Key Functionality**:
  - **GameOver()**: Activates a "Lose Screen" and makes the cursor visible, unlocking it from the center of the screen.

## 8. DestroyAfterTime
- **Purpose**: Destroys a GameObject after a set duration.
- **Key Functionality**:
  - **Start()**: Calls `Invoke()` to destroy the object after a specified time (`duration`).
  - **DestroyAfter()**: Destroys the object after the duration expires.

## 9. Explosion
- **Purpose**: Handles explosion effects, including particle systems and damage.
- **Key Functionality**:
  - **OnCollisionEnter()**: Instantiates explosion effects, applies damage, and adds an explosion force to nearby objects. It also damages the player and other affected entities.
  - **DestroyAll()**: Destroys the particle effects and the explosive object after the explosion completes.

## 10. CalculateFps
- **Purpose**: Displays the FPS (frames per second) counter on the UI.
- **Key Functionality**:
  - **Update()**: Continuously tracks FPS and updates the counter on the screen every second.

## 11. DontDestroy
- **Purpose**: Ensures that a GameObject persists across scene transitions.
- **Key Functionality**:
  - **Awake()**: Calls `DontDestroyOnLoad()` to keep the object from being destroyed when loading a new scene.

## 12. MainMenuController
- **Purpose**: Controls the main menu and options menu, handling UI interactions and settings.
- **Key Functionality**:
  - **Start()**: Initializes UI components (fullscreen toggle, resolution dropdown, FPS options, etc.) and sets their default values.
  - **OpenOptions()**: Opens the options panel and hides the main menu.
  - **OpenMainMenu()**: Opens the main menu and hides the options panel.
  - **SetFullscreen()**: Toggles fullscreen mode.
  - **SetResolution()**: Changes screen resolution based on the dropdown selection.
  - **SetFPSCounter()**: Shows or hides the FPS counter based on user preference.
  - **SetMaxFPS()**: Sets the maximum FPS using the dropdown selection, allowing for frame rate capping.

## Game Mechanics

The gameplay revolves around dynamic interactions between the player and zombies, each with specific systems for health, stamina, and actions.

### Health and Stamina
- **Health**: Both the player and zombies have health points, which decrease when damage is taken. Health can regenerate over time for both entities. The player's health is displayed via a UI slider.
- **Stamina**: The player has stamina, which is necessary for dashing. Stamina regenerates over time and is consumed each time the player uses the dash mechanic.

### Zombies
- Zombies periodically spawn at designated points and chase the player when they are within range. When close enough, they attack the player. Zombies have a set amount of health, and when defeated, they are destroyed.

### Player Mechanics
- **Movement**: The player uses physics-based controls for movement (W, A, S, D keys). The player can jump and dash in any direction, with dash consuming stamina.
- **Jumping**: The player can jump with a cooldown and limited jump count, allowing for evasive maneuvers.
- **Dashing**: Dashing provides a burst of speed, enabling the player to avoid zombie attacks. Stamina is required for dashing, and the player must manage their stamina carefully.

### Explosion Mechanics
- The **Explosion** script instantiates an explosion effect on collision, applying damage to nearby objects and pushing them with an explosion force. The explosion and the associated effects are destroyed after a set time.

### FPS Counter
- The **CalculateFps** script continuously monitors the frame rate and displays it on the UI, providing feedback to the player on performance.

### Main Menu & Options
- The main menu provides toggles for fullscreen mode, resolution settings, and an FPS counter. Players can adjust graphical settings, including capping the frame rate (Max FPS).

### Game Flow
- The **GameController** class tracks the game state, including handling game-over scenarios. The **DestroyAfterTime** class ensures objects are automatically destroyed after a set duration, helping manage the game’s flow.

### Key Interactions
- **Zombie-Player Interaction**: Zombies track and attack the player when they are within range. The player can evade or fight back by using movement techniques like dashing or jumping.
- **Spawner Mechanics**: Zombies are spawned at fixed intervals, and the player has the option to destroy all zombies or return them to the spawn point if needed.

## Conclusion
This dynamic game system creates an exciting environment where zombies chase and attack the player, while the player uses movement mechanics (like jumping and dashing) to evade danger. Health and stamina management are essential for survival, and the camera follows the player’s actions for an immersive experience. The spawner ensures a constant flow of zombies, challenging the player to survive by managing resources effectively.
