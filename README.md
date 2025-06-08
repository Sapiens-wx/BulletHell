# Games For EEG
---
## Hide And Seek
---
### Table of Contents
1. [Usage](#usage)
2. [Parameters](#parameters)
### Usage
---
1. Press the play button
2. Use A/D or Left/Right Arrow to control the avatar's movement
3. Reach the child on the left/right side of the screen to earn a score
4. The total score and average formula score appears on the bottom left of the screen
5. To quit the game, press the stop button. The game score and formula score of each round is logged to EventCollector.

### Parameters
---
__GameManager.cs__
|Variable Name|Description|
|-|-|
|Rest Interval|Rest time between each round in second|
|Appear Duration|How long is each round in seconds|

__PlayerCtrl.cs__
|Variable Name|Description|
|-|-|
|Walk Duration|How long will it take for the avatar to walk from the middle to the leftmost position (in seconds)|
|Correct Movement|True: player cannot move in the opposite direction.<br>False: player can move in the opposite direction|
