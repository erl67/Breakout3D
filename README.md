# Breakout 3D 

Inspired by the classic [Breakout on Atari 2600](https://www.youtube.com/watch?v=Up-a5x3coC0) and [Ricochet](https://www.youtube.com/watch?v=cBIedv-i8eo)

An [Eric Laslo](https://github.com/erl67) and [Yanbo Wang](https://github.com/YanboWang76) production


## Instructions:

Position controller as desired, hit any key to begin (or wait 5s)

Balls Available per level - [1, 2, 4, 6, 6]

Board will rotate based on score, 50% score bonus when moving

Score scales based on block height.

You earn points on the level 1 by breaking blocks, level 2+ you earn points by catching blocks.


## Controls:

### Mouse: 

Move - Move paddle

Left Click - Launch ball

Right Click - Reset Camera

Middle Click - Self Destruct (useful if no balls remain)


### Controller:

Move or Tilt - Move Paddle

Tilt Up - Launch Ball (with 5s cooldown between launches)

Tilt Down - Reset Camera / New Life

Shift Key - Toggle between Accelerometer/Gyro mode
   (starts in gyro, accelerometer is unpredictable so you get a bigger paddle)


### Camera: 

Scroll Wheel - Zoom

WASD - Tilt

Z/X - Rotate left/right toggle

Spacebar / R_Mouse - Reset Camera

### Game Options: 

P – Pause

Q – Quit

Middle Mouse – self-destruct

1,2,3,4,5 - select level

0 - restart current level (useful if you let paddle escape)

R – reset on game over

\`

***

#### Arduino Resources: 

To use sensor, use the calibration sketch first.
Then copy the offsets and apply them to the paddle sketch.

https://www.cdiweb.com/datasheets/invensense/MPU-6050_DataSheet_V3%204.pdf

http://playground.arduino.cc/Main/MPU-6050

https://github.com/jrowberg/i2cdevlib

https://funnyvale.com/connecting-mpu6050-gyroscope-accelerometer-to-arduino/

https://www.alanzucconi.com/2015/10/07/how-to-integrate-arduino-with-unity/

http://42bots.com/tutorials/arduino-script-for-mpu-6050-auto-calibration/

https://www.i2cdevlib.com/forums/topic/91-how-to-decide-gyro-and-accelerometer-offsett/

http://wired.chillibasket.com/2015/01/calibrating-mpu6050/


#### Controller Components

Arduino Uno R3: https://www.amazon.com/gp/product/B074WMHLQ4

Sensor: https://www.amazon.com/gp/product/B008BOPN40

Board: https://www.amazon.com/gp/product/B01N9MIH1T

Wires: https://www.amazon.com/gp/product/B01MSMVZQZ

Hook&Loop Tape: https://www.amazon.com/gp/product/B00FQ937NM

Paddle: https://www.amazon.com/gp/product/B002LIJCP8
